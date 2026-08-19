/*
	DROP PROCEDURE [erv].[usp_PropagateProductRevisionByEntityRecordChange]

	EXEC [erv].[usp_PropagateProductRevisionByEntityRecordChange] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagateProductRevisionByEntityRecordChange] 'F94D0DAB-8C85-4A73-830E-A8168078B6AD'
	EXEC [erv].[usp_PropagateProductRevisionByEntityRecordChange] '1bb8c558-5277-47a5-90ae-2461bbd1eff7'
	EXEC [erv].[usp_PropagateProductRevisionByEntityRecordChange] '80B08634-D356-4569-B9A2-CD36DF955BD0'

*/


CREATE PROCEDURE [erv].[usp_PropagateProductRevisionByEntityRecordChange]
(
	@SourceProductGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagateProductRevisionByEntityRecordChange] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate the current revision of a given Product entity record down the site hierarchy, according to the rules established by the Field Level Control configurations.
	-- This Stored Procedure is to be used to propagate the effect of an entity record change down to all its children record versions.
	-- Notes:
	-- 1. @SourceProductGuid: Guid of the Product record that needs to be propagated down the site hierarchy. This should correspond to the exact record version that has been 
	--    changed (and not the parent record of the entity record).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
/*
	DECLARE @SourceProductGuid uniqueidentifier
	SET @SourceProductGuid = '886AA683-C97D-461C-AFB6-AD9A4579E51D'
*/
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Product'

		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		SELECT @ownerSiteGuid = SiteGuid, @masterRecordGuid = _MasterRecordGuid FROM tblProducts
		WHERE ProductGuid = @SourceProductGuid

		IF ((@masterRecordGuid IS NULL) OR (@ownerSiteGuid IS NULL))
		BEGIN
			RAISERROR('Cannot locate the source record for data propagation.',16,1); 
			RETURN;
		END

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

		DECLARE @assignedFromSiteGroupGuid uniqueidentifier
		IF (@SourceProductGuid = @masterRecordGuid)
		BEGIN
			SET @assignedFromSiteGroupGuid = @ownerSiteGuid
		END
		ELSE
		BEGIN
			SET @assignedFromSiteGroupGuid = (SELECT [erv].[udf_GetEntityAssignedFromSite] (@EntityTypeId, @SourceProductGuid, Null))
			IF (@assignedFromSiteGroupGuid IS NULL)
			BEGIN
				RAISERROR('Cannot locate the assignment information for the selected child record version.',16,1); 
				RETURN;
			END
		END				

		-- Retrieve the Entity To Site hierarchy below the owner sitegroup of the entity record whose changes are to be propagated
		-- This corresponds to all the child record versions who derives, directly or indirectly, from the given record version.
		DECLARE @tblEntityToSiteHierarchy TABLE
		(
			SiteGuid uniqueidentifier
			, SiteId nvarchar(30)
			, HierarchyLevel int
			, Processed bit
		);

		INSERT INTO @tblEntityToSiteHierarchy
		(SiteGuid, SiteId, HierarchyLevel, Processed)
		SELECT SiteGuid, SiteId, HierarchyLevel, 0
		FROM [erv].[udf_GetProductToSiteHierarchyByRecordVersionGuid](@SourceProductGuid)
		WHERE HierarchyLevel > 0
		ORDER BY HierarchyLevel, SiteGuid


		--Retrieve the VersionSpecific fields for the owner sitegroup of the entity record whose changes need to be propagated
		DECLARE @tblSourceVersionSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			Processed bit
		)
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @masterRecordGuid, @ownerSiteGuid, 'VersionSpecific', @callingRef2Guid 

		INSERT @tblSourceVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName)
		SELECT TargetField, IsExternalAttribute, InternalFieldName FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid

		--Build a table that has one flag column for each column of the tblProducts table, and set the flag according to whether the field is VersionSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempProductRecordVersioningFlag
		(ProductGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.ProductGuid, a.SiteGuid, @callingRef1Guid FROM tblProducts a
		INNER JOIN @tblEntityToSiteHierarchy b
		ON b.SiteGuid = a.SiteGuid
		WHERE a._MasterRecordGuid = @masterRecordGuid

		DECLARE @tblTargetChildRecordVersions TABLE
		(
			ProductGuid uniqueidentifier,
			SiteGuid uniqueidentifier,
			HierarchyLevel int,
			Processed bit
		)

		INSERT INTO @tblTargetChildRecordVersions
		(ProductGuid, SiteGuid, HierarchyLevel, Processed)
		SELECT a.ProductGuid, b.SiteGuid, c.HierarchyLevel, 0 FROM erv.tblTempProductRecordVersioningFlag a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		INNER JOIN @tblEntityToSiteHierarchy c
		ON c.SiteGuid = b.SiteGuid
		WHERE b._MasterRecordGuid = @masterRecordGuid
		AND a._CallingReferenceGuid = @callingRef1Guid


		IF (NOT EXISTS (SELECT * FROM erv.tblTempProductRecordVersioningFlag WHERE _CallingReferenceGuid = @callingRef1Guid))
		BEGIN				
			/*	No child record versions to update.	*/
			RETURN;
		END

		EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] @EntityTypeId, @masterRecordGuid, @ownerSiteGuid, @callingRef2Guid, @callingRef1Guid

		DELETE erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid
		

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateToChildRecordVersions
            SET @BeginTran = 1   
		END  	
		

		-- Update all the internal non-VersionSpecific fields for all applicable child record versions
		UPDATE a
		SET	a.[ApplyDensityLimits] = (CASE d.[ApplyDensityLimits_RVFlag] WHEN 1 THEN a.[ApplyDensityLimits] ELSE b.[ApplyDensityLimits] END),
			a.[ApplyStandardDensity] = (CASE d.[ApplyStandardDensity_RVFlag] WHEN 1 THEN a.[ApplyStandardDensity] ELSE b.[ApplyStandardDensity] END),
			a.[ApplyTemperatureLimits] = (CASE d.[ApplyTemperatureLimits_RVFlag] WHEN 1 THEN a.[ApplyTemperatureLimits] ELSE b.[ApplyTemperatureLimits] END),
			a.[ApplyVolumeCorrection] = (CASE d.[ApplyVolumeCorrection_RVFlag] WHEN 1 THEN a.[ApplyVolumeCorrection] ELSE b.[ApplyVolumeCorrection] END),
			a.[AutomaticCloseout] = (CASE d.[AutomaticCloseout_RVFlag] WHEN 1 THEN a.[AutomaticCloseout] ELSE b.[AutomaticCloseout] END),
			a.[AviationFuelFlag] = (CASE d.[AviationFuelFlag_RVFlag] WHEN 1 THEN a.[AviationFuelFlag] ELSE b.[AviationFuelFlag] END),
			a.[Bonded] = (CASE d.[Bonded_RVFlag] WHEN 1 THEN a.[Bonded] ELSE b.[Bonded] END),
			a.[Capitalize] = (CASE d.[Capitalize_RVFlag] WHEN 1 THEN a.[Capitalize] ELSE b.[Capitalize] END),
			a.[ComponentTolerance] = (CASE d.[ComponentTolerance_RVFlag] WHEN 1 THEN a.[ComponentTolerance] ELSE b.[ComponentTolerance] END),
			a.[ContaminationPromptLoadRackText] = (CASE d.[ContaminationPromptLoadRackText_RVFlag] WHEN 1 THEN a.[ContaminationPromptLoadRackText] ELSE b.[ContaminationPromptLoadRackText] END),
			a.[DensityDeadband] = (CASE d.[DensityDeadband_RVFlag] WHEN 1 THEN a.[DensityDeadband] ELSE b.[DensityDeadband] END),
			a.[DensityDecimalPlaces] = (CASE d.[DensityDecimalPlaces_RVFlag] WHEN 1 THEN a.[DensityDecimalPlaces] ELSE b.[DensityDecimalPlaces] END),
			a.[DensityHighLimit] = (CASE d.[DensityHighLimit_RVFlag] WHEN 1 THEN a.[DensityHighLimit] ELSE b.[DensityHighLimit] END),
			a.[DensityLowLimit] = (CASE d.[DensityLowLimit_RVFlag] WHEN 1 THEN a.[DensityLowLimit] ELSE b.[DensityLowLimit] END),
			a.[DensityUnitIndex] = (CASE d.[DensityUnitIndex_RVFlag] WHEN 1 THEN a.[DensityUnitIndex] ELSE b.[DensityUnitIndex] END),
			a.[Description] = (CASE d.[Description_RVFlag] WHEN 1 THEN a.[Description] ELSE b.[Description] END),
			a.[DielectricTolerance] = (CASE d.[DielectricTolerance_RVFlag] WHEN 1 THEN a.[DielectricTolerance] ELSE b.[DielectricTolerance] END),
			a.[FlowDecimalPlaces] = (CASE d.[FlowDecimalPlaces_RVFlag] WHEN 1 THEN a.[FlowDecimalPlaces] ELSE b.[FlowDecimalPlaces] END),
			a.[FlowUnitIndex] = (CASE d.[FlowUnitIndex_RVFlag] WHEN 1 THEN a.[FlowUnitIndex] ELSE b.[FlowUnitIndex] END),
			a.[GenericType] = (CASE d.[GenericType_RVFlag] WHEN 1 THEN a.[GenericType] ELSE b.[GenericType] END),
			a.[GroundFuel] = (CASE d.[GroundFuel_RVFlag] WHEN 1 THEN a.[GroundFuel] ELSE b.[GroundFuel] END),
			a.[HazardousMaterial] = (CASE d.[HazardousMaterial_RVFlag] WHEN 1 THEN a.[HazardousMaterial] ELSE b.[HazardousMaterial] END),
			a.[HiddenDate] = (CASE d.[HiddenDate_RVFlag] WHEN 1 THEN a.[HiddenDate] ELSE b.[HiddenDate] END),
			a.[InhibitAccounting] = (CASE d.[InhibitAccounting_RVFlag] WHEN 1 THEN a.[InhibitAccounting] ELSE b.[InhibitAccounting] END),
			a.[LevelDecimalPlaces] = (CASE d.[LevelDecimalPlaces_RVFlag] WHEN 1 THEN a.[LevelDecimalPlaces] ELSE b.[LevelDecimalPlaces] END),
			a.[LevelUnitIndex] = (CASE d.[LevelUnitIndex_RVFlag] WHEN 1 THEN a.[LevelUnitIndex] ELSE b.[LevelUnitIndex] END),
			a.[LoadByWeight] = (CASE d.[LoadByWeight_RVFlag] WHEN 1 THEN a.[LoadByWeight] ELSE b.[LoadByWeight] END),
			a.[LoadRackDisplayText] = (CASE d.[LoadRackDisplayText_RVFlag] WHEN 1 THEN a.[LoadRackDisplayText] ELSE b.[LoadRackDisplayText] END),
			a.[LockedOut] = (CASE d.[LockedOut_RVFlag] WHEN 1 THEN a.[LockedOut] ELSE b.[LockedOut] END),
			a.[LockedOutDate] = (CASE d.[LockedOutDate_RVFlag] WHEN 1 THEN a.[LockedOutDate] ELSE b.[LockedOutDate] END),
			a.[LockedOutReason] = (CASE d.[LockedOutReason_RVFlag] WHEN 1 THEN a.[LockedOutReason] ELSE b.[LockedOutReason] END),
			a.[LookupProductTypeIndex] = (CASE d.[LookupProductTypeIndex_RVFlag] WHEN 1 THEN a.[LookupProductTypeIndex] ELSE b.[LookupProductTypeIndex] END),
			a.[LowStockWarning] = (CASE d.[LowStockWarning_RVFlag] WHEN 1 THEN a.[LowStockWarning] ELSE b.[LowStockWarning] END),
			a.[MassDecimalPlaces] = (CASE d.[MassDecimalPlaces_RVFlag] WHEN 1 THEN a.[MassDecimalPlaces] ELSE b.[MassDecimalPlaces] END),
			a.[MassPackageSize] = (CASE d.[MassPackageSize_RVFlag] WHEN 1 THEN a.[MassPackageSize] ELSE b.[MassPackageSize] END),
			a.[MassUnitIndex] = (CASE d.[MassUnitIndex_RVFlag] WHEN 1 THEN a.[MassUnitIndex] ELSE b.[MassUnitIndex] END),
			a.[OctaneNumber] = (CASE d.[OctaneNumber_RVFlag] WHEN 1 THEN a.[OctaneNumber] ELSE b.[OctaneNumber] END),
			a.[PatternColor] = (CASE d.[PatternColor_RVFlag] WHEN 1 THEN a.[PatternColor] ELSE b.[PatternColor] END),
			a.[PatternNumber] = (CASE d.[PatternNumber_RVFlag] WHEN 1 THEN a.[PatternNumber] ELSE b.[PatternNumber] END),
			a.[PIDXCode] = (CASE d.[PIDXCode_RVFlag] WHEN 1 THEN a.[PIDXCode] ELSE b.[PIDXCode] END),
			a.[PIDXFamilyCode] = (CASE d.[PIDXFamilyCode_RVFlag] WHEN 1 THEN a.[PIDXFamilyCode] ELSE b.[PIDXFamilyCode] END),
			a.[IsEthanol] = b.[IsEthanol],
			a.[PressureDecimalPlaces] = (CASE d.[PressureDecimalPlaces_RVFlag] WHEN 1 THEN a.[PressureDecimalPlaces] ELSE b.[PressureDecimalPlaces] END),
			a.[PressureUnitIndex] = (CASE d.[PressureUnitIndex_RVFlag] WHEN 1 THEN a.[PressureUnitIndex] ELSE b.[PressureUnitIndex] END),
			a.[Price] = (CASE d.[Price_RVFlag] WHEN 1 THEN a.[Price] ELSE b.[Price] END),
			a.[ProductCode] = (CASE d.[ProductCode_RVFlag] WHEN 1 THEN a.[ProductCode] ELSE b.[ProductCode] END),
			a.[ProductColor] = (CASE d.[ProductColor_RVFlag] WHEN 1 THEN a.[ProductColor] ELSE b.[ProductColor] END),
			a.[ProductID] = (CASE d.[ProductID_RVFlag] WHEN 1 THEN a.[ProductID] ELSE b.[ProductID] END),
			a.[RegulatoryClass] = (CASE d.[RegulatoryClass_RVFlag] WHEN 1 THEN a.[RegulatoryClass] ELSE b.[RegulatoryClass] END),
			a.[ReidVaporPressure] = (CASE d.[ReidVaporPressure_RVFlag] WHEN 1 THEN a.[ReidVaporPressure] ELSE b.[ReidVaporPressure] END),
			a.[StandardDensity] = (CASE d.[StandardDensity_RVFlag] WHEN 1 THEN a.[StandardDensity] ELSE b.[StandardDensity] END),
			a.[StockResetDate] = (CASE d.[StockResetDate_RVFlag] WHEN 1 THEN a.[StockResetDate] ELSE b.[StockResetDate] END),
			a.[StockTrack] = (CASE d.[StockTrack_RVFlag] WHEN 1 THEN a.[StockTrack] ELSE b.[StockTrack] END),
			a.[TaxCode] = (CASE d.[TaxCode_RVFlag] WHEN 1 THEN a.[TaxCode] ELSE b.[TaxCode] END),
			a.[TemperatureDeadband] = (CASE d.[TemperatureDeadband_RVFlag] WHEN 1 THEN a.[TemperatureDeadband] ELSE b.[TemperatureDeadband] END),
			a.[TemperatureDecimalPlaces] = (CASE d.[TemperatureDecimalPlaces_RVFlag] WHEN 1 THEN a.[TemperatureDecimalPlaces] ELSE b.[TemperatureDecimalPlaces] END),
			a.[TemperatureHighLimit] = (CASE d.[TemperatureHighLimit_RVFlag] WHEN 1 THEN a.[TemperatureHighLimit] ELSE b.[TemperatureHighLimit] END),
			a.[TemperatureHiHiLimit] = (CASE d.[TemperatureHiHiLimit_RVFlag] WHEN 1 THEN a.[TemperatureHiHiLimit] ELSE b.[TemperatureHiHiLimit] END),
			a.[TemperatureLoLoLimit] = (CASE d.[TemperatureLoLoLimit_RVFlag] WHEN 1 THEN a.[TemperatureLoLoLimit] ELSE b.[TemperatureLoLoLimit] END),
			a.[TemperatureLowLimit] = (CASE d.[TemperatureLowLimit_RVFlag] WHEN 1 THEN a.[TemperatureLowLimit] ELSE b.[TemperatureLowLimit] END),
			a.[TemperatureUnitIndex] = (CASE d.[TemperatureUnitIndex_RVFlag] WHEN 1 THEN a.[TemperatureUnitIndex] ELSE b.[TemperatureUnitIndex] END),
			a.[TrackingProductGuid] = (CASE d.[TrackingProductGuid_RVFlag] WHEN 1 THEN a.[TrackingProductGuid] ELSE b.[TrackingProductGuid] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UserData1] = (CASE d.[UserData1_RVFlag] WHEN 1 THEN a.[UserData1] ELSE b.[UserData1] END),
			a.[UserData2] = (CASE d.[UserData2_RVFlag] WHEN 1 THEN a.[UserData2] ELSE b.[UserData2] END),
			a.[UserData3] = (CASE d.[UserData3_RVFlag] WHEN 1 THEN a.[UserData3] ELSE b.[UserData3] END),
			a.[UserData4] = (CASE d.[UserData4_RVFlag] WHEN 1 THEN a.[UserData4] ELSE b.[UserData4] END),
			a.[UserData5] = (CASE d.[UserData5_RVFlag] WHEN 1 THEN a.[UserData5] ELSE b.[UserData5] END),
			a.[UserData6] = (CASE d.[UserData6_RVFlag] WHEN 1 THEN a.[UserData6] ELSE b.[UserData6] END),
			a.[UserData7] = (CASE d.[UserData7_RVFlag] WHEN 1 THEN a.[UserData7] ELSE b.[UserData7] END),
			a.[UserData8] = (CASE d.[UserData8_RVFlag] WHEN 1 THEN a.[UserData8] ELSE b.[UserData8] END),
			a.[VaporRecovery] = (CASE d.[VaporRecovery_RVFlag] WHEN 1 THEN a.[VaporRecovery] ELSE b.[VaporRecovery] END),
			a.[VarianceTolerance] = (CASE d.[VarianceTolerance_RVFlag] WHEN 1 THEN a.[VarianceTolerance] ELSE b.[VarianceTolerance] END),
			a.[VcfModuleSettings] = (CASE d.[VcfModuleSettings_RVFlag] WHEN 1 THEN a.[VcfModuleSettings] ELSE b.[VcfModuleSettings] END),
			a.[VolumeDecimalPlaces] = (CASE d.[VolumeDecimalPlaces_RVFlag] WHEN 1 THEN a.[VolumeDecimalPlaces] ELSE b.[VolumeDecimalPlaces] END),
			a.[VolumePackageSize] = (CASE d.[VolumePackageSize_RVFlag] WHEN 1 THEN a.[VolumePackageSize] ELSE b.[VolumePackageSize] END),
			a.[VolumeUnitIndex] = (CASE d.[VolumeUnitIndex_RVFlag] WHEN 1 THEN a.[VolumeUnitIndex] ELSE b.[VolumeUnitIndex] END)					
		FROM tblProducts a
		INNER JOIN tblProducts b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN @tblEntityToSiteHierarchy c
		ON a.SiteGuid = c.SiteGuid
		INNER JOIN erv.tblTempProductRecordVersioningFlag d
		ON d.ProductGuid = a.ProductGuid
		WHERE b.ProductGuid = @SourceProductGuid
		AND d._CallingReferenceGuid = @callingRef1Guid

		DELETE erv.tblTempProductRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRef1Guid 


		
		/*Process those non-VersionSpecific External fields whose propagation require custom handling. */		
		-- Process [AuthorizedCustomers] External Field - [map].[tblProductToCompany]
		-- Company is both an External Attribute of Product (i.e. Company-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedCustomers') = 0)
		BEGIN
			--Delete the child record version Company mappings that are not supported anymore in the parent Product and that are not tied to a local Company or a Company child record version whose mappings to Product is VersionSpecific (so that the local Company or the Company child record version does not loose its Product mappings when Product RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToCompany] a
			INNER JOIN @tblTargetChildRecordVersions b
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
			WHERE
			(
				(  -- mappings at a lower sitegroup/site to a child record version of the same Company record
					c.SiteGuid = b.SiteGuid
					AND c.CompanyGuid <> c._MasterRecordGuid
					AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a Company child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

				)		
				OR
				( -- mappings to the same Company master record, but at a lower sitegroup/site
					c.SiteGuid <> b.SiteGuid
					AND c.CompanyGuid = c._MasterRecordGuid
				)	
			)
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToCompany] d				
				WHERE d.ProductGuid = @SourceProductGuid
				AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @ownerSiteGuid)
			)
													
			--Update the child record version mappings that have been modified in the parent Product
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
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblProductToCompany] d
			ON d.ProductGuid = b.ProductGuid
			AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid)
			WHERE a.ProductGuid = @SourceProductGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblProductToCompany]
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), 
			 b.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToCompany] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToCompany] d
				WHERE d.ProductGuid = b.ProductGuid
				AND d.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
			)
		END

		
		-- Process [AuthorizedCustomers] External Field - [map].[tblProductToCompanyGroup]
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedCustomers') = 0)
		BEGIN
			--CompanyGroup is both an External Attribute of Product (i.e. Product-To-CompanyGroup mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Product-To-CompanyGroup mappings are also maintained as part of the CompanyGroup entity, i.e. outside of the Product entity)
			--Delete the child record version mappings that are not supported anymore in the parent Product, and which are not owned by a local CompanyGroup.
			--Local CompanyGroups can create mappings with Products, even if [AuthorizedCustomers] is set as non-VersionSpecifc.
			DELETE a FROM [map].[tblProductToCompanyGroup] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN tblApplicationString c
			ON c.ApplicationStringGuid = a.AssignedToApplicationStringGuid
			WHERE c.SiteGuid <> b.SiteGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToCompanyGroup] d				
				WHERE d.ProductGuid = @SourceProductGuid
				AND d.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
			)		

			--Update the child record version mappings that have been modified in the parent Product
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
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [map].[tblProductToCompanyGroup] d
			ON d.ProductGuid = b.ProductGuid
			AND d.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
			WHERE a.ProductGuid = @SourceProductGuid
					
			--Insert a new mapping for each parent mapping not found in the child record versions	
			INSERT INTO [map].[tblProductToCompanyGroup]
			(AssignedToApplicationStringGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT a.AssignedToApplicationStringGuid,
			 b.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToCompanyGroup] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToCompanyGroup] d
				WHERE d.ProductGuid = b.ProductGuid
				AND d.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
			)
		END


		-- Process [Messages] External Field - Regular Product Messages
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Messages') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent Product
			DELETE a FROM [map].[tblApplicationStringToProductMessage] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.ProductGuid = a.ProductGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblApplicationStringToProductMessage] d				
				WHERE d.ProductGuid = @SourceProductGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)	

			--Update the child record version mappings that have been modified in the parent Product
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblApplicationStringToProductMessage] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [map].[tblApplicationStringToProductMessage] d
			ON d.ProductGuid = b.ProductGuid
			AND d.ApplicationStringGuid = a.ApplicationStringGuid
			WHERE a.ProductGuid = @SourceProductGuid
						
			--Insert a new mapping for each parent mapping not found in the child record versions	
			INSERT INTO [map].[tblApplicationStringToProductMessage]
			(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT a.ApplicationStringGuid,
			 b.ProductGuid, a.Sequence, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblApplicationStringToProductMessage] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblApplicationStringToProductMessage] d
				WHERE d.ProductGuid = b.ProductGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)
		END

		-- Process [Messages] External Field - DOT Hazardous Product Messages
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Messages') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent Product
			DELETE a FROM [map].[tblApplicationStringToDotHazardousMessage] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.ProductGuid = a.ProductGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblApplicationStringToDotHazardousMessage] d				
				WHERE d.ProductGuid = @SourceProductGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)	

			--Update the child record version mappings that have been modified in the parent Product
			UPDATE d
			SET d.Sequence = a.Sequence, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblApplicationStringToDotHazardousMessage] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [map].[tblApplicationStringToDotHazardousMessage] d
			ON d.ProductGuid = b.ProductGuid
			AND d.ApplicationStringGuid = a.ApplicationStringGuid
			WHERE a.ProductGuid = @SourceProductGuid
					
			--Insert a new mapping for each parent mapping not found in the child record versions		
			INSERT INTO [map].[tblApplicationStringToDotHazardousMessage]
			(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT a.ApplicationStringGuid,
			 b.ProductGuid, a.Sequence, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblApplicationStringToDotHazardousMessage] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblApplicationStringToDotHazardousMessage] d
				WHERE d.ProductGuid = b.ProductGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)
		END

		-- Process [UnavailableInventories] External Field
		-- Company is both an External Attribute of Product (i.e. Company-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UnavailableInventories') = 0)
		BEGIN
			--Delete the child record version Company mappings that are not supported anymore in the parent Product and that are not tied to a local Company or a Company child record version whose mappings to Product is VersionSpecific (so that the local Company or the Company child record version does not loose its Product mappings when Product RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN @tblTargetChildRecordVersions b
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
			WHERE
			(
				(  -- mappings at a lower sitegroup/site to a child record version of the same Company record
					c.SiteGuid = b.SiteGuid
					AND c.CompanyGuid <> c._MasterRecordGuid
					AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a Company child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

				)		
				OR
				( -- mappings to the same Company master record, but at a lower sitegroup/site
					c.SiteGuid <> b.SiteGuid
					AND c.CompanyGuid = c._MasterRecordGuid
				)	
			)
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d				
				WHERE d.ProductGuid = @SourceProductGuid
				AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @ownerSiteGuid)
			)		
													
			--Update the child record version mappings that have been modified in the parent Product
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
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblProductToUnavailableInventoryCompany] d
			ON d.ProductGuid = b.ProductGuid
			AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid)
			WHERE a.ProductGuid = @SourceProductGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), 
			 b.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToUnavailableInventoryCompany] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d
				WHERE d.ProductGuid = b.ProductGuid
				AND d.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
			)
		END

		-- Process [SupplierAuthorizedProducts] External Field
		-- Company is both an External Attribute of Product (i.e. Company-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'SupplierAuthorizedProducts') = 0)
		BEGIN
			--Delete the child record version Company mappings that are not supported anymore in the parent Product and that are not tied to a local Company or a Company child record version whose mappings to Product is VersionSpecific (so that the local Company or the Company child record version does not loose its Product mappings when Product RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN @tblTargetChildRecordVersions b
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
			WHERE
			(
				(  -- mappings at a lower sitegroup/site to a child record version of the same Company record
					c.SiteGuid = b.SiteGuid
					AND c.CompanyGuid <> c._MasterRecordGuid
					AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a Company child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

				)		
				OR
				( -- mappings to the same Company master record, but at a lower sitegroup/site
					c.SiteGuid <> b.SiteGuid
					AND c.CompanyGuid = c._MasterRecordGuid
				)	
			)
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] d				
				WHERE d.ProductGuid = @SourceProductGuid
				AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @ownerSiteGuid)
			)		
													
			--Update the child record version mappings that have been modified in the parent Product
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
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblProductToSupplierProductCompany] d
			ON d.ProductGuid = b.ProductGuid
			AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid)
			WHERE a.ProductGuid = @SourceProductGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblProductToSupplierProductCompany]
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), 
			 b.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToSupplierProductCompany] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] d
				WHERE d.ProductGuid = b.ProductGuid
				AND d.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
			)
		END

		-- Process [TransactionAliasExclusion] External Field
		-- TransactionAlias is both an External Attribute of Product (i.e. TransactionAlias-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. TransactionAlias-To-Product mappings are also maintained as part of the TransactionAlias entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'TransactionAliasExclusion') = 0)
		BEGIN
			--Delete the child record version TransactionAlias mappings that are not supported anymore in the parent Product and that are not tied to a local TransactionAlias or a TransactionAlias child record version whose mappings to Product is VersionSpecific (so that the local Company or the Company child record version does not loose its Product mappings when Product RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.ProductGuid = a.ProductGuid
			INNER JOIN tblTransactionAliases c
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
			WHERE
			(
				(  -- mappings at a lower sitegroup/site to a child record version of the same TransactionAlias record
					c.SiteGuid = b.SiteGuid
					AND c.TransactionAliasGuid <> c._MasterRecordGuid
					AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a TransactionAlias child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

				)		
				OR
				( -- mappings to the same TransactionAlias master record, but at a lower sitegroup/site
					c.SiteGuid <> b.SiteGuid
					AND c.TransactionAliasGuid = c._MasterRecordGuid
				)	
			)
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d				
				WHERE d.ProductGuid = @SourceProductGuid
				AND d.AssignedToTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, @ownerSiteGuid)
			)			
													
			--Update the child record version mappings that have been modified in the parent Product
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
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			INNER JOIN [map].[tblProductToTransactionAliasExclusion] d
			ON d.ProductGuid = b.ProductGuid
			AND d.AssignedToTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, b.SiteGuid)
			WHERE a.ProductGuid = @SourceProductGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblProductToTransactionAliasExclusion]
			(AssignedToTransactionAliasGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, b.SiteGuid), a.AssignedToTransactionAliasGuid), 
			 b.ProductGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToTransactionAliasExclusion] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			WHERE a.ProductGuid = @SourceProductGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d
				WHERE d.ProductGuid = b.ProductGuid
				AND d.AssignedToTransactionAliasGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, b.SiteGuid), a.AssignedToTransactionAliasGuid)
			)
		END

		
		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --PropagateToChildRecordVersions
		END
	END TRY
	BEGIN CATCH        
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --PropagateToChildRecordVersions
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
						+ 'Procedure Name: [erv].usp_PropagateProductRevisionByEntityRecordChange' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
