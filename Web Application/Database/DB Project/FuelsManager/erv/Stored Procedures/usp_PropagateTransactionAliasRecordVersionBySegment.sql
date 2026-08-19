/*
	DROP PROCEDURE [erv].[usp_PropagateTransactionAliasRecordVersionBySegment]

	EXEC [erv].[usp_PropagateTransactionAliasRecordVersionBySegment] '7C313838-6CA6-4484-9DF2-2E21B6159B10', '00000000-0000-0000-0000-000000000001'

*/

CREATE PROCEDURE [erv].[usp_PropagateTransactionAliasRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagateTransactionAliasRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate all the Parent Specific fields of all the record versions in a TransactionAlias segment from a given sitegroup down to all the sites/sitegroups that have a direct assignment from the given sitegroup.
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

		--Capture the Site/SiteGroup, MasterRecordGuid, and TransactionAliasGuid of the child record versions that need to be updated.
		--This includes all the child record versions down the site hierarchy that have the same masterrecordguid as those owned by the SourceSiteGroup and which share the same filter value as the segment being processed, irrespective of where they were assigned from.		
		IF (@entityTypeId = 'Transaction_Alias')
		BEGIN
			INSERT INTO erv.tblTempTargetEntitySite
			(SiteGuid, MasterRecordGuid, EntityGuid, ParentEntityGuid, _CallingReferenceGuid)
			SELECT a.SiteGuid, a._MasterRecordGuid, a.TransactionAliasGuid, d.TransactionAliasGuid, @callingRefGuid
			FROM [dbo].[tblTransactionAliases] a
			INNER JOIN map.tblEntityTransactionAliasToSite b
			ON b.TransactionAliasGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid
			INNER JOIN tblTransactionAliases d
			ON d._MasterRecordGuid = b.TransactionAliasGuid
			AND d.SiteGuid = b.AssignedFromSiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about updating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no child record version to update in any case.
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND a.TransactionAliasGuid <> a._MasterRecordGuid
		END											
		
		IF (NOT EXISTS (SELECT * FROM erv.tblTempTargetEntitySite WHERE _CallingReferenceGuid = @callingRefGuid))
		BEGIN							
			RETURN;
		END

		--Build a table that has one flag column for each column of the tblTransactionAliases table, and set the flag according to whether the field is VersionSpecific or not.
		INSERT INTO erv.tblTempTransactionAliasRecordVersioningFlag
		(TransactionAliasGuid, _CallingReferenceGuid)
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
			SET	a.[AdditiveProfileCycleAmountUnitIndex] = (CASE e.[AdditiveProfileCycleAmountUnitIndex_RVFlag] WHEN 1 THEN a.[AdditiveProfileCycleAmountUnitIndex] ELSE b.[AdditiveProfileCycleAmountUnitIndex] END),
			a.[AdditiveProfileRateUnitIndex] = (CASE e.[AdditiveProfileRateUnitIndex_RVFlag] WHEN 1 THEN a.[AdditiveProfileRateUnitIndex] ELSE b.[AdditiveProfileRateUnitIndex] END),
			a.[AdditiveVolumeDecimalPlaces] = (CASE e.[AdditiveVolumeDecimalPlaces_RVFlag] WHEN 1 THEN a.[AdditiveVolumeDecimalPlaces] ELSE b.[AdditiveVolumeDecimalPlaces] END),
			a.[AdditiveVolumeUnitIndex] = (CASE e.[AdditiveVolumeUnitIndex_RVFlag] WHEN 1 THEN a.[AdditiveVolumeUnitIndex] ELSE b.[AdditiveVolumeUnitIndex] END),
			a.[AggregateAssocTrans] = (CASE e.[AggregateAssocTrans_RVFlag] WHEN 1 THEN a.[AggregateAssocTrans] ELSE b.[AggregateAssocTrans] END),
			a.[AliasName] = (CASE e.[AliasName_RVFlag] WHEN 1 THEN a.[AliasName] ELSE b.[AliasName] END),
			a.[AssociatedPreloadReport] = (CASE e.[AssociatedPreloadReport_RVFlag] WHEN 1 THEN a.[AssociatedPreloadReport] ELSE b.[AssociatedPreloadReport] END),
			a.[AssociatedReport] = (CASE e.[AssociatedReport_RVFlag] WHEN 1 THEN a.[AssociatedReport] ELSE b.[AssociatedReport] END),
			a.[AssociatedTransactionAliasGuid] = (CASE e.[AssociatedTransactionAliasGuid_RVFlag] WHEN 1 THEN a.[AssociatedTransactionAliasGuid] ELSE b.[AssociatedTransactionAliasGuid] END),
			a.[BulkShipment] = (CASE e.[BulkShipment_RVFlag] WHEN 1 THEN a.[BulkShipment] ELSE b.[BulkShipment] END),
			a.[DensityDecimalPlaces] = (CASE e.[DensityDecimalPlaces_RVFlag] WHEN 1 THEN a.[DensityDecimalPlaces] ELSE b.[DensityDecimalPlaces] END),
			a.[DensityUnitIndex] = (CASE e.[DensityUnitIndex_RVFlag] WHEN 1 THEN a.[DensityUnitIndex] ELSE b.[DensityUnitIndex] END),
			a.[DestinationEquipmentTypes1] = (CASE e.[DestinationEquipmentTypes1_RVFlag] WHEN 1 THEN a.[DestinationEquipmentTypes1] ELSE b.[DestinationEquipmentTypes1] END),
			a.[DestinationEquipmentTypes2] = (CASE e.[DestinationEquipmentTypes2_RVFlag] WHEN 1 THEN a.[DestinationEquipmentTypes2] ELSE b.[DestinationEquipmentTypes2] END),
			a.[DestinationEquipmentTypes3] = (CASE e.[DestinationEquipmentTypes3_RVFlag] WHEN 1 THEN a.[DestinationEquipmentTypes3] ELSE b.[DestinationEquipmentTypes3] END),
			a.[DistributedImpact] = (CASE e.[DistributedImpact_RVFlag] WHEN 1 THEN a.[DistributedImpact] ELSE b.[DistributedImpact] END),
			a.[EnableAutoCompleteControls] = (CASE e.[EnableAutoCompleteControls_RVFlag] WHEN 1 THEN a.[EnableAutoCompleteControls] ELSE b.[EnableAutoCompleteControls] END),
			a.[EnableQuantityToleranceExceededWarning] = (CASE e.[EnableQuantityToleranceExceededWarning_RVFlag] WHEN 1 THEN a.[EnableQuantityToleranceExceededWarning] ELSE b.[EnableQuantityToleranceExceededWarning] END),
			a.[EnableTotalQuantityExceededWarning] = (CASE e.[EnableTotalQuantityExceededWarning_RVFlag] WHEN 1 THEN a.[EnableTotalQuantityExceededWarning] ELSE b.[EnableTotalQuantityExceededWarning] END),
			a.[EnableTotalValueExceededWarning] = (CASE e.[EnableTotalValueExceededWarning_RVFlag] WHEN 1 THEN a.[EnableTotalValueExceededWarning] ELSE b.[EnableTotalValueExceededWarning] END),
			a.[EnableValueToleranceExceededWarning] = (CASE e.[EnableValueToleranceExceededWarning_RVFlag] WHEN 1 THEN a.[EnableValueToleranceExceededWarning] ELSE b.[EnableValueToleranceExceededWarning] END),
			a.[FlowDecimalPlaces] = (CASE e.[FlowDecimalPlaces_RVFlag] WHEN 1 THEN a.[FlowDecimalPlaces] ELSE b.[FlowDecimalPlaces] END),
			a.[FlowUnitIndex] = (CASE e.[FlowUnitIndex_RVFlag] WHEN 1 THEN a.[FlowUnitIndex] ELSE b.[FlowUnitIndex] END),
			a.[IncludeInDispatch] = (CASE e.[IncludeInDispatch_RVFlag] WHEN 1 THEN a.[IncludeInDispatch] ELSE b.[IncludeInDispatch] END),
			a.[LevelDecimalPlaces] = (CASE e.[LevelDecimalPlaces_RVFlag] WHEN 1 THEN a.[LevelDecimalPlaces] ELSE b.[LevelDecimalPlaces] END),
			a.[LevelUnitIndex] = (CASE e.[LevelUnitIndex_RVFlag] WHEN 1 THEN a.[LevelUnitIndex] ELSE b.[LevelUnitIndex] END),
			a.[LimitSelectionsBasedOnHierarchy] = (CASE e.[LimitSelectionsBasedOnHierarchy_RVFlag] WHEN 1 THEN a.[LimitSelectionsBasedOnHierarchy] ELSE b.[LimitSelectionsBasedOnHierarchy] END),
			a.[LineItemEditControl] = (CASE e.[LineItemEditControl_RVFlag] WHEN 1 THEN a.[LineItemEditControl] ELSE b.[LineItemEditControl] END),
			a.[LookupDefaultStatusIndex] = (CASE e.[LookupDefaultStatusIndex_RVFlag] WHEN 1 THEN a.[LookupDefaultStatusIndex] ELSE b.[LookupDefaultStatusIndex] END),
			a.[LookupTransTypeIndex] = (CASE e.[LookupTransTypeIndex_RVFlag] WHEN 1 THEN a.[LookupTransTypeIndex] ELSE b.[LookupTransTypeIndex] END),
			a.[MassDecimalPlaces] = (CASE e.[MassDecimalPlaces_RVFlag] WHEN 1 THEN a.[MassDecimalPlaces] ELSE b.[MassDecimalPlaces] END),
			a.[MassUnitIndex] = (CASE e.[MassUnitIndex_RVFlag] WHEN 1 THEN a.[MassUnitIndex] ELSE b.[MassUnitIndex] END),
			a.[MeterCloseout] = (CASE e.[MeterCloseout_RVFlag] WHEN 1 THEN a.[MeterCloseout] ELSE b.[MeterCloseout] END),
			a.[MultipleLineItems] = (CASE e.[MultipleLineItems_RVFlag] WHEN 1 THEN a.[MultipleLineItems] ELSE b.[MultipleLineItems] END),
			a.[MultipleTransportLineItems] = (CASE e.[MultipleTransportLineItems_RVFlag] WHEN 1 THEN a.[MultipleTransportLineItems] ELSE b.[MultipleTransportLineItems] END),
			a.[MultipleWeightReadings] = (CASE e.[MultipleWeightReadings_RVFlag] WHEN 1 THEN a.[MultipleWeightReadings] ELSE b.[MultipleWeightReadings] END),
			a.[PermitNonReferenceData] = (CASE e.[PermitNonReferenceData_RVFlag] WHEN 1 THEN a.[PermitNonReferenceData] ELSE b.[PermitNonReferenceData] END),
			a.[PressureDecimalPlaces] = (CASE e.[PressureDecimalPlaces_RVFlag] WHEN 1 THEN a.[PressureDecimalPlaces] ELSE b.[PressureDecimalPlaces] END),
			a.[PressureUnitIndex] = (CASE e.[PressureUnitIndex_RVFlag] WHEN 1 THEN a.[PressureUnitIndex] ELSE b.[PressureUnitIndex] END),
			a.[ShowCompanyName] = (CASE e.[ShowCompanyName_RVFlag] WHEN 1 THEN a.[ShowCompanyName] ELSE b.[ShowCompanyName] END),
			a.[SourceEquipmentTypes1] = (CASE e.[SourceEquipmentTypes1_RVFlag] WHEN 1 THEN a.[SourceEquipmentTypes1] ELSE b.[SourceEquipmentTypes1] END),
			a.[SourceEquipmentTypes2] = (CASE e.[SourceEquipmentTypes2_RVFlag] WHEN 1 THEN a.[SourceEquipmentTypes2] ELSE b.[SourceEquipmentTypes2] END),
			a.[SourceEquipmentTypes3] = (CASE e.[SourceEquipmentTypes3_RVFlag] WHEN 1 THEN a.[SourceEquipmentTypes3] ELSE b.[SourceEquipmentTypes3] END),
			a.[TemperatureDecimalPlaces] = (CASE e.[TemperatureDecimalPlaces_RVFlag] WHEN 1 THEN a.[TemperatureDecimalPlaces] ELSE b.[TemperatureDecimalPlaces] END),
			a.[TemperatureUnitIndex] = (CASE e.[TemperatureUnitIndex_RVFlag] WHEN 1 THEN a.[TemperatureUnitIndex] ELSE b.[TemperatureUnitIndex] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UseComboBoxControls] = (CASE e.[UseComboBoxControls_RVFlag] WHEN 1 THEN a.[UseComboBoxControls] ELSE b.[UseComboBoxControls] END),
			a.[VolumeDecimalPlaces] = (CASE e.[VolumeDecimalPlaces_RVFlag] WHEN 1 THEN a.[VolumeDecimalPlaces] ELSE b.[VolumeDecimalPlaces] END),
			a.[VolumeUnitIndex] = (CASE e.[VolumeUnitIndex_RVFlag] WHEN 1 THEN a.[VolumeUnitIndex] ELSE b.[VolumeUnitIndex] END),
			a.[WeightReadingEditControl] = (CASE e.[WeightReadingEditControl_RVFlag] WHEN 1 THEN a.[WeightReadingEditControl] ELSE b.[WeightReadingEditControl] END)		
		FROM tblTransactionAliases a
		INNER JOIN tblTransactionAliases b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempTargetEntitySite c
		ON c.EntityGuid = a.TransactionAliasGuid
		INNER JOIN erv.tblTempTargetEntitySite d
		ON d.ParentEntityGuid = b.TransactionAliasGuid
		INNER JOIN erv.tblTempTransactionAliasRecordVersioningFlag e
		ON e.TransactionAliasGuid = a._MasterRecordGuid
		WHERE e._CallingReferenceGuid = @callingRefGuid		
		AND c._CallingReferenceGuid = @callingRefGuid
		AND d._CallingReferenceGuid = @callingRefGuid

		DELETE erv.tblTempTransactionAliasRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRefGuid 

		-- Process those ParentSpecific External fields whose propagation require custom handling.
		DECLARE @tblParentSpecificExternalFields TABLE
		(
			TargetField nvarchar(100)
		)

		/*Process those ParentSpecific External fields whose propagation require custom handling. */

		-- Process [Associations] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Associations') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblAssociatedTransactionAliases] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.ParentTransactionAliasGuid
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.ChildTransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblAssociatedTransactionAliases] d				
				WHERE d.ParentTransactionAliasGuid = b.ParentEntityGuid
				AND d.ChildTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
			
			--No characteristics of the TransactionAlias Association mappings to update. The mappings are either inserted or deleted.
			
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblAssociatedTransactionAliases]
			(ParentTransactionAliasGuid, ChildTransactionAliasGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, b.SiteGuid), a.ChildTransactionAliasGuid), 			 
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblAssociatedTransactionAliases] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.ParentTransactionAliasGuid
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.ChildTransactionAliasGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblAssociatedTransactionAliases] d
				WHERE d.ParentTransactionAliasGuid = b.EntityGuid
				AND d.ChildTransactionAliasGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, b.SiteGuid), a.ChildTransactionAliasGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [Fields] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Fields') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [dbo].[tblTransactionAliasFields] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionAliasFields] d				
				WHERE d.TransactionAliasGuid = b.ParentEntityGuid
				AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
				AND d.DbName = a.DbName
			)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.AliasID = d.AliasID, 
			a.ClearOnNew = d.ClearOnNew,
			a.DispatchField = d.DispatchField,
			a.DisplayName = d.DisplayName,		
			a.Required = d.Required,
			a.UserGroupGuid = d.UserGroupGuid,
			a.Virtual = d.Virtual,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [dbo].[tblTransactionAliasFields] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.TransactionAliasGuid
			INNER JOIN [dbo].[tblTransactionAliasFields] d
			ON d.TransactionAliasGuid = b.ParentEntityGuid
			AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
			AND d.DbName = a.DbName
			WHERE b._CallingReferenceGuid = @callingRefGuid			
													
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [dbo].[tblTransactionAliasFields]
			(TransactionAliasGuid, AliasId, DbName, DisplayOrder, DisplayName, Required, Virtual, LookupTransactionFieldTypeIndex, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, 
			a.AliasID, a.DbName, a.DisplayOrder, a.DisplayName, a.Required, a.Virtual, a.LookupTransactionFieldTypeIndex, a.UserGroupGuid, a.DispatchField, a.ClearOnNew, 
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblTransactionAliasFields] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [dbo].[tblTransactionAliasFields] d
				WHERE d.TransactionAliasGuid = b.EntityGuid
				AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
				AND d.DbName = a.DbName
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [Products] External Field
		-- Product is both an External Attribute of TransactionAlias (i.e. TransactionAlias-To-Product mappings are maintained as part of the TransactionAlias entity), and an External Client of TransactionAlias (i.e. TransactionAlias-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the TransactionAlias entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Products') = 0)
		BEGIN
			--Only delete the child record version Product mappings that are not supported anymore in the parent TransactionAlias and that are not tied to a local Product or a Product child record version whose mappings to TransactionAlias is VersionSpecific (so that the local Product or the Product child record version does not loose its TransactionAlias mappings when TransactionAlias RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.AssignedToTransactionAliasGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN map.tblEntityProductToSite d
			ON d.ProductGuid = c._MasterRecordGuid
			AND d.SiteGuid = b.SiteGuid
			LEFT OUTER JOIN 
			(
				SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
				FROM erv.tblEntityRecordVersioningFieldConfig e1
				INNER JOIN erv.tblEntitySegmentTemplate e2
				ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
				WHERE e2.EntityTypeId = 'Product'
				AND TargetField = 'TransactionAliasExclusion'
			) e
			ON e.SiteGroupGuid = d.AssignedFromSiteGuid			
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.ProductGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local Product or by a Product child record version whose TransactionAliasExclusion field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d				
				WHERE d.AssignedToTransactionAliasGuid = b.ParentEntityGuid
				AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @SourceSiteGroupGuid)
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
			ON b.EntityGuid = a.AssignedToTransactionAliasGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToTransactionAliasExclusion] d
			ON d.AssignedToTransactionAliasGuid = b.ParentEntityGuid
			AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblProductToTransactionAliasExclusion]
			(ProductGuid, AssignedToTransactionAliasGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid), 
			 b.EntityGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.AssignedToTransactionAliasGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d
				WHERE d.AssignedToTransactionAliasGuid = b.EntityGuid
				AND d.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [Statuses] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Statuses') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblTransactionAliasToStatus] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblTransactionAliasToStatus] d				
				WHERE d.TransactionAliasGuid = b.ParentEntityGuid
				AND d.LookupTransactionStatusIndex = a.LookupTransactionStatusIndex
			)	
			AND b._CallingReferenceGuid = @callingRefGuid
													
			--No characteristics of the TransactionAliasStatuses mappings to update. The mappings are either inserted or deleted.

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblTransactionAliasToStatus]
			(LookupTransactionStatusIndex, TransactionAliasGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.LookupTransactionStatusIndex, b.EntityGuid, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblTransactionAliasToStatus] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblTransactionAliasToStatus] d
				WHERE d.TransactionAliasGuid = b.EntityGuid
				AND d.LookupTransactionStatusIndex = a.LookupTransactionStatusIndex
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [UserData] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UserData') = 0)
		BEGIN
			--UserData - [dbo].[tblUserDataFieldTransactionAlias] and [dbo].[tblUserDataListValueTransactionAlias]
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [dbo].[tblUserDataListValueTransactionAlias] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
			ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
			INNER JOIN erv.tblTempTargetEntitySite c
			ON c.EntityGuid = b.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAlias] e
				INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] f
				ON f.UserDataFieldTransactionAliasGuid = e.UserDataFieldTransactionAliasGuid				
				WHERE f.TransactionAliasGuid = c.ParentEntityGuid
				AND e.Value = a.Value
			)
			AND c._CallingReferenceGuid = @callingRefGuid

			--No characteristics of the TransactionAlias UserData ListValue mappings to update. There is only one field to set, the Value field, which during propagation is simply either inserted or deleted.
			
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [dbo].[tblUserDataFieldTransactionAlias] a
			INNER JOIN erv.tblTempTargetEntitySite c
			ON c.EntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAlias] e
				WHERE e.TransactionAliasGuid = c.ParentEntityGuid
				AND e.SiteGuid = @SourceSiteGroupGuid
				AND e.DisplayName = a.DisplayName
			)
			AND c._CallingReferenceGuid = @callingRefGuid
													
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.ClearOnNew = d.ClearOnNew, 
			a.DispatchField = d.DispatchField,
			a.LookupUserDataTypeIndex = d.LookupUserDataTypeIndex,
			a.Number = d.Number,
			a.UserGroupGuid = d.UserGroupGuid,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAlias] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.TransactionAliasGuid
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] d
			ON d.TransactionAliasGuid = b.ParentEntityGuid
			AND d.SiteGuid = @SourceSiteGroupGuid
			AND d.DisplayName = a.DisplayName
			WHERE b._CallingReferenceGuid = @callingRefGuid

			-- Insert a UserData record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData items.
			INSERT INTO [dbo].[tblUserDataFieldTransactionAlias] 
			(DisplayName, TransactionAliasGuid, SiteGuid, Number, DisplayOrder, LookupUserDataTypeIndex, Required, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.DisplayName, b.EntityGuid, b.SiteGuid, a.Number, a.DisplayOrder, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAlias] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAlias]  d
				WHERE d.TransactionAliasGuid = b.EntityGuid
				AND d.DisplayName = a.DisplayName
			)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Insert a UserData ListValue record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData ListValue items.
			INSERT INTO [dbo].[tblUserDataListValueTransactionAlias] 
			(UserDataFieldTransactionAliasGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT e.UserDataFieldTransactionAliasGuid, a.Value, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblUserDataListValueTransactionAlias] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
			ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
			INNER JOIN erv.tblTempTargetEntitySite c
			ON c.ParentEntityGuid = b.TransactionAliasGuid
			INNER JOIN tblTransactionAliases d
			ON d._MasterRecordGuid = c.MasterRecordGuid
			AND d.SiteGuid = c.SiteGuid
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] e
			ON e.TransactionAliasGuid = d.TransactionAliasGuid
			AND e.DisplayName = b.DisplayName
			WHERE d._MasterRecordGuid <> d.TransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAlias] f
				INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] g
				ON g.UserDataFieldTransactionAliasGuid = f.UserDataFieldTransactionAliasGuid
				WHERE g.TransactionAliasGuid = d.TransactionAliasGuid
				AND f.Value = a.Value
			)
			AND c._CallingReferenceGuid = @callingRefGuid

			--UserData - [dbo].[tblUserDataFieldTransactionAliasLineItem] and [dbo].[tblUserDataListValueTransactionAliasLineItem]
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
			ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
			INNER JOIN erv.tblTempTargetEntitySite c
			ON c.EntityGuid = b.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] e
				INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] f
				ON f.UserDataFieldTransactionAliasLineItemGuid = e.UserDataFieldTransactionAliasLineItemGuid				
				WHERE f.TransactionAliasGuid = c.ParentEntityGuid
				AND e.Value = a.Value
			)
			AND c._CallingReferenceGuid = @callingRefGuid

			--No characteristics of the TransactionAlias UserData ListValue mappings to update. There is only one field to set, the Value field, which during propagation is simply either inserted or deleted.
			


			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			INNER JOIN erv.tblTempTargetEntitySite c
			ON c.EntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] e
				WHERE e.TransactionAliasGuid = c.ParentEntityGuid
				AND e.SiteGuid = @SourceSiteGroupGuid
				AND e.DisplayName = a.DisplayName
			)
			AND c._CallingReferenceGuid = @callingRefGuid
													
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.ClearOnNew = d.ClearOnNew, 
			a.DispatchField = d.DispatchField,
			a.LookupUserDataTypeIndex = d.LookupUserDataTypeIndex,
			a.Number = d.Number,
			a.UserGroupGuid = d.UserGroupGuid,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.TransactionAliasGuid
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] d
			ON d.TransactionAliasGuid = b.ParentEntityGuid
			AND d.SiteGuid = @SourceSiteGroupGuid
			AND d.DisplayName = a.DisplayName
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Insert a UserData Line Item record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData Line Item items.
			INSERT INTO [dbo].[tblUserDataFieldTransactionAliasLineItem] 
			(DisplayName, TransactionAliasGuid, SiteGuid, Number, DisplayOrder, LookupUserDataTypeIndex, Required, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.DisplayName, b.EntityGuid, b.SiteGuid, a.Number, a.DisplayOrder, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem]  a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAliasLineItem]   d
				WHERE d.TransactionAliasGuid = b.EntityGuid
				AND d.DisplayName = a.DisplayName
			)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Insert a UserData LineItem ListValue record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData ListValue items.
			INSERT INTO [dbo].[tblUserDataListValueTransactionAliasLineItem] 
			(UserDataFieldTransactionAliasLineItemGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT e.UserDataFieldTransactionAliasLineItemGuid, a.Value, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblUserDataListValueTransactionAliasLineItem]  a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
			ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
			INNER JOIN erv.tblTempTargetEntitySite c
			ON c.ParentEntityGuid = b.TransactionAliasGuid
			INNER JOIN tblTransactionAliases d
			ON d._MasterRecordGuid = c.MasterRecordGuid
			AND d.SiteGuid = c.SiteGuid
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] e
			ON e.TransactionAliasGuid = d.TransactionAliasGuid
			AND e.DisplayName = b.DisplayName
			WHERE d._MasterRecordGuid <> d.TransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] f
				INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] g
				ON g.UserDataFieldTransactionAliasLineItemGuid = f.UserDataFieldTransactionAliasLineItemGuid
				WHERE g.TransactionAliasGuid = d.TransactionAliasGuid
				AND f.Value = a.Value
			)
			AND c._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [UserGroups] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UserGroups') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblGroupToTransactionAlias] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblGroupToTransactionAlias] d				
				WHERE d.TransactionAliasGuid = b.ParentEntityGuid
				AND d.GroupGuid= a.GroupGuid
			)	
			AND b._CallingReferenceGuid = @callingRefGuid
													
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.LookupRightIndex = d.LookupRightIndex,		
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblGroupToTransactionAlias] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.TransactionAliasGuid
			INNER JOIN [map].[tblGroupToTransactionAlias] d
			ON d.TransactionAliasGuid = b.ParentEntityGuid
			AND d.GroupGuid = a.GroupGuid
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblGroupToTransactionAlias]
			(TransactionAliasGuid, GroupGuid, LookupRightIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, a.GroupGuid, a.LookupRightIndex, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblGroupToTransactionAlias] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblGroupToTransactionAlias] d
				WHERE d.TransactionAliasGuid = b.EntityGuid
				AND d.GroupGuid = a.GroupGuid
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
						+ 'Procedure Name: [erv].usp_PropagateTransactionAliasRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO
