/*
	DROP PROCEDURE [erv].[usp_PropagateProductRecordVersionBySegment]

	EXEC [erv].[usp_PropagateProductRecordVersionBySegment] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagateProductRecordVersionBySegment] '1eacc1d7-292d-4932-bc59-9c02740c6c19'

*/

CREATE PROCEDURE [erv].[usp_PropagateProductRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagateProductRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate all the Parent Specific fields of all the record versions in a Product segment from a given sitegroup down to all the sites/sitegroups that have a direct assignment from the given sitegroup.
	-- This Stored Procedure is to be used to enforce the effect of fields being changed from VersionSpecific to ParentSpecific as a result of Field Level Control configuration changes.
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Entity Segment Template that needs to be processed.
	-- 3. @SourceSiteGroupGuid: Guid of the segment SiteGroup from which the ParentSpecific fields are to be propagated.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		--Retrieve the VersionSpecific fields for the entity record
		DECLARE @tblSourceVersionSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL,
			Processed bit
		)
		INSERT @tblSourceVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		EXEC erv.usp_GetVersionSpecificFieldsBySegment @EntitySegmentTemplateGuid, NULL, @SourceSiteGroupGuid

		IF (NOT EXISTS (SELECT * FROM @tblSourceVersionSpecificFields))
		BEGIN				
			/*
				All fields are ParentSpecific. This means that there will be no child record versions of the entity record for any site/sitegroup in the hierarchy below owner 
				sitegroup of the entity record, i.e. Record Versioning field data propagation does not apply.
			*/
			RETURN;
		END

		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()

		--Capture the Site/SiteGroup, MasterRecordGuid, and ProductGuid of the child record versions that need to be updated.
		--This includes all the child record versions down the site hierarchy that have the same masterrecordguid as those owned by the SourceSiteGroup and which share the same filter value as the segment being processed, irrespective of where they were assigned from.
		IF (@entityTypeId = 'Product')
		BEGIN
			INSERT INTO erv.tblTempTargetEntitySite
			(SiteGuid, MasterRecordGuid, EntityGuid, ParentEntityGuid, _CallingReferenceGuid)
			SELECT a.SiteGuid, a._MasterRecordGuid, a.ProductGuid, d.ProductGuid, @callingRefGuid
			FROM [dbo].[tblProducts] a
			INNER JOIN map.tblEntityProductToSite b
			ON b.ProductGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid
			INNER JOIN tblProducts d
			ON d._MasterRecordGuid = b.ProductGuid
			AND d.SiteGuid = b.AssignedFromSiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about updating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no child record version to update in any case.
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND a.ProductGuid <> a._MasterRecordGuid
		END											
		
		IF (NOT EXISTS (SELECT * FROM erv.tblTempTargetEntitySite WHERE _CallingReferenceGuid = @callingRefGuid))
		BEGIN							
			RETURN;
		END

		--Build a table that has one flag column for each column of the tblProducts table, and set the flag according to whether the field is VersionSpecific or not.
		INSERT INTO erv.tblTempProductRecordVersioningFlag
		(ProductGuid, _CallingReferenceGuid)
		SELECT DISTINCT MasterRecordGuid, @callingRefGuid FROM erv.tblTempTargetEntitySite WHERE _CallingReferenceGuid = @callingRefGuid

		EXEC [erv].[usp_PivotFLCConfigurationsForSegment] @EntitySegmentTemplateGuid, NULL, @SourceSiteGroupGuid, NULL, @callingRefGuid

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateToChildRecordVersions
            SET @BeginTran = 1   
		END  		

		-- Update all the internal ParentSpecific fields for all applicable sites and sitegroups
		UPDATE a
		SET	
			a.[ApplyStandardDensity] = (CASE e.[ApplyStandardDensity_RVFlag] WHEN 1 THEN a.[ApplyStandardDensity] ELSE b.[ApplyStandardDensity] END),
			a.[ApplyTemperatureLimits] = (CASE e.[ApplyTemperatureLimits_RVFlag] WHEN 1 THEN a.[ApplyTemperatureLimits] ELSE b.[ApplyTemperatureLimits] END),
			a.[ApplyVolumeCorrection] = (CASE e.[ApplyVolumeCorrection_RVFlag] WHEN 1 THEN a.[ApplyVolumeCorrection] ELSE b.[ApplyVolumeCorrection] END),
			a.[AutomaticCloseout] = (CASE e.[AutomaticCloseout_RVFlag] WHEN 1 THEN a.[AutomaticCloseout] ELSE b.[AutomaticCloseout] END),
			a.[AviationFuelFlag] = (CASE e.[AviationFuelFlag_RVFlag] WHEN 1 THEN a.[AviationFuelFlag] ELSE b.[AviationFuelFlag] END),
			a.[Bonded] = (CASE e.[Bonded_RVFlag] WHEN 1 THEN a.[Bonded] ELSE b.[Bonded] END),
			a.[Capitalize] = (CASE e.[Capitalize_RVFlag] WHEN 1 THEN a.[Capitalize] ELSE b.[Capitalize] END),
			a.[ComponentTolerance] = (CASE e.[ComponentTolerance_RVFlag] WHEN 1 THEN a.[ComponentTolerance] ELSE b.[ComponentTolerance] END),
			a.[ContaminationPromptLoadRackText] = (CASE e.[ContaminationPromptLoadRackText_RVFlag] WHEN 1 THEN a.[ContaminationPromptLoadRackText] ELSE b.[ContaminationPromptLoadRackText] END),
			a.[DensityDeadband] = (CASE e.[DensityDeadband_RVFlag] WHEN 1 THEN a.[DensityDeadband] ELSE b.[DensityDeadband] END),
			a.[DensityDecimalPlaces] = (CASE e.[DensityDecimalPlaces_RVFlag] WHEN 1 THEN a.[DensityDecimalPlaces] ELSE b.[DensityDecimalPlaces] END),
			a.[DensityHighLimit] = (CASE e.[DensityHighLimit_RVFlag] WHEN 1 THEN a.[DensityHighLimit] ELSE b.[DensityHighLimit] END),
			a.[DensityLowLimit] = (CASE e.[DensityLowLimit_RVFlag] WHEN 1 THEN a.[DensityLowLimit] ELSE b.[DensityLowLimit] END),
			a.[DensityUnitIndex] = (CASE e.[DensityUnitIndex_RVFlag] WHEN 1 THEN a.[DensityUnitIndex] ELSE b.[DensityUnitIndex] END),
			a.[Description] = (CASE e.[Description_RVFlag] WHEN 1 THEN a.[Description] ELSE b.[Description] END),
			a.[DielectricTolerance] = (CASE e.[DielectricTolerance_RVFlag] WHEN 1 THEN a.[DielectricTolerance] ELSE b.[DielectricTolerance] END),
			a.[FlowDecimalPlaces] = (CASE e.[FlowDecimalPlaces_RVFlag] WHEN 1 THEN a.[FlowDecimalPlaces] ELSE b.[FlowDecimalPlaces] END),
			a.[FlowUnitIndex] = (CASE e.[FlowUnitIndex_RVFlag] WHEN 1 THEN a.[FlowUnitIndex] ELSE b.[FlowUnitIndex] END),
			a.[GenericType] = (CASE e.[GenericType_RVFlag] WHEN 1 THEN a.[GenericType] ELSE b.[GenericType] END),
			a.[GroundFuel] = (CASE e.[GroundFuel_RVFlag] WHEN 1 THEN a.[GroundFuel] ELSE b.[GroundFuel] END),
			a.[HazardousMaterial] = (CASE e.[HazardousMaterial_RVFlag] WHEN 1 THEN a.[HazardousMaterial] ELSE b.[HazardousMaterial] END),
			a.[HiddenDate] = (CASE e.[HiddenDate_RVFlag] WHEN 1 THEN a.[HiddenDate] ELSE b.[HiddenDate] END),
			a.[InhibitAccounting] = (CASE e.[InhibitAccounting_RVFlag] WHEN 1 THEN a.[InhibitAccounting] ELSE b.[InhibitAccounting] END),
			a.[LevelDecimalPlaces] = (CASE e.[LevelDecimalPlaces_RVFlag] WHEN 1 THEN a.[LevelDecimalPlaces] ELSE b.[LevelDecimalPlaces] END),
			a.[LevelUnitIndex] = (CASE e.[LevelUnitIndex_RVFlag] WHEN 1 THEN a.[LevelUnitIndex] ELSE b.[LevelUnitIndex] END),
			a.[LoadByWeight] = (CASE e.[LoadByWeight_RVFlag] WHEN 1 THEN a.[LoadByWeight] ELSE b.[LoadByWeight] END),
			a.[LoadRackDisplayText] = (CASE e.[LoadRackDisplayText_RVFlag] WHEN 1 THEN a.[LoadRackDisplayText] ELSE b.[LoadRackDisplayText] END),
			a.[LockedOut] = (CASE e.[LockedOut_RVFlag] WHEN 1 THEN a.[LockedOut] ELSE b.[LockedOut] END),
			a.[LockedOutDate] = (CASE e.[LockedOutDate_RVFlag] WHEN 1 THEN a.[LockedOutDate] ELSE b.[LockedOutDate] END),
			a.[LockedOutReason] = (CASE e.[LockedOutReason_RVFlag] WHEN 1 THEN a.[LockedOutReason] ELSE b.[LockedOutReason] END),
			a.[LookupProductTypeIndex] = (CASE e.[LookupProductTypeIndex_RVFlag] WHEN 1 THEN a.[LookupProductTypeIndex] ELSE b.[LookupProductTypeIndex] END),
			a.[LowStockWarning] = (CASE e.[LowStockWarning_RVFlag] WHEN 1 THEN a.[LowStockWarning] ELSE b.[LowStockWarning] END),
			a.[MassDecimalPlaces] = (CASE e.[MassDecimalPlaces_RVFlag] WHEN 1 THEN a.[MassDecimalPlaces] ELSE b.[MassDecimalPlaces] END),
			a.[MassPackageSize] = (CASE e.[MassPackageSize_RVFlag] WHEN 1 THEN a.[MassPackageSize] ELSE b.[MassPackageSize] END),
			a.[MassUnitIndex] = (CASE e.[MassUnitIndex_RVFlag] WHEN 1 THEN a.[MassUnitIndex] ELSE b.[MassUnitIndex] END),
			a.[OctaneNumber] = (CASE e.[OctaneNumber_RVFlag] WHEN 1 THEN a.[OctaneNumber] ELSE b.[OctaneNumber] END),
			a.[PatternColor] = (CASE e.[PatternColor_RVFlag] WHEN 1 THEN a.[PatternColor] ELSE b.[PatternColor] END),
			a.[PatternNumber] = (CASE e.[PatternNumber_RVFlag] WHEN 1 THEN a.[PatternNumber] ELSE b.[PatternNumber] END),
			a.[PIDXCode] = (CASE e.[PIDXCode_RVFlag] WHEN 1 THEN a.[PIDXCode] ELSE b.[PIDXCode] END),
			a.[PIDXFamilyCode] = (CASE e.[PIDXFamilyCode_RVFlag] WHEN 1 THEN a.[PIDXFamilyCode] ELSE b.[PIDXFamilyCode] END),
			a.[IsEthanol] = b.[IsEthanol],
			a.[PressureDecimalPlaces] = (CASE e.[PressureDecimalPlaces_RVFlag] WHEN 1 THEN a.[PressureDecimalPlaces] ELSE b.[PressureDecimalPlaces] END),
			a.[PressureUnitIndex] = (CASE e.[PressureUnitIndex_RVFlag] WHEN 1 THEN a.[PressureUnitIndex] ELSE b.[PressureUnitIndex] END),
			a.[Price] = (CASE e.[Price_RVFlag] WHEN 1 THEN a.[Price] ELSE b.[Price] END),
			a.[ProductCode] = (CASE e.[ProductCode_RVFlag] WHEN 1 THEN a.[ProductCode] ELSE b.[ProductCode] END),
			a.[ProductColor] = (CASE e.[ProductColor_RVFlag] WHEN 1 THEN a.[ProductColor] ELSE b.[ProductColor] END),
			a.[ProductID] = (CASE e.[ProductID_RVFlag] WHEN 1 THEN a.[ProductID] ELSE b.[ProductID] END),
			a.[RegulatoryClass] = (CASE e.[RegulatoryClass_RVFlag] WHEN 1 THEN a.[RegulatoryClass] ELSE b.[RegulatoryClass] END),
			a.[ReidVaporPressure] = (CASE e.[ReidVaporPressure_RVFlag] WHEN 1 THEN a.[ReidVaporPressure] ELSE b.[ReidVaporPressure] END),
			a.[StandardDensity] = (CASE e.[StandardDensity_RVFlag] WHEN 1 THEN a.[StandardDensity] ELSE b.[StandardDensity] END),
			a.[StockResetDate] = (CASE e.[StockResetDate_RVFlag] WHEN 1 THEN a.[StockResetDate] ELSE b.[StockResetDate] END),
			a.[StockTrack] = (CASE e.[StockTrack_RVFlag] WHEN 1 THEN a.[StockTrack] ELSE b.[StockTrack] END),
			a.[TaxCode] = (CASE e.[TaxCode_RVFlag] WHEN 1 THEN a.[TaxCode] ELSE b.[TaxCode] END),
			a.[TemperatureDeadband] = (CASE e.[TemperatureDeadband_RVFlag] WHEN 1 THEN a.[TemperatureDeadband] ELSE b.[TemperatureDeadband] END),
			a.[TemperatureDecimalPlaces] = (CASE e.[TemperatureDecimalPlaces_RVFlag] WHEN 1 THEN a.[TemperatureDecimalPlaces] ELSE b.[TemperatureDecimalPlaces] END),
			a.[TemperatureHighLimit] = (CASE e.[TemperatureHighLimit_RVFlag] WHEN 1 THEN a.[TemperatureHighLimit] ELSE b.[TemperatureHighLimit] END),
			a.[TemperatureHiHiLimit] = (CASE e.[TemperatureHiHiLimit_RVFlag] WHEN 1 THEN a.[TemperatureHiHiLimit] ELSE b.[TemperatureHiHiLimit] END),
			a.[TemperatureLoLoLimit] = (CASE e.[TemperatureLoLoLimit_RVFlag] WHEN 1 THEN a.[TemperatureLoLoLimit] ELSE b.[TemperatureLoLoLimit] END),
			a.[TemperatureLowLimit] = (CASE e.[TemperatureLowLimit_RVFlag] WHEN 1 THEN a.[TemperatureLowLimit] ELSE b.[TemperatureLowLimit] END),
			a.[TemperatureUnitIndex] = (CASE e.[TemperatureUnitIndex_RVFlag] WHEN 1 THEN a.[TemperatureUnitIndex] ELSE b.[TemperatureUnitIndex] END),
			a.[TrackingProductGuid] = (CASE e.[TrackingProductGuid_RVFlag] WHEN 1 THEN a.[TrackingProductGuid] ELSE b.[TrackingProductGuid] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UserData1] = (CASE e.[UserData1_RVFlag] WHEN 1 THEN a.[UserData1] ELSE b.[UserData1] END),
			a.[UserData2] = (CASE e.[UserData2_RVFlag] WHEN 1 THEN a.[UserData2] ELSE b.[UserData2] END),
			a.[UserData3] = (CASE e.[UserData3_RVFlag] WHEN 1 THEN a.[UserData3] ELSE b.[UserData3] END),
			a.[UserData4] = (CASE e.[UserData4_RVFlag] WHEN 1 THEN a.[UserData4] ELSE b.[UserData4] END),
			a.[UserData5] = (CASE e.[UserData5_RVFlag] WHEN 1 THEN a.[UserData5] ELSE b.[UserData5] END),
			a.[UserData6] = (CASE e.[UserData6_RVFlag] WHEN 1 THEN a.[UserData6] ELSE b.[UserData6] END),
			a.[UserData7] = (CASE e.[UserData7_RVFlag] WHEN 1 THEN a.[UserData7] ELSE b.[UserData7] END),
			a.[UserData8] = (CASE e.[UserData8_RVFlag] WHEN 1 THEN a.[UserData8] ELSE b.[UserData8] END),
			a.[VaporRecovery] = (CASE e.[VaporRecovery_RVFlag] WHEN 1 THEN a.[VaporRecovery] ELSE b.[VaporRecovery] END),
			a.[VarianceTolerance] = (CASE e.[VarianceTolerance_RVFlag] WHEN 1 THEN a.[VarianceTolerance] ELSE b.[VarianceTolerance] END),
			a.[VcfModuleSettings] = (CASE e.[VcfModuleSettings_RVFlag] WHEN 1 THEN a.[VcfModuleSettings] ELSE b.[VcfModuleSettings] END),
			a.[VolumeDecimalPlaces] = (CASE e.[VolumeDecimalPlaces_RVFlag] WHEN 1 THEN a.[VolumeDecimalPlaces] ELSE b.[VolumeDecimalPlaces] END),
			a.[VolumePackageSize] = (CASE e.[VolumePackageSize_RVFlag] WHEN 1 THEN a.[VolumePackageSize] ELSE b.[VolumePackageSize] END),
			a.[VolumeUnitIndex] = (CASE e.[VolumeUnitIndex_RVFlag] WHEN 1 THEN a.[VolumeUnitIndex] ELSE b.[VolumeUnitIndex] END)
		FROM tblProducts a
		INNER JOIN tblProducts b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempTargetEntitySite c
		ON c.EntityGuid = a.ProductGuid
		INNER JOIN erv.tblTempTargetEntitySite d
		ON d.ParentEntityGuid = b.ProductGuid
		INNER JOIN erv.tblTempProductRecordVersioningFlag e
		ON e.ProductGuid = a._MasterRecordGuid
		WHERE e._CallingReferenceGuid = @callingRefGuid
		AND c._CallingReferenceGuid = @callingRefGuid
		AND d._CallingReferenceGuid = @callingRefGuid

		DELETE erv.tblTempProductRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRefGuid 

		-- Process those ParentSpecific External fields whose propagation require custom handling.
		DECLARE @tblParentSpecificExternalFields TABLE
		(
			TargetField nvarchar(100)
		)

		/*Process those ParentSpecific External fields whose propagation require custom handling. */
		-- Process [AuthorizedCustomers] External Field - [map].[tblProductToCompany]
		-- Company is both an External Attribute of Product (i.e. Company-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedCustomers') = 0)
		BEGIN
			--Delete the child record version Company mappings that are not supported anymore in the parent Product and that are not tied to a local Company or a Company child record version whose mappings to Product is VersionSpecific (so that the local Company or the Company child record version does not loose its Product mappings when Product RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
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
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.CompanyGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local Company or by a Company child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToCompany] d				
				WHERE d.ProductGuid = b.ParentEntityGuid
				AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
			
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.BlendPercentage = d.BlendPercentage,
			a.AdditiveRate = d.AdditiveRate,
			a.Ratio = d.Ratio,
			a.AdditiveCycleVolume = d.AdditiveCycleVolume,
			a.Tolerance = d.Tolerance,		
			a.PresetNumber = d.PresetNumber,
			a.AdditiveProfileGuid = d.AdditiveProfileGuid,
			a.TankGuid = d.TankGuid,
			a.MeterID = d.MeterID,
			a.ShipToProductID = d.ShipToProductID,
			a.ShipToProductCode = d.ShipToProductCode,
			a.ShipToLoadRackDisplayText = d.ShipToLoadRackDisplayText,
			a.UnavailableInventoryGross = d.UnavailableInventoryGross,
			a.UnavailableInventoryNet = d.UnavailableInventoryNet,
			a.SpecialInstructionNote = d.SpecialInstructionNote,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblProductToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblProductToCompany] d
			ON d.ProductGuid = b.ParentEntityGuid
			AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid
			
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblProductToCompany]
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), 
			 b.EntityGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToCompany] d
				WHERE d.ProductGuid = b.EntityGuid
				AND d.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
				
		END


		-- Process [AuthorizedCustomers] External Field - [map].[tblProductToCompanyGroup]
		--CompanyGroup is both an External Attribute of Product (i.e. Product-To-CompanyGroup mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Product-To-CompanyGroup mappings are also maintained as part of the CompanyGroup entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedCustomers') = 0)
		BEGIN	
			--Delete the child record version mappings that are not supported anymore in the parent Product, and which are not owned by a local CompanyGroup.
			--Local CompanyGroups can create mappings with Products, even if [AuthorizedCustomers] is set as ParentSpecifc.
			DELETE a FROM [map].[tblProductToCompanyGroup] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			INNEr JOIN tblApplicationString c
			ON c.ApplicationStringGuid = a.AssignedToApplicationStringGuid
			WHERE  c.SiteGuid <> b.SiteGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToCompanyGroup] d				
				WHERE d.ProductGuid = b.ParentEntityGuid
				AND d.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.BlendPercentage = d.BlendPercentage,
			a.AdditiveRate = d.AdditiveRate,
			a.Ratio = d.Ratio,
			a.AdditiveCycleVolume = d.AdditiveCycleVolume,
			a.Tolerance = d.Tolerance,		
			a.PresetNumber = d.PresetNumber,
			a.AdditiveProfileGuid = d.AdditiveProfileGuid,
			a.TankGuid = d.TankGuid,
			a.MeterID = d.MeterID,
			a.ShipToProductID = d.ShipToProductID,
			a.ShipToProductCode = d.ShipToProductCode,
			a.ShipToLoadRackDisplayText = d.ShipToLoadRackDisplayText,
			a.UnavailableInventoryGross = d.UnavailableInventoryGross,
			a.UnavailableInventoryNet = d.UnavailableInventoryNet,
			a.SpecialInstructionNote = d.SpecialInstructionNote,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblProductToCompanyGroup] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToCompanyGroup] d
			ON d.ProductGuid = b.ParentEntityGuid
			AND d.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
			WHERE b._CallingReferenceGuid = @callingRefGuid			

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblProductToCompanyGroup]
			(AssignedToApplicationStringGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.AssignedToApplicationStringGuid,
			 b.EntityGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToCompanyGroup] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.ProductGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToCompanyGroup] d
				WHERE d.ProductGuid = b.EntityGuid
				AND d.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [Messages] External Field - Regular Product Messages
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Messages') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblApplicationStringToProductMessage] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblApplicationStringToProductMessage] d				
				WHERE d.ProductGuid = b.ParentEntityGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
			
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblApplicationStringToProductMessage] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			INNER JOIN [map].[tblApplicationStringToProductMessage] d
			ON d.ProductGuid = b.ParentEntityGuid
			AND d.ApplicationStringGuid = a.ApplicationStringGuid
			WHERE b._CallingReferenceGuid = @callingRefGuid

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblApplicationStringToProductMessage]
			(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.ApplicationStringGuid, b.EntityGuid, a.Sequence, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblApplicationStringToProductMessage] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.ProductGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblApplicationStringToProductMessage] d
				WHERE d.ProductGuid = b.EntityGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid

		END

		-- Process [Messages] External Field - DOT Hazardous Product Messages
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Messages') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblApplicationStringToDotHazardousMessage] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblApplicationStringToDotHazardousMessage] d				
				WHERE d.ProductGuid = b.ParentEntityGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
			
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblApplicationStringToDotHazardousMessage] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			INNER JOIN [map].[tblApplicationStringToDotHazardousMessage] d
			ON d.ProductGuid = b.ParentEntityGuid
			AND d.ApplicationStringGuid = a.ApplicationStringGuid
			WHERE b._CallingReferenceGuid = @callingRefGuid

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblApplicationStringToDotHazardousMessage]
			(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.ApplicationStringGuid, b.EntityGuid, a.Sequence, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblApplicationStringToDotHazardousMessage] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.ProductGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblApplicationStringToDotHazardousMessage] d
				WHERE d.ProductGuid = b.EntityGuid
				AND d.ApplicationStringGuid = a.ApplicationStringGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END


		-- Process [UnavailableInventories] External Field
		-- Company is both an External Attribute of Product (i.e. Company-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UnavailableInventories') = 0)
		BEGIN
			--Delete the child record version Company mappings that are not supported anymore in the parent Product and that are not tied to a local Company or a Company child record version whose mappings to Product is VersionSpecific (so that the local Company or the Company child record version does not loose its Product mappings when Product RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
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
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.CompanyGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local Company or by a Company child record version whose UnavailableInventories field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d				
				WHERE d.ProductGuid = b.ParentEntityGuid
				AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
			
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.BlendPercentage = d.BlendPercentage,
			a.AdditiveRate = d.AdditiveRate,
			a.Ratio = d.Ratio,
			a.AdditiveCycleVolume = d.AdditiveCycleVolume,
			a.Tolerance = d.Tolerance,		
			a.PresetNumber = d.PresetNumber,
			a.AdditiveProfileGuid = d.AdditiveProfileGuid,
			a.TankGuid = d.TankGuid,
			a.MeterID = d.MeterID,
			a.ShipToProductID = d.ShipToProductID,
			a.ShipToProductCode = d.ShipToProductCode,
			a.ShipToLoadRackDisplayText = d.ShipToLoadRackDisplayText,
			a.UnavailableInventoryGross = d.UnavailableInventoryGross,
			a.UnavailableInventoryNet = d.UnavailableInventoryNet,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblProductToUnavailableInventoryCompany] d
			ON d.ProductGuid = b.ParentEntityGuid
			AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid
			
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), 
			 b.EntityGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d
				WHERE d.ProductGuid = b.EntityGuid
				AND d.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid						
		END

		-- Process [SupplierAuthorizedProducts] External Field
		-- Company is both an External Attribute of Product (i.e. Company-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. Company-To-Product mappings are also maintained as part of the Company entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'SupplierAuthorizedProducts') = 0)
		BEGIN
			--Delete the child record version Company mappings that are not supported anymore in the parent Product and that are not tied to a local Company or a Company child record version whose mappings to Product is VersionSpecific (so that the local Company or the Company child record version does not loose its Product mappings when Product RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
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
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.CompanyGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local Company or by a Company child record version whose SupplierAuthorizedProducts field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] d				
				WHERE d.ProductGuid = b.ParentEntityGuid
				AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
			
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.BlendPercentage = d.BlendPercentage,
			a.AdditiveRate = d.AdditiveRate,
			a.Ratio = d.Ratio,
			a.AdditiveCycleVolume = d.AdditiveCycleVolume,
			a.Tolerance = d.Tolerance,		
			a.PresetNumber = d.PresetNumber,
			a.AdditiveProfileGuid = d.AdditiveProfileGuid,
			a.TankGuid = d.TankGuid,
			a.MeterID = d.MeterID,
			a.ShipToProductID = d.ShipToProductID,
			a.ShipToProductCode = d.ShipToProductCode,
			a.ShipToLoadRackDisplayText = d.ShipToLoadRackDisplayText,
			a.UnavailableInventoryGross = d.UnavailableInventoryGross,
			a.UnavailableInventoryNet = d.UnavailableInventoryNet,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblProductToSupplierProductCompany] d
			ON d.ProductGuid = b.ParentEntityGuid
			AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid
			
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblProductToSupplierProductCompany]
			(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), 
			 b.EntityGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.ProductGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] d
				WHERE d.ProductGuid = b.EntityGuid
				AND d.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
			)	
			AND b._CallingReferenceGuid = @callingRefGuid						
		END


		-- Process [TransactionAliasExclusion] External Field
		-- TransactionAlias is both an External Attribute of Product (i.e. TransactionAlias-To-Product mappings are maintained as part of the Product entity), and an External Client of Product (i.e. TransactionAlias-To-Product mappings are also maintained as part of the TransactionAlias entity, i.e. outside of the Product entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'SupplierAuthorizedProducts') = 0)
		BEGIN
			--Delete the child record version Company mappings that are not supported anymore in the parent Product and that are not tied to a local TransactionAlias or a TransactionAlias child record version whose mappings to Product is VersionSpecific (so that the local TransactionAlias or the TransactionAlias child record version does not loose its Product mappings when Product RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
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
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.TransactionAliasGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local TransactionAlias or by a TransactionAlias child record version whose Products field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d				
				WHERE d.ProductGuid = b.ParentEntityGuid
				AND d.AssignedToTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
			
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.BlendPercentage = d.BlendPercentage,
			a.AdditiveRate = d.AdditiveRate,
			a.Ratio = d.Ratio,
			a.AdditiveCycleVolume = d.AdditiveCycleVolume,
			a.Tolerance = d.Tolerance,		
			a.PresetNumber = d.PresetNumber,
			a.AdditiveProfileGuid = d.AdditiveProfileGuid,
			a.TankGuid = d.TankGuid,
			a.MeterID = d.MeterID,
			a.ShipToProductID = d.ShipToProductID,
			a.ShipToProductCode = d.ShipToProductCode,
			a.ShipToLoadRackDisplayText = d.ShipToLoadRackDisplayText,
			a.UnavailableInventoryGross = d.UnavailableInventoryGross,
			a.UnavailableInventoryNet = d.UnavailableInventoryNet,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ProductGuid
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			INNER JOIN [map].[tblProductToTransactionAliasExclusion] d
			ON d.ProductGuid = b.ParentEntityGuid
			AND d.AssignedToTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid
			
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblProductToTransactionAliasExclusion]
			(AssignedToTransactionAliasGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, b.SiteGuid), a.AssignedToTransactionAliasGuid), 
			 b.EntityGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.ProductGuid
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d
				WHERE d.ProductGuid = b.EntityGuid
				AND d.AssignedToTransactionAliasGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, b.SiteGuid), a.AssignedToTransactionAliasGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid										
		END

		DELETE erv.tblTempTargetEntitySite
		WHERE _CallingReferenceGuid = @callingRefGuid

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
						+ 'Procedure Name: [erv].usp_PropagateProductRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO
