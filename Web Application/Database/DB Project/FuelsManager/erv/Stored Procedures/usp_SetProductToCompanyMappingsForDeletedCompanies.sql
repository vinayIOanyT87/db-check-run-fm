
/*
	DECLARE @tblTargetCompanies erv.utt_EntityRecordVersions
	INSERT INTO @tblTargetCompanies
	(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid)
	VALUES ('Company', 'BCAD83C8-CBCD-4A4A-8BBD-2CA14AD0E7A9', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Company', 'B98881AA-540D-4127-92F8-E4CC75586D0A', 'B4E4B396-1366-4BEA-BDD6-D08F35863E87', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Company', '6F5FEF48-72B2-4AE0-A1F0-C74296D78487', '5D108063-0B46-49DA-8DAE-C37C07804EA8', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Company', '31062FCF-ADDC-428B-860D-D185862E1E8E', '5D108063-0B46-49DA-8DAE-C37C07804EA8', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'),
	('Company', 'EE7C5B83-39D7-4956-BFBF-45869B1B06C7', '80B08634-D356-4569-B9A2-CD36DF955BD0', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'),
	('Company', 'F16C052E-2549-4B00-81EC-1AD7818F6A49', '80B08634-D356-4569-B9A2-CD36DF955BD0', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421')
	EXEC [erv].[usp_SetProductToCompanyMappingsForDeletedCompanies] @tblTargetCompanies


*/


CREATE PROCEDURE [erv].[usp_SetProductToCompanyMappingsForDeletedCompanies]
(
	@tblTargetCompanies erv.utt_EntityRecordVersions READONLY
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_SetProductToCompanyMappingsForDeletedCompanies] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete, Add, or Update the Product-To-Company mappings to the support the deletions of a set of Company child record versions as a result of FLC configuration changes.	
	-- Notes:
	-- 1. @tblTargetCompanies: Table containing the Company record versions whose deletion impact on the Product-to-Company mappings need to be addressed.
	-- 2. This procedure addresses the Shared Mappings needs of the Product-to-Company mappings when a Company record version is deleted.
	-- 3. This procedure is to be executed before the actual deletion of the Company child record versions.
	-- 4. This procedure assumes that the Products will still be mapped to the target site/sitegroup after the deletion, even though the Company child record version will be deleted, i.e.
	--    this procedure is not to be used in the case of Product-to-Site mapping deletions.
	-- 5. It handles the Product-to-Company mappings managed by the following tables: map.tblProductToCompany, map.tblProductToUnavailableInventoryCompany, and map.tblProductToSupplierProductCompany
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

			------------------------------------------------------------map.tblProductToCompany--------------------------------------------------------------------------------
			--For each Company that is mapped to a Product record version owned by the same site/sitegroup as the Company owner site or by a lower site/sitegroup, retrieve the details of the Product record version that is a parent to the Product record version that is tied in the mapping.
			INSERT INTO erv.tblTempProdToCompanyForParentProduct
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, ProductParentSiteGuid, ParentProductGuid, CompanyParentSiteGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, e.SiteGuid, e.ProductGuid, g.AssignedFromSiteGuid, 0, 1, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblProductToCompany b
			ON b.AssignedToCompanyGuid = a.EntityGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = b.ProductGuid  -- This covers both Products that are owned by the same site/sitegroup as the target Companies, and those Products that are owned by a lower site/sitegroup.
			INNER JOIN map.tblEntityProductToSite d
			ON d.ProductGuid = c._MasterRecordGuid
			AND d.SiteGuid = c.SiteGuid
			INNER JOIN tblProducts e
			ON e._MasterRecordGuid = c._MasterRecordGuid
			AND e.SiteGuid = d.AssignedFromSiteGuid  -- Products that own the record at their own AssignedFrom sitegroup because those are the only ones that can maintain their own mappings.
			INNER JOIN tblCompanies f
			ON f.CompanyGuid = a.EntityGuid
			INNER JOIN map.tblEntityCompanyToSite g
			ON g.CompanyGuid = f._MasterRecordGuid
			AND g.SiteGuid = f.SiteGuid
			WHERE f.CompanyGuid <> f._MasterRecordGuid  --Operation limited to Company child record versions
			AND f.SiteGuid = a.SiteGuid
			AND a.EntityTypeId = 'Company'

			--Retrieve the first available parent Company record version applicable for all the Company records captured in erv.tblTempProdToCompanyForParentProduct, starting from the CompanyParentSiteGuid.
			--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it does not insert one record per parent, instead it just updates the AssignedFromSiteGuid and the EntityGuid of the initial record to reflect the parent record.
			DECLARE @callingRef2Guid uniqueidentifier
			SET @callingRef2Guid = NEWID()

			INSERT INTO erv.tblTempEntityMappingHierarchy
			(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
			SELECT a.CompanyMasterRecordGuid, b.CompanyGuid, a.CompanyParentSiteGuid, 0, @callingRef2Guid
			FROM erv.tblTempProdToCompanyForParentProduct a
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

			-- Retrieve the first available Company record applicable for the Company Parent Sitegroup. Note: Unlike with the Parent Product record, the CompanyGuidForParentProduct does not have to be owned by the parent sitegroup. It can be owned by any sitegroup further up the site hierarchy. 
			UPDATE a 
			SET a.CompanyGuidForParentProduct = b.EntityGuid
			FROM erv.tblTempProdToCompanyForParentProduct a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.CompanyMasterRecordGuid
			AND b.AssignedToSiteGuid = a.CompanyParentSiteGuid
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef2Guid

			DELETE erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef2Guid
			
			--Mark all Products that have a master record at either the target (AssignedTo) sitegroup of the Company or lower, as a MasterRecordProduct
			UPDATE a 
			SET a.IsMasterRecordProduct = 1
			FROM erv.tblTempProdToCompanyForParentProduct a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.CompanyMasterRecordGuid
			WHERE a.ProductMasterRecordGuid = a.ParentProductGuid
			AND a.ProductParentSiteGuid = b.AssignedToSiteGuid
			AND a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef1Guid

			-- Retrieve the Forward Control Mode of the Product field that is used to control the map.tblProductToCompany from the Product side
			UPDATE a 
			SET a.AuthorizedCustomersFCM = b.ForwardControlMode
			FROM erv.tblTempProdToCompanyForParentProduct a
			INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
			ON b.SiteGroupGuid = ProductParentSiteGuid
			INNER JOIN erv.tblEntitySegmentTemplate c
			ON c.EntitySegmentTemplateGuid = b.EntitySegmentTemplateGuid
			WHERE c.EntityTypeId = 'Product'
			AND b.TargetField = 'AuthorizedCustomers'
			AND a._CallingReferenceGuid = @callingRef1Guid

			UPDATE erv.tblTempProdToCompanyForParentProduct
			SET AuthorizedCustomersFCM = 'ParentSpecific'
			WHERE _CallingReferenceGuid = @callingRef1Guid
			AND AuthorizedCustomersFCM IS NULL
			AND IsMasterRecordProduct <> 1


			--Capture the Company mappings with Products which do not have a record that is owned by their AssignedFrom sitegroup
			INSERT INTO erv.tblTempProdToCompanyForParentProduct
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblProductToCompany b
			ON b.AssignedToCompanyGuid = a.EntityGuid
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
				SELECT * FROM erv.tblTempProdToCompanyForParentProduct f
				WHERE f.CompanyGuid = a.EntityGuid
				AND f.ProductGuid = b.ProductGuid
				AND f._CallingReferenceGuid = @callingRef1Guid
			)


			--Capture Company mappings with Products which are not even mapped to the target site (This can happen as a result of Record Versioning cloning. All Mappings are cloned, irrespective of whether the associated/opposite entity is mapped to the target site or not.)
			INSERT INTO erv.tblTempProdToCompanyForParentProduct
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblProductToCompany b
			ON b.AssignedToCompanyGuid = a.EntityGuid
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
			SELECT * FROM erv.tblTempProdToCompanyForParentProduct e
			WHERE e.CompanyGuid = a.EntityGuid
			AND e.ProductGuid = b.ProductGuid
			AND e._CallingReferenceGuid = @callingRef1Guid
		)


			--Delete all the mappings owned by the target Company child record versions if the FMC of the Product.ShipToAuthorizedProducts is 'ParentSpecific', i.e. if the corresponding Product record in the mapping is not allowed to have its own version of the Product-to-Company mappings.
			DELETE a 
			FROM map.tblProductToCompany a
			INNER JOIN erv.tblTempProdToCompanyForParentProduct b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			AND b.ProductGuid = a.ProductGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND ((b.AuthorizedCustomersFCM = 'ParentSpecific') OR (b.ProductOwnsRecordAtAssignedFromSitegroup = 0))
			AND b.IsMasterRecordProduct <> 1
		
			-- If a Product in the mappings owned by the target Company child record versions has a Parent Product record which itself has a mapping with the Parent Company record, then clone that parent mapping for the child Product record version associated with the target Company child record version.
			INSERT INTO map.tblProductToCompany 
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.AssignedToCompanyGuid, b.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
			a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote, GETDATE(), a.CreatedBy, 
			GETDATE(), a.UpdatedBy
			FROM map.tblProductToCompany a 
			INNER JOIN erv.tblTempProdToCompanyForParentProduct b
			ON b.ParentProductGuid = a.ProductGuid
			AND b.CompanyGuidForParentProduct = a.AssignedToCompanyGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.ProductOwnsRecordAtAssignedFromSitegroup = 1
			AND b.AuthorizedCustomersFCM = 'ParentSpecific'
			AND b.IsMasterRecordProduct <> 1
			AND b.CompanyGuidForParentProduct IS NOT NULL
			AND NOT EXISTS
			(
				SELECT * FROM map.tblProductToCompany c
				WHERE c.AssignedToCompanyGuid = a.AssignedToCompanyGuid
				AND c.ProductGuid = b.ProductGuid
			)

			-- If the corresponding Product record in the target Company child record version mapping is allowed to have its own version of the Product-to-Company mappings, then do not delete that mapping, but simply modify it to point to the Parent Company Guid, instead of the target Company child record version (that is marked for deletion).
			UPDATE a 
			SET a.AssignedToCompanyGuid = b.CompanyGuidForParentProduct, a.UpdatedDate = GETDATE()
			FROM map.tblProductToCompany a
			INNER JOIN erv.tblTempProdToCompanyForParentProduct b
			ON b.ProductGuid = a.ProductGuid
			AND b.CompanyGuid = a.AssignedToCompanyGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.ProductOwnsRecordAtAssignedFromSitegroup = 1
			AND
			(
				(b.AuthorizedCustomersFCM = 'VersionSpecific')
				OR 
				(b.IsMasterRecordProduct = 1)
			)
			AND b.CompanyGuidForParentProduct IS NOT NULL

			
			------------------------------------------------------------map.tblProductToUnavailableInventoryCompany--------------------------------------------------------------------------------
		
			DELETE erv.tblTempProdToCompanyForParentProduct WHERE _CallingReferenceGuid = @callingRef1Guid

			--For each Company that is mapped to a Product record version owned by the same site/sitegroup as the Company owner site or by a lower site/sitegroup, retrieve the details of the Product record version that is a parent to the Product record version that is tied in the mapping.
			INSERT INTO erv.tblTempProdToCompanyForParentProduct
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, ProductParentSiteGuid, ParentProductGuid, CompanyParentSiteGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, e.SiteGuid, e.ProductGuid, g.AssignedFromSiteGuid, 0, 1, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblProductToUnavailableInventoryCompany b
			ON b.AssignedToCompanyGuid = a.EntityGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = b.ProductGuid -- This covers both Products that are owned by the same site/sitegroup as the target Companies, and those Products that are owned by a lower site/sitegroup.
			INNER JOIN map.tblEntityProductToSite d
			ON d.ProductGuid = c._MasterRecordGuid
			AND d.SiteGuid = c.SiteGuid
			INNER JOIN tblProducts e
			ON e._MasterRecordGuid = c._MasterRecordGuid
			AND e.SiteGuid = d.AssignedFromSiteGuid  -- Products that own the record at their own AssignedFrom sitegroup because those are the only ones that can maintain their own mappings.
			INNER JOIN tblCompanies f
			ON f.CompanyGuid = a.EntityGuid
			INNER JOIN map.tblEntityCompanyToSite g
			ON g.CompanyGuid = f._MasterRecordGuid
			AND g.SiteGuid = f.SiteGuid
			WHERE f.CompanyGuid <> f._MasterRecordGuid  --Operation limited to Company child record versions
			AND f.SiteGuid = a.SiteGuid
			AND a.EntityTypeId = 'Company'


			--Retrieve the first available parent Company record version applicable for all the Company records captured in erv.tblTempProdToCompanyForParentProduct, starting from the CompanyParentSiteGuid.
			--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it does not insert one record per parent, instead it just updates the AssignedFromSiteGuid and the EntityGuid of the initial record to reflect the parent record.
			INSERT INTO erv.tblTempEntityMappingHierarchy
			(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
			SELECT a.CompanyMasterRecordGuid, b.CompanyGuid, a.CompanyParentSiteGuid, 0, @callingRef2Guid
			FROM erv.tblTempProdToCompanyForParentProduct a
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

			-- Retrieve the first available Company record applicable for the Company Parent Sitegroup. Note: Unlike with the Parent Product record, the CompanyGuidForParentProduct does not have to be owned by the parent sitegroup. It can be owned by any sitegroup further up the site hierarchy. 
			UPDATE a 
			SET a.CompanyGuidForParentProduct = b.EntityGuid
			FROM erv.tblTempProdToCompanyForParentProduct a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.CompanyMasterRecordGuid
			AND b.AssignedToSiteGuid = a.CompanyParentSiteGuid
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef2Guid

			DELETE erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef2Guid
		
			--Mark all Products that have a master record at either the target (AssignedTo) sitegroup of the Company or lower, as a MasterRecordProduct
			UPDATE a 
			SET a.IsMasterRecordProduct = 1
			FROM erv.tblTempProdToCompanyForParentProduct a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.CompanyMasterRecordGuid
			WHERE a.ProductMasterRecordGuid = a.ParentProductGuid
			AND a.ProductParentSiteGuid = b.AssignedToSiteGuid
			AND a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef1Guid

			-- Retrieve the Forward Control Mode of the Product field that is used to control the map.tblProductToUnavailableInventoryCompany from the Product side
			UPDATE a 
			SET a.UnavailableInventoriesFCM = b.ForwardControlMode
			FROM erv.tblTempProdToCompanyForParentProduct a
			INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
			ON b.SiteGroupGuid = ProductParentSiteGuid
			INNER JOIN erv.tblEntitySegmentTemplate c
			ON c.EntitySegmentTemplateGuid = b.EntitySegmentTemplateGuid
			WHERE c.EntityTypeId = 'Product'
			AND TargetField = 'UnavailableInventories'
			AND a._CallingReferenceGuid = @callingRef1Guid

			UPDATE erv.tblTempProdToCompanyForParentProduct
			SET UnavailableInventoriesFCM = 'ParentSpecific'
			WHERE _CallingReferenceGuid = @callingRef1Guid
			AND UnavailableInventoriesFCM IS NULL
			AND IsMasterRecordProduct <> 1		

			--Capture the Company mappings with Products which do not have a record that is owned by their AssignedFrom sitegroup
			INSERT INTO erv.tblTempProdToCompanyForParentProduct
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblProductToUnavailableInventoryCompany b
			ON b.AssignedToCompanyGuid = a.EntityGuid
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
				SELECT * FROM erv.tblTempProdToCompanyForParentProduct f
				WHERE f.CompanyGuid = a.EntityGuid
				AND f.ProductGuid = b.ProductGuid
				AND f._CallingReferenceGuid = @callingRef1Guid
			)

			--Capture Company mappings with Products which are not even mapped to the target site (This can happen as a result of Record Versioning cloning. All Mappings are cloned, irrespective of whether the associated/opposite entity is mapped to the target site or not.)
			INSERT INTO erv.tblTempProdToCompanyForParentProduct
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblProductToUnavailableInventoryCompany b
			ON b.AssignedToCompanyGuid = a.EntityGuid
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
				SELECT * FROM erv.tblTempProdToCompanyForParentProduct e
				WHERE e.CompanyGuid = a.EntityGuid
				AND e.ProductGuid = b.ProductGuid
				AND e._CallingReferenceGuid = @callingRef1Guid
			)


			--Delete all the mappings owned by the target Company child record versions if the FMC of the Company.UnavailableInventoriesFCM is 'ParentSpecific', i.e. if the corresponding Product record in the mapping is not allowed to have its own version of the Product-to-Company mappings.
			DELETE a 
			FROM map.tblProductToUnavailableInventoryCompany a
			INNER JOIN erv.tblTempProdToCompanyForParentProduct b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			AND b.ProductGuid = a.ProductGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND ((b.UnavailableInventoriesFCM = 'ParentSpecific') OR (b.ProductOwnsRecordAtAssignedFromSitegroup = 0))
			AND b.IsMasterRecordProduct <> 1
		
			-- If a Product in the mappings owned by the target Company child record versions has a Parent Product record which itself has a mapping with the Parent Company record, then clone that parent mapping for the child Product record version associated with the target Company child record version.
			INSERT INTO map.tblProductToUnavailableInventoryCompany 
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.AssignedToCompanyGuid, b.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
			a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, GETDATE(), a.CreatedBy, 
			GETDATE(), a.UpdatedBy
			FROM map.tblProductToUnavailableInventoryCompany a 
			INNER JOIN erv.tblTempProdToCompanyForParentProduct b
			ON b.ParentProductGuid = a.ProductGuid
			AND b.CompanyGuidForParentProduct = a.AssignedToCompanyGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.ProductOwnsRecordAtAssignedFromSitegroup = 1
			AND b.UnavailableInventoriesFCM = 'ParentSpecific'
			AND b.IsMasterRecordProduct <> 1
			AND b.CompanyGuidForParentProduct IS NOT NULL
			AND NOT EXISTS
			(
				SELECT * FROM map.tblProductToUnavailableInventoryCompany c
				WHERE c.AssignedToCompanyGuid = a.AssignedToCompanyGuid
				AND c.ProductGuid = b.ProductGuid
			)

			-- If the corresponding Product record in the target Company child record version mapping is allowed to have its own version of the Product-to-Company mappings, then do not delete that mapping, but simply modify it to point to the Parent Company Guid, instead of the target Company child record version (that is marked for deletion).
			UPDATE a 
			SET a.AssignedToCompanyGuid = b.CompanyGuidForParentProduct, a.UpdatedDate = GETDATE()
			FROM map.tblProductToUnavailableInventoryCompany a
			INNER JOIN erv.tblTempProdToCompanyForParentProduct b
			ON b.ProductGuid = a.ProductGuid
			AND b.CompanyGuid = a.AssignedToCompanyGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.ProductOwnsRecordAtAssignedFromSitegroup = 1
			AND
			(
				(b.UnavailableInventoriesFCM = 'VersionSpecific')
				OR 
				(b.IsMasterRecordProduct = 1)
			)
			AND b.CompanyGuidForParentProduct IS NOT NULL


			------------------------------------------------------------map.tblProductToSupplierProductCompany--------------------------------------------------------------------------------

			DELETE erv.tblTempProdToCompanyForParentProduct WHERE _CallingReferenceGuid = @callingRef1Guid

			--For each Company that is mapped to a Product record version owned by the same site/sitegroup as the Company owner site or by a lower site/sitegroup, retrieve the details of the Product record version that is a parent to the Product record version that is tied in the mapping.
			INSERT INTO erv.tblTempProdToCompanyForParentProduct
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, ProductParentSiteGuid, ParentProductGuid, CompanyParentSiteGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, e.SiteGuid, e.ProductGuid, g.AssignedFromSiteGuid, 0, 1, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblProductToSupplierProductCompany b
			ON b.AssignedToCompanyGuid = a.EntityGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = b.ProductGuid -- This covers both Products that are owned by the same site/sitegroup as the target Companies, and those Products that are owned by a lower site/sitegroup.
			INNER JOIN map.tblEntityProductToSite d
			ON d.ProductGuid = c._MasterRecordGuid
			AND d.SiteGuid = c.SiteGuid
			INNER JOIN tblProducts e
			ON e._MasterRecordGuid = c._MasterRecordGuid
			AND e.SiteGuid = d.AssignedFromSiteGuid  -- Products that own the record at their own AssignedFrom sitegroup because those are the only ones that can maintain their own mappings.
			INNER JOIN tblCompanies f
			ON f.CompanyGuid = a.EntityGuid
			INNER JOIN map.tblEntityCompanyToSite g
			ON g.CompanyGuid = f._MasterRecordGuid
			AND g.SiteGuid = f.SiteGuid
			WHERE f.CompanyGuid <> f._MasterRecordGuid  --Operation limited to Company child record versions
			AND f.SiteGuid = a.SiteGuid
			AND a.EntityTypeId = 'Company'

			--Retrieve the first available parent Company record version applicable for all the Company records captured in erv.tblTempProdToCompanyForParentProduct, starting from the CompanyParentSiteGuid.
			--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it does not insert one record per parent, instead it just updates the AssignedFromSiteGuid and the EntityGuid of the initial record to reflect the parent record.
			INSERT INTO erv.tblTempEntityMappingHierarchy
			(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
			SELECT a.CompanyMasterRecordGuid, b.CompanyGuid, a.CompanyParentSiteGuid, 0, @callingRef2Guid
			FROM erv.tblTempProdToCompanyForParentProduct a
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

			-- Retrieve the first available Company record applicable for the Company Parent Sitegroup. Note: Unlike with the Parent Product record, the CompanyGuidForParentProduct does not have to be owned by the parent sitegroup. It can be owned by any sitegroup further up the site hierarchy. 
			UPDATE a 
			SET a.CompanyGuidForParentProduct = b.EntityGuid
			FROM erv.tblTempProdToCompanyForParentProduct a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.CompanyMasterRecordGuid
			AND b.AssignedToSiteGuid = a.CompanyParentSiteGuid
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef2Guid

			DELETE erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef2Guid

		
			--Mark all Products that have a master record at either the target (AssignedTo) sitegroup of the Company or lower, as a MasterRecordProduct
			UPDATE a 
			SET a.IsMasterRecordProduct = 1
			FROM erv.tblTempProdToCompanyForParentProduct a
			INNER JOIN erv.tblTempEntityMappingHierarchy b
			ON b.EntityMasterGuid = a.CompanyMasterRecordGuid
			WHERE a.ProductMasterRecordGuid = a.ParentProductGuid
			AND a.ProductParentSiteGuid = b.AssignedToSiteGuid
			AND a._CallingReferenceGuid = @callingRef1Guid
			AND b._CallingReferenceGuid = @callingRef1Guid

			-- Retrieve the Forward Control Mode of the Product field that is used to control the map.tblProductToSupplierProductCompany from the Product side
			UPDATE a 
			SET a.SupplierAuthorizedProductsFCM = b.ForwardControlMode
			FROM erv.tblTempProdToCompanyForParentProduct a
			INNER JOIN erv.tblEntityRecordVersioningFieldConfig b
			ON b.SiteGroupGuid = ProductParentSiteGuid
			INNER JOIN erv.tblEntitySegmentTemplate c
			ON c.EntitySegmentTemplateGuid = b.EntitySegmentTemplateGuid
			WHERE c.EntityTypeId = 'Product'
			AND b.TargetField = 'SupplierAuthorizedProducts'
			AND a._CallingReferenceGuid = @callingRef1Guid

			UPDATE erv.tblTempProdToCompanyForParentProduct
			SET SupplierAuthorizedProductsFCM = 'ParentSpecific'
			WHERE _CallingReferenceGuid = @callingRef1Guid
			AND SupplierAuthorizedProductsFCM IS NULL
			AND IsMasterRecordProduct <> 1


			--Capture the Company mappings with Products which do not have a record that is owned by their AssignedFrom sitegroup
			INSERT INTO erv.tblTempProdToCompanyForParentProduct
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblProductToSupplierProductCompany b
			ON b.AssignedToCompanyGuid = a.EntityGuid
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
				SELECT * FROM erv.tblTempProdToCompanyForParentProduct f
				WHERE f.CompanyGuid = a.EntityGuid
				AND f.ProductGuid = b.ProductGuid
				AND f._CallingReferenceGuid = @callingRef1Guid
			)

			--Capture Company mappings with Products which are not even mapped to the target site (This can happen as a result of Record Versioning cloning. All Mappings are cloned, irrespective of whether the associated/opposite entity is mapped to the target site or not.)
			INSERT INTO erv.tblTempProdToCompanyForParentProduct
			(CompanyGuid, CompanyMasterRecordGuid, TargetSiteGuid, ProductGuid, ProductMasterRecordGuid, IsMasterRecordProduct, ProductOwnsRecordAtAssignedFromSitegroup, Processed, _CallingReferenceGuid)
			SELECT a.EntityGuid, a.MasterRecordGuid, a.SiteGuid, b.ProductGuid, c._MasterRecordGuid, 0, 0, 0, @callingRef1Guid
			FROM @tblTargetCompanies a
			INNER JOIN map.tblProductToSupplierProductCompany b
			ON b.AssignedToCompanyGuid = a.EntityGuid
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
				SELECT * FROM erv.tblTempProdToCompanyForParentProduct e
				WHERE e.CompanyGuid = a.EntityGuid
				AND e.ProductGuid = b.ProductGuid
				AND e._CallingReferenceGuid = @callingRef1Guid
			)

			--Delete all the mappings owned by the target Company child record versions if the FMC of the Product.SupplierAuthorizedProductsFCM is 'ParentSpecific', i.e. if the corresponding Product record in the mapping is not allowed to have its own version of the Product-to-Company mappings.
			DELETE a 
			FROM map.tblProductToSupplierProductCompany a
			INNER JOIN erv.tblTempProdToCompanyForParentProduct b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			AND b.ProductGuid = a.ProductGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND ((b.SupplierAuthorizedProductsFCM = 'ParentSpecific') OR (b.ProductOwnsRecordAtAssignedFromSitegroup = 0))
			AND b.IsMasterRecordProduct <> 1
		
			-- If a Product in the mappings owned by the target Company child record versions has a Parent Product record which itself has a mapping with the Parent Company record, then clone that parent mapping for the child Product record version associated with the target Company child record version.
			INSERT INTO map.tblProductToSupplierProductCompany 
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.AssignedToCompanyGuid, b.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
			a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, GETDATE(), a.CreatedBy, 
			GETDATE(), a.UpdatedBy
			FROM map.tblProductToSupplierProductCompany a 
			INNER JOIN erv.tblTempProdToCompanyForParentProduct b
			ON b.ParentProductGuid = a.ProductGuid
			AND b.CompanyGuidForParentProduct = a.AssignedToCompanyGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.ProductOwnsRecordAtAssignedFromSitegroup = 1
			AND b.SupplierAuthorizedProductsFCM = 'ParentSpecific'
			AND b.IsMasterRecordProduct <> 1
			AND b.CompanyGuidForParentProduct IS NOT NULL
			AND NOT EXISTS
			(
				SELECT * FROM map.tblProductToSupplierProductCompany c
				WHERE c.AssignedToCompanyGuid = a.AssignedToCompanyGuid
				AND c.ProductGuid = b.ProductGuid
			)

			-- If the corresponding Product record in the target Company child record version mapping is allowed to have its own version of the Product-to-Company mappings, then do not delete that mapping, but simply modify it to point to the Parent Company Guid, instead of the target Company child record version (that is marked for deletion).
			UPDATE a 
			SET a.AssignedToCompanyGuid = b.CompanyGuidForParentProduct, a.UpdatedDate = GETDATE()
			FROM map.tblProductToSupplierProductCompany a
			INNER JOIN erv.tblTempProdToCompanyForParentProduct b
			ON b.ProductGuid = a.ProductGuid
			AND b.CompanyGuid = a.AssignedToCompanyGuid
			WHERE b._CallingReferenceGuid = @callingRef1Guid
			AND b.ProductOwnsRecordAtAssignedFromSitegroup = 1
			AND
			(
				(b.SupplierAuthorizedProductsFCM = 'VersionSpecific')
				OR 
				(b.IsMasterRecordProduct = 1)
			)
			AND b.CompanyGuidForParentProduct IS NOT NULL

			DELETE erv.tblTempProdToCompanyForParentProduct WHERE _CallingReferenceGuid = @callingRef1Guid
			
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
						+ 'Procedure Name: [erv].usp_SetProductToCompanyMappingsForDeletedCompanies' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
