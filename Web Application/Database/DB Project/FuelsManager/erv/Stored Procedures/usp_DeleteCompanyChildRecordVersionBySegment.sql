/*
	DROP PROCEDURE [erv].[usp_DeleteCompanyChildRecordVersionBySegment]

	EXEC [erv].[usp_DeleteCompanyChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '00000000-0000-0000-0000-000000000001'
	EXEC [erv].[usp_DeleteCompanyChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'

*/
CREATE PROCEDURE [erv].[usp_DeleteCompanyChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @IncludeChildRecordVersionsAssignedToSourceSiteGroup bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_DeleteCompanyChildRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes all the Company child record versions for all the entity assignments of a given Company segment (note: there is only one Segment Template for Companies) 
	-- from a given SiteGroup (if @IncludeChildRecordVersionsAssignedToSourceSiteGroup = 0) or both from and to the given sitegroup (if @IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template that needs to be processed.	
	-- 2. @SourceSiteGroupGuid: SiteGroup parent from which the child record versions to be deleted were created. This would correspond to the AssignedFrom Sitegroup.
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.	
	-- 3. @IncludeChildRecordVersionsAssignedToSourceSiteGroup: 
	--			0 (Default Mode). Only delete the child record versions assigned from the sitegroup.
	--			1: Delete both the child record versions assigned from and to the sitegroup.
	-- 4. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @EmptyGuid uniqueidentifier
		SET @EmptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()

		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@entityTypeId = 'Company')
		BEGIN
			--Capture the Site/SiteGroup, MasterRecordGuid, and the Entity Guid of the child record versions to be deleted.
			INSERT INTO [erv].[tblTempEntityRecordVersion]
			(SiteGuid, MasterRecordGuid, EntityGuid, AssignedFromSiteGuid, _CallingReferenceGuid)
			SELECT b.SiteGuid, b.CompanyGuid, a.CompanyGuid, b.AssignedFromSiteGuid, @callingRefGuid
			FROM tblCompanies a
			INNER JOIN map.tblEntityCompanyToSite b
			ON b.CompanyGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid  
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND b.SiteGuid <> b.AssignedFromSiteGuid
			AND a.CompanyGuid <> a._MasterRecordGuid

			IF (@IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
			BEGIN
				INSERT INTO [erv].[tblTempEntityRecordVersion]
				(SiteGuid, MasterRecordGuid, EntityGuid, AssignedFromSiteGuid, _CallingReferenceGuid)
				SELECT b.SiteGuid, b.CompanyGuid, a.CompanyGuid, b.AssignedFromSiteGuid, @callingRefGuid
				FROM tblCompanies a
				INNER JOIN map.tblEntityCompanyToSite b
				ON b.CompanyGuid = a._MasterRecordGuid
				AND b.SiteGuid = a.SiteGuid  
				WHERE b.SiteGuid = @SourceSiteGroupGuid
				AND b.SiteGuid <> b.AssignedFromSiteGuid
				AND a.CompanyGuid <> a._MasterRecordGuid
			END
		END

		IF (@IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
		BEGIN
			UPDATE a 
			SET a.ParentRecordGuid = erv.udf_GetFirstParentRecordVersionGuid('Company', a.MasterRecordGuid, b.AssignedFromSiteGuid)
			FROM [erv].[tblTempEntityRecordVersion] a	
			INNER JOIN map.tblEntityCompanyToSite b
			ON b.CompanyGuid = a.MasterRecordGuid		
			AND b.SiteGuid = a.AssignedFromSiteGuid
			WHERE a._CallingReferenceGuid = @callingRefGuid
		END
		ELSE
		BEGIN
			UPDATE a 
			SET a.ParentRecordGuid = erv.udf_GetFirstParentRecordVersionGuid('Company', a.MasterRecordGuid, a.AssignedFromSiteGuid)
			FROM [erv].[tblTempEntityRecordVersion] a
			WHERE a._CallingReferenceGuid = @callingRefGuid		
		END

		--Delete the external attributes of the parent record version
		
		--Equipments. The relationship between Company and Equipment is maintained fully on the Equipment side, and references the Company using the Company MasterRecordGuid, 
		--and as such, there are no record versioning changes to be applied to the Company-Equipment relationships during a CompanyToSite mapping deletion.
			
		--Delete the AuthorizedShipTo mappings of the child record versions
		DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.CompanyGuid
		INNER JOIN dbo.tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Reset the Shared Mappings between Product and Company. This will delete, update, and clone the applicable Product-to-Company mappings as necessary.
		--The following Target fields and mapping tables are covered by this process: 
		--(ShipToAuthorizedProducts)->[map].[tblProductToCompany], 
		--(UnavailableInventories)->[map].[tblProductToUnavailableInventoryCompany], 
		--(SupplierAuthorizedProducts)->[map].[tblProductToSupplierProductCompany]
		DECLARE @tblTargetCompanies erv.utt_EntityRecordVersions
		INSERT INTO @tblTargetCompanies
		(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
		SELECT 'Company', EntityGuid, MasterRecordGuid, SiteGuid FROM [erv].[tblTempEntityRecordVersion] WHERE _CallingReferenceGuid = @callingRefGuid

		EXEC [erv].[usp_SetProductToCompanyMappingsForDeletedCompanies] @tblTargetCompanies

		--Reset the Shared Mappings between Personnel and Company. This will delete, update, and clone the applicable Personnel-to-Company mappings as necessary.
		EXEC [erv].[usp_SetPersonnelToCompanyMappingsForDeletedCompanies] @tblTargetCompanies

		--Delete the AuthorizedCarriers mappings of the child record versions
		DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.AssignedToCompanyGuid
		INNER JOIN dbo.tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Delete the AccessSchedule mappings of the child record versions
		DELETE a FROM [dbo].[tblScheduleCompanyAccess] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.CompanyGuid
		INNER JOIN dbo.tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Delete the CertificatesAndPermits mappings of the child record versions
		DELETE a FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.CompanyGuid
		INNER JOIN dbo.tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		
		--UserGroup is both an External Attribute of Company (i.e. Company-To-UserGroup mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-UserGroup mappings are also maintained as part of the UserGroup entity, i.e. outside of the Company entity)
		--Delete all the Company-to-UserGroup mappings of the Company child record version that are not present in the Assigned From Site 	
		DELETE a FROM [map].[tblCompanyCompanyToUserGroup] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.MasterRecordGuid = a.CompanyGuid AND a.SiteGuid = b.SiteGuid
		WHERE NOT EXISTS
		(
			SELECT 1 FROM [map].[tblCompanyCompanyToUserGroup] c 
			WHERE ISNULL(a.CompanyGuid, @EmptyGuid) = ISNULL(c.CompanyGuid, @EmptyGuid)
			AND a.GroupGuid = c.GroupGuid 
			AND c.SiteGuid = b.AssignedFromSiteGuid
		)
		AND b._CallingReferenceGuid = @callingRefGuid

		-- Insert a new Company-UserGroup child record mapping for each child site in the Entity To Site hierarchy where CompanyGuid is NOT NULL
		INSERT INTO [map].[tblCompanyCompanyToUserGroup]
			(CompanyGuid, GroupGuid, SiteGuid, ID)
		SELECT DISTINCT a.CompanyGuid, a.GroupGuid, b.SiteGuid, '' FROM [map].[tblCompanyCompanyToUserGroup] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.MasterRecordGuid = a.CompanyGuid AND a.SiteGuid = b.AssignedFromSiteGuid
		INNER JOIN [map].[tblEntityUserGroupToSite] c
		ON c.GroupGuid = a.GroupGuid
		WHERE NOT EXISTS(SELECT 1 FROM [map].[tblCompanyCompanyToUserGroup] d WHERE d.CompanyGuid IS NOT NULL AND a.CompanyGuid = d.CompanyGuid AND a.GroupGuid = d.GroupGuid AND d.SiteGuid = b.SiteGuid)
		AND b._CallingReferenceGuid = @callingRefGuid

		-- Insert a new Company-UserGroup child record mapping for each child site in the Entity To Site hierarchy where CompanyGuid is NULL
		INSERT INTO [map].[tblCompanyCompanyToUserGroup]
			(CompanyGuid, GroupGuid, SiteGuid, ID)
		SELECT DISTINCT a.CompanyGuid, a.GroupGuid, b.SiteGuid, '' FROM [map].[tblCompanyCompanyToUserGroup] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON a.SiteGuid = b.AssignedFromSiteGuid
		INNER JOIN [map].[tblEntityUserGroupToSite] c
		ON c.GroupGuid = a.GroupGuid
		WHERE a.CompanyGuid IS NULL
		AND NOT EXISTS(SELECT 1 FROM [map].[tblCompanyCompanyToUserGroup] d WHERE d.CompanyGuid IS NULL AND d.GroupGuid = a.GroupGuid AND d.SiteGuid = b.SiteGuid)
		AND b._CallingReferenceGuid = @callingRefGuid


		--Delete the CompanyRoles mappings of the child record versions
		--Company Roles are created/cloned and deleted independently of Record Versioning during company-to-site assignments. They are maintained separately in map.tblCompanyToRole for each company-to-site assignment, using a combination of MasterRecordGuid and Siteguid.				

		--Delete the child record versions
		DELETE a FROM tblCompanies a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.EntityGuid = a.CompanyGuid
		WHERE b._CallingReferenceGuid = @callingRefGuid
		AND a.CompanyGuid <> a._MasterRecordGuid

		DELETE [erv].[tblTempEntityRecordVersion] WHERE _CallingReferenceGuid = @callingRefGuid
		
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [erv].usp_DeleteCompanyChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH
	
END
