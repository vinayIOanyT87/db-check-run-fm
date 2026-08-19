
/*
	DECLARE @tblTargetProducts erv.utt_EntityRecordVersions
	INSERT INTO @tblTargetProducts
	(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
	VALUES ('Product', 'BCAD83C8-CBCD-4A4A-8BBD-2CA14AD0E7A9', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Product', 'B98881AA-540D-4127-92F8-E4CC75586D0A', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Product', '6F5FEF48-72B2-4AE0-A1F0-C74296D78487', '5D108063-0B46-49DA-8DAE-C37C07804EA8', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Product', '31062FCF-ADDC-428B-860D-D185862E1E8E', '5D108063-0B46-49DA-8DAE-C37C07804EA8', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Product', 'EE7C5B83-39D7-4956-BFBF-45869B1B06C7', '80B08634-D356-4569-B9A2-CD36DF955BD0', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Product', 'F16C052E-2549-4B00-81EC-1AD7818F6A49', '80B08634-D356-4569-B9A2-CD36DF955BD0', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421')
	EXEC [erv].[usp_SetProductToTransactionAliasMappingsForDeletedProducts] @tblTargetProducts

	DECLARE @tblTargetProducts erv.utt_EntityRecordVersions
	INSERT INTO @tblTargetProducts
	(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
	VALUES ('Product', 'EEDD179A-3C23-4E95-8845-8CC93762E289', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', '46426312-E408-4AF8-85FD-338B622B32BF'),
	('Product', '2B8513FD-1041-409F-89E8-9554B4A3CA0F', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'B7BD440B-674F-46F6-977A-CEFC540B1A90'),
	('Product', '2238851C-3D9A-4D1B-A12F-8651172B264A', '5D108063-0B46-49DA-8DAE-C37C07804EA8', '46426312-E408-4AF8-85FD-338B622B32BF'),
	('Product', '91E94F2E-0032-4320-9CAF-406544768D26', '5D108063-0B46-49DA-8DAE-C37C07804EA8', 'B7BD440B-674F-46F6-977A-CEFC540B1A90'),
	('Product', '8A73B209-E500-4777-B1F7-AA7E4A9B6221', '80B08634-D356-4569-B9A2-CD36DF955BD0', '46426312-E408-4AF8-85FD-338B622B32BF'),
	('Product', '459CF841-B562-4B04-AE4D-9DADE4F606EA', '80B08634-D356-4569-B9A2-CD36DF955BD0', 'B7BD440B-674F-46F6-977A-CEFC540B1A90')
	EXEC [erv].[usp_SetProductToTransactionAliasMappingsForDeletedProducts] @tblTargetProducts

*/


CREATE PROCEDURE [erv].[usp_SetProductToTransactionAliasMappingsForDeletedProducts]
(
	@tblTargetProducts erv.utt_EntityRecordVersions READONLY
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_ProductToTransactionAliasMappingsForDeletedProducts] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete, Add, or Update the Product-To-TransactionAlias mappings to the support the deletions of a set of Product child record versions as a result of FLC configuration changes.	
	-- Notes:
	-- 1. @tblTargetProducts: Table containing the Product record versions whose deletion impact on the Product-to-TransactionAlias mappings need to be addressed.
	-- 2. This procedure addresses the Shared Mappings needs of the Product-to-TransactionAlias mappings when a Product record version is deleted.
	-- 3. This procedure is to be executed before the actual deletion of the Product child record versions.
	-- 4. This procedure assumes that the Products will still be mapped to the target site/sitegroup after the deletion, even though the Product child record version will be deleted, i.e.
	--    this procedure is not to be used in the case of Product-to-Site mapping deletions.
	-- 5. It handles the Product-to-TransactionAlias mappings managed by the following table: [map].[tblProductToTransactionAliasExclusion]
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @mappingCount int
		DECLARE @level int

		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		--For each Product that is mapped to a TransactionAlias record version owned by the same site/sitegroup as the Product owner site or by a lower site/sitegroup, retrieve the details of the TransactionAlias record version that is a parent to the TransactionAlias record version that is tied in the mapping.
		INSERT INTO erv.tblTempProdToTransactionAliasForParentTransactionAlias
		(ProductGuid, ProductMasterRecordGuid, TargetSiteGuid, TransactionAliasGuid, TransactionAliasMasterRecordGuid, TransactionAliasParentSiteGuid, ParentTransactionAliasGuid, ProductParentSiteGuid, IsMasterRecordTransactionAlias, TransactionAliasOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
		SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.AssignedToTransactionAliasGuid, c._MasterRecordGuid, e.SiteGuid, e.TransactionAliasGuid, g.AssignedFromSiteGuid, 0, 1, 0, @callingRef1Guid
		FROM @tblTargetProducts a
		INNER JOIN map.tblProductToTransactionAliasExclusion b
		ON b.ProductGuid = a.EntityGuid
		INNER JOIN tblTransactionAliases c
		ON c.TransactionAliasGuid = b.AssignedToTransactionAliasGuid  -- This covers both Transaction Aliases that are owned by the same site/sitegroup as the target Products, and those Transaction Aliases that are owned by a lower site/sitegroup.
		INNER JOIN map.tblEntityTransactionAliasToSite d
		ON d.TransactionAliasGuid = c._MasterRecordGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblTransactionAliases e
		ON e._MasterRecordGuid = c._MasterRecordGuid
		AND e.SiteGuid = d.AssignedFromSiteGuid  -- TransactionAliases that own the record at their own AssignedFrom sitegroup because those are the only ones that can maintain their own mappings.
		INNER JOIN tblProducts f
		ON f.ProductGuid = a.EntityGuid
		INNER JOIN map.tblEntityProductToSite g
		ON g.ProductGuid = f._MasterRecordGuid
		AND g.SiteGuid = f.SiteGuid
		WHERE f.ProductGuid <> f._MasterRecordGuid  --Operation limited to Product child record versions
		AND f.SiteGuid = a.SiteGuid
		AND a.EntityTypeId = 'Product'


		--Retrieve the first available parent Product record version applicable for all the Product records captured in erv.tblTempProdToTransactionAliasForParentProduct, starting from the ProductParentSiteGuid.
		--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it does not insert one record per parent, instead it just updates the AssignedFromSiteGuid and the EntityGuid of the initial record to reflect the parent record.
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		INSERT INTO erv.tblTempEntityMappingHierarchy
		(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
		SELECT a.ProductMasterRecordGuid, b.ProductGuid, a.ProductParentSiteGuid, 0, @callingRef2Guid
		FROM erv.tblTempProdToTransactionAliasForParentTransactionAlias a
		LEFT OUTER JOIN tblProducts b
		ON b._MasterRecordGuid = a.ProductMasterRecordGuid
		AND b.SiteGuid = a.ProductParentSiteGuid
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
			SET a.AssignedFromSiteGuid = b.SiteGuid, a.EntityGuid = c.ProductGuid
			FROM erv.tblTempEntityMappingHierarchy a
			INNER JOIN map.tblEntityProductToSite b
			ON b.ProductGuid = a.EntityMasterGuid
			AND b.SiteGuid = a.AssignedFromSiteGuid
			LEFT OUTER JOIN tblProducts c
			ON c._MasterRecordGuid = b.ProductGuid
			AND c.SiteGuid = b.SiteGuid
			WHERE a._CallingReferenceGuid = @callingRef2Guid
			AND a.EntityGuid IS NULL
		END								



		-- Retrieve the first available Product record applicable for the Product Parent Sitegroup. Note: Unlike with the Parent TransactionAlias record, the ProductGuidForParentTransactionAlias does not have to be owned by the parent sitegroup. It can be owned by any sitegroup further up the site hierarchy. 
		UPDATE a 
		SET a.ProductGuidForParentTransactionAlias = b.EntityGuid
		FROM erv.tblTempProdToTransactionAliasForParentTransactionAlias a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ProductMasterRecordGuid
		AND b.AssignedToSiteGuid = a.ProductParentSiteGuid
		WHERE a._CallingReferenceGuid = @callingRef1Guid
		AND b._CallingReferenceGuid = @callingRef2Guid

		DELETE erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef2Guid
		
		--Mark all TransactionAliases that have a master record at either the target (AssignedTo) sitegroup of the Product or lower, as a MasterRecordTransactionAlias
		UPDATE a 
		SET a.IsMasterRecordTransactionAlias = 1
		FROM erv.tblTempProdToTransactionAliasForParentTransactionAlias a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ProductMasterRecordGuid		
		WHERE a.TransactionAliasMasterRecordGuid = a.ParentTransactionAliasGuid
		AND a.TransactionAliasParentSiteGuid = b.AssignedToSiteGuid
		AND a._CallingReferenceGuid = @callingRef1Guid
		AND b._CallingReferenceGuid = @callingRef1Guid
		
		-- Retrieve the Forward Control Mode of the TransactionAlias field that is used to control the [map].[tblProductToTransactionAliasExclusion] from the TransactionAlias side
		UPDATE a 
		SET a.ProductFCM = b.ForwardControlMode
		FROM erv.tblTempProdToTransactionAliasForParentTransactionAlias a
		INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
		ON b.SiteGroupGuid = TransactionAliasParentSiteGuid
		INNER JOIN erv.tblEntitySegmentTemplate c
		ON c.EntitySegmentTemplateGuid = b.EntitySegmentTemplateGuid
		WHERE c.EntityTypeId = 'Transaction_Alias'
		AND TargetField = 'Product'
		AND a._CallingReferenceGuid = @callingRef1Guid

		UPDATE erv.tblTempProdToTransactionAliasForParentTransactionAlias
		SET ProductFCM = 'ParentSpecific'
		WHERE _CallingReferenceGuid = @callingRef1Guid
		AND ProductFCM IS NULL
		AND IsMasterRecordTransactionAlias <> 1

		--Product mappings with Transaction Aliases which do not have a record that is owned by their AssignedFrom sitegroup
		INSERT INTO erv.tblTempProdToTransactionAliasForParentTransactionAlias
		(ProductGuid, ProductMasterRecordGuid, TargetSiteGuid, TransactionAliasGuid, TransactionAliasMasterRecordGuid, IsMasterRecordTransactionAlias, TransactionAliasOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
		SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.AssignedToTransactionAliasGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
		FROM @tblTargetProducts a
		INNER JOIN map.tblProductToTransactionAliasExclusion b
		ON b.ProductGuid = a.EntityGuid
		INNER JOIN tblTransactionAliases c
		ON c.TransactionAliasGuid = b.AssignedToTransactionAliasGuid
		INNER JOIN map.tblEntityTransactionAliasToSite d
		ON d.TransactionAliasGuid = c._MasterRecordGuid
		AND d.SiteGuid = a.SiteGuid
		WHERE c.SiteGuid <> a.SiteGuid
		AND NOT EXISTS
		(
			SELECT * FROM tblTransactionAliases e
			WHERE e._MasterRecordGuid = c._MasterRecordGuid
			AND e.SiteGuid = d.AssignedFromSiteGuid
		)
		AND NOT EXISTS 
		(
			SELECT * FROM erv.tblTempProdToTransactionAliasForParentTransactionAlias f
			WHERE f.ProductGuid = a.EntityGuid
			AND f.TransactionAliasGuid = b.AssignedToTransactionAliasGuid
			AND f._CallingReferenceGuid = @callingRef1Guid
		)

		--Capture Product mappings with Transaction Aliases which are not even mapped to the target site (This can happen as a result of Record Versioning cloning. All Mappings are cloned, irrespective of whether the associated/opposite entity is mapped to the target site or not.)
		INSERT INTO erv.tblTempProdToTransactionAliasForParentTransactionAlias
		(ProductGuid, ProductMasterRecordGuid, TargetSiteGuid, TransactionAliasGuid, TransactionAliasMasterRecordGuid, IsMasterRecordTransactionAlias, TransactionAliasOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
		SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.AssignedToTransactionAliasGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
		FROM @tblTargetProducts a
		INNER JOIN map.tblProductToTransactionAliasExclusion b
		ON b.ProductGuid = a.EntityGuid
		INNER JOIN tblTransactionAliases c
		ON c.TransactionAliasGuid = b.AssignedToTransactionAliasGuid
		WHERE c.SiteGuid <> a.SiteGuid
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityTransactionAliasToSite d
			WHERE d.TransactionAliasGuid = c._MasterRecordGuid
			AND d.SiteGuid = a.SiteGuid
		)
		AND NOT EXISTS 
		(
			SELECT * FROM erv.tblTempProdToTransactionAliasForParentTransactionAlias e
			WHERE e.ProductGuid = a.EntityGuid
			AND e.TransactionAliasGuid = b.AssignedToTransactionAliasGuid
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
			--Delete all the mappings owned by the target Product child record versions if the FMC of theTransactionAlias.Product is 'ParentSpecific', i.e. if the corresponding TransactionAlias record in the mapping is not allowed to have its own version of the Product-to-TransactionAlias mappings.
			DELETE a 
			FROM map.tblProductToTransactionAliasExclusion a
			INNER JOIN erv.tblTempProdToTransactionAliasForParentTransactionAlias b
			ON b.ProductGuid = a.ProductGuid
			AND b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND ((b.ProductFCM = 'ParentSpecific') OR (TransactionAliasOwnsRecordAtAssignedFromSitegroup = 0))
			AND b.IsMasterRecordTransactionAlias <> 1
		
			-- If a TransactionAlias in the mappings owned by the target Product child record versions has a Parent TransactionAlias record which itself has a mapping with the Parent Product record, then clone that parent mapping for the child TransactionAlias record version associated with the target Product child record version.
			INSERT INTO [map].[tblProductToTransactionAliasExclusion]
			(ProductGuid, AssignedToTransactionAliasGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.ProductGuid, b.TransactionAliasGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, 
			a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM map.tblProductToTransactionAliasExclusion a 
			INNER JOIN erv.tblTempProdToTransactionAliasForParentTransactionAlias b
			ON b.ParentTransactionAliasGuid = a.AssignedToTransactionAliasGuid
			AND b.ProductGuidForParentTransactionAlias = a.ProductGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.TransactionAliasOwnsRecordAtAssignedFromSitegroup = 1
			AND b.ProductFCM = 'ParentSpecific'
			AND b.IsMasterRecordTransactionAlias <> 1
			AND b.ProductGuidForParentTransactionAlias IS NOT NULL
			AND NOT EXISTS
			(
				SELECT * FROM map.tblProductToTransactionAliasExclusion c
				WHERE c.ProductGuid = a.ProductGuid
				AND c.AssignedToTransactionAliasGuid = b.TransactionAliasGuid
			)

			-- If the corresponding TransactionAlias record in the target Product child record version mapping is allowed to have its own version of the Product-to-TransactionAlias mappings, then do not delete that mapping, but simply modify it to point to the Parent Product Guid, instead of the target Product child record version (that is marked for deletion).
			UPDATE a 
			SET a.ProductGuid = b.ProductGuidForParentTransactionAlias, a.UpdatedDate = GETDATE()
			FROM map.tblProductToTransactionAliasExclusion a
			INNER JOIN erv.tblTempProdToTransactionAliasForParentTransactionAlias b
			ON b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			AND b.ProductGuid = a.ProductGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.TransactionAliasOwnsRecordAtAssignedFromSitegroup = 1
			AND
			(
				(b.ProductFCM = 'VersionSpecific')
				OR 
				(b.IsMasterRecordTransactionAlias = 1)
			)
			AND b.ProductGuidForParentTransactionAlias IS NOT NULL

			--Clean up temporary data
			DELETE erv.tblTempProdToTransactionAliasForParentTransactionAlias WHERE _CallingReferenceGuid = @callingRef1Guid
			
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
						+ 'Procedure Name: [erv].usp_SetProductToTransactionAliasMappingsForDeletedProducts' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END    