/*
	DROP PROCEDURE [erv].[usp_SetPersonnelToCompanyMappingsForDeletedCompanies]

	DECLARE @tblTargetCompanies erv.utt_EntityRecordVersions
	INSERT INTO @tblTargetCompanies
	(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
	VALUES ('Company', 'BCAD83C8-CBCD-4A4A-8BBD-2CA14AD0E7A9', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Company', 'B98881AA-540D-4127-92F8-E4CC75586D0A', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Company', '6F5FEF48-72B2-4AE0-A1F0-C74296D78487', '5D108063-0B46-49DA-8DAE-C37C07804EA8', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Company', '31062FCF-ADDC-428B-860D-D185862E1E8E', '5D108063-0B46-49DA-8DAE-C37C07804EA8', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Company', 'EE7C5B83-39D7-4956-BFBF-45869B1B06C7', '80B08634-D356-4569-B9A2-CD36DF955BD0', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Company', 'F16C052E-2549-4B00-81EC-1AD7818F6A49', '80B08634-D356-4569-B9A2-CD36DF955BD0', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421')
	EXEC [erv].[usp_SetPersonnelToCompanyMappingsForDeletedCompanies] @tblTargetCompanies

*/
CREATE PROCEDURE [erv].[usp_SetPersonnelToCompanyMappingsForDeletedCompanies]
(
	@tblTargetCompanies erv.utt_EntityRecordVersions READONLY
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_SetPersonnelToCompanyMappingsForDeletedCompanies] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete, Add, or Update the Personnel-To-Company mappings to the support the deletions of a set of Company child record versions as a result of FLC configuration changes.	
	-- Notes:
	-- 1. @tblTargetCompanies: Table containing the Company record versions whose deletion impact on the Personnel-to-Company mappings need to be addressed.
	-- 2. This procedure addresses the Shared Mappings needs of the Personnel-to-Company mappings when a Company record version is deleted.
	-- 3. This procedure is to be executed before the actual deletion of the Company child record versions.
	-- 4. This procedure assumes that the Personnel will still be mapped to the target site/sitegroup after the deletion, even though the Company child record version will be deleted, i.e.
	--    this procedure is not to be used in the case of Personnel-to-Site mapping deletions.
	-- 5. It handles the Personnel-to-Company mappings managed by the [map].[tblCompanyPersonnelAssignedToCompany] table
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		
		DECLARE @mappingCount int
		DECLARE @level int		

		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION 
            SET @BeginTran = 1   
		END  
		BEGIN TRY

			------------------------------------------------------------map.tblCompanyPersonnelAssignedToCompany--------------------------------------------------------------------------------
			--For each Company that is mapped to a Personnel record version owned by the same site/sitegroup as the Company owner site or by a lower site/sitegroup, retrieve the details of the Personnel record version that is a parent to the Personnel record version that is tied in the mapping.
			INSERT INTO erv.tblTempPersonnelToCompanyForParentPersonnel
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, PersonnelGuid, PersonnelMasterRecordGuid, PersonnelParentSiteGuid, ParentPersonnelGuid, CompanyParentSiteGuid, IsMasterRecordPersonnel, PersonnelOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.PersonnelGuid, c._MasterRecordGuid, e.SiteGuid, e.PersonnelGuid, g.AssignedFromSiteGuid, 0, 1, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblCompanyPersonnelAssignedToCompany b
			ON b.CompanyGuid = a.EntityGuid
			INNER JOIN tblPersonnel c
			ON c.PersonnelGuid = b.PersonnelGuid  -- This covers both Personnel that are owned by the same site/sitegroup as the target Companies, and those Personnel that are owned by a lower site/sitegroup.
			INNER JOIN map.tblEntityPersonnelToSite d
			ON d.PersonnelGuid = c._MasterRecordGuid
			AND d.SiteGuid = c.SiteGuid
			INNER JOIN tblPersonnel e
			ON e._MasterRecordGuid = c._MasterRecordGuid
			AND e.SiteGuid = d.AssignedFromSiteGuid  -- Personnel that own the record at their own AssignedFrom sitegroup because those are the only ones that can maintain their own mappings.
			INNER JOIN tblCompanies f
			ON f.CompanyGuid = a.EntityGuid
			INNER JOIN map.tblEntityCompanyToSite g
			ON g.CompanyGuid = f._MasterRecordGuid
			AND g.SiteGuid = f.SiteGuid
			WHERE f.CompanyGuid <> f._MasterRecordGuid  --Operation limited to Company child record versions
			AND f.SiteGuid = a.SiteGuid
			AND a.EntityTypeId = 'Company'

			--Retrieve the first available parent Company record version applicable for all the Company records captured in erv.tblTempPersonnelToCompanyForParentPersonnel, starting from the CompanyParentSiteGuid.
			--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it does not insert one record per parent, instead it just updates the AssignedFromSiteGuid and the EntityGuid of the initial record to reflect the parent record.
			DECLARE @callingRef2Guid uniqueidentifier
			SET @callingRef2Guid = NEWID()

			INSERT INTO erv.tblTempEntityMappingHierarchy
			(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
			SELECT a.CompanyMasterRecordGuid, b.CompanyGuid, a.CompanyParentSiteGuid, 0, @callingRef2Guid
			FROM erv.tblTempPersonnelToCompanyForParentPersonnel a
			LEFT OUTER JOIN tblCompanies b
			ON b._MasterRecordGuid = a.CompanyMasterRecordGuid
			AND b.SiteGuid = a.CompanyParentSiteGuid
			AND a._CallingReferenceGuid = @callingRef1Guid

			SET @level = 0

			WHILE ((SELECT COUNT(*) FROM erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef2Guid AND EntityGuid IS NULL) > 0)
			BEGIN
				SET @level = @level - 1
				IF (@level < -20)
				BEGIN
					RAISERROR('Maximum iteration of mapping hierarchy reached.',16,1);   --safeguard against infinite looping
					RETURN;
				END
				UPDATE a 
				SET a.AssignedFromSiteGuid = b.SiteGuid, a.EntityGuid = c.CompanyGuid
				FROM erv.tblTempEntityMappingHierarchy a
				INNER JOIN map.tblEntityCompanyToSite b
				ON b.CompanyGuid = a.EntityMasterGuid
				AND b.SiteGuid = a.AssignedFromSiteGuid
				LEFT OUTER JOIN tblCompanies c
				ON c._MasterRecordGuid = b.CompanyGuid
				AND c.SiteGuid = b.SiteGuid
				WHERE a._CallingReferenceGuid = @callingRef2Guid
				AND a.EntityGuid IS NULL
			END					

			-- Retrieve the first available Company record applicable for the Company Parent Sitegroup. Note: Unlike with the Parent Personnel record, the CompanyGuidForParentPersonnel does not have to be owned by the parent sitegroup. It can be owned by any sitegroup further up the site hierarchy. 
			UPDATE a 
			SET a.CompanyGuidForParentPersonnel = b.EntityGuid
			FROM erv.tblTempPersonnelToCompanyForParentPersonnel a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.CompanyMasterRecordGuid
			AND b.AssignedToSiteGuid = a.CompanyParentSiteGuid
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef2Guid

			DELETE erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef2Guid
			
			--Mark all Personnel that have a master record at either the target (AssignedTo) sitegroup of the Company or lower, as a MasterRecordPersonnel
			UPDATE a 
			SET a.IsMasterRecordPersonnel = 1
			FROM erv.tblTempPersonnelToCompanyForParentPersonnel a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.CompanyMasterRecordGuid
			WHERE a.PersonnelMasterRecordGuid = a.ParentPersonnelGuid
			AND a.PersonnelParentSiteGuid = b.AssignedToSiteGuid
			AND a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef1Guid

			-- Retrieve the Forward Control Mode of the Personnel field that is used to control the map.tblPersonnelToCompany from the Personnel side
			UPDATE a 
			SET a.CarrierFCM = b.ForwardControlMode
			FROM erv.tblTempPersonnelToCompanyForParentPersonnel a
			INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
			ON b.SiteGroupGuid = PersonnelParentSiteGuid
			INNER JOIN erv.tblEntitySegmentTemplate c
			ON c.EntitySegmentTemplateGuid = b.EntitySegmentTemplateGuid
			WHERE c.EntityTypeId = 'Personnel'
			AND b.TargetField = 'Carrier'
			AND a._CallingReferenceGuid = @callingRef1Guid

			UPDATE erv.tblTempPersonnelToCompanyForParentPersonnel
			SET CarrierFCM = 'ParentSpecific'
			WHERE _CallingReferenceGuid = @callingRef1Guid
			AND CarrierFCM IS NULL
			AND IsMasterRecordPersonnel <> 1


			--Capture the Company mappings with Personnel which do not have a record that is owned by their AssignedFrom sitegroup
			INSERT INTO erv.tblTempPersonnelToCompanyForParentPersonnel
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, PersonnelGuid, PersonnelMasterRecordGuid, IsMasterRecordPersonnel, PersonnelOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.PersonnelGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblCompanyPersonnelAssignedToCompany b
			ON b.CompanyGuid = a.EntityGuid
			INNER JOIN tblPersonnel c
			ON c.PersonnelGuid = b.PersonnelGuid
			INNER JOIN map.tblEntityPersonnelToSite d
			ON d.PersonnelGuid = c._MasterRecordGuid
			AND d.SiteGuid = a.SiteGuid
			WHERE c.SiteGuid <> a.SiteGuid
			AND NOT EXISTS
			(
				SELECT * FROM tblPersonnel e
				WHERE e._MasterRecordGuid = c._MasterRecordGuid
				AND e.SiteGuid = d.AssignedFromSiteGuid
			)
			AND NOT EXISTS 
			(
				SELECT * FROM erv.tblTempPersonnelToCompanyForParentPersonnel f
				WHERE f.CompanyGuid = a.EntityGuid
				AND f.PersonnelGuid = b.PersonnelGuid
				AND f._CallingReferenceGuid = @callingRef1Guid
			)


			--Capture Company mappings with Personnel which are not even mapped to the target site (This can happen as a result of Record Versioning cloning. All Mappings are cloned, irrespective of whether the associated/opposite entity is mapped to the target site or not.)
			INSERT INTO erv.tblTempPersonnelToCompanyForParentPersonnel
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, PersonnelGuid, PersonnelMasterRecordGuid, IsMasterRecordPersonnel, PersonnelOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.PersonnelGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblCompanyPersonnelAssignedToCompany b
			ON b.CompanyGuid = a.EntityGuid
			INNER JOIN tblPersonnel c
			ON c.PersonnelGuid = b.PersonnelGuid
			WHERE c.SiteGuid <> a.SiteGuid
			AND NOT EXISTS
			(
				SELECT * FROM map.tblEntityPersonnelToSite d
				WHERE d.PersonnelGuid = c._MasterRecordGuid
				AND d.SiteGuid = a.SiteGuid
			)
			AND NOT EXISTS 
		(
			SELECT * FROM erv.tblTempPersonnelToCompanyForParentPersonnel e
			WHERE e.CompanyGuid = a.EntityGuid
			AND e.PersonnelGuid = b.PersonnelGuid
			AND e._CallingReferenceGuid = @callingRef1Guid
		)


			--Delete all the mappings owned by the target Company child record versions if the FMC of the Personnel.Carrier is 'ParentSpecific', i.e. if the corresponding Personnel record in the mapping is not allowed to have its own version of the Personnel-to-Company mappings.
			DELETE a 
			FROM map.tblCompanyPersonnelAssignedToCompany a
			INNER JOIN erv.tblTempPersonnelToCompanyForParentPersonnel b
			ON b.CompanyGuid = a.CompanyGuid
			AND b.PersonnelGuid = a.PersonnelGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND ((b.CarrierFCM = 'ParentSpecific') OR (b.PersonnelOwnsRecordAtAssignedFromSitegroup = 0))
			AND b.IsMasterRecordPersonnel <> 1
		
			-- If a Personnel in the mappings owned by the target Company child record versions has a Parent Personnel record which itself has a mapping with the Parent Company record, then clone that parent mapping for the child Personnel record version associated with the target Company child record version.
			INSERT INTO map.tblCompanyPersonnelAssignedToCompany 
			(CompanyGuid, PersonnelGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.CompanyGuid, b.PersonnelGuid, b.TargetSiteGuid, a.ID, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM map.tblCompanyPersonnelAssignedToCompany a 
			INNER JOIN erv.tblTempPersonnelToCompanyForParentPersonnel b
			ON b.ParentPersonnelGuid = a.PersonnelGuid
			AND b.CompanyGuidForParentPersonnel = a.CompanyGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.PersonnelOwnsRecordAtAssignedFromSitegroup = 1
			AND b.CarrierFCM = 'ParentSpecific'
			AND b.IsMasterRecordPersonnel <> 1
			AND b.CompanyGuidForParentPersonnel IS NOT NULL
			AND NOT EXISTS
			(
				SELECT * FROM map.tblCompanyPersonnelAssignedToCompany c
				WHERE c.CompanyGuid = a.CompanyGuid
				AND c.PersonnelGuid = b.PersonnelGuid
			)

			-- If the corresponding Personnel record in the target Company child record version mapping is allowed to have its own version of the Personnel-to-Company mappings, then do not delete that mapping, but simply modify it to point to the Parent Company Guid, instead of the target Company child record version (that is marked for deletion).
			UPDATE a 
			SET a.CompanyGuid = b.CompanyGuidForParentPersonnel, a.UpdatedDate = GETDATE()
			FROM map.tblCompanyPersonnelAssignedToCompany a
			INNER JOIN erv.tblTempPersonnelToCompanyForParentPersonnel b
			ON b.PersonnelGuid = a.PersonnelGuid
			AND b.CompanyGuid = a.CompanyGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.PersonnelOwnsRecordAtAssignedFromSitegroup = 1
			AND
			(
				(b.CarrierFCM = 'VersionSpecific')
				OR 
				(b.IsMasterRecordPersonnel = 1)
			)
			AND b.CompanyGuidForParentPersonnel IS NOT NULL
			
			DELETE erv.tblTempPersonnelToCompanyForParentPersonnel WHERE _CallingReferenceGuid = @callingRef1Guid
			
			IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
				COMMIT TRANSACTION 

		END TRY
		BEGIN CATCH
			IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
				ROLLBACK TRANSACTION 
			DECLARE @ErrorMessage NVARCHAR(4000);
			DECLARE @ErrorSeverity INT;
			DECLARE @ErrorState INT;
			SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
			RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
		END CATCH
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
						+ 'Procedure Name: [erv].usp_SetPersonnelToCompanyMappingsForDeletedCompanies' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END