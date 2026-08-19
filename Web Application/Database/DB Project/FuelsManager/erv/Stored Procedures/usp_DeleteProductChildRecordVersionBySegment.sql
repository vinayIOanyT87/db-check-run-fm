

/*

	EXEC [erv].[usp_DeleteProductChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '00000000-0000-0000-0000-000000000001'
	EXEC [erv].[usp_DeleteProductChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'

	EXEC [erv].[usp_DeleteProductChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '46426312-E408-4AF8-85FD-338B622B32BF'

*/

CREATE PROCEDURE [erv].[usp_DeleteProductChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @IncludeChildRecordVersionsAssignedToSourceSiteGroup bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_DeleteProductChildRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Deletes all the Product child record versions for all the entity assignments of a given Product segment (note: there is only one Segment Template for Products) 
	-- from a given SiteGroup (if @IncludeChildRecordVersionsAssignedToSourceSiteGroup = 0) or both from and to the given sitegroup (if @IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template that needs to be processed.	
	-- 2. @SourceSiteGroupGuid: SiteGroup parent from which the child record versions to be deleted were created. This would correspond to the AssignedFrom Sitegroup.
	-- 3. @IncludeChildRecordVersionsAssignedToSourceSiteGroup: 
	--			0 (Default Mode). Only delete the child record versions assigned from the sitegroup.
	--			1: Delete both the child record versions assigned from and to the sitegroup.
	-- 4. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()
		
		DECLARE @tblTargetProducts erv.utt_EntityRecordVersions
		
		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@entityTypeId = 'Product')
		BEGIN
			--Capture the Site/SiteGroup, MasterRecordGuid, and the Entity Guid of the child record versions to be deleted.		
			INSERT INTO [erv].[tblTempEntityRecordVersion]
			(SiteGuid, MasterRecordGuid, EntityGuid, AssignedFromSiteGuid, _CallingReferenceGuid)
			SELECT b.SiteGuid, b.ProductGuid, a.ProductGuid, b.AssignedFromSiteGuid, @callingRefGuid
			FROM tblProducts a
			INNER JOIN map.tblEntityProductToSite b
			ON b.ProductGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid  
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND b.SiteGuid <> b.AssignedFromSiteGuid
			AND a.ProductGuid <> a._MasterRecordGuid

			IF (@IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
			BEGIN
				INSERT INTO [erv].[tblTempEntityRecordVersion]
				(SiteGuid, MasterRecordGuid, EntityGuid, AssignedFromSiteGuid, _CallingReferenceGuid)
				SELECT b.SiteGuid, b.ProductGuid, a.ProductGuid, b.AssignedFromSiteGuid, @callingRefGuid
				FROM tblProducts a
				INNER JOIN map.tblEntityProductToSite b
				ON b.ProductGuid = a._MasterRecordGuid
				AND b.SiteGuid = a.SiteGuid  
				WHERE b.SiteGuid = @SourceSiteGroupGuid
				AND b.SiteGuid <> b.AssignedFromSiteGuid
				AND a.ProductGuid <> a._MasterRecordGuid
			END
		END
		IF (@IncludeChildRecordVersionsAssignedToSourceSiteGroup = 1)
		BEGIN
			UPDATE a 
			SET a.ParentRecordGuid = erv.udf_GetFirstParentRecordVersionGuid('Product', a.MasterRecordGuid, b.AssignedFromSiteGuid)
			FROM [erv].[tblTempEntityRecordVersion] a
			INNER JOIN map.tblEntityProductToSite b
			ON b.ProductGuid = a.MasterRecordGuid		
			AND b.SiteGuid = a.AssignedFromSiteGuid
			WHERE a._CallingReferenceGuid = @callingRefGuid
		END
		ELSE
		BEGIN
			UPDATE a 
			SET a.ParentRecordGuid = erv.udf_GetFirstParentRecordVersionGuid('Product', a.MasterRecordGuid, a.AssignedFromSiteGuid)
			FROM [erv].[tblTempEntityRecordVersion] a
			WHERE a._CallingReferenceGuid = @callingRefGuid	
		END

		--Delete the external attributes of the parent record version
		
		--Reset the Shared Mappings between Product and Company. This will delete, update, and clone the applicable Product-to-Company mappings as necessary.
		--The following Target fields and mapping tables are covered by this process: 
		--(Authorised Customers)->[map].[tblProductToCompany], 
		--(UnavailableInventories)->[map].[tblProductToUnavailableInventoryCompany], 
		--(SupplierAuthorizedProducts)->[map].[tblProductToSupplierProductCompany]
		DELETE @tblTargetProducts
		INSERT INTO @tblTargetProducts
		(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
		SELECT 'Product', EntityGuid, MasterRecordGuid, SiteGuid FROM [erv].[tblTempEntityRecordVersion] WHERE _CallingReferenceGuid = @callingRefGuid 

		EXEC [erv].[usp_SetProductToCompanyMappingsForDeletedProducts] @tblTargetProducts
		
		--Authorised Customers - ProductToCompanyGroup
		--CompanyGroup is both an External Attribute of Product (i.e. Product-To-CompanyGroup mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Product-To-CompanyGroup mappings are also maintained as part of the CompanyGroup entity, i.e. outside of the Product entity)
		--Reset the Product reference of those Product-to-CompanyGroup mappings that are tied to a local CompanyGroup to point to the Parent Product (so that the local CompanyGroup does not loose its Product mappings when Product RecordVersioning is turned off).
		UPDATE a
		SET a.ProductGuid = b.ParentRecordGuid
		FROM [map].[tblProductToCompanyGroup] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.ProductGuid
		INNER JOIN tblApplicationString c
		ON c.ApplicationStringGuid = a.AssignedToApplicationStringGuid
		WHERE c.SiteGuid = b.SiteGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Delete all the Product-to-CompanyGroup mappings of the Product child record version
		DELETE a FROM [map].[tblProductToCompanyGroup] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Assigned Messages - Regular Product Messages
		DELETE a FROM [map].[tblApplicationStringToProductMessage] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid
		AND b._CallingReferenceGuid = @callingRefGuid

		--Assigned Messages - DOT Hazardous Messages
		DELETE a FROM [map].[tblApplicationStringToDotHazardousMessage] a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.EntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid	
		AND b._CallingReferenceGuid = @callingRefGuid	

		--TransactionAliasExclusion
		--Reset the Shared Mappings between Product and TransactionAlias. This will delete, update, and clone the applicable Product-to-TransactionAlias mappings as necessary.
		--The following Target fields and mapping tables are covered by this process: 
		--(Products)->[map].[tblProductToTransactionAliasExclusion]
		DELETE @tblTargetProducts
		INSERT INTO @tblTargetProducts
		(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
		SELECT 'Product', EntityGuid, MasterRecordGuid, SiteGuid FROM [erv].[tblTempEntityRecordVersion] WHERE _CallingReferenceGuid = @callingRefGuid

		EXEC [erv].[usp_SetProductToTransactionAliasMappingsForDeletedProducts] @tblTargetProducts


		--Delete the child record versions
		DELETE a FROM tblProducts a
		INNER JOIN [erv].[tblTempEntityRecordVersion] b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.EntityGuid = a.ProductGuid
		WHERE b._CallingReferenceGuid = @callingRefGuid
		AND a.ProductGuid <> a._MasterRecordGuid

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
						+ 'Procedure Name: [erv].usp_DeleteProductChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     