
/*
	DECLARE @tblTargetTransactionAliases erv.utt_EntityRecordVersions
	INSERT INTO @tblTargetTransactionAliases
	(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
	VALUES ('Transaction_Alias', 'BCAD83C8-CBCD-4A4A-8BBD-2CA14AD0E7A9', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Transaction_Alias', 'B98881AA-540D-4127-92F8-E4CC75586D0A', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Transaction_Alias', '6F5FEF48-72B2-4AE0-A1F0-C74296D78487', '5D108063-0B46-49DA-8DAE-C37C07804EA8', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Transaction_Alias', '31062FCF-ADDC-428B-860D-D185862E1E8E', '5D108063-0B46-49DA-8DAE-C37C07804EA8', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Transaction_Alias', 'EE7C5B83-39D7-4956-BFBF-45869B1B06C7', '80B08634-D356-4569-B9A2-CD36DF955BD0', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Transaction_Alias', 'F16C052E-2549-4B00-81EC-1AD7818F6A49', '80B08634-D356-4569-B9A2-CD36DF955BD0', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421')
	EXEC [erv].[usp_SetProductToTransactionAliasMappingsForDeletedTransactionAliases] @tblTargetTransactionAliases


*/


CREATE PROCEDURE [erv].[usp_SetProductToTransactionAliasMappingsForDeletedTransactionAliases]
(
	@tblTargetTransactionAliases erv.utt_EntityRecordVersions READONLY
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_SetProductToTransactionAliasMappingsForDeletedTransactionAliases] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete, Add, or Update the Product-To-TransactionAlias mappings to the support the deletions of a set of TransactionAlias child record versions as a result of FLC configuration changes.	
	-- Notes:
	-- 1. @tblTargetTransactionAliases: Table containing the TransactionAlias record versions whose deletion impact on the Product-to-TransactionAlias mappings need to be addressed.
	-- 2. This procedure addresses the Shared Mappings needs of the Product-to-TransactionAlias mappings when a TransactionAlias record version is deleted.
	-- 3. This procedure is to be executed before the actual deletion of the TransactionAlias child record versions.
	-- 4. This procedure assumes that the Products will still be mapped to the target site/sitegroup after the deletion, even though the TransactionAlias child record version will be deleted, i.e.
	--    this procedure is not to be used in the case of Product-to-Site mapping deletions.
	-- 5. It handles the Product-to-TransactionAlias mappings managed by the following table: [map].[tblProductToTransactionAliasExclusion]
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @mappingCount int
		DECLARE @level int
		

		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()


		--For each TransactionAlias that is mapped to a Product record version owned by the same site/sitegroup as the TransactionAlias owner site or by a lower site/sitegroup, retrieve the details of the Product record version that is a parent to the Product record version that is tied in the mapping.		
		INSERT INTO erv.tblTempProdToTransactionAliasForParentProduct
		(TransactionAliasGuid, TransactionAliasMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, ProductParentSiteGuid, ParentProductGuid, TransactionAliasParentSiteGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
		SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, e.SiteGuid, e.ProductGuid, g.AssignedFromSiteGuid, 0, 1, 0, @callingRef1Guid
		FROM @tblTargetTransactionAliases a
		INNER JOIN map.tblProductToTransactionAliasExclusion b
		ON b.AssignedToTransactionAliasGuid = a.EntityGuid
		INNER JOIN tblProducts c
		ON c.ProductGuid = b.ProductGuid -- This covers both Products that are owned by the same site/sitegroup as the target Transaction Aliases, and those Products that are owned by a lower site/sitegroup.
		INNER JOIN map.tblEntityProductToSite d
		ON d.ProductGuid = c._MasterRecordGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts e
		ON e._MasterRecordGuid = c._MasterRecordGuid
		AND e.SiteGuid = d.AssignedFromSiteGuid  -- Products that own the record at their own AssignedFrom sitegroup because those are the only ones that can maintain their own mappings.
		INNER JOIN tblTransactionAliases f
		ON f.TransactionAliasGuid = a.EntityGuid
		INNER JOIN map.tblEntityTransactionAliasToSite g
		ON g.TransactionAliasGuid = f._MasterRecordGuid
		AND g.SiteGuid = f.SiteGuid
		WHERE f.TransactionAliasGuid <> f._MasterRecordGuid  --Operation limited to TransactionAlias child record versions
		AND f.SiteGuid = a.SiteGuid
		AND a.EntityTypeId = 'Transaction_Alias'


		--Retrieve the first available parent Transaction Alias record version applicable for all the TransactionAlias records captured in erv.tblTempProdToTransactionAliasForParentProduct, starting from the TransactionAliasParentSiteGuid.
		--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it does not insert one record per parent, instead it just updates the AssignedFromSiteGuid and the EntityGuid of the initial record to reflect the parent record.
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		INSERT INTO erv.tblTempEntityMappingHierarchy
		(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
		SELECT a.TransactionAliasMasterRecordGuid, b.TransactionAliasGuid, a.TransactionAliasParentSiteGuid, 0, @callingRef2Guid
		FROM erv.tblTempProdToTransactionAliasForParentProduct a
		LEFT OUTER JOIN tblTransactionAliases b
		ON b._MasterRecordGuid = a.TransactionAliasMasterRecordGuid
		AND b.SiteGuid = a.TransactionAliasParentSiteGuid
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
			SET a.AssignedFromSiteGuid = b.SiteGuid, a.EntityGuid = c.TransactionAliasGuid
			FROM erv.tblTempEntityMappingHierarchy a
			INNER JOIN map.tblEntityTransactionAliasToSite b
			ON b.TransactionAliasGuid = a.EntityMasterGuid
			AND b.SiteGuid = a.AssignedFromSiteGuid
			LEFT OUTER JOIN tblTransactionAliases c
			ON c._MasterRecordGuid = b.TransactionAliasGuid
			AND c.SiteGuid = b.SiteGuid
			WHERE a._CallingReferenceGuid = @callingRef2Guid
			AND a.EntityGuid IS NULL
		END								


		-- Retrieve the first available TransactionAlias record applicable for the TransactionAlias Parent Sitegroup. Note: Unlike with the Parent Product record, the TransactionAliasGuidForParentProduct does not have to be owned by the parent sitegroup. It can be owned by any sitegroup further up the site hierarchy. 
		UPDATE a 
		SET a.TransactionAliasGuidForParentProduct = b.EntityGuid
		FROM erv.tblTempProdToTransactionAliasForParentProduct a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.TransactionAliasMasterRecordGuid
		AND b.AssignedToSiteGuid = a.TransactionAliasParentSiteGuid
		WHERE a._CallingReferenceGuid = @callingRef1Guid
		AND b._CallingReferenceGuid = @callingRef2Guid

		DELETE erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef2Guid
		
		--Mark all Products that have a master record at either the target (AssignedTo) sitegroup of the TransactionAlias or lower, as a MasterRecordProduct
		UPDATE a 
		SET a.IsMasterRecordProduct = 1
		FROM erv.tblTempProdToTransactionAliasForParentProduct a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.TransactionAliasMasterRecordGuid
		WHERE a.ProductMasterRecordGuid = a.ParentProductGuid
		AND a.ProductParentSiteGuid = b.AssignedToSiteGuid
		AND a._CallingReferenceGuid = @callingRef1Guid
		AND b._CallingReferenceGuid = @callingRef1Guid

		-- Retrieve the Forward Control Mode of the Product field that is used to control the map.tblProductToTransactionAliasExclusion from the Product side
		UPDATE a 
		SET a.TransactionAliasExclusionFCM = b.ForwardControlMode
		FROM erv.tblTempProdToTransactionAliasForParentProduct a
		INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
		ON b.SiteGroupGuid = ProductParentSiteGuid
		INNER JOIN erv.tblEntitySegmentTemplate c
		ON c.EntitySegmentTemplateGuid = b.EntitySegmentTemplateGuid
		WHERE c.EntityTypeId = 'Product'
		AND b.TargetField = 'TransactionAliasExclusion'
		AND a._CallingReferenceGuid = @callingRef1Guid
		
		UPDATE erv.tblTempProdToTransactionAliasForParentProduct
		SET TransactionAliasExclusionFCM = 'ParentSpecific'
		WHERE _CallingReferenceGuid = @callingRef1Guid
		AND TransactionAliasExclusionFCM IS NULL
		AND IsMasterRecordProduct <> 1		


		--Capture the TransactionAlias mappings with Products which do not have a record that is owned by their AssignedFrom sitegroup
		INSERT INTO erv.tblTempProdToTransactionAliasForParentProduct
		(TransactionAliasGuid, TransactionaliasMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
		SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
		FROM @tblTargetTransactionAliases a
		INNER JOIN map.tblProductToTransactionAliasExclusion b
		ON b.AssignedToTransactionAliasGuid = a.EntityGuid
		INNER JOIN tblProducts c
		ON c.ProductGuid = b.ProductGuid
		INNER JOIN map.tblEntityProductToSite d
		ON d.ProductGuid = c._MasterRecordGuid
		AND d.SiteGuid = a.SiteGuid
		WHERE c.SiteGuid <> a.SiteGuid
		AND NOT EXISTS
		(
			SELECT * FROM tblProducts e
			WHERE e._MasterRecordGuid = c._MasterRecordGuid
			AND e.SiteGuid = d.AssignedFromSiteGuid
		)
		AND NOT EXISTS 
		(
			SELECT * FROM erv.tblTempProdToTransactionAliasForParentProduct f
			WHERE f.TransactionAliasGuid = a.EntityGuid
			AND f.ProductGuid = b.ProductGuid
			AND f._CallingReferenceGuid = @callingRef1Guid
		)

		--Capture Transaction Alias mappings with Products which are not even mapped to the target site (This can happen as a result of Record Versioning cloning. All Mappings are cloned, irrespective of whether the associated/opposite entity is mapped to the target site or not.)
		INSERT INTO erv.tblTempProdToTransactionAliasForParentProduct
		(TransactionAliasGuid, TransactionaliasMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
		SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
		FROM @tblTargetTransactionAliases a
		INNER JOIN map.tblProductToTransactionAliasExclusion b
		ON b.AssignedToTransactionAliasGuid = a.EntityGuid
		INNER JOIN tblProducts c
		ON c.ProductGuid = b.ProductGuid
		WHERE c.SiteGuid <> a.SiteGuid
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityProductToSite d
			WHERE d.ProductGuid = c._MasterRecordGuid
			AND d.SiteGuid = a.SiteGuid
		)
		AND NOT EXISTS 
		(
			SELECT * FROM erv.tblTempProdToTransactionAliasForParentProduct e
			WHERE e.TransactionAliasGuid = a.EntityGuid
			AND e.ProductGuid = b.ProductGuid
			AND e._CallingReferenceGuid = @callingRef1Guid
		)


		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION 
            SET @BeginTran = 1   
		END  
		BEGIN TRY

			------------------------------------------------------------[map].[tblProductToTransactionAliasExclusion]--------------------------------------------------------------------------------
			--Delete all the mappings owned by the target TransactionAlias child record versions if the FMC of the Product.ShipToAuthorizedProducts is 'ParentSpecific', i.e. if the corresponding Product record in the mapping is not allowed to have its own version of the Product-to-TransactionAlias mappings.
			DELETE a 
			FROM map.tblProductToTransactionAliasExclusion a
			INNER JOIN erv.tblTempProdToTransactionAliasForParentProduct b
			ON b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			AND b.ProductGuid = a.ProductGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND ((b.TransactionAliasExclusionFCM = 'ParentSpecific') OR (b.ProductOwnsRecordAtAssignedFromSitegroup = 0))
			AND b.IsMasterRecordProduct <> 1

		
			-- If a Product in the mappings owned by the target TransactionAlias child record versions has a Parent Product record which itself has a mapping with the Parent TransactionAlias record, then clone that parent mapping for the child Product record version associated with the target TransactionAlias child record version.
			INSERT INTO [map].[tblProductToTransactionAliasExclusion]
			(AssignedToTransactionAliasGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.AssignedToTransactionAliasGuid, b.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, 
			a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM map.tblProductToTransactionAliasExclusion a 
			INNER JOIN erv.tblTempProdToTransactionAliasForParentProduct b
			ON b.ParentProductGuid = a.ProductGuid
			AND b.TransactionAliasGuidForParentProduct = a.AssignedToTransactionAliasGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.ProductOwnsRecordAtAssignedFromSitegroup = 1
			AND b.TransactionAliasExclusionFCM = 'ParentSpecific'
			AND b.IsMasterRecordProduct <> 1
			AND b.TransactionAliasGuidForParentProduct IS NOT NULL
			AND NOT EXISTS
			(
				SELECT * FROM map.tblProductToTransactionAliasExclusion c
				WHERE c.AssignedToTransactionAliasGuid = a.AssignedToTransactionAliasGuid
				AND c.ProductGuid = b.ProductGuid
			)

			-- If the corresponding Product record in the target TransactionAlias child record version mapping is allowed to have its own version of the Product-to-TransactionAlias mappings, then do not delete that mapping, but simply modify it to point to the Parent TransactionAlias Guid, instead of the target TransactionAlias child record version (that is marked for deletion).
			UPDATE a 
			SET a.AssignedToTransactionAliasGuid = b.TransactionAliasGuidForParentProduct, a.UpdatedDate = GETDATE()
			FROM map.tblProductToTransactionAliasExclusion a
			INNER JOIN erv.tblTempProdToTransactionAliasForParentProduct b
			ON b.ProductGuid = a.ProductGuid
			AND b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.ProductOwnsRecordAtAssignedFromSitegroup = 1
			AND
			(
				(b.TransactionAliasExclusionFCM = 'VersionSpecific')
				OR 
				(b.IsMasterRecordProduct = 1)
			)
			AND b.TransactionAliasGuidForParentProduct IS NOT NULL
			
			
			--Clean up temporary data
			DELETE erv.tblTempProdToTransactionAliasForParentProduct WHERE _CallingReferenceGuid = @callingRef1Guid
			
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
						+ 'Procedure Name: [erv].usp_SetProductToTransactionAliasMappingsForDeletedTransactionAliases' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END    