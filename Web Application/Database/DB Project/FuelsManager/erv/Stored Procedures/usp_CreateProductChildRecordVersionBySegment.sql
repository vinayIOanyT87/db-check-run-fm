/*
	DROP PROCEDURE [erv].[usp_CreateProductChildRecordVersionBySegment]

	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	EXEC [erv].[usp_CreateProductChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '00000000-0000-0000-0000-000000000001', @dt, 'HB'
	--EXEC [erv].[usp_CreateProductChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', @dt, 'HB'

*/

CREATE PROCEDURE [erv].[usp_CreateProductChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreateProductChildRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new Product record version for each of the existing entity assignments of a given Product segment from a given SiteGroup.
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
			ProductGuid uniqueidentifier  -- The child record version ProductGuid is not initially available since the process will be creating the new Product child record versions, but it is populated and used further down the process when handling the external attributes.
		)

		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@entityTypeId = 'Product')
		BEGIN
			INSERT INTO @tblTargetEntitySite
			(SiteGuid, MasterRecordGuid, ParentEntityGuid)
			SELECT b.SiteGuid, b.ProductGuid, a.ProductGuid
			FROM tblProducts a
			INNER JOIN map.tblEntityProductToSite b
			ON b.ProductGuid = a._MasterRecordGuid
			AND b.AssignedFromSiteGuid = a.SiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about creating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no need to create a child record version in any case.
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND NOT EXISTS
			(SELECT * FROM tblProducts c
			WHERE c._MasterRecordGuid = a._MasterRecordGuid
			AND c.SiteGuid = b.SiteGuid)
			AND b.SiteGuid <> b.AssignedFromSiteGuid
		END
				

		--Create the child record versions by cloning the internal fields of the parent record version
		INSERT INTO tblProducts
		(ProductID,SiteGuid,Description,GenericType,StockResetDate,StockTrack,DensityHighLimit,DensityLowLimit,DensityDeadband,TemperatureHiHiLimit,TemperatureHighLimit,TemperatureLowLimit,TemperatureLoLoLimit,TemperatureDeadband,Bonded,LowStockWarning,GroundFuel,ProductCode,Price,AviationFuelFlag,StandardDensity,ApplyVolumeCorrection,ApplyStandardDensity,ApplyDensityLimits,ApplyTemperatureLimits,VolumeUnitIndex,TemperatureUnitIndex,DensityUnitIndex,VolumeDecimalPlaces,TemperatureDecimalPlaces,DensityDecimalPlaces,Capitalize,OctaneNumber,ReidVaporPressure,HazardousMaterial,RegulatoryClass,LoadRackDisplayText,ComponentTolerance,VaporRecovery,LockedOut,LockedOutReason,LockedOutDate,VarianceTolerance,LoadByWeight,PIDXCode,ContaminationPromptLoadRackText,InhibitAccounting,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,MassUnitIndex,LevelUnitIndex,FlowUnitIndex,PressureUnitIndex,MassDecimalPlaces,LevelDecimalPlaces,FlowDecimalPlaces,PressureDecimalPlaces,VolumePackageSize,MassPackageSize,LookupProductTypeIndex,TrackingProductGuid,TaxCode,VcfModuleSettings,ProductColor,PatternColor,PatternNumber,_MasterRecordGuid, AutomaticCloseout, DielectricTolerance, HiddenDate, PIDXFamilyCode, IsEthanol)		
		SELECT a.ProductID, b.SiteGuid, a.Description, a.GenericType, a.StockResetDate, a.StockTrack, a.DensityHighLimit, a.DensityLowLimit, a.DensityDeadband, a.TemperatureHiHiLimit, a.TemperatureHighLimit, a.TemperatureLowLimit, a.TemperatureLoLoLimit, a.TemperatureDeadband, a.Bonded, a.LowStockWarning, a.GroundFuel, a.ProductCode, a.Price, a.AviationFuelFlag, a.StandardDensity, a.ApplyVolumeCorrection, a.ApplyStandardDensity, a.ApplyDensityLimits,a .ApplyTemperatureLimits, a.VolumeUnitIndex, a.TemperatureUnitIndex, a.DensityUnitIndex, a.VolumeDecimalPlaces, a.TemperatureDecimalPlaces, a.DensityDecimalPlaces, a.Capitalize, a.OctaneNumber, a.ReidVaporPressure, a.HazardousMaterial, a.RegulatoryClass, a.LoadRackDisplayText, a.ComponentTolerance, a.VaporRecovery, a.LockedOut, a.LockedOutReason, a.LockedOutDate, a.VarianceTolerance, a.LoadByWeight, a.PIDXCode, a.ContaminationPromptLoadRackText, a.InhibitAccounting, a.UserData1, a.UserData2, a.UserData3, a.UserData4, a.UserData5, a.UserData6, a.UserData7, a.UserData8, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy, a.MassUnitIndex, a.LevelUnitIndex, a.FlowUnitIndex, a.PressureUnitIndex, a.MassDecimalPlaces, a.LevelDecimalPlaces, a.FlowDecimalPlaces, a.PressureDecimalPlaces, a.VolumePackageSize, a.MassPackageSize, a.LookupProductTypeIndex, a.TrackingProductGuid, a.TaxCode, a.VcfModuleSettings, a.ProductColor, a.PatternColor,a.PatternNumber,a._MasterRecordGuid, a.AutomaticCloseout, a.DielectricTolerance, a.HiddenDate, a.PIDXFamilyCode, a.IsEthanol
		FROM tblProducts a
		INNER JOIN @tblTargetEntitySite b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.ParentEntityGuid = a.ProductGuid


		--Clone the external attributes of the parent record version

		--Retrieve the first available Product record version applicable for all Product mappings to @SourceSiteGroupGuid
		--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it just updates the AssignedFromSiteGuid and the EntityGuid of the initial mapping record to reflect the actual parent record.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempEntityMappingHierarchy
		(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
		SELECT a.ProductGuid, b.ProductGuid, a.SiteGuid, 0, @callingRef1Guid
		FROM map.tblEntityProductToSite a
		LEFT OUTER JOIN tblProducts b
		ON b._MasterRecordGuid = a.ProductGuid
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
			SET a.AssignedFromSiteGuid = b.SiteGuid, a.EntityGuid = c.ProductGuid
			FROM erv.tblTempEntityMappingHierarchy a
			INNER JOIN map.tblEntityProductToSite b
			ON b.ProductGuid = a.EntityMasterGuid
			AND b.SiteGuid = a.AssignedFromSiteGuid
			LEFT OUTER JOIN tblProducts c
			ON c._MasterRecordGuid = b.ProductGuid
			AND c.SiteGuid = b.SiteGuid
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND a.EntityGuid IS NULL
		END				

		--AuthorisedCustomers - ProductToCompany
		-- For all the ProductToCompany mappings that reference a Parent Product record version instead of the actual Product child record version, because Record Versioning 
		-- was previously OFF for Product for that site, update the Product field of the mapping to point to the newly created Product child record versions.		
		UPDATE a 
		SET a.ProductGuid = e.ProductGuid
		FROM [map].[tblProductToCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ProductGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblCompanies d
		ON d.CompanyGuid = a.AssignedToCompanyGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts f
		ON f.ProductGuid = a.ProductGuid
		WHERE e._MasterRecordGuid <> e.ProductGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid	
		
		--Clone the ProductToCompany mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a Company owned by a sitegroup/site lower than the SourceSiteGroup. Company is also an External Client of Product, which allows a Company at a lower site/sitegroup 
		--      to establish a relationship with a Product assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Product 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the Company (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Product-to-Company relationships, i.e Product-to-Company relationships that did not exist prior to turning Product Record Versioning ON.
		-- Note: Mappings against a Company not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblProductToCompany]
		(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.ProductGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', e._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), --Clone the mapping even if the AssignedToCompany is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToCompany is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblCompanies e
		ON e.CompanyGuid = a.AssignedToCompanyGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) f
			WHERE f.HierarchyLevel > 0
			AND f.SiteGuid = e.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToCompany] g
			WHERE g.ProductGuid = c.ProductGuid
			AND g.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', e._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
		)


		--AuthorisedCustomers - ProductToCompanyGroup
		-- For all the ProductToCompanyGroup mappings that reference a Parent Product record version instead of the actual Product child record version, because Record Versioning 
		-- was previously OFF for Product for that site, update the Product field of the mapping to point to the newly created Product child record versions.		
		UPDATE a 
		SET a.ProductGuid = e.ProductGuid
		FROM [map].[tblProductToCompanyGroup] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ProductGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblApplicationString d
		ON d.ApplicationStringGuid = a.AssignedToApplicationStringGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts f
		ON f.ProductGuid = a.ProductGuid
		WHERE e._MasterRecordGuid <> e.ProductGuid
		AND f.SiteGuid <> c.SiteGuid	
		AND b._CallingReferenceGuid = @callingRef1Guid

		--Clone the ProductToCompanyGroup mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a CompanyGroup owned by a sitegroup/site lower than the SourceSiteGroup. CompanyGroup is also an External Client of Product, which allows a CompanyGroup at a lower site/sitegroup 
		--      to establish a relationship with a Product assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Product 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the CompanyGroup (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Product-to-CompanyGroup relationships, i.e Product-to-CompanyGroup relationships that did not exist prior to turning Product Record Versioning ON.
		-- Note: Mappings against a CompanyGroup not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblProductToCompanyGroup]
		(AssignedToApplicationStringGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterID, 
		ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, SpecialInstructionNote, UnavailableInventoryGross, UnavailableInventoryNet, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT a.AssignedToApplicationStringGuid, c.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
		a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.SpecialInstructionNote, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
		@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToCompanyGroup] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblApplicationString d
		ON d.ApplicationStringGuid = a.AssignedToApplicationStringGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) e
			WHERE e.HierarchyLevel > 0
			AND e.SiteGuid = d.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToCompanyGroup] f
			WHERE f.ProductGuid = c.ProductGuid
			AND f.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
		)


		--Assigned Messages - Regular Product Messages
		UPDATE a 
		SET a.ProductGuid = e.ProductGuid
		FROM [map].[tblApplicationStringToProductMessage] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ProductGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblApplicationString d
		ON d.ApplicationStringGuid = a.ApplicationStringGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts f
		ON f.ProductGuid = a.ProductGuid
		WHERE e._MasterRecordGuid <> e.ProductGuid
		AND f.SiteGuid <> c.SiteGuid	
		AND b._CallingReferenceGuid = @callingRef1Guid
		
		----Clone the ProductToMessage mappings
		-- The Regular Product Messages are strictly External Attributes of Product, i.e. they cannot establish a relationship to a Product from their side. Therefore we do not need to filter out those external relationships during cloning.
		INSERT INTO [map].[tblApplicationStringToProductMessage]
		(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT a.ApplicationStringGuid, c.ProductGuid, a.Sequence, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblApplicationStringToProductMessage] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblApplicationStringToProductMessage] d
			WHERE d.ProductGuid = c.ProductGuid
			AND d.ApplicationStringGuid = a.ApplicationStringGuid
		)


		--Assigned Messages - DOT Hazardous Messages
		UPDATE a 
		SET a.ProductGuid = e.ProductGuid
		FROM [map].[tblApplicationStringToDotHazardousMessage] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ProductGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblApplicationString d
		ON d.ApplicationStringGuid = a.ApplicationStringGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts f
		ON f.ProductGuid = a.ProductGuid
		WHERE e._MasterRecordGuid <> e.ProductGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid

		----Clone the ProductToDOTHazardousMessage mappings
		-- The DOT Hazardous Product Messages are strictly External Attributes of Product, i.e. they cannot establish a relationship to a Product from their side. Therefore we do not need to filter out those external relationships during cloning.
		INSERT INTO [map].[tblApplicationStringToDotHazardousMessage]
		(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT a.ApplicationStringGuid, c.ProductGuid, a.Sequence, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblApplicationStringToDotHazardousMessage] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblApplicationStringToDotHazardousMessage] d
			WHERE d.ProductGuid = c.ProductGuid
			AND d.ApplicationStringGuid = a.ApplicationStringGuid
		)
		

		--UnavailableInventories
		-- For all the ProductToCompany mappings that reference a Parent Product record version instead of the actual Product child record version, because Record Versioning 
		-- was previously OFF for Product for that site, update the Product field of the mapping to point to the newly created Product child record versions.	
		UPDATE a 
		SET a.ProductGuid = e.ProductGuid
		FROM [map].[tblProductToUnavailableInventoryCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ProductGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblCompanies d
		ON d.CompanyGuid = a.AssignedToCompanyGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts f
		ON f.ProductGuid = a.ProductGuid
		WHERE e._MasterRecordGuid <> e.ProductGuid
		AND f.SiteGuid <> c.SiteGuid	
		AND b._CallingReferenceGuid = @callingRef1Guid

		--Clone the ProductToCompany mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a Company owned by a sitegroup/site lower than the SourceSiteGroup. Company is also an External Client of Product, which allows a Company at a lower site/sitegroup 
		--      to establish a relationship with a Product assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Product 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the Company (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Product-to-Company relationships, i.e Product-to-Company relationships that did not exist prior to turning Product Record Versioning ON.
		-- Note: Mappings against a Company not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
		(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.ProductGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', e._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), --Clone the mapping even if the AssignedToCompany is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToCompany is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToUnavailableInventoryCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblCompanies e
		ON e.CompanyGuid = a.AssignedToCompanyGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) f
			WHERE f.HierarchyLevel > 0
			AND f.SiteGuid = e.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] g
			WHERE g.ProductGuid = c.ProductGuid
			AND g.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', e._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
		)


		--SupplierAuthorizedProducts
		-- For all the ProductToCompany mappings that reference a Parent Product record version instead of the actual Product child record version, because Record Versioning 
		-- was previously OFF for Product for that site, update the Product field of the mapping to point to the newly created Product child record versions.	
		UPDATE a 
		SET a.ProductGuid = e.ProductGuid
		FROM [map].[tblProductToSupplierProductCompany] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ProductGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblCompanies d
		ON d.CompanyGuid = a.AssignedToCompanyGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts f
		ON f.ProductGuid = a.ProductGuid
		WHERE e._MasterRecordGuid <> e.ProductGuid
		AND f.SiteGuid <> c.SiteGuid
		AND b._CallingReferenceGuid = @callingRef1Guid
		
		--Clone the ProductToCompany mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a Company owned by a sitegroup/site lower than the SourceSiteGroup. Company is also an External Client of Product, which allows a Company at a lower site/sitegroup 
		--      to establish a relationship with a Product assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Product 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the Company (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Product-to-Company relationships, i.e Product-to-Company relationships that did not exist prior to turning Product Record Versioning ON.
		-- Note: Mappings against a Company not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblProductToSupplierProductCompany]
		(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.ProductGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', d._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), --Clone the mapping even if the AssignedToCompany is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToCompany is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToSupplierProductCompany] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblCompanies d
		ON d.CompanyGuid = a.AssignedToCompanyGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) e
			WHERE e.HierarchyLevel > 0
			AND e.SiteGuid = d.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToSupplierProductCompany] f
			WHERE f.ProductGuid = c.ProductGuid
			AND f.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', d._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
		)


		--TransactionAliasExclusion
		-- For all the ProductToTransactionAlias mappings that reference a Parent Product record version instead of the actual Product child record version, because Record Versioning 
		-- was previously OFF for Product for that site, update the Product field of the mapping to point to the newly created Product child record versions.	
		UPDATE a 
		SET a.ProductGuid = e.ProductGuid
		FROM [map].[tblProductToTransactionAliasExclusion] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ProductGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblTransactionAliases d
		ON d.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblProducts f
		ON f.ProductGuid = a.ProductGuid
		WHERE e._MasterRecordGuid <> e.ProductGuid
		AND f.SiteGuid <> c.SiteGuid	
		AND b._CallingReferenceGuid = @callingRef1Guid

		
		--Clone the ProductToTransactionAlias mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a TransactionAlias owned by a sitegroup/site lower than the SourceSiteGroup. TransactionAlias is also an External Client of Product, which allows a TransactionAlias at a lower site/sitegroup 
		--      to establish a relationship with a Product assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right Product 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the TransactionAlias (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Product-to-TransactionAlias relationships, i.e Product-to-TransactionAlias relationships that did not exist prior to turning Product Record Versioning ON.
		-- Note: Mappings against a TransactionAlias not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblProductToTransactionAliasExclusion]
		(ProductGuid, AssignedToTransactionAliasGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.ProductGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Transaction_Alias', d._MasterRecordGuid, b.SiteGuid), a.AssignedToTransactionAliasGuid), --Clone the mapping even if the AssignedToTransactionAliasGuid is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToTransactionAliasGuid is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToTransactionAliasExclusion] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.ProductGuid
		INNER JOIN tblProducts c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblTransactionAliases d
		ON d.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
		WHERE c._MasterRecordGuid <> c.ProductGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) e
			WHERE e.HierarchyLevel > 0
			AND e.SiteGuid = d.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToTransactionAliasExclusion] f
			WHERE f.ProductGuid = c.ProductGuid
			AND f.AssignedToTransactionAliasGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Transaction_Alias', d._MasterRecordGuid, b.SiteGuid), a.AssignedToTransactionAliasGuid)
		)

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
						+ 'Procedure Name: [erv].usp_CreateProductChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
