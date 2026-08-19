/*
	DROP PROCEDURE [erv].[usp_CreateCompanyChildRecordVersionBySegment]

	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	EXEC [erv].[usp_CreateCompanyChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '00000000-0000-0000-0000-000000000001', @dt, 'HB'
	--EXEC [erv].[usp_CreateCompanyChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', @dt, 'HB'

*/

CREATE PROCEDURE [erv].[usp_CreateCompanyChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreateCompanyChildRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new Company record version for each of the existing entity assignments of a given Company segment from a given SiteGroup.
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template that needs to be processed.
	-- 2. @SourceSiteGroupGuid: SiteGroup parent from which the record version are to be created. This would correspond to the AssignedFrom Sitegroup.
	-- 3. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		--Verify if RecordVersioning is turned ON for the source sitegroup before creating new child record versions from it.	
		DECLARE @tblVersionSpecificFields TABLE
		(
			TargetField nvarchar(100) NOT NULL,
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL
		);
		INSERT @tblVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		EXEC erv.usp_GetVersionSpecificFieldsBySegment @EntitySegmentTemplateGuid, NULL, @SourceSiteGroupGuid

		IF ((SELECT COUNT(*) FROM @tblVersionSpecificFields) = 0)
		BEGIN
			RETURN
		END


		--Capture the Site/SiteGroup, MasterRecordGuid, and the parent record versions for the entity assignments from which new record versions need to be created/cloned.
		DECLARE @tblTargetEntitySite TABLE
		(
			SiteGuid uniqueidentifier,
			MasterRecordGuid uniqueidentifier,
			ParentEntityGuid uniqueidentifier,
			CompanyGuid uniqueidentifier  -- The child record version CompanyGuid is not initially available since the process will be creating the new Company child record versions, but it is populated and used further down the process when handling the external attributes.
		)

		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@entityTypeId = 'Company')
		BEGIN
			INSERT INTO @tblTargetEntitySite
			(SiteGuid, MasterRecordGuid, ParentEntityGuid)
			SELECT b.SiteGuid, b.CompanyGuid, a.CompanyGuid
			FROM tblCompanies a
			INNER JOIN map.tblEntityCompanyToSite b
			ON b.CompanyGuid = a._MasterRecordGuid
			AND b.AssignedFromSiteGuid = a.SiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about creating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no need to create a child record version in any case.
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND NOT EXISTS
			(SELECT * FROM tblCompanies c
			WHERE c._MasterRecordGuid = a._MasterRecordGuid
			AND c.SiteGuid = b.SiteGuid)
			AND b.SiteGuid <> b.AssignedFromSiteGuid
		END
				

		--Create the child record versions by cloning the internal fields of the parent record version
		INSERT INTO tblCompanies
		(ID,SiteGuid, _MasterRecordGuid,Code,Name,Address1,Address2,City,State,Zip,Country,Phone,FAX,EmergencyContact,EmergencyPhone,FlightPrefix,EffectiveDate,ExpirationDate,OnHold,PickupFLights,StockTrack,SufferLossGain,LowStockWarning,LockedOut,LockedOutReason,LockedOutDate,ReceivableAccount,RefinerCode,LastActivityDate,CreditOK,AdditiveAccounting,PurchaseOrderRequired,EPANumber,FederalID,TaxNumber,FlushPermitted,PumpOffPermitted,DeliveryToTerminalPermitted,LicenseNumber,LicenseExpiration,InsuranceCompany,InsurancePolicy,LiabilityAmount,HazardousMaterialExclusion,InsuranceExpiration,AllowDriverEntry,PINRequired,MaximumVehicleWeight,WeightUnits,AccountNumber,SCACCode,DisableOwnerAllocationsCheck,DisableShipperAllocationsCheck,DisableBillToAllocationsCheck,DisableShipToAllocationsCheck,LoadRackDisplayText,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IATAGuid,ShipperTypeApplicationStringGuid,CustomerBillToTypeApplicationStringGuid,CustomerShipToTypeApplicationStringGuid,Contact1Name,Contact1Address1,Contact1Address2,Contact1City,Contact1State,Contact1Zip,Contact1Country,Contact1PhoneOffice,Contact1Fax,Contact1EmailAddress,Contact2Name,Contact2Address1,Contact2Address2,Contact2City,Contact2State,Contact2Zip,Contact2Country,Contact2PhoneOffice,Contact2Fax,Contact2EmailAddress,Contact1PhoneMobile,Contact2PhoneMobile,Note,HiddenDate,ConsortiumTypeIndex,FederalID2,FederalID3,FederalID4,FederalID5,ScullyRequired,ShortName,StateID)
		SELECT a.ID,b.SiteGuid,a._MasterRecordGuid,a.Code,a.Name,a.Address1,a.Address2,a.City,a.State,a.Zip,a.Country,a.Phone,a.FAX,a.EmergencyContact,a.EmergencyPhone,a.FlightPrefix,a.EffectiveDate,a.ExpirationDate,a.OnHold,a.PickupFLights,a.StockTrack,a.SufferLossGain,a.LowStockWarning,a.LockedOut,a.LockedOutReason,a.LockedOutDate,a.ReceivableAccount,a.RefinerCode,a.LastActivityDate,a.CreditOK,a.AdditiveAccounting,a.PurchaseOrderRequired,a.EPANumber,a.FederalID,a.TaxNumber,a.FlushPermitted,a.PumpOffPermitted,a.DeliveryToTerminalPermitted,a.LicenseNumber,a.LicenseExpiration,a.InsuranceCompany,a.InsurancePolicy,a.LiabilityAmount,a.HazardousMaterialExclusion,a.InsuranceExpiration,a.AllowDriverEntry,a.PINRequired,a.MaximumVehicleWeight,a.WeightUnits,a.AccountNumber,a.SCACCode,a.DisableOwnerAllocationsCheck,a.DisableShipperAllocationsCheck,a.DisableBillToAllocationsCheck,a.DisableShipToAllocationsCheck,a.LoadRackDisplayText,a.UserData1,a.UserData2,a.UserData3,a.UserData4,a.UserData5,a.UserData6,a.UserData7,a.UserData8,@CreatedBy,@CreatedDate,@CreatedBy,@CreatedDate,a.IATAGuid,a.ShipperTypeApplicationStringGuid,a.CustomerBillToTypeApplicationStringGuid,a.CustomerShipToTypeApplicationStringGuid,a.Contact1Name,a.Contact1Address1,a.Contact1Address2,a.Contact1City,a.Contact1State,a.Contact1Zip,a.Contact1Country,a.Contact1PhoneOffice,a.Contact1Fax,a.Contact1EmailAddress,a.Contact2Name,a.Contact2Address1,a.Contact2Address2,a.Contact2City,a.Contact2State,a.Contact2Zip,a.Contact2Country,a.Contact2PhoneOffice,a.Contact2Fax,a.Contact2EmailAddress,a.Contact1PhoneMobile,a.Contact2PhoneMobile,a.Note,a.HiddenDate,a.ConsortiumTypeIndex,a.FederalID2,a.FederalID3,a.FederalID4,a.FederalID5,a.ScullyRequired,a.ShortName,a.StateID
		FROM tblCompanies a
		INNER JOIN @tblTargetEntitySite b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.ParentEntityGuid = a.CompanyGuid


		--Clone the external attributes of the parent record version
		--Retrieve the first available Company record version applicable for all Company mappings to @SourceSiteGroupGuid
		--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it just updates the AssignedFromSiteGuid and the EntityGuid of the initial mapping record to reflect the actual parent record.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempEntityMappingHierarchy
		(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
		SELECT a.CompanyGuid, b.CompanyGuid, a.SiteGuid, 0, @callingRef1Guid
		FROM map.tblEntityCompanyToSite a
		LEFT OUTER JOIN tblCompanies b
		ON b._MasterRecordGuid = a.CompanyGuid
		AND b.SiteGuid = a.SiteGuid
		WHERE a.SiteGuid = @SourceSiteGroupGuid

		DECLARE @level int
		SET @level = 0

		WHILE ((SELECT COUNT(*) FROM erv.tblTempEntityMappingHierarchy WHERE _CallingReferenceGuid = @callingRef1Guid AND EntityGuid IS NULL) > 0)
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
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND a.EntityGuid IS NULL
		END				

		--Equipments. 
		--The relationship between tblCompanies and tblEquipment is maintained fully on the tblEquipment side, which references the Company using the Company MasterRecordGuid. 
		--Therefore newly created Company child record versions are automatically going to inherit the applicable tblCompanies-tblEquipment relationships
		--and there are no actions to be taken for the new Company child record versions as far as the tblCompanies-tblEquipment relationships are concerned.
		
		----AuthorizedShipTo
		UPDATE a 
		SET a.CompanyGuid = e.CompanyGuid
		FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.CompanyGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN tblCompanies d
		ON d.CompanyGuid = a.AssignedToCompanyGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies f
		ON f.CompanyGuid = a.CompanyGuid
		WHERE e._MasterRecordGuid <> e.CompanyGuid
		AND f.SiteGuid <> c.SiteGuid	
		AND b._CallingReferenceGuid = @callingRef1Guid

		INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
		(CompanyGuid, AssignedToCompanyGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.CompanyGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', d._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), --Clone the mapping even if the AssignedToCompany is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToCompany is eventually mapped to the site.
		b.SiteGuid, a.ID, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.CompanyGuid
		INNER JOIN tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblCompanies d
		ON d.CompanyGuid = a.AssignedToCompanyGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] e
			WHERE e.CompanyGuid = c.CompanyGuid
			AND e.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', d._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
		)

		--Drivers.
		--The relationship between tblCompanies and tblPersonnel is maintained fully on the tblPersonnel side, which references the Company using the Company MasterRecordGuid. 
		--Therefore newly created Company child record versions are automatically going to inherit the applicable tblCompanies-tblPersonnel relationships
		--and there are no actions to be taken for the new Company child record versions as far as the tblCompanies-tblPersonnel relationships are concerned.


		--UnavailableInventories
		-- For all the ProductToCompany mappings that reference a Parent Company record version instead of the actual Company child record version, because Record Versioning 
		-- was previously OFF for Company for that site, update the Company field of the mapping to point to the newly created Company child record versions.		
		UPDATE a 
		SET a.AssignedToCompanyGuid = e.CompanyGuid
		FROM [map].[tblProductToUnavailableInventoryCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.AssignedToCompanyGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblProducts d
		ON d.ProductGuid = a.ProductGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies f
		ON f.CompanyGuid = a.AssignedToCompanyGuid
		WHERE e._MasterRecordGuid <> e.CompanyGuid
		AND f.SiteGuid <> c.SiteGuid	
		AND b._CallingReferenceGuid = @callingRef1Guid

		--Clone the ProductToCompany mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a Product owned by a sitegroup/site lower than the SourceSiteGroup. Product is also an External Client of Company, which allows a Product at a lower site/sitegroup 
		--      to establish a relationship with a Company assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Company 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the Product (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Product-to-Company relationships, i.e Product-to-Company relationships that did not exist prior to turning Company Record Versioning ON.
		-- Note: Mappings against a Product not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
		(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.CompanyGuid,
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', d._MasterRecordGuid, b.SiteGuid), a.ProductGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Product is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToUnavailableInventoryCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblProducts d
		ON d.ProductGuid = a.ProductGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) e
			WHERE e.HierarchyLevel > 0
			AND e.SiteGuid = d.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] f
			WHERE f.AssignedToCompanyGuid = c.CompanyGuid
			AND f.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', d._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
		)


		--ShipToAuthorizedProducts
		-- For all the ProductToCompany mappings that reference a Parent Company record version instead of the actual Company child record version, because Record Versioning 
		-- was previously OFF for Company for that site, update the Company field of the mapping to point to the newly created Company child record versions.
		UPDATE a 
		SET a.AssignedToCompanyGuid = e.CompanyGuid
		FROM [map].[tblProductToCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.AssignedToCompanyGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblProducts d
		ON d.ProductGuid = a.ProductGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies f
		ON f.CompanyGuid = a.AssignedToCompanyGuid
		WHERE e._MasterRecordGuid <> e.CompanyGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid

		--Clone the ProductToCompany mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a Product owned by a sitegroup/site lower than the SourceSiteGroup. Product is also an External Client of Company, which allows a Product at a lower site/sitegroup 
		--      to establish a relationship with a Company assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Company 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the Product (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Product-to-Company relationships, i.e Product-to-Company relationships that did not exist prior to turning Company Record Versioning ON.
		-- Note: Mappings against a Product not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblProductToCompany]
		(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.CompanyGuid,
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', d._MasterRecordGuid, b.SiteGuid), a.ProductGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Product is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblProducts d
		ON d.ProductGuid = a.ProductGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) e
			WHERE e.HierarchyLevel > 0
			AND e.SiteGuid = d.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToCompany] f 
			WHERE f.AssignedToCompanyGuid = c.CompanyGuid
			AND f.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', d._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
		)


		--AuthorizedCarriers
		UPDATE a 
		SET a.AssignedToCompanyGuid = e.CompanyGuid
		FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.AssignedToCompanyGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN tblCompanies d
		ON d.CompanyGuid = a.CompanyGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies f
		ON f.CompanyGuid = a.AssignedToCompanyGuid
		WHERE e._MasterRecordGuid <> e.CompanyGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid	

		INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
		(AssignedToCompanyGuid, CompanyGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.CompanyGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', d._MasterRecordGuid, b.SiteGuid), a.CompanyGuid), --Clone the mapping even if the AssignedToCompany is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToCompany is eventually mapped to the site.
		b.SiteGuid, a.ID, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblCompanies d
		ON d.CompanyGuid = a.CompanyGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] e
			WHERE e.AssignedToCompanyGuid = c.CompanyGuid
			AND e.CompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', d._MasterRecordGuid, b.SiteGuid), a.CompanyGuid)
		)


		--SupplierAuthorizedProducts
		-- For all the ProductToCompany mappings that reference a Parent Company record version instead of the actual Company child record version, because Record Versioning 
		-- was previously OFF for Company for that site, update the Company field of the mapping to point to the newly created Company child record versions.		
		UPDATE a 
		SET a.AssignedToCompanyGuid = e.CompanyGuid
		FROM [map].[tblProductToSupplierProductCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.AssignedToCompanyGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblProducts d
		ON d.ProductGuid = a.ProductGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies f
		ON f.CompanyGuid = a.AssignedToCompanyGuid
		WHERE e._MasterRecordGuid <> e.CompanyGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid	

		--Clone the ProductToCompany mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a Product owned by a sitegroup/site lower than the SourceSiteGroup. Product is also an External Client of Company, which allows a Product at a lower site/sitegroup 
		--      to establish a relationship with a Company assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Company 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the Product (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Product-to-Company relationships, i.e Product-to-Company relationships that did not exist prior to turning Company Record Versioning ON.
		-- Note: Mappings against a Product not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblProductToSupplierProductCompany]
		(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.CompanyGuid,
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', d._MasterRecordGuid, b.SiteGuid), a.ProductGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Product is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToSupplierProductCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.AssignedToCompanyGuid
		INNER JOIN tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblProducts d
		ON d.ProductGuid = a.ProductGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) e
			WHERE e.HierarchyLevel > 0
			AND e.SiteGuid = d.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToSupplierProductCompany] f 
			WHERE f.AssignedToCompanyGuid = c.CompanyGuid
			AND f.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', d._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
		)


		--AccessSchedule
		INSERT INTO [dbo].[tblScheduleCompanyAccess]
		(CompanyGuid, LookupDayOfWeekIndex, Enabled, OpeningTime, ClosingTime, EndOfDayEnabled, EndOfDayTime, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.CompanyGuid,
		a.LookupDayOfWeekIndex, a.Enabled, a.OpeningTime, a.ClosingTime, a.EndOfDayEnabled, a.EndOfDayTime, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblScheduleCompanyAccess] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.CompanyGuid
		INNER JOIN tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblScheduleCompanyAccess] e 
			WHERE e.CompanyGuid = c.CompanyGuid
			AND e.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
		)

		--CertificatesAndPermits
		UPDATE a 
		SET a.CompanyGuid = e.CompanyGuid
		FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.CompanyGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblQualifications d
		ON d.QualificationGuid = a.QualificationGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies f
		ON f.CompanyGuid = a.CompanyGuid
		WHERE e._MasterRecordGuid <> e.CompanyGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid	

		INSERT INTO [map].[tblQualificationCompanyCertificateAndPermitToCompany]
		(CompanyGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.CompanyGuid, a.QualificationGuid,
		a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.ID, a.Rating, a.HistoricalRecord, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.CompanyGuid
		INNER JOIN tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblQualifications d
		ON d.QualificationGuid = a.QualificationGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] e 
			WHERE e.CompanyGuid = c.CompanyGuid
			AND e.QualificationGuid = a.QualificationGuid
		)

		--UserGroups


		--Drivers
		-- For all the CompanyPersonnelAssignedToCompany mappings that reference a Parent Company record version instead of the actual Company child record version, because Record Versioning 
		-- was previously OFF for Company for that site, update the Company field of the mapping to point to the newly created Company child record versions.
		UPDATE a 
		SET a.CompanyGuid = e.CompanyGuid
		FROM [map].[tblCompanyPersonnelAssignedToCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.CompanyGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblPersonnel d
		ON d.PersonnelGuid = a.PersonnelGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblCompanies f
		ON f.CompanyGuid = a.CompanyGuid
		WHERE e._MasterRecordGuid <> e.CompanyGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid

		--Clone the CompanyPersonnelAssignedToCompany mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a Personnel owned by a sitegroup/site lower than the SourceSiteGroup. Personnel is also an External Client of Company, which allows a Personnel at a lower site/sitegroup 
		--      to establish a relationship with a Company assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Company 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the Personnel (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Personnel-to-Company relationships, i.e Personnel-to-Company relationships that did not exist prior to turning Company Record Versioning ON.
		-- Note: Mappings against a Personnel not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblCompanyPersonnelAssignedToCompany]
		(CompanyGuid, PersonnelGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.CompanyGuid,
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Personnel', d._MasterRecordGuid, b.SiteGuid), a.PersonnelGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Personnel is eventually mapped to the site.
		b.SiteGuid, a.ID, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblCompanyPersonnelAssignedToCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.CompanyGuid
		INNER JOIN tblCompanies c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblPersonnel d
		ON d.PersonnelGuid = a.PersonnelGuid
		WHERE c._MasterRecordGuid <> c.CompanyGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) e
			WHERE e.HierarchyLevel > 0
			AND e.SiteGuid = d.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] f 
			WHERE f.CompanyGuid = c.CompanyGuid
			AND f.PersonnelGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Personnel', d._MasterRecordGuid, b.SiteGuid), a.PersonnelGuid)
		)

		--CompanyRoles
		--Company Roles are created/cloned and deleted independently of Record Versioning during company-to-site assignments. They are maintained separately in map.tblCompanyToRole for each company-to-site assignment, using a combination of MasterRecordGuid and Siteguid.		

		DELETE erv.tblTempEntityMappingHierarchy
		WHERE _CallingReferenceGuid = @callingRef1Guid

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
						+ 'Procedure Name: [erv].usp_CreateCompanyChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
