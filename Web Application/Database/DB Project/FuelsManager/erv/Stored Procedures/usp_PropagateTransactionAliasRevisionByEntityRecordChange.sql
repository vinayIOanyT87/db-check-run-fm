/*
	DROP PROCEDURE [erv].[usp_PropagateTransactionAliasRevisionByEntityRecordChange]

	EXEC [erv].[usp_PropagateTransactionAliasRevisionByEntityRecordChange] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagateTransactionAliasRevisionByEntityRecordChange] '0DC68ACA-11AD-4F43-AD2B-87609738C453'
*/

CREATE PROCEDURE [erv].[usp_PropagateTransactionAliasRevisionByEntityRecordChange]
(
	@SourceTransactionAliasGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagateTransactionAliasRevisionByEntityRecordChange] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate the current revision of a given TransactionAlias entity record down the site hierarchy, according to the rules established by the Field Level Control configurations.
	-- This Stored Procedure is to be used to propagate the effect of an entity record change down to all its children record versions.
	-- Notes:
	-- 1. @SourceTransactionAliasGuid: Guid of the TransactionAlias record that needs to be propagated down the site hierarchy. This should correspond to the exact record version that has been 
	--    changed (and not the parent record of the entity record).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Transaction_Alias'

		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		SELECT @ownerSiteGuid = SiteGuid, @masterRecordGuid = _MasterRecordGuid FROM tblTransactionAliases
		WHERE TransactionAliasGuid = @SourceTransactionAliasGuid

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
		FROM [erv].[udf_GetEntitySegmentsByEntityGuid] (@EntityTypeId, @SourceTransactionAliasGuid)
		
		IF NOT EXISTS (SELECT * FROM @tblSegmentInfo)
		BEGIN
			RAISERROR('Cannot locate the segment information for the selected entity record.',16,1); 
			RETURN;
		END

		DECLARE @assignedFromSiteGroupGuid uniqueidentifier
		IF (@SourceTransactionAliasGuid = @masterRecordGuid)
		BEGIN
			SET @assignedFromSiteGroupGuid = @ownerSiteGuid
		END
		ELSE
		BEGIN
			SET @assignedFromSiteGroupGuid = (SELECT [erv].[udf_GetEntityAssignedFromSite] (@EntityTypeId, @SourceTransactionAliasGuid, Null))
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
		FROM [erv].[udf_GetTransactionAliasToSiteHierarchyByRecordVersionGuid](@SourceTransactionAliasGuid)
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


		--Build a table that has one flag column for each column of the tblTransactionAliases table, and set the flag according to whether the field is VersionSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempTransactionAliasRecordVersioningFlag
		(TransactionAliasGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.TransactionAliasGuid, a.SiteGuid, @callingRef1Guid FROM tblTransactionAliases a
		INNER JOIN @tblEntityToSiteHierarchy b
		ON b.SiteGuid = a.SiteGuid
		WHERE a._MasterRecordGuid = @masterRecordGuid

		DECLARE @tblTargetChildRecordVersions TABLE
		(
			TransactionAliasGuid uniqueidentifier,
			SiteGuid uniqueidentifier,
			HierarchyLevel int,
			Processed bit
		)

		INSERT INTO @tblTargetChildRecordVersions
		(TransactionAliasGuid, SiteGuid, HierarchyLevel, Processed)
		SELECT a.TransactionAliasGuid, b.SiteGuid, c.HierarchyLevel, 0 FROM erv.tblTempTransactionAliasRecordVersioningFlag a
		INNER JOIN tblTransactionAliases b
		ON b.TransactionAliasGuid = a.TransactionAliasGuid
		INNER JOIN @tblEntityToSiteHierarchy c
		ON c.SiteGuid = b.SiteGuid
		WHERE b._MasterRecordGuid = @masterRecordGuid
		AND a._CallingReferenceGuid = @callingRef1Guid


		IF (NOT EXISTS (SELECT * FROM erv.tblTempTransactionAliasRecordVersioningFlag WHERE _CallingReferenceGuid = @callingRef1Guid))
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
		SET	a.[AdditiveProfileCycleAmountUnitIndex] = (CASE d.[AdditiveProfileCycleAmountUnitIndex_RVFlag] WHEN 1 THEN a.[AdditiveProfileCycleAmountUnitIndex] ELSE b.[AdditiveProfileCycleAmountUnitIndex] END),
			a.[AdditiveProfileRateUnitIndex] = (CASE d.[AdditiveProfileRateUnitIndex_RVFlag] WHEN 1 THEN a.[AdditiveProfileRateUnitIndex] ELSE b.[AdditiveProfileRateUnitIndex] END),
			a.[AdditiveVolumeDecimalPlaces] = (CASE d.[AdditiveVolumeDecimalPlaces_RVFlag] WHEN 1 THEN a.[AdditiveVolumeDecimalPlaces] ELSE b.[AdditiveVolumeDecimalPlaces] END),
			a.[AdditiveVolumeUnitIndex] = (CASE d.[AdditiveVolumeUnitIndex_RVFlag] WHEN 1 THEN a.[AdditiveVolumeUnitIndex] ELSE b.[AdditiveVolumeUnitIndex] END),
			a.[AggregateAssocTrans] = (CASE d.[AggregateAssocTrans_RVFlag] WHEN 1 THEN a.[AggregateAssocTrans] ELSE b.[AggregateAssocTrans] END),
			a.[AliasName] = (CASE d.[AliasName_RVFlag] WHEN 1 THEN a.[AliasName] ELSE b.[AliasName] END),
			a.[AssociatedPreloadReport] = (CASE d.[AssociatedPreloadReport_RVFlag] WHEN 1 THEN a.[AssociatedPreloadReport] ELSE b.[AssociatedPreloadReport] END),
			a.[AssociatedReport] = (CASE d.[AssociatedReport_RVFlag] WHEN 1 THEN a.[AssociatedReport] ELSE b.[AssociatedReport] END),
			a.[AssociatedTransactionAliasGuid] = (CASE d.[AssociatedTransactionAliasGuid_RVFlag] WHEN 1 THEN a.[AssociatedTransactionAliasGuid] ELSE b.[AssociatedTransactionAliasGuid] END),
			a.[BulkShipment] = (CASE d.[BulkShipment_RVFlag] WHEN 1 THEN a.[BulkShipment] ELSE b.[BulkShipment] END),
			a.[DensityDecimalPlaces] = (CASE d.[DensityDecimalPlaces_RVFlag] WHEN 1 THEN a.[DensityDecimalPlaces] ELSE b.[DensityDecimalPlaces] END),
			a.[DensityUnitIndex] = (CASE d.[DensityUnitIndex_RVFlag] WHEN 1 THEN a.[DensityUnitIndex] ELSE b.[DensityUnitIndex] END),
			a.[DestinationEquipmentTypes1] = (CASE d.[DestinationEquipmentTypes1_RVFlag] WHEN 1 THEN a.[DestinationEquipmentTypes1] ELSE b.[DestinationEquipmentTypes1] END),
			a.[DestinationEquipmentTypes2] = (CASE d.[DestinationEquipmentTypes2_RVFlag] WHEN 1 THEN a.[DestinationEquipmentTypes2] ELSE b.[DestinationEquipmentTypes2] END),
			a.[DestinationEquipmentTypes3] = (CASE d.[DestinationEquipmentTypes3_RVFlag] WHEN 1 THEN a.[DestinationEquipmentTypes3] ELSE b.[DestinationEquipmentTypes3] END),
			a.[DistributedImpact] = (CASE d.[DistributedImpact_RVFlag] WHEN 1 THEN a.[DistributedImpact] ELSE b.[DistributedImpact] END),
			a.[EnableAutoCompleteControls] = (CASE d.[EnableAutoCompleteControls_RVFlag] WHEN 1 THEN a.[EnableAutoCompleteControls] ELSE b.[EnableAutoCompleteControls] END),
			a.[EnableQuantityToleranceExceededWarning] = (CASE d.[EnableQuantityToleranceExceededWarning_RVFlag] WHEN 1 THEN a.[EnableQuantityToleranceExceededWarning] ELSE b.[EnableQuantityToleranceExceededWarning] END),
			a.[EnableTotalQuantityExceededWarning] = (CASE d.[EnableTotalQuantityExceededWarning_RVFlag] WHEN 1 THEN a.[EnableTotalQuantityExceededWarning] ELSE b.[EnableTotalQuantityExceededWarning] END),
			a.[EnableTotalValueExceededWarning] = (CASE d.[EnableTotalValueExceededWarning_RVFlag] WHEN 1 THEN a.[EnableTotalValueExceededWarning] ELSE b.[EnableTotalValueExceededWarning] END),
			a.[EnableValueToleranceExceededWarning] = (CASE d.[EnableValueToleranceExceededWarning_RVFlag] WHEN 1 THEN a.[EnableValueToleranceExceededWarning] ELSE b.[EnableValueToleranceExceededWarning] END),
			a.[FlowDecimalPlaces] = (CASE d.[FlowDecimalPlaces_RVFlag] WHEN 1 THEN a.[FlowDecimalPlaces] ELSE b.[FlowDecimalPlaces] END),
			a.[FlowUnitIndex] = (CASE d.[FlowUnitIndex_RVFlag] WHEN 1 THEN a.[FlowUnitIndex] ELSE b.[FlowUnitIndex] END),
			a.[IncludeInDispatch] = (CASE d.[IncludeInDispatch_RVFlag] WHEN 1 THEN a.[IncludeInDispatch] ELSE b.[IncludeInDispatch] END),
			a.[LevelDecimalPlaces] = (CASE d.[LevelDecimalPlaces_RVFlag] WHEN 1 THEN a.[LevelDecimalPlaces] ELSE b.[LevelDecimalPlaces] END),
			a.[LevelUnitIndex] = (CASE d.[LevelUnitIndex_RVFlag] WHEN 1 THEN a.[LevelUnitIndex] ELSE b.[LevelUnitIndex] END),
			a.[LimitSelectionsBasedOnHierarchy] = (CASE d.[LimitSelectionsBasedOnHierarchy_RVFlag] WHEN 1 THEN a.[LimitSelectionsBasedOnHierarchy] ELSE b.[LimitSelectionsBasedOnHierarchy] END),
			a.[LineItemEditControl] = (CASE d.[LineItemEditControl_RVFlag] WHEN 1 THEN a.[LineItemEditControl] ELSE b.[LineItemEditControl] END),
			a.[LookupDefaultStatusIndex] = (CASE d.[LookupDefaultStatusIndex_RVFlag] WHEN 1 THEN a.[LookupDefaultStatusIndex] ELSE b.[LookupDefaultStatusIndex] END),
			a.[LookupTransTypeIndex] = (CASE d.[LookupTransTypeIndex_RVFlag] WHEN 1 THEN a.[LookupTransTypeIndex] ELSE b.[LookupTransTypeIndex] END),
			a.[MassDecimalPlaces] = (CASE d.[MassDecimalPlaces_RVFlag] WHEN 1 THEN a.[MassDecimalPlaces] ELSE b.[MassDecimalPlaces] END),
			a.[MassUnitIndex] = (CASE d.[MassUnitIndex_RVFlag] WHEN 1 THEN a.[MassUnitIndex] ELSE b.[MassUnitIndex] END),
			a.[MeterCloseout] = (CASE d.[MeterCloseout_RVFlag] WHEN 1 THEN a.[MeterCloseout] ELSE b.[MeterCloseout] END),
			a.[MultipleLineItems] = (CASE d.[MultipleLineItems_RVFlag] WHEN 1 THEN a.[MultipleLineItems] ELSE b.[MultipleLineItems] END),
			a.[MultipleTransportLineItems] = (CASE d.[MultipleTransportLineItems_RVFlag] WHEN 1 THEN a.[MultipleTransportLineItems] ELSE b.[MultipleTransportLineItems] END),
			a.[MultipleWeightReadings] = (CASE d.[MultipleWeightReadings_RVFlag] WHEN 1 THEN a.[MultipleWeightReadings] ELSE b.[MultipleWeightReadings] END),
			a.[PermitNonReferenceData] = (CASE d.[PermitNonReferenceData_RVFlag] WHEN 1 THEN a.[PermitNonReferenceData] ELSE b.[PermitNonReferenceData] END),
			a.[PressureDecimalPlaces] = (CASE d.[PressureDecimalPlaces_RVFlag] WHEN 1 THEN a.[PressureDecimalPlaces] ELSE b.[PressureDecimalPlaces] END),
			a.[PressureUnitIndex] = (CASE d.[PressureUnitIndex_RVFlag] WHEN 1 THEN a.[PressureUnitIndex] ELSE b.[PressureUnitIndex] END),
			a.[ShowCompanyName] = (CASE d.[ShowCompanyName_RVFlag] WHEN 1 THEN a.[ShowCompanyName] ELSE b.[ShowCompanyName] END),
			a.[SourceEquipmentTypes1] = (CASE d.[SourceEquipmentTypes1_RVFlag] WHEN 1 THEN a.[SourceEquipmentTypes1] ELSE b.[SourceEquipmentTypes1] END),
			a.[SourceEquipmentTypes2] = (CASE d.[SourceEquipmentTypes2_RVFlag] WHEN 1 THEN a.[SourceEquipmentTypes2] ELSE b.[SourceEquipmentTypes2] END),
			a.[SourceEquipmentTypes3] = (CASE d.[SourceEquipmentTypes3_RVFlag] WHEN 1 THEN a.[SourceEquipmentTypes3] ELSE b.[SourceEquipmentTypes3] END),
			a.[TemperatureDecimalPlaces] = (CASE d.[TemperatureDecimalPlaces_RVFlag] WHEN 1 THEN a.[TemperatureDecimalPlaces] ELSE b.[TemperatureDecimalPlaces] END),
			a.[TemperatureUnitIndex] = (CASE d.[TemperatureUnitIndex_RVFlag] WHEN 1 THEN a.[TemperatureUnitIndex] ELSE b.[TemperatureUnitIndex] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UseComboBoxControls] = (CASE d.[UseComboBoxControls_RVFlag] WHEN 1 THEN a.[UseComboBoxControls] ELSE b.[UseComboBoxControls] END),
			a.[VolumeDecimalPlaces] = (CASE d.[VolumeDecimalPlaces_RVFlag] WHEN 1 THEN a.[VolumeDecimalPlaces] ELSE b.[VolumeDecimalPlaces] END),
			a.[VolumeUnitIndex] = (CASE d.[VolumeUnitIndex_RVFlag] WHEN 1 THEN a.[VolumeUnitIndex] ELSE b.[VolumeUnitIndex] END),
			a.[WeightReadingEditControl] = (CASE d.[WeightReadingEditControl_RVFlag] WHEN 1 THEN a.[WeightReadingEditControl] ELSE b.[WeightReadingEditControl] END)		
		FROM tblTransactionAliases a
		INNER JOIN tblTransactionAliases b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN @tblEntityToSiteHierarchy c
		ON a.SiteGuid = c.SiteGuid
		INNER JOIN erv.tblTempTransactionAliasRecordVersioningFlag d
		ON d.TransactionAliasGuid = a.TransactionAliasGuid
		WHERE b.TransactionAliasGuid = @SourceTransactionAliasGuid
		AND d._CallingReferenceGuid = @callingRef1Guid

		DELETE erv.tblTempTransactionAliasRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRef1Guid 


		
		/*Process those non-VersionSpecific External fields whose propagation require custom handling. */		
		-- Process [Associations] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Associations') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent TransactionAlias
			DELETE a FROM [map].[tblAssociatedTransactionAliases] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.TransactionAliasGuid = a.ParentTransactionAliasGuid
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.ChildTransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblAssociatedTransactionAliases] d				
				WHERE d.ParentTransactionAliasGuid = @SourceTransactionAliasGuid
				AND d.ChildTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, @ownerSiteGuid)
			)		
													
			--No characteristics of the TransactionAlias Association mappings to update. The mappings are either inserted or deleted.

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblAssociatedTransactionAliases]
			(ParentTransactionAliasGuid, ChildTransactionAliasGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.TransactionAliasGuid, ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, b.SiteGuid), a.ChildTransactionAliasGuid), 
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblAssociatedTransactionAliases] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.ChildTransactionAliasGuid
			WHERE a.ParentTransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblAssociatedTransactionAliases] d
				WHERE d.ParentTransactionAliasGuid = b.TransactionAliasGuid
				AND d.ChildTransactionAliasGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, b.SiteGuid), a.ChildTransactionAliasGuid)
			)
		END


		-- Process [Fields] External Fields
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Fields') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent TransactionAlias
			DELETE a FROM [dbo].[tblTransactionAliasFields] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionAliasFields] d				
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
				AND d.DbName = a.DbName
			)		
													
			--Update the child record version mappings that have been modified in the parent TransactionAlias
			UPDATE d
			SET d.AliasID = a.AliasID, 
			d.ClearOnNew = a.ClearOnNew,
			d.DispatchField = a.DispatchField,
			d.DisplayName = a.DisplayName,		
			d.Required = a.Required,
			d.UserGroupGuid = a.UserGroupGuid,
			d.Virtual = a.Virtual,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblTransactionAliasFields]  a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [dbo].[tblTransactionAliasFields]  d
			ON d.TransactionAliasGuid = b.TransactionAliasGuid
			AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
			AND d.DbName = a.DbName			
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [dbo].[tblTransactionAliasFields]
			(TransactionAliasGuid, AliasId, DbName, DisplayOrder, DisplayName, Required, Virtual, LookupTransactionFieldTypeIndex, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.TransactionAliasGuid, 
			a.AliasID, a.DbName, a.DisplayOrder, a.DisplayName, a.Required, a.Virtual, a.LookupTransactionFieldTypeIndex, a.UserGroupGuid, a.DispatchField, a.ClearOnNew, 
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblTransactionAliasFields] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblTransactionAliasFields] d
				WHERE d.TransactionAliasGuid = b.TransactionAliasGuid
				AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
				AND d.DbName = a.DbName
			)
		END

		-- Process [Products] External Field
		-- Product is both an External Attribute of TransactionAlias (i.e. TransactionAlias-To-Product mappings are maintained as part of the TransactionAlias entity), and an External Client of TransactionAlias (i.e. TransactionAlias-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the TransactionAlias entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Products') = 0)
		BEGIN
			--Only delete the child record version Product mappings that are not supported anymore in the parent TransactionAlias and that are not tied to a local Product or a Product child record version whose mappings to TransactionAlias is VersionSpecific (so that the local Product or the Product child record version does not loose its TransactionAlias mappings when TransactionAlias RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
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
			WHERE
			(
				(  -- mappings at a lower sitegroup/site to a child record version of the same Product record
					c.SiteGuid = b.SiteGuid
					AND c.ProductGuid <> c._MasterRecordGuid
					AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a Product child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

				)		
				OR
				( -- mappings to the same Product master record, but at a lower sitegroup/site
					c.SiteGuid <> b.SiteGuid
					AND c.ProductGuid = c._MasterRecordGuid
				)	
			)
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d				
				WHERE d.AssignedToTransactionAliasGuid = @SourceTransactionAliasGuid
				AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @ownerSiteGuid)
			)
													
			--Update the child record version mappings that have been modified in the parent TransactionAlias
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
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToTransactionAliasExclusion] d
			ON d.AssignedToTransactionAliasGuid= b.TransactionAliasGuid
			AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid)
			WHERE a.AssignedToTransactionAliasGuid = @SourceTransactionAliasGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblProductToTransactionAliasExclusion]
			(ProductGuid, AssignedToTransactionAliasGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, 
			ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid), 
			b.TransactionAliasGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
			a.MeterID, a.ShipToProductId, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToTransactionAliasExclusion] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			WHERE a.AssignedToTransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d
				WHERE d.AssignedToTransactionAliasGuid = b.TransactionAliasGuid
				AND d.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
			)
		END

		-- Process [Statuses] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Statuses') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent TransactionAlias
			DELETE a FROM [map].[tblTransactionAliasToStatus] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblTransactionAliasToStatus] d				
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND d.LookupTransactionStatusIndex = a.LookupTransactionStatusIndex
			)		
													
			--No characteristics of the TransactionAliasStatuses mappings to update. The mappings are either inserted or deleted.

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblTransactionAliasToStatus]
			(LookupTransactionStatusIndex, TransactionAliasGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.LookupTransactionStatusIndex, b.TransactionAliasGuid, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblTransactionAliasToStatus] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblTransactionAliasToStatus] d
				WHERE d.TransactionAliasGuid = b.TransactionAliasGuid
				AND d.LookupTransactionStatusIndex = a.LookupTransactionStatusIndex
			)
		END

		-- Process [UserData] External Field		
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UserData') = 0)
		BEGIN
			--UserData - [dbo].[tblUserDataFieldTransactionAlias] and [dbo].[tblUserDataListValueTransactionAlias]
			--Delete the child record version mappings that are not supported anymore in the parent TransactionAlias
			DELETE a FROM [dbo].[tblUserDataListValueTransactionAlias] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
			ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
			INNER JOIN @tblTargetChildRecordVersions c
			ON c.TransactionAliasGuid = b.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAlias] d	
				INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] e	
				ON e.UserDataFieldTransactionAliasGuid = d.UserDataFieldTransactionAliasGuid
				WHERE e.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND d.Value = a.Value
			)		
													
			--No characteristics of the TransactionAlias UserData ListValue mappings to update. There is only one field to set, the Value field, which during propagation is simply either inserted or deleted.

			--Delete the child record version mappings that are not supported anymore in the parent TransactionAlias
			DELETE a FROM [dbo].[tblUserDataFieldTransactionAlias] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAlias] d				
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND d.SiteGuid = @ownerSiteGuid
				AND d.DisplayName = a.DisplayName
			)		

			--Update the child record version mappings that have been modified in the parent TransactionAlias
			UPDATE d
			SET d.ClearOnNew = a.ClearOnNew, 
			d.DispatchField = a.DispatchField,
			d.LookupUserDataTypeIndex = a.LookupUserDataTypeIndex,
			d.Number = a.Number,
			d.UserGroupGuid = a.UserGroupGuid,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAlias] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] d
			ON d.TransactionAliasGuid = b.TransactionAliasGuid
			AND d.SiteGuid = b.SiteGuid
			AND d.DisplayName = a.DisplayName
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND a.SiteGuid = @ownerSiteGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [dbo].[tblUserDataFieldTransactionAlias] 
			(TransactionAliasGuid, SiteGuid, Number, DisplayOrder, DisplayName, LookupUserDataTypeIndex, Required, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.TransactionAliasGuid, b.SiteGuid, a.Number, a.DisplayOrder, a.DisplayName, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAlias]  a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAlias] d
				WHERE d.TransactionAliasGuid = b.TransactionAliasGuid
				AND d.SiteGuid = b.SiteGuid
				AND d.DisplayName = a.DisplayName
			)

			-- Insert a UserData ListValue record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData ListValue items.
			DECLARE @tblTargetUserDataListValueRecordVersions TABLE
			(
				TransactionAliasGuid uniqueidentifier,
				SiteGuid uniqueidentifier,
				DisplayName nvarchar(30),
				Value nvarchar(120),
				UserDataFieldTransactionAliasGuid uniqueidentifier,
				CreatedBy [dbo].[udtUserID] NULL,
				UpdatedBy [dbo].[udtUserID] NULL 
			)
			INSERT INTO @tblTargetUserDataListValueRecordVersions
			(TransactionAliasGuid, SiteGuid, DisplayName, Value, CreatedBy, UpdatedBy)
			SELECT c.TransactionAliasGuid, c.SiteGuid, b.DisplayName, a.Value, a.CreatedBy, a.UpdatedBy
			FROM [dbo].[tblUserDataListValueTransactionAlias] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
			ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
			CROSS JOIN @tblTargetChildRecordVersions c
			WHERE b.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAlias] d
				INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] e
				ON e.UserDataFieldTransactionAliasGuid = d.UserDataFieldTransactionAliasGuid
				WHERE e.TransactionAliasGuid = c.TransactionAliasGuid
				AND d.Value = a.Value
			)

			UPDATE a
			SET a.UserDataFieldTransactionAliasGuid = b.UserDataFieldTransactionAliasGuid
			FROM  @tblTargetUserDataListValueRecordVersions a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			AND b.SiteGuid = a.SiteGuid
			AND b.DisplayName = a.DisplayName

			INSERT INTO [dbo].[tblUserDataListValueTransactionAlias] 
			(UserDataFieldTransactionAliasGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT UserDataFieldTransactionAliasGuid, Value, GETDATE(), CreatedBy, GETDATE(), UpdatedBy
			FROM @tblTargetUserDataListValueRecordVersions

			--UserData - [dbo].[tblUserDataFieldTransactionAliasLineItem] and [dbo].[tblUserDataListValueTransactionAliasLineItem]
			--Delete the child record version mappings that are not supported anymore in the parent TransactionAlias
			DELETE a FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
			ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
			INNER JOIN @tblTargetChildRecordVersions c
			ON c.TransactionAliasGuid = b.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] d	
				INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] e	
				ON e.UserDataFieldTransactionAliasLineItemGuid = d.UserDataFieldTransactionAliasLineItemGuid
				WHERE e.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND d.Value = a.Value
			)		
													
			--No characteristics of the TransactionAlias UserData ListValue mappings to update. There is only one field to set, the Value field, which during propagation is simply either inserted or deleted.

			--Delete the child record version mappings that are not supported anymore in the parent TransactionAlias
			DELETE a FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] d				
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND d.SiteGuid = @ownerSiteGuid
				AND d.DisplayName = a.DisplayName
			)		

			--Update the child record version mappings that have been modified in the parent TransactionAlias
			UPDATE d
			SET d.ClearOnNew = a.ClearOnNew, 
			d.DispatchField = a.DispatchField,
			d.LookupUserDataTypeIndex = a.LookupUserDataTypeIndex,
			d.Number = a.Number,
			d.UserGroupGuid = a.UserGroupGuid,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] d
			ON d.TransactionAliasGuid = b.TransactionAliasGuid
			AND d.SiteGuid = b.SiteGuid
			AND d.DisplayName = a.DisplayName
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND a.SiteGuid = @ownerSiteGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [dbo].[tblUserDataFieldTransactionAliasLineItem]
			(TransactionAliasGuid, SiteGuid, Number, DisplayOrder, DisplayName, LookupUserDataTypeIndex, Required, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.TransactionAliasGuid, b.SiteGuid, a.Number, a.DisplayOrder, a.DisplayName, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] d
				WHERE d.TransactionAliasGuid = b.TransactionAliasGuid
				AND d.SiteGuid = b.SiteGuid
				AND d.DisplayName = a.DisplayName
			)

			-- Insert a UserData ListValue record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData ListValue items.
			DECLARE @tblTargetUserDataListValueLineItemRecordVersions TABLE
			(
				TransactionAliasGuid uniqueidentifier,
				SiteGuid uniqueidentifier,
				DisplayName nvarchar(30),
				Value nvarchar(120),
				UserDataFieldTransactionAliasLineItemGuid uniqueidentifier,
				CreatedBy [dbo].[udtUserID] NULL,
				UpdatedBy [dbo].[udtUserID] NULL 
			)
			
			INSERT INTO @tblTargetUserDataListValueLineItemRecordVersions 
			(TransactionAliasGuid, SiteGuid, DisplayName, Value, CreatedBy, UpdatedBy)
			SELECT c.TransactionAliasGuid, c.SiteGuid, b.DisplayName, a.Value, a.CreatedBy, a.UpdatedBy
			FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem]	 b
			ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
			CROSS JOIN @tblTargetChildRecordVersions c
			WHERE b.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAliasLineItem]d
				INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] e
				ON e.UserDataFieldTransactionAliasLineItemGuid = d.UserDataFieldTransactionAliasLineItemGuid
				WHERE e.TransactionAliasGuid = c.TransactionAliasGuid
				AND d.Value = a.Value
			)

			UPDATE a
			SET a.UserDataFieldTransactionAliasLineItemGuid = b.UserDataFieldTransactionAliasLineItemGuid
			FROM  @tblTargetUserDataListValueLineItemRecordVersions a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			AND b.SiteGuid = a.SiteGuid
			AND b.DisplayName = a.DisplayName

			INSERT INTO [dbo].[tblUserDataListValueTransactionAliasLineItem]	
			(UserDataFieldTransactionAliasLineItemGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT UserDataFieldTransactionAliasLineItemGuid, Value, GETDATE(), CreatedBy, GETDATE(), UpdatedBy
			FROM @tblTargetUserDataListValueLineItemRecordVersions
		END

		-- Process [UserGroups] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UserGroups') = 0)
		BEGIN
			--Delete the child record version mappings that are not supported anymore in the parent TransactionAlias
			DELETE a FROM [map].[tblGroupToTransactionAlias] a
			INNER JOIN @tblTargetChildRecordVersions b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblGroupToTransactionAlias] d				
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND d.GroupGuid = a.GroupGuid
			)		
													
			--Update the child record version mappings that have been modified in the parent TransactionAlias
			UPDATE d
			SET d.LookupRightIndex = a.LookupRightIndex,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblGroupToTransactionAlias] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [map].[tblGroupToTransactionAlias] d
			ON d.TransactionAliasGuid = b.TransactionAliasGuid
			AND d.GroupGuid = a.GroupGuid
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid

			--Insert a new mapping for each parent mapping not found in the child record versions
			INSERT INTO [map].[tblGroupToTransactionAlias]
			(TransactionAliasGuid, GroupGuid, LookupRightIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.TransactionAliasGuid, a.GroupGuid, a.LookupRightIndex, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblGroupToTransactionAlias] a
			CROSS JOIN @tblTargetChildRecordVersions b
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblGroupToTransactionAlias] d
				WHERE d.TransactionAliasGuid = b.TransactionAliasGuid
				AND d.GroupGuid = a.GroupGuid
			)
		END

		-- Process [FieldOrder] External Field
		--FieldOrder is maintained through the DisplayOrder field that is located in three separate tables: [dbo].[tblTransactionAliasFields], [dbo].[tblUserDataFieldTransactionAlias], and0 [dbo].[tblUserDataFieldTransactionAliasLineItem].
		--The Insertion and Deletion propagation that has already been taken care of further up this Stored Prcedure, would have also taken care of the Insert and Delete propagation needs of the FieldOrder external field.
		--Only the FieldOrder Update propagation is left to be handled separately below.
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'FieldOrder') = 0)
		BEGIN													
			UPDATE d
			SET d.DisplayOrder = a.DisplayOrder, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblTransactionAliasFields]  a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [dbo].[tblTransactionAliasFields]  d
			ON d.TransactionAliasGuid = b.TransactionAliasGuid
			AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
			AND d.DbName = a.DbName			
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid

			UPDATE d
			SET d.DisplayOrder = a.DisplayOrder, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAlias] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] d
			ON d.TransactionAliasGuid = b.TransactionAliasGuid
			AND d.SiteGuid = b.SiteGuid
			AND d.DisplayName = a.DisplayName
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND a.SiteGuid = @ownerSiteGuid

			UPDATE d
			SET d.DisplayOrder = a.DisplayOrder, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			CROSS JOIN @tblTargetChildRecordVersions b
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] d
			ON d.TransactionAliasGuid = b.TransactionAliasGuid
			AND d.SiteGuid = b.SiteGuid
			AND d.DisplayName = a.DisplayName
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND a.SiteGuid = @ownerSiteGuid
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
						+ 'Procedure Name: [erv].usp_PropagateTransactionAliasRevisionByEntityRecordChange' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
