/*
	DROP PROCEDURE [erv].[usp_ReplicateProductGSChangesOnMaster]

	EXEC [erv].[usp_ReplicateProductGSChangesOnMaster] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_ReplicateProductGSChangesOnMaster] 'F94D0DAB-8C85-4A73-830E-A8168078B6AD'
	EXEC [erv].[usp_ReplicateProductGSChangesOnMaster] '1bb8c558-5277-47a5-90ae-2461bbd1eff7'
	EXEC [erv].[usp_ReplicateProductGSChangesOnMaster] '80B08634-D356-4569-B9A2-CD36DF955BD0'

*/


CREATE PROCEDURE [erv].[usp_ReplicateProductGSChangesOnMaster]
(
	@SourceProductGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_ReplicateProductGSChangesOnMaster] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Replicate the Global Specific field values of a Product child record version onto the Master Record copy.
	--          By replicating those field values onto the master record, we ensure that when the non-VersionSpecific
	--          fields of the master record are propagated down the site hierarchy, that all the GlobalSpecific changes made onto the
	--          the child record version will get propagated onto all the sitegroups and sites where the master record is assigned.
	-- Notes:
	-- 1. @SourceProductGuid: Guid of the Product child record version record whose GlobalSpecific fields needs to be replicated to its local Master Record copy 
	--    (and not the parent record of the entity record).
	-- 2. Whereas RecordVersioning propagation is limited to child record versions, the GlobalSpecific field replication targets the master records and allows
	--    modifications to the master records. This also applies to external attributres that represent a reference to another RecordVersioning entity (e.g. Company).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Product'

		DECLARE @masterSiteGuid uniqueidentifier
		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		DECLARE @assignedFromSiteGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid FROM dbo.tblProducts
		WHERE ProductGuid = @SourceProductGuid
		AND ProductGuid <> _MasterRecordGuid

		IF (@masterRecordGuid IS NULL)
		BEGIN
			RAISERROR('Cannot locate the source child record for data replication.',16,1); 
			RETURN;
		END

		IF ((SELECT COUNT(*) FROM dbo.tblProducts WHERE ProductGuid = @masterRecordGuid AND _MasterRecordGuid = @masterRecordGuid) = 0)
		BEGIN
			RAISERROR('Cannot locate the target master record for data replication.',16,1); 
			RETURN;
		END

		SELECT @masterSiteGuid = SiteGuid FROM dbo.tblProducts
		WHERE ProductGuid = @masterRecordGuid
		AND ProductGuid = _MasterRecordGuid

		SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityProductToSite 
		WHERE ProductGuid = @masterRecordGuid 
		AND SiteGuid = @ownerSiteGuid


		DECLARE @tblSegmentInfo TABLE
		(
			FilterValueGuid uniqueidentifier NULL,
			EntitySegmentTemplateGuid uniqueidentifier NOT NULL
		);		
		--Fetch all the entity segments that apply to the record. This query will usually return a single record.
		--The only situation where the query can return more than one record is that there is more than one entity segment (i.e. more than one filter field) are defined for 
		--the entity type of the entity record.
		INSERT INTO @tblSegmentInfo
		(FilterValueGuid, EntitySegmentTemplateGuid)
		SELECT FilterValueGuid, EntitySegmentTemplateGuid
		FROM [erv].[udf_GetEntitySegmentsByEntityGuid] (@EntityTypeId, @SourceProductGuid)
		
		IF NOT EXISTS (SELECT * FROM @tblSegmentInfo)
		BEGIN
			RAISERROR('Cannot locate the segment information for the selected entity record.',16,1); 
			RETURN;
		END
		

		--Retrieve the GlobalSpecific fields for the AssignedFrom sitegroup of the child record version whose changes need to be replicated
		DECLARE @tblSourceGlobalSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL,
			Processed bit
		)
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @masterRecordGuid, @assignedFromSiteGuid, 'GlobalSpecific', @callingRef2Guid 

		INSERT @tblSourceGlobalSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		SELECT TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid

		IF (NOT EXISTS (SELECT * FROM @tblSourceGlobalSpecificFields))
		BEGIN				
			/*	No GlobalSpecific fields to update.	*/
			RETURN;
		END

		--Build a table that has one flag column for each column of the tblProducts table, and set the flag according to whether the field is GlobalSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempProductRecordVersioningFlag
		(ProductGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.ProductGuid, a.SiteGuid, @callingRef1Guid FROM tblProducts a
		WHERE a._MasterRecordGuid = @masterRecordGuid
		AND a.ProductGuid = a._MasterRecordGuid

		EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] @EntityTypeId, @masterRecordGuid, @assignedFromSiteGuid, @callingRef2Guid, @callingRef1Guid

		DELETE erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid
		

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --ReplicateToMasterRecord
            SET @BeginTran = 1   
		END  	
		

		-- Update all the GlobalSpecific fields of the Master record version
		UPDATE a
		SET	a.[ApplyDensityLimits] = (CASE d.[ApplyDensityLimits_RVFlag] WHEN 1 THEN b.[ApplyDensityLimits] ELSE a.[ApplyDensityLimits] END),
			a.[ApplyStandardDensity] = (CASE d.[ApplyStandardDensity_RVFlag] WHEN 1 THEN b.[ApplyStandardDensity] ELSE a.[ApplyStandardDensity] END),
			a.[ApplyTemperatureLimits] = (CASE d.[ApplyTemperatureLimits_RVFlag] WHEN 1 THEN b.[ApplyTemperatureLimits] ELSE a.[ApplyTemperatureLimits] END),
			a.[ApplyVolumeCorrection] = (CASE d.[ApplyVolumeCorrection_RVFlag] WHEN 1 THEN b.[ApplyVolumeCorrection] ELSE a.[ApplyVolumeCorrection] END),
			a.[AutomaticCloseout] = (CASE d.[AutomaticCloseout_RVFlag] WHEN 1 THEN b.[AutomaticCloseout] ELSE a.[AutomaticCloseout] END),
			a.[AviationFuelFlag] = (CASE d.[AviationFuelFlag_RVFlag] WHEN 1 THEN b.[AviationFuelFlag] ELSE a.[AviationFuelFlag] END),
			a.[Bonded] = (CASE d.[Bonded_RVFlag] WHEN 1 THEN b.[Bonded] ELSE a.[Bonded] END),
			a.[Capitalize] = (CASE d.[Capitalize_RVFlag] WHEN 1 THEN b.[Capitalize] ELSE a.[Capitalize] END),
			a.[ComponentTolerance] = (CASE d.[ComponentTolerance_RVFlag] WHEN 1 THEN b.[ComponentTolerance] ELSE a.[ComponentTolerance] END),
			a.[ContaminationPromptLoadRackText] = (CASE d.[ContaminationPromptLoadRackText_RVFlag] WHEN 1 THEN b.[ContaminationPromptLoadRackText] ELSE a.[ContaminationPromptLoadRackText] END),
			a.[DensityDeadband] = (CASE d.[DensityDeadband_RVFlag] WHEN 1 THEN b.[DensityDeadband] ELSE a.[DensityDeadband] END),
			a.[DensityDecimalPlaces] = (CASE d.[DensityDecimalPlaces_RVFlag] WHEN 1 THEN b.[DensityDecimalPlaces] ELSE a.[DensityDecimalPlaces] END),
			a.[DensityHighLimit] = (CASE d.[DensityHighLimit_RVFlag] WHEN 1 THEN b.[DensityHighLimit] ELSE a.[DensityHighLimit] END),
			a.[DensityLowLimit] = (CASE d.[DensityLowLimit_RVFlag] WHEN 1 THEN b.[DensityLowLimit] ELSE a.[DensityLowLimit] END),
			a.[DensityUnitIndex] = (CASE d.[DensityUnitIndex_RVFlag] WHEN 1 THEN b.[DensityUnitIndex] ELSE a.[DensityUnitIndex] END),
			a.[Description] = (CASE d.[Description_RVFlag] WHEN 1 THEN b.[Description] ELSE a.[Description] END),
			a.[DielectricTolerance] = (CASE d.[DielectricTolerance_RVFlag] WHEN 1 THEN b.[DielectricTolerance] ELSE a.[DielectricTolerance] END),
			a.[FlowDecimalPlaces] = (CASE d.[FlowDecimalPlaces_RVFlag] WHEN 1 THEN b.[FlowDecimalPlaces] ELSE a.[FlowDecimalPlaces] END),
			a.[FlowUnitIndex] = (CASE d.[FlowUnitIndex_RVFlag] WHEN 1 THEN b.[FlowUnitIndex] ELSE a.[FlowUnitIndex] END),
			a.[GenericType] = (CASE d.[GenericType_RVFlag] WHEN 1 THEN b.[GenericType] ELSE a.[GenericType] END),
			a.[GroundFuel] = (CASE d.[GroundFuel_RVFlag] WHEN 1 THEN b.[GroundFuel] ELSE a.[GroundFuel] END),
			a.[HazardousMaterial] = (CASE d.[HazardousMaterial_RVFlag] WHEN 1 THEN b.[HazardousMaterial] ELSE a.[HazardousMaterial] END),
			a.[HiddenDate] = (CASE d.[HiddenDate_RVFlag] WHEN 1 THEN b.[HiddenDate] ELSE a.[HiddenDate] END),
			a.[InhibitAccounting] = (CASE d.[InhibitAccounting_RVFlag] WHEN 1 THEN b.[InhibitAccounting] ELSE a.[InhibitAccounting] END),
			a.[LevelDecimalPlaces] = (CASE d.[LevelDecimalPlaces_RVFlag] WHEN 1 THEN b.[LevelDecimalPlaces] ELSE a.[LevelDecimalPlaces] END),
			a.[LevelUnitIndex] = (CASE d.[LevelUnitIndex_RVFlag] WHEN 1 THEN b.[LevelUnitIndex] ELSE a.[LevelUnitIndex] END),
			a.[LoadByWeight] = (CASE d.[LoadByWeight_RVFlag] WHEN 1 THEN b.[LoadByWeight] ELSE a.[LoadByWeight] END),
			a.[LoadRackDisplayText] = (CASE d.[LoadRackDisplayText_RVFlag] WHEN 1 THEN b.[LoadRackDisplayText] ELSE a.[LoadRackDisplayText] END),
			a.[LockedOut] = (CASE d.[LockedOut_RVFlag] WHEN 1 THEN b.[LockedOut] ELSE a.[LockedOut] END),
			a.[LockedOutDate] = (CASE d.[LockedOutDate_RVFlag] WHEN 1 THEN b.[LockedOutDate] ELSE a.[LockedOutDate] END),
			a.[LockedOutReason] = (CASE d.[LockedOutReason_RVFlag] WHEN 1 THEN b.[LockedOutReason] ELSE a.[LockedOutReason] END),
			a.[LookupProductTypeIndex] = (CASE d.[LookupProductTypeIndex_RVFlag] WHEN 1 THEN b.[LookupProductTypeIndex] ELSE a.[LookupProductTypeIndex] END),
			a.[LowStockWarning] = (CASE d.[LowStockWarning_RVFlag] WHEN 1 THEN b.[LowStockWarning] ELSE a.[LowStockWarning] END),
			a.[MassDecimalPlaces] = (CASE d.[MassDecimalPlaces_RVFlag] WHEN 1 THEN b.[MassDecimalPlaces] ELSE a.[MassDecimalPlaces] END),
			a.[MassPackageSize] = (CASE d.[MassPackageSize_RVFlag] WHEN 1 THEN b.[MassPackageSize] ELSE a.[MassPackageSize] END),
			a.[MassUnitIndex] = (CASE d.[MassUnitIndex_RVFlag] WHEN 1 THEN b.[MassUnitIndex] ELSE a.[MassUnitIndex] END),
			a.[OctaneNumber] = (CASE d.[OctaneNumber_RVFlag] WHEN 1 THEN b.[OctaneNumber] ELSE a.[OctaneNumber] END),
			a.[PatternColor] = (CASE d.[PatternColor_RVFlag] WHEN 1 THEN b.[PatternColor] ELSE a.[PatternColor] END),
			a.[PatternNumber] = (CASE d.[PatternNumber_RVFlag] WHEN 1 THEN b.[PatternNumber] ELSE a.[PatternNumber] END),
			a.[PIDXCode] = (CASE d.[PIDXCode_RVFlag] WHEN 1 THEN b.[PIDXCode] ELSE a.[PIDXCode] END),
			a.[PIDXFamilyCode] = (CASE d.[PIDXFamilyCode_RVFlag] WHEN 1 THEN b.[PIDXFamilyCode] ELSE a.[PIDXFamilyCode] END),
			a.[IsEthanol] = b.[IsEthanol],
			a.[PressureDecimalPlaces] = (CASE d.[PressureDecimalPlaces_RVFlag] WHEN 1 THEN b.[PressureDecimalPlaces] ELSE a.[PressureDecimalPlaces] END),
			a.[PressureUnitIndex] = (CASE d.[PressureUnitIndex_RVFlag] WHEN 1 THEN b.[PressureUnitIndex] ELSE a.[PressureUnitIndex] END),
			a.[Price] = (CASE d.[Price_RVFlag] WHEN 1 THEN b.[Price] ELSE a.[Price] END),
			a.[ProductCode] = (CASE d.[ProductCode_RVFlag] WHEN 1 THEN b.[ProductCode] ELSE a.[ProductCode] END),
			a.[ProductColor] = (CASE d.[ProductColor_RVFlag] WHEN 1 THEN b.[ProductColor] ELSE a.[ProductColor] END),
			a.[ProductID] = (CASE d.[ProductID_RVFlag] WHEN 1 THEN b.[ProductID] ELSE a.[ProductID] END),
			a.[RegulatoryClass] = (CASE d.[RegulatoryClass_RVFlag] WHEN 1 THEN b.[RegulatoryClass] ELSE a.[RegulatoryClass] END),
			a.[ReidVaporPressure] = (CASE d.[ReidVaporPressure_RVFlag] WHEN 1 THEN b.[ReidVaporPressure] ELSE a.[ReidVaporPressure] END),
			a.[StandardDensity] = (CASE d.[StandardDensity_RVFlag] WHEN 1 THEN b.[StandardDensity] ELSE a.[StandardDensity] END),
			a.[StockResetDate] = (CASE d.[StockResetDate_RVFlag] WHEN 1 THEN b.[StockResetDate] ELSE a.[StockResetDate] END),
			a.[StockTrack] = (CASE d.[StockTrack_RVFlag] WHEN 1 THEN b.[StockTrack] ELSE a.[StockTrack] END),
			a.[TaxCode] = (CASE d.[TaxCode_RVFlag] WHEN 1 THEN b.[TaxCode] ELSE a.[TaxCode] END),
			a.[TemperatureDeadband] = (CASE d.[TemperatureDeadband_RVFlag] WHEN 1 THEN b.[TemperatureDeadband] ELSE a.[TemperatureDeadband] END),
			a.[TemperatureDecimalPlaces] = (CASE d.[TemperatureDecimalPlaces_RVFlag] WHEN 1 THEN b.[TemperatureDecimalPlaces] ELSE a.[TemperatureDecimalPlaces] END),
			a.[TemperatureHighLimit] = (CASE d.[TemperatureHighLimit_RVFlag] WHEN 1 THEN b.[TemperatureHighLimit] ELSE a.[TemperatureHighLimit] END),
			a.[TemperatureHiHiLimit] = (CASE d.[TemperatureHiHiLimit_RVFlag] WHEN 1 THEN b.[TemperatureHiHiLimit] ELSE a.[TemperatureHiHiLimit] END),
			a.[TemperatureLoLoLimit] = (CASE d.[TemperatureLoLoLimit_RVFlag] WHEN 1 THEN b.[TemperatureLoLoLimit] ELSE a.[TemperatureLoLoLimit] END),
			a.[TemperatureLowLimit] = (CASE d.[TemperatureLowLimit_RVFlag] WHEN 1 THEN b.[TemperatureLowLimit] ELSE a.[TemperatureLowLimit] END),
			a.[TemperatureUnitIndex] = (CASE d.[TemperatureUnitIndex_RVFlag] WHEN 1 THEN b.[TemperatureUnitIndex] ELSE a.[TemperatureUnitIndex] END),
			a.[TrackingProductGuid] = (CASE d.[TrackingProductGuid_RVFlag] WHEN 1 THEN b.[TrackingProductGuid] ELSE a.[TrackingProductGuid] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UserData1] = (CASE d.[UserData1_RVFlag] WHEN 1 THEN b.[UserData1] ELSE a.[UserData1] END),
			a.[UserData2] = (CASE d.[UserData2_RVFlag] WHEN 1 THEN b.[UserData2] ELSE a.[UserData2] END),
			a.[UserData3] = (CASE d.[UserData3_RVFlag] WHEN 1 THEN b.[UserData3] ELSE a.[UserData3] END),
			a.[UserData4] = (CASE d.[UserData4_RVFlag] WHEN 1 THEN b.[UserData4] ELSE a.[UserData4] END),
			a.[UserData5] = (CASE d.[UserData5_RVFlag] WHEN 1 THEN b.[UserData5] ELSE a.[UserData5] END),
			a.[UserData6] = (CASE d.[UserData6_RVFlag] WHEN 1 THEN b.[UserData6] ELSE a.[UserData6] END),
			a.[UserData7] = (CASE d.[UserData7_RVFlag] WHEN 1 THEN b.[UserData7] ELSE a.[UserData7] END),
			a.[UserData8] = (CASE d.[UserData8_RVFlag] WHEN 1 THEN b.[UserData8] ELSE a.[UserData8] END),
			a.[VaporRecovery] = (CASE d.[VaporRecovery_RVFlag] WHEN 1 THEN b.[VaporRecovery] ELSE a.[VaporRecovery] END),
			a.[VarianceTolerance] = (CASE d.[VarianceTolerance_RVFlag] WHEN 1 THEN b.[VarianceTolerance] ELSE a.[VarianceTolerance] END),
			a.[VcfModuleSettings] = (CASE d.[VcfModuleSettings_RVFlag] WHEN 1 THEN b.[VcfModuleSettings] ELSE a.[VcfModuleSettings] END),
			a.[VolumeDecimalPlaces] = (CASE d.[VolumeDecimalPlaces_RVFlag] WHEN 1 THEN b.[VolumeDecimalPlaces] ELSE a.[VolumeDecimalPlaces] END),
			a.[VolumePackageSize] = (CASE d.[VolumePackageSize_RVFlag] WHEN 1 THEN b.[VolumePackageSize] ELSE a.[VolumePackageSize] END),
			a.[VolumeUnitIndex] = (CASE d.[VolumeUnitIndex_RVFlag] WHEN 1 THEN b.[VolumeUnitIndex] ELSE a.[VolumeUnitIndex] END)
		FROM tblProducts a
		INNER JOIN tblProducts b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempProductRecordVersioningFlag d
		ON d.ProductGuid = a.ProductGuid
		WHERE b.ProductGuid = @SourceProductGuid
		AND d._CallingReferenceGuid = @callingRef1Guid
		AND a.ProductGuid = a._MasterRecordGuid

		DELETE erv.tblTempProductRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRef1Guid 


		
		/*Process those GlobalSpecific External fields whose replication require custom handling. */		
		-- Process [AuthorizedCustomers] External Field - [map].[tblProductToCompany]
		-- Company is both an External Attribute of Product (i.e. Company-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedCustomers') > 0)
		BEGIN
			--Delete the master record version Company mappings that are not supported anymore in the child Product record 			
			DELETE a FROM [map].[tblProductToCompany] a
			INNER JOIN [dbo].tblProducts b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN map.tblEntityCompanyToSite d
			ON d.CompanyGuid = c._MasterRecordGuid
			AND d.SiteGuid = b.SiteGuid
			LEFT OUTER JOIN 
			(
				SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
				FROM erv.tblEntityRecordVersioningFieldConfig e1
				INNER JOIN erv.tblEntitySegmentTemplate e2
				ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
				WHERE e2.EntityTypeId = 'Company'
				AND TargetField = 'ShipToAuthorizedProducts'
			) e
			ON e.SiteGroupGuid = d.AssignedFromSiteGuid
			WHERE b.ProductGuid = @masterRecordGuid
			AND b.ProductGuid = b._MasterRecordGuid			
			AND ((c.CompanyGuid = c._MasterRecordGuid) OR (NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific'))) --Exclude the mappings that are owned by a Company child record version whose ShipToAuthorizedProducts field is set as VersionSpecific. Note: Deletion of mappings owned by a local Product record (Company Guid = Company MasterRecordGuid) is permitted because by definition GlobalSpecific fields support the update of master records.					
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToCompany] f
				INNER JOIN [dbo].tblProducts g
				ON g.ProductGuid = f.ProductGuid
				INNER JOIN [dbo].tblCompanies h
				ON h.CompanyGuid = f.AssignedToCompanyGuid				
				WHERE f.ProductGuid = @SourceProductGuid
				AND b._MasterRecordGuid = g._MasterRecordGuid
				AND c._MasterRecordGuid = h._MasterRecordGuid				
			)		
													
			--Update the master record version mappings that have been modified in the child Product record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.BlendPercentage = a.BlendPercentage,
			d.AdditiveRate = a.AdditiveRate,
			d.Ratio = a.Ratio,
			d.AdditiveCycleVolume = a.AdditiveCycleVolume,
			d.Tolerance = a.Tolerance,		
			d.PresetNumber = a.PresetNumber,
			d.AdditiveProfileGuid = a.AdditiveProfileGuid,
			d.TankGuid = a.TankGuid,
			d.MeterID = a.MeterID,
			d.ShipToProductID = a.ShipToProductID,
			d.ShipToProductCode = a.ShipToProductCode,
			d.ShipToLoadRackDisplayText = a.ShipToLoadRackDisplayText,
			d.UnavailableInventoryGross = a.UnavailableInventoryGross,
			d.UnavailableInventoryNet = a.UnavailableInventoryNet,
			d.SpecialInstructionNote = a.SpecialInstructionNote,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblProductToCompany] a
			INNER JOIN [dbo].[tblProducts] b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN dbo.tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblProductToCompany] d
			ON d.ProductGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblCompanies e
			ON e.CompanyGuid = d.AssignedToCompanyGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.ProductGuid = @SourceProductGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblProductToCompany]
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @masterSiteGuid), c._MasterRecordGuid), 
			 b._MasterRecordGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToCompany] a
			INNER JOIN dbo.tblProducts b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToCompany] d
				INNER JOIN dbo.tblCompanies e
				ON e.CompanyGuid = d.AssignedToCompanyGuid
				WHERE d.ProductGuid = b._MasterRecordGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		
		-- Process [AuthorizedCustomers] External Field - [map].[tblProductToCompanyGroup]
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedCustomers') > 0)
		BEGIN
			--CompanyGroup is both an External Attribute of Product (i.e. Product-To-CompanyGroup mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Product-To-CompanyGroup mappings are also maintained as part of the CompanyGroup entity, i.e. outside of the Product entity)
			--Delete the master record version mappings that are not supported anymore in the child Product record.
			DELETE a FROM [map].[tblProductToCompanyGroup] a
			INNER JOIN [dbo].tblProducts b
			ON b.ProductGuid = a.ProductGuid
			WHERE b.ProductGuid = @masterRecordGuid
			AND b.ProductGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToCompanyGroup] d
				INNER JOIN [dbo].tblProducts e
				ON e.ProductGuid = d.ProductGuid			
				WHERE d.ProductGuid = @SourceProductGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid			
			)						

			--Update the master record version mappings that have been modified in the child Product record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.BlendPercentage = a.BlendPercentage,
			d.AdditiveRate = a.AdditiveRate,
			d.Ratio = a.Ratio,
			d.AdditiveCycleVolume = a.AdditiveCycleVolume,
			d.Tolerance = a.Tolerance,		
			d.PresetNumber = a.PresetNumber,
			d.AdditiveProfileGuid = a.AdditiveProfileGuid,
			d.TankGuid = a.TankGuid,
			d.MeterID = a.MeterID,
			d.ShipToProductID = a.ShipToProductID,
			d.ShipToProductCode = a.ShipToProductCode,
			d.ShipToLoadRackDisplayText = a.ShipToLoadRackDisplayText,
			d.UnavailableInventoryGross = a.UnavailableInventoryGross,
			d.UnavailableInventoryNet = a.UnavailableInventoryNet,
			d.SpecialInstructionNote = a.SpecialInstructionNote,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblProductToCompanyGroup] a
			INNER JOIN [dbo].[tblProducts] b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToCompanyGroup] d
			ON d.ProductGuid = b._MasterRecordGuid
			AND d.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
			WHERE a.ProductGuid = @SourceProductGuid
					
			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblProductToCompanyGroup]
			(AssignedToApplicationStringGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT a.AssignedToApplicationStringGuid,
			 b._MasterRecordGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToCompanyGroup] a
			INNER JOIN dbo.tblProducts b
			ON b.ProductGuid = a.ProductGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToCompanyGroup] d
				WHERE d.ProductGuid = b._MasterRecordGuid
				AND d.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
			)
		END


		-- Process [Messages] External Field - Regular Product Messages
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Messages') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Product record
			DELETE a FROM [map].[tblApplicationStringToProductMessage] a
			INNER JOIN [dbo].tblProducts b
			ON b.ProductGuid = a.ProductGuid
			WHERE b.ProductGuid = @masterRecordGuid
			AND b.ProductGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblApplicationStringToProductMessage] d
				INNER JOIN [dbo].tblProducts e
				ON e.ProductGuid = d.ProductGuid			
				WHERE d.ProductGuid = @SourceProductGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)		

			--Update the master record version mappings that have been modified in the child Product record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblApplicationStringToProductMessage] a
			INNER JOIN [dbo].[tblProducts] b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblApplicationStringToProductMessage] d
			ON d.ProductGuid = b._MasterRecordGuid
			AND d.ApplicationStringGuid = a.ApplicationStringGuid
			WHERE a.ProductGuid = @SourceProductGuid
						
			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblApplicationStringToProductMessage]
			(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT a.ApplicationStringGuid,
			 b._MasterRecordGuid, a.Sequence, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblApplicationStringToProductMessage] a
			INNER JOIN dbo.tblProducts b
			ON b.ProductGuid = a.ProductGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblApplicationStringToProductMessage] d
				WHERE d.ProductGuid = b._MasterRecordGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)
		END

		-- Process [Messages] External Field - DOT Hazardous Product Messages
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Messages') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Product record
			DELETE a FROM [map].[tblApplicationStringToDotHazardousMessage] a
			INNER JOIN [dbo].tblProducts b
			ON b.ProductGuid = a.ProductGuid
			WHERE b.ProductGuid = @masterRecordGuid
			AND b.ProductGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblApplicationStringToDotHazardousMessage] d
				INNER JOIN [dbo].tblProducts e
				ON e.ProductGuid = d.ProductGuid			
				WHERE d.ProductGuid = @SourceProductGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)		

			--Update the master record version mappings that have been modified in the child Product record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblApplicationStringToDotHazardousMessage] a
			INNER JOIN [dbo].[tblProducts] b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblApplicationStringToDotHazardousMessage] d
			ON d.ProductGuid = b._MasterRecordGuid
			AND d.ApplicationStringGuid = a.ApplicationStringGuid
			WHERE a.ProductGuid = @SourceProductGuid
					
			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblApplicationStringToDotHazardousMessage]
			(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT a.ApplicationStringGuid,
			 b._MasterRecordGuid, a.Sequence, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblApplicationStringToDotHazardousMessage] a
			INNER JOIN dbo.tblProducts b
			ON b.ProductGuid = a.ProductGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblApplicationStringToDotHazardousMessage] d
				WHERE d.ProductGuid = b._MasterRecordGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)
		END

		-- Process [UnavailableInventories] External Field
		-- Company is both an External Attribute of Product (i.e. Company-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UnavailableInventories') > 0)
		BEGIN
			--Delete the master record version Company mappings that are not supported anymore in the child Product record 			
			DELETE a FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN [dbo].tblProducts b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN map.tblEntityCompanyToSite d
			ON d.CompanyGuid = c._MasterRecordGuid
			AND d.SiteGuid = b.SiteGuid
			LEFT OUTER JOIN 
			(
				SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
				FROM erv.tblEntityRecordVersioningFieldConfig e1
				INNER JOIN erv.tblEntitySegmentTemplate e2
				ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
				WHERE e2.EntityTypeId = 'Company'
				AND TargetField = 'UnavailableInventories'
			) e
			ON e.SiteGroupGuid = d.AssignedFromSiteGuid
			WHERE b.ProductGuid = @masterRecordGuid
			AND b.ProductGuid = b._MasterRecordGuid			
			AND ((c.CompanyGuid = c._MasterRecordGuid) OR (NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific'))) --Exclude the mappings that are owned by a Company child record version whose UnavailableInventories field is set as VersionSpecific. Note: Deletion of mappings owned by a local Product record (Company Guid = Company MasterRecordGuid) is permitted because by definition GlobalSpecific fields support the update of master records.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] f
				INNER JOIN [dbo].tblProducts g
				ON g.ProductGuid = f.ProductGuid
				INNER JOIN [dbo].tblCompanies h
				ON h.CompanyGuid = f.AssignedToCompanyGuid				
				WHERE f.ProductGuid = @SourceProductGuid
				AND b._MasterRecordGuid = g._MasterRecordGuid
				AND c._MasterRecordGuid = h._MasterRecordGuid				
			)		
													
			--Update the master record version mappings that have been modified in the child Product record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.BlendPercentage = a.BlendPercentage,
			d.AdditiveRate = a.AdditiveRate,
			d.Ratio = a.Ratio,
			d.AdditiveCycleVolume = a.AdditiveCycleVolume,
			d.Tolerance = a.Tolerance,		
			d.PresetNumber = a.PresetNumber,
			d.AdditiveProfileGuid = a.AdditiveProfileGuid,
			d.TankGuid = a.TankGuid,
			d.MeterID = a.MeterID,
			d.ShipToProductID = a.ShipToProductID,
			d.ShipToProductCode = a.ShipToProductCode,
			d.ShipToLoadRackDisplayText = a.ShipToLoadRackDisplayText,
			d.UnavailableInventoryGross = a.UnavailableInventoryGross,
			d.UnavailableInventoryNet = a.UnavailableInventoryNet,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN [dbo].[tblProducts] b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN dbo.tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblProductToUnavailableInventoryCompany] d
			ON d.ProductGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblCompanies e
			ON e.CompanyGuid = d.AssignedToCompanyGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.ProductGuid = @SourceProductGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @masterSiteGuid), a.AssignedToCompanyGuid), 
			 b._MasterRecordGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN dbo.tblProducts b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d
				INNER JOIN dbo.tblCompanies e
				ON e.CompanyGuid = d.AssignedToCompanyGuid
				WHERE d.ProductGuid = b._MasterRecordGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		-- Process [SupplierAuthorizedProducts] External Field
		-- Company is both an External Attribute of Product (i.e. Company-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'SupplierAuthorizedProducts') > 0)
		BEGIN
			--Delete the master record version Company mappings that are not supported anymore in the child Product record 			
			DELETE a FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN [dbo].tblProducts b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN map.tblEntityCompanyToSite d
			ON d.CompanyGuid = c._MasterRecordGuid
			AND d.SiteGuid = b.SiteGuid
			LEFT OUTER JOIN 
			(
				SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
				FROM erv.tblEntityRecordVersioningFieldConfig e1
				INNER JOIN erv.tblEntitySegmentTemplate e2
				ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
				WHERE e2.EntityTypeId = 'Company'
				AND TargetField = 'SupplierAuthorizedProducts'
			) e
			ON e.SiteGroupGuid = d.AssignedFromSiteGuid
			WHERE b.ProductGuid = @masterRecordGuid
			AND b.ProductGuid = b._MasterRecordGuid			
			AND ((c.CompanyGuid = c._MasterRecordGuid) OR (NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific'))) --Exclude the mappings that are owned by a Company child record version whose SupplierAuthorizedProducts field is set as VersionSpecific. Note: Deletion of mappings owned by a local Product record (Company Guid = Company MasterRecordGuid) is permitted because by definition GlobalSpecific fields support the update of master records.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] f
				INNER JOIN [dbo].tblProducts g
				ON g.ProductGuid = f.ProductGuid
				INNER JOIN [dbo].tblCompanies h
				ON h.CompanyGuid = f.AssignedToCompanyGuid				
				WHERE f.ProductGuid = @SourceProductGuid
				AND b._MasterRecordGuid = g._MasterRecordGuid
				AND c._MasterRecordGuid = h._MasterRecordGuid				
			)		
																	
			--Update the master record version mappings that have been modified in the child Product record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.BlendPercentage = a.BlendPercentage,
			d.AdditiveRate = a.AdditiveRate,
			d.Ratio = a.Ratio,
			d.AdditiveCycleVolume = a.AdditiveCycleVolume,
			d.Tolerance = a.Tolerance,		
			d.PresetNumber = a.PresetNumber,
			d.AdditiveProfileGuid = a.AdditiveProfileGuid,
			d.TankGuid = a.TankGuid,
			d.MeterID = a.MeterID,
			d.ShipToProductID = a.ShipToProductID,
			d.ShipToProductCode = a.ShipToProductCode,
			d.ShipToLoadRackDisplayText = a.ShipToLoadRackDisplayText,
			d.UnavailableInventoryGross = a.UnavailableInventoryGross,
			d.UnavailableInventoryNet = a.UnavailableInventoryNet,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN [dbo].[tblProducts] b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN dbo.tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblProductToSupplierProductCompany] d
			ON d.ProductGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblCompanies e
			ON e.CompanyGuid = d.AssignedToCompanyGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.ProductGuid = @SourceProductGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblProductToSupplierProductCompany]
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @masterSiteGuid), a.AssignedToCompanyGuid), 
			 b._MasterRecordGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN dbo.tblProducts b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] d
				INNER JOIN dbo.tblCompanies e
				ON e.CompanyGuid = d.AssignedToCompanyGuid
				WHERE d.ProductGuid = b._MasterRecordGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		-- Process [TransactionAliasExclusion] External Field
		-- TransactionAlias is both an External Attribute of Product (i.e. TransactionAlias-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. TransactionAlias-To-Product mappings are also maintained as part of the TransactionAlias entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'TransactionAliasExclusion') > 0)
		BEGIN
			--Delete the master record version TransactionAlias mappings that are not supported anymore in the child Product record 			
			DELETE a FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN [dbo].tblProducts b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN dbo.tblTransactionAliases c
			ON c.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			INNER JOIN map.tblEntityTransactionAliasToSite d
			ON d.TransactionAliasGuid = c._MasterRecordGuid
			AND d.SiteGuid = b.SiteGuid
			LEFT OUTER JOIN 
			(
				SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
				FROM erv.tblEntityRecordVersioningFieldConfig e1
				INNER JOIN erv.tblEntitySegmentTemplate e2
				ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
				WHERE e2.EntityTypeId = 'Transaction_Alias'
				AND TargetField = 'Products'
			) e
			ON e.SiteGroupGuid = d.AssignedFromSiteGuid
			WHERE b.ProductGuid = @masterRecordGuid
			AND b.ProductGuid = b._MasterRecordGuid			
			AND ((c.TransactionAliasGuid = c._MasterRecordGuid) OR (NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific'))) --Exclude the mappings that are owned by a TransactionAlias child record version whose Products field is set as VersionSpecific. Note: Deletion of mappings owned by a local Product record (TransactionAlias Guid = TransactionAlias MasterRecordGuid) is permitted because by definition GlobalSpecific fields support the update of master records.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] f
				INNER JOIN [dbo].tblProducts g
				ON g.ProductGuid = f.ProductGuid
				INNER JOIN [dbo].tblTransactionAliases h
				ON h.TransactionAliasGuid = f.AssignedToTransactionAliasGuid			
				WHERE f.ProductGuid = @SourceProductGuid
				AND b._MasterRecordGuid = g._MasterRecordGuid
				AND c._MasterRecordGuid = h._MasterRecordGuid				
			)		
						
			--Update the master record version mappings that have been modified in the child Product record
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.BlendPercentage = a.BlendPercentage,
			d.AdditiveRate = a.AdditiveRate,
			d.Ratio = a.Ratio,
			d.AdditiveCycleVolume = a.AdditiveCycleVolume,
			d.Tolerance = a.Tolerance,		
			d.PresetNumber = a.PresetNumber,
			d.AdditiveProfileGuid = a.AdditiveProfileGuid,
			d.TankGuid = a.TankGuid,
			d.MeterID = a.MeterID,
			d.ShipToProductID = a.ShipToProductID,
			d.ShipToProductCode = a.ShipToProductCode,
			d.ShipToLoadRackDisplayText = a.ShipToLoadRackDisplayText,
			d.UnavailableInventoryGross = a.UnavailableInventoryGross,
			d.UnavailableInventoryNet = a.UnavailableInventoryNet,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN [dbo].[tblProducts] b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN dbo.tblTransactionAliases c
			ON c.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			INNER JOIN [map].[tblProductToTransactionAliasExclusion] d
			ON d.ProductGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblTransactionAliases e
			ON e.TransactionAliasGuid = d.AssignedToTransactionAliasGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.ProductGuid = @SourceProductGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblProductToTransactionAliasExclusion]
			(AssignedToTransactionAliasGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, @masterSiteGuid), a.AssignedToTransactionAliasGuid), 
			 b._MasterRecordGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN dbo.tblProducts b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN dbo.tblTransactionAliases c
			ON c.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d
				INNER JOIN dbo.tblTransactionAliases e
				ON e.TransactionAliasGuid= d.AssignedToTransactionAliasGuid
				WHERE d.ProductGuid = b._MasterRecordGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		
		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --ReplicateToMasterRecord
		END
	END TRY
	BEGIN CATCH        
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --ReplicateToMasterRecord
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
						+ 'Procedure Name: [erv].usp_ReplicateProductGSChangesOnMaster' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END