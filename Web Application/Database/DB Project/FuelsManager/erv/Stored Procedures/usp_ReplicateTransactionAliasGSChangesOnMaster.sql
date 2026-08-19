/*
	DROP PROCEDURE [erv].[usp_ReplicateTransactionAliasGSChangesOnMaster]

	EXEC [erv].[usp_ReplicateTransactionAliasGSChangesOnMaster] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_ReplicateTransactionAliasGSChangesOnMaster] '0DC68ACA-11AD-4F43-AD2B-87609738C453'
*/

CREATE PROCEDURE [erv].[usp_ReplicateTransactionAliasGSChangesOnMaster]
(
	@SourceTransactionAliasGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_ReplicateTransactionAliasGSChangesOnMaster] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Replicate the Global Specific field values of a TransactionAlias child record version onto the Master Record copy.
	--          By replicating those field values onto the master record, we ensure that when the non-VersionSpecific
	--          fields of the master record are propagated down the site hierarchy, that all the GlobalSpecific changes made onto the
	--          the child record version will get propagated onto all the sitegroups and sites where the master record is assigned.
	-- Notes:
	-- 1. @SourceTransactionAliasGuid: Guid of the TransactionAlias child record version record whose GlobalSpecific fields needs to be replicated to its local Master Record copy 
	--    (and not the parent record of the entity record).
	-- 2. Whereas RecordVersioning propagation is limited to child record versions, the GlobalSpecific field replication targets the master records and allows
	--    modifications to the master records. This also applies to external attributres that represent a reference to another RecordVersioning entity (e.g. Product).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Transaction_Alias'

		DECLARE @masterSiteGuid uniqueidentifier
		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		DECLARE @assignedFromSiteGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid FROM dbo.tblTransactionAliases
		WHERE TransactionAliasGuid = @SourceTransactionAliasGuid
		AND TransactionAliasGuid <> _MasterRecordGuid

		IF (@masterRecordGuid IS NULL)
		BEGIN
			RAISERROR('Cannot locate the source child record for data replication.',16,1); 
			RETURN;
		END

		IF ((SELECT COUNT(*) FROM dbo.tblTransactionAliases WHERE TransactionAliasGuid = @masterRecordGuid AND _MasterRecordGuid = @masterRecordGuid) = 0)
		BEGIN
			RAISERROR('Cannot locate the target master record for data replication.',16,1); 
			RETURN;
		END

		SELECT @masterSiteGuid = SiteGuid FROM dbo.tblTransactionAliases
		WHERE TransactionAliasGuid = @masterRecordGuid
		AND TransactionAliasGuid = _MasterRecordGuid

		SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityTransactionAliasToSite 
		WHERE TransactionAliasGuid = @masterRecordGuid 
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
		FROM [erv].[udf_GetEntitySegmentsByEntityGuid] (@EntityTypeId, @SourceTransactionAliasGuid)
		
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

		--Build a table that has one flag column for each column of the tblTransactionAliases table, and set the flag according to whether the field is GlobalSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempTransactionAliasRecordVersioningFlag
		(TransactionAliasGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.TransactionAliasGuid, a.SiteGuid, @callingRef1Guid FROM tblTransactionAliases a
		WHERE a._MasterRecordGuid = @masterRecordGuid
		AND a.TransactionAliasGuid = a._MasterRecordGuid

		EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] @EntityTypeId, @masterRecordGuid, @assignedFromSiteGuid, @callingRef2Guid, @callingRef1Guid

		DELETE erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid
		

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --ReplicateToMasterRecord
            SET @BeginTran = 1   
		END  	
		

		-- Update all the internal ParentSpecific fields for all applicable child record versions
		UPDATE a
		SET	a.[AdditiveProfileCycleAmountUnitIndex] = (CASE d.[AdditiveProfileCycleAmountUnitIndex_RVFlag] WHEN 1 THEN b.[AdditiveProfileCycleAmountUnitIndex] ELSE a.[AdditiveProfileCycleAmountUnitIndex] END),
			a.[AdditiveProfileRateUnitIndex] = (CASE d.[AdditiveProfileRateUnitIndex_RVFlag] WHEN 1 THEN b.[AdditiveProfileRateUnitIndex] ELSE a.[AdditiveProfileRateUnitIndex] END),
			a.[AdditiveVolumeDecimalPlaces] = (CASE d.[AdditiveVolumeDecimalPlaces_RVFlag] WHEN 1 THEN b.[AdditiveVolumeDecimalPlaces] ELSE a.[AdditiveVolumeDecimalPlaces] END),
			a.[AdditiveVolumeUnitIndex] = (CASE d.[AdditiveVolumeUnitIndex_RVFlag] WHEN 1 THEN b.[AdditiveVolumeUnitIndex] ELSE a.[AdditiveVolumeUnitIndex] END),
			a.[AggregateAssocTrans] = (CASE d.[AggregateAssocTrans_RVFlag] WHEN 1 THEN b.[AggregateAssocTrans] ELSE a.[AggregateAssocTrans] END),
			a.[AliasName] = (CASE d.[AliasName_RVFlag] WHEN 1 THEN b.[AliasName] ELSE a.[AliasName] END),
			a.[AssociatedPreloadReport] = (CASE d.[AssociatedPreloadReport_RVFlag] WHEN 1 THEN b.[AssociatedPreloadReport] ELSE a.[AssociatedPreloadReport] END),
			a.[AssociatedReport] = (CASE d.[AssociatedReport_RVFlag] WHEN 1 THEN b.[AssociatedReport] ELSE a.[AssociatedReport] END),
			a.[AssociatedTransactionAliasGuid] = (CASE d.[AssociatedTransactionAliasGuid_RVFlag] WHEN 1 THEN b.[AssociatedTransactionAliasGuid] ELSE a.[AssociatedTransactionAliasGuid] END),
			a.[BulkShipment] = (CASE d.[BulkShipment_RVFlag] WHEN 1 THEN b.[BulkShipment] ELSE a.[BulkShipment] END),
			a.[DensityDecimalPlaces] = (CASE d.[DensityDecimalPlaces_RVFlag] WHEN 1 THEN b.[DensityDecimalPlaces] ELSE a.[DensityDecimalPlaces] END),
			a.[DensityUnitIndex] = (CASE d.[DensityUnitIndex_RVFlag] WHEN 1 THEN b.[DensityUnitIndex] ELSE a.[DensityUnitIndex] END),
			a.[DestinationEquipmentTypes1] = (CASE d.[DestinationEquipmentTypes1_RVFlag] WHEN 1 THEN b.[DestinationEquipmentTypes1] ELSE a.[DestinationEquipmentTypes1] END),
			a.[DestinationEquipmentTypes2] = (CASE d.[DestinationEquipmentTypes2_RVFlag] WHEN 1 THEN b.[DestinationEquipmentTypes2] ELSE a.[DestinationEquipmentTypes2] END),
			a.[DestinationEquipmentTypes3] = (CASE d.[DestinationEquipmentTypes3_RVFlag] WHEN 1 THEN b.[DestinationEquipmentTypes3] ELSE a.[DestinationEquipmentTypes3] END),
			a.[DistributedImpact] = (CASE d.[DistributedImpact_RVFlag] WHEN 1 THEN b.[DistributedImpact] ELSE a.[DistributedImpact] END),
			a.[EnableAutoCompleteControls] = (CASE d.[EnableAutoCompleteControls_RVFlag] WHEN 1 THEN b.[EnableAutoCompleteControls] ELSE a.[EnableAutoCompleteControls] END),
			a.[EnableQuantityToleranceExceededWarning] = (CASE d.[EnableQuantityToleranceExceededWarning_RVFlag] WHEN 1 THEN b.[EnableQuantityToleranceExceededWarning] ELSE a.[EnableQuantityToleranceExceededWarning] END),
			a.[EnableTotalQuantityExceededWarning] = (CASE d.[EnableTotalQuantityExceededWarning_RVFlag] WHEN 1 THEN b.[EnableTotalQuantityExceededWarning] ELSE a.[EnableTotalQuantityExceededWarning] END),
			a.[EnableTotalValueExceededWarning] = (CASE d.[EnableTotalValueExceededWarning_RVFlag] WHEN 1 THEN b.[EnableTotalValueExceededWarning] ELSE a.[EnableTotalValueExceededWarning] END),
			a.[EnableValueToleranceExceededWarning] = (CASE d.[EnableValueToleranceExceededWarning_RVFlag] WHEN 1 THEN b.[EnableValueToleranceExceededWarning] ELSE a.[EnableValueToleranceExceededWarning] END),
			a.[FlowDecimalPlaces] = (CASE d.[FlowDecimalPlaces_RVFlag] WHEN 1 THEN b.[FlowDecimalPlaces] ELSE a.[FlowDecimalPlaces] END),
			a.[FlowUnitIndex] = (CASE d.[FlowUnitIndex_RVFlag] WHEN 1 THEN b.[FlowUnitIndex] ELSE a.[FlowUnitIndex] END),
			a.[IncludeInDispatch] = (CASE d.[IncludeInDispatch_RVFlag] WHEN 1 THEN b.[IncludeInDispatch] ELSE a.[IncludeInDispatch] END),
			a.[LevelDecimalPlaces] = (CASE d.[LevelDecimalPlaces_RVFlag] WHEN 1 THEN b.[LevelDecimalPlaces] ELSE a.[LevelDecimalPlaces] END),
			a.[LevelUnitIndex] = (CASE d.[LevelUnitIndex_RVFlag] WHEN 1 THEN b.[LevelUnitIndex] ELSE a.[LevelUnitIndex] END),
			a.[LimitSelectionsBasedOnHierarchy] = (CASE d.[LimitSelectionsBasedOnHierarchy_RVFlag] WHEN 1 THEN b.[LimitSelectionsBasedOnHierarchy] ELSE a.[LimitSelectionsBasedOnHierarchy] END),
			a.[LineItemEditControl] = (CASE d.[LineItemEditControl_RVFlag] WHEN 1 THEN b.[LineItemEditControl] ELSE a.[LineItemEditControl] END),
			a.[LookupDefaultStatusIndex] = (CASE d.[LookupDefaultStatusIndex_RVFlag] WHEN 1 THEN b.[LookupDefaultStatusIndex] ELSE a.[LookupDefaultStatusIndex] END),
			a.[LookupTransTypeIndex] = (CASE d.[LookupTransTypeIndex_RVFlag] WHEN 1 THEN b.[LookupTransTypeIndex] ELSE a.[LookupTransTypeIndex] END),
			a.[MassDecimalPlaces] = (CASE d.[MassDecimalPlaces_RVFlag] WHEN 1 THEN b.[MassDecimalPlaces] ELSE a.[MassDecimalPlaces] END),
			a.[MassUnitIndex] = (CASE d.[MassUnitIndex_RVFlag] WHEN 1 THEN b.[MassUnitIndex] ELSE a.[MassUnitIndex] END),
			a.[MeterCloseout] = (CASE d.[MeterCloseout_RVFlag] WHEN 1 THEN b.[MeterCloseout] ELSE a.[MeterCloseout] END),
			a.[MultipleLineItems] = (CASE d.[MultipleLineItems_RVFlag] WHEN 1 THEN b.[MultipleLineItems] ELSE a.[MultipleLineItems] END),
			a.[MultipleTransportLineItems] = (CASE d.[MultipleTransportLineItems_RVFlag] WHEN 1 THEN b.[MultipleTransportLineItems] ELSE a.[MultipleTransportLineItems] END),
			a.[MultipleWeightReadings] = (CASE d.[MultipleWeightReadings_RVFlag] WHEN 1 THEN b.[MultipleWeightReadings] ELSE a.[MultipleWeightReadings] END),
			a.[PermitNonReferenceData] = (CASE d.[PermitNonReferenceData_RVFlag] WHEN 1 THEN b.[PermitNonReferenceData] ELSE a.[PermitNonReferenceData] END),
			a.[PressureDecimalPlaces] = (CASE d.[PressureDecimalPlaces_RVFlag] WHEN 1 THEN b.[PressureDecimalPlaces] ELSE a.[PressureDecimalPlaces] END),
			a.[PressureUnitIndex] = (CASE d.[PressureUnitIndex_RVFlag] WHEN 1 THEN b.[PressureUnitIndex] ELSE a.[PressureUnitIndex] END),
			a.[ShowCompanyName] = (CASE d.[ShowCompanyName_RVFlag] WHEN 1 THEN b.[ShowCompanyName] ELSE a.[ShowCompanyName] END),
			a.[SourceEquipmentTypes1] = (CASE d.[SourceEquipmentTypes1_RVFlag] WHEN 1 THEN b.[SourceEquipmentTypes1] ELSE a.[SourceEquipmentTypes1] END),
			a.[SourceEquipmentTypes2] = (CASE d.[SourceEquipmentTypes2_RVFlag] WHEN 1 THEN b.[SourceEquipmentTypes2] ELSE a.[SourceEquipmentTypes2] END),
			a.[SourceEquipmentTypes3] = (CASE d.[SourceEquipmentTypes3_RVFlag] WHEN 1 THEN b.[SourceEquipmentTypes3] ELSE a.[SourceEquipmentTypes3] END),
			a.[TemperatureDecimalPlaces] = (CASE d.[TemperatureDecimalPlaces_RVFlag] WHEN 1 THEN b.[TemperatureDecimalPlaces] ELSE a.[TemperatureDecimalPlaces] END),
			a.[TemperatureUnitIndex] = (CASE d.[TemperatureUnitIndex_RVFlag] WHEN 1 THEN b.[TemperatureUnitIndex] ELSE a.[TemperatureUnitIndex] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UseComboBoxControls] = (CASE d.[UseComboBoxControls_RVFlag] WHEN 1 THEN b.[UseComboBoxControls] ELSE a.[UseComboBoxControls] END),
			a.[VolumeDecimalPlaces] = (CASE d.[VolumeDecimalPlaces_RVFlag] WHEN 1 THEN b.[VolumeDecimalPlaces] ELSE a.[VolumeDecimalPlaces] END),
			a.[VolumeUnitIndex] = (CASE d.[VolumeUnitIndex_RVFlag] WHEN 1 THEN b.[VolumeUnitIndex] ELSE a.[VolumeUnitIndex] END),
			a.[WeightReadingEditControl] = (CASE d.[WeightReadingEditControl_RVFlag] WHEN 1 THEN b.[WeightReadingEditControl] ELSE a.[WeightReadingEditControl] END)		
		FROM tblTransactionAliases a
		INNER JOIN tblTransactionAliases b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempTransactionAliasRecordVersioningFlag d
		ON d.TransactionAliasGuid = a.TransactionAliasGuid
		WHERE b.TransactionAliasGuid = @SourceTransactionAliasGuid
		AND d._CallingReferenceGuid = @callingRef1Guid
		AND a.TransactionAliasGuid = a._MasterRecordGuid

		DELETE erv.tblTempTransactionAliasRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRef1Guid 


		
		/*Process those ParentSpecific External fields whose propagation require custom handling. */		
		-- Process [Associations] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Associations') > 0)
		BEGIN
			--Delete the master record version Associations mappings that are not supported anymore in the child TransactionAlias record 			
			DELETE a FROM [map].[tblAssociatedTransactionAliases] a
			INNER JOIN [dbo].tblTransactionAliases b
			ON b.TransactionAliasGuid = a.ParentTransactionAliasGuid
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.ChildTransactionAliasGuid
			WHERE b.TransactionAliasGuid = @masterRecordGuid
			AND b.TransactionAliasGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblAssociatedTransactionAliases] d
				INNER JOIN [dbo].tblTransactionAliases e
				ON e.TransactionAliasGuid = d.ParentTransactionAliasGuid
				INNER JOIN [dbo].tblTransactionAliases f
				ON f.TransactionAliasGuid = d.ChildTransactionAliasGuid			
				WHERE d.ParentTransactionAliasGuid = @SourceTransactionAliasGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND c._MasterRecordGuid = f._MasterRecordGuid				
			)			
													
			--No characteristics of the TransactionAlias Association mappings to update. The mappings are either inserted or deleted.

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblAssociatedTransactionAliases]
			(ParentTransactionAliasGuid, ChildTransactionAliasGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT b._MasterRecordGuid, ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', c._MasterRecordGuid, @masterSiteGuid), c._MasterRecordGuid), 			  
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblAssociatedTransactionAliases] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.ParentTransactionAliasGuid
			INNER JOIN tblTransactionAliases c
			ON c.TransactionAliasGuid = a.ChildTransactionAliasGuid
			WHERE a.ParentTransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblAssociatedTransactionAliases] d
				INNER JOIN dbo.tblTransactionAliases e
				ON e.TransactionAliasGuid = d.ChildTransactionAliasGuid
				WHERE d.ParentTransactionAliasGuid = b._MasterRecordGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
			)
		END


		-- Process [Fields] External Fields
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Fields') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child TransactionAlias record
			DELETE a FROM [dbo].[tblTransactionAliasFields] a
			INNER JOIN [dbo].tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE b.TransactionAliasGuid = @masterRecordGuid
			AND b.TransactionAliasGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblTransactionAliasFields] d
				INNER JOIN [dbo].tblTransactionAliases e
				ON e.TransactionAliasGuid = d.TransactionAliasGuid			
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
				AND d.DbName = a.DbName
			)		
													
			--Update the master record version mappings that have been modified in the child TransactionAlias record
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
			FROM [dbo].[tblTransactionAliasFields] a
			INNER JOIN [dbo].[tblTransactionAliases] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN [dbo].[tblTransactionAliasFields] d
			ON d.TransactionAliasGuid = b._MasterRecordGuid
			AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
			AND d.DbName = a.DbName			
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [dbo].[tblTransactionAliasFields]
			(TransactionAliasGuid, AliasId, DbName, DisplayOrder, DisplayName, Required, Virtual, LookupTransactionFieldTypeIndex, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT b._MasterRecordGuid, a.AliasID, a.DbName, a.DisplayOrder, a.DisplayName, a.Required, a.Virtual, a.LookupTransactionFieldTypeIndex, a.UserGroupGuid, a.DispatchField, a.ClearOnNew, 
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblTransactionAliasFields] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblTransactionAliasFields] d
				WHERE d.TransactionAliasGuid = b._MasterRecordGuid
				AND d.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
				AND d.DbName = a.DbName
			)
		END

		-- Process [Products] External Field
		-- Product is both an External Attribute of TransactionAlias (i.e. TransactionAlias-To-Product mappings are maintained as part of the TransactionAlias entity), and an External Client of TransactionAlias (i.e. TransactionAlias-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the TransactionAlias entity)
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Products') > 0)
		BEGIN
			--Delete the master record version Product mappings that are not supported anymore in the child TransactionAlias record 
			DELETE a FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN [dbo].tblTransactionAliases b
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
			WHERE b.TransactionAliasGuid = @masterRecordGuid
			AND b.TransactionAliasGuid = b._MasterRecordGuid
			AND ((c.ProductGuid = c._MasterRecordGuid) OR (NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific'))) --Exclude the mappings that are owned by a Product child record version whose TransactionAliasExclusion field is set as VersionSpecific. Note: Deletion of mappings owned by a local Product record (Product Guid = Product MasterRecordGuid) is permitted because by definition GlobalSpecific fields support the update of master records.					
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] f
				INNER JOIN [dbo].tblTransactionAliases g
				ON g.TransactionAliasGuid = f.AssignedToTransactionAliasGuid
				INNER JOIN [dbo].tblProducts h
				ON h.ProductGuid = f.ProductGuid
				WHERE f.ProductGuid = @SourceTransactionAliasGuid
				AND b._MasterRecordGuid = g._MasterRecordGuid
				AND c._MasterRecordGuid = h._MasterRecordGuid				
			)	
													
			--Update the master record version mappings that have been modified in the child TransactionAlias record
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
			INNER JOIN [dbo].[tblTransactionAliases] b
			ON b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			INNER JOIN dbo.tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToTransactionAliasExclusion] d
			ON d.AssignedToTransactionAliasGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblProducts e
			ON e.ProductGuid = d.ProductGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.AssignedToTransactionAliasGuid = @SourceTransactionAliasGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblProductToTransactionAliasExclusion]
			(ProductGuid, AssignedToTransactionAliasGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, 
			ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @masterSiteGuid), a.ProductGuid), 
			 b._MasterRecordGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
			a.MeterID, a.ShipToProductId, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToTransactionAliasExclusion] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			WHERE a.AssignedToTransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToTransactionAliasExclusion] d
				INNER JOIN dbo.tblProducts e
				ON e.ProductGuid = d.ProductGuid
				WHERE d.AssignedToTransactionAliasGuid = b._MasterRecordGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		-- Process [Statuses] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Statuses') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child TransactionAlias record
			DELETE a FROM [map].[tblTransactionAliasToStatus] a
			INNER JOIN [dbo].tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE b.TransactionAliasGuid = @masterRecordGuid
			AND b.TransactionAliasGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblTransactionAliasToStatus] d
				INNER JOIN [dbo].tblTransactionAliases e
				ON e.TransactionAliasGuid = d.TransactionAliasGuid			
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.LookupTransactionStatusIndex = a.LookupTransactionStatusIndex
			)		
													
			--No characteristics of the TransactionAliasStatuses mappings to update. The mappings are either inserted or deleted.

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblTransactionAliasToStatus]
			(LookupTransactionStatusIndex, TransactionAliasGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT a.LookupTransactionStatusIndex, b._MasterRecordGuid, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblTransactionAliasToStatus] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblTransactionAliasToStatus] d
				WHERE d.TransactionAliasGuid = b._MasterRecordGuid
				AND d.LookupTransactionStatusIndex = a.LookupTransactionStatusIndex
			)
		END

		-- Process [UserData] External Field		
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UserData') > 0)
		BEGIN
			--UserData - [dbo].[tblUserDataFieldTransactionAlias] and [dbo].[tblUserDataListValueTransactionAlias]
			--Delete the master record version mappings that are not supported anymore in the child TransactionAlias record
			DELETE a FROM [dbo].[tblUserDataListValueTransactionAlias] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
			ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
			INNER JOIN [dbo].tblTransactionAliases c
			ON c.TransactionAliasGuid = b.TransactionAliasGuid
			WHERE c.TransactionAliasGuid = @masterRecordGuid
			AND c.TransactionAliasGuid = c._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAlias] d
				INNER JOIN [dbo].tblTransactionAliases e
				ON e.TransactionAliasGuid = d.UserDataFieldTransactionAliasGuid			
				WHERE d.UserDataFieldTransactionAliasGuid = @SourceTransactionAliasGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
				AND d.Value = a.Value
			)	

			--Delete the master record version mappings that are not supported anymore in the child TransactionAlias record
			DELETE a FROM [dbo].[tblUserDataFieldTransactionAlias] a
			INNER JOIN [dbo].tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE b.TransactionAliasGuid = @masterRecordGuid
			AND b.TransactionAliasGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAlias] d
				INNER JOIN [dbo].tblTransactionAliases e
				ON e.TransactionAliasGuid = d.TransactionAliasGuid		
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.DisplayName = a.DisplayName
			)	
													
			--No characteristics of the TransactionAlias UserData ListValue mappings to update. There is only one field to set, the Value field, which during propagation is simply either inserted or deleted.
					
			--Update the master record version mappings that have been modified in the child TransactionAlias record
			UPDATE d
			SET d.ClearOnNew = a.ClearOnNew, 
			d.DispatchField = a.DispatchField,
			d.LookupUserDataTypeIndex = a.LookupUserDataTypeIndex,
			d.Number = a.Number,
			d.UserGroupGuid = a.UserGroupGuid,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAlias] a
			INNER JOIN [dbo].[tblTransactionAliases] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] d
			ON d.TransactionAliasGuid = b._MasterRecordGuid
			AND d.DisplayName = a.DisplayName
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [dbo].[tblUserDataFieldTransactionAlias] 
			(TransactionAliasGuid, SiteGuid, Number, DisplayOrder, DisplayName, LookupUserDataTypeIndex, Required, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT b._MasterRecordGuid, c.SiteGuid, a.Number, a.DisplayOrder, a.DisplayName, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAlias] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN dbo.tblTransactionAliases c
			ON c.TransactionAliasGuid = b._MasterRecordGuid
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAlias] d
				WHERE d.TransactionAliasGuid = b._MasterRecordGuid
				AND d.DisplayName = a.DisplayName
			)

			--Insert a new mapping for each child record mapping not found in the master record
			DECLARE @tblTargetUserDataListValueMasterRecord TABLE
			(
				TransactionAliasGuid uniqueidentifier,
				SiteGuid uniqueidentifier,
				DisplayName nvarchar(30),
				Value nvarchar(120),
				UserDataFieldTransactionAliasGuid uniqueidentifier,
				CreatedBy [dbo].[udtUserID] NULL,
				UpdatedBy [dbo].[udtUserID] NULL 
			)
			INSERT INTO @tblTargetUserDataListValueMasterRecord
			(TransactionAliasGuid, SiteGuid, DisplayName, Value, CreatedBy, UpdatedBy)
			SELECT c._MasterRecordGuid, d.SiteGuid, b.DisplayName, a.Value, a.CreatedBy, a.UpdatedBy
			FROM [dbo].[tblUserDataListValueTransactionAlias] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
			ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
			INNER JOIN dbo.tblTransactionAliases c
			ON c.TransactionAliasGuid = b.TransactionAliasGuid
			INNER JOIN dbo.tblTransactionAliases d
			ON d.TransactionAliasGuid = c._MasterRecordGuid
			WHERE c.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAlias] d
				INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] e
				ON e.UserDataFieldTransactionAliasGuid = d.UserDataFieldTransactionAliasGuid
				INNER JOIN dbo.tblTransactionAliases f
				ON f.TransactionAliasGuid = e.TransactionAliasGuid
				WHERE f.TransactionAliasGuid = c._MasterRecordGuid
				AND d.Value = a.Value
			)

			UPDATE a
			SET a.UserDataFieldTransactionAliasGuid = b.UserDataFieldTransactionAliasGuid
			FROM  @tblTargetUserDataListValueMasterRecord a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			AND b.SiteGuid = a.SiteGuid
			AND b.DisplayName = a.DisplayName

			INSERT INTO [dbo].[tblUserDataListValueTransactionAlias] 
			(UserDataFieldTransactionAliasGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT UserDataFieldTransactionAliasGuid, Value, GETDATE(), CreatedBy, GETDATE(), UpdatedBy
			FROM @tblTargetUserDataListValueMasterRecord

			--UserData - [dbo].[tblUserDataFieldTransactionAliasLineItem] and [dbo].[tblUserDataListValueTransactionAliasLineItem]
			--Delete the master record version mappings that are not supported anymore in the child TransactionAlias record
			DELETE a FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
			ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
			INNER JOIN [dbo].tblTransactionAliases c
			ON c.TransactionAliasGuid = b.TransactionAliasGuid
			WHERE c.TransactionAliasGuid = @masterRecordGuid
			AND c.TransactionAliasGuid = c._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] d
				INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] e
				ON e.UserDataFieldTransactionAliasLineItemGuid = d.UserDataFieldTransactionAliasLineItemGuid
				INNER JOIN [dbo].tblTransactionAliases f
				ON f.TransactionAliasGuid = e.UserDataFieldTransactionAliasLineItemGuid			
				WHERE f.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND f._MasterRecordGuid = c._MasterRecordGuid
				AND d.Value = a.Value
			)			
													
			--No characteristics of the TransactionAlias UserData ListValue mappings to update. There is only one field to set, the Value field, which during propagation is simply either inserted or deleted.

			--Delete the master record version mappings that are not supported anymore in the child TransactionAlias record
			DELETE a FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			INNER JOIN [dbo].tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE b.TransactionAliasGuid = @masterRecordGuid
			AND b.TransactionAliasGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] d
				INNER JOIN [dbo].tblTransactionAliases e
				ON e.TransactionAliasGuid = d.TransactionAliasGuid		
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.DisplayName = a.DisplayName
			)	
				
			--Update the master record version mappings that have been modified in the child TransactionAlias record
			UPDATE d
			SET d.ClearOnNew = a.ClearOnNew, 
			d.DispatchField = a.DispatchField,
			d.LookupUserDataTypeIndex = a.LookupUserDataTypeIndex,
			d.Number = a.Number,
			d.UserGroupGuid = a.UserGroupGuid,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			INNER JOIN [dbo].[tblTransactionAliases] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] d
			ON d.TransactionAliasGuid = b._MasterRecordGuid
			AND d.DisplayName = a.DisplayName
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND d.SiteGuid = @masterSiteGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [dbo].[tblUserDataFieldTransactionAliasLineItem]
			(TransactionAliasGuid, SiteGuid, Number, DisplayOrder, DisplayName, LookupUserDataTypeIndex, Required, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT b._MasterRecordGuid, c.SiteGuid, a.Number, a.DisplayOrder, a.DisplayName, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN dbo.tblTransactionAliases c
			ON c.TransactionAliasGuid = b._MasterRecordGuid
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] d
				WHERE d.TransactionAliasGuid = b._MasterRecordGuid
				AND d.DisplayName = a.DisplayName
			)

			-- Insert a UserData ListValue record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData ListValue items.
			DECLARE @tblTargetUserDataListValueLineItemMasterRecord TABLE
			(
				TransactionAliasGuid uniqueidentifier,
				SiteGuid uniqueidentifier,
				DisplayName nvarchar(30),
				Value nvarchar(120),
				UserDataFieldTransactionAliasLineItemGuid uniqueidentifier,
				CreatedBy [dbo].[udtUserID] NULL,
				UpdatedBy [dbo].[udtUserID] NULL 
			)
			
			INSERT INTO @tblTargetUserDataListValueLineItemMasterRecord
			(TransactionAliasGuid, SiteGuid, DisplayName, Value, CreatedBy, UpdatedBy)
			SELECT c._MasterRecordGuid, d.SiteGuid, b.DisplayName, a.Value, a.CreatedBy, a.UpdatedBy
			FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
			ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
			INNER JOIN dbo.tblTransactionAliases c
			ON c.TransactionAliasGuid = b.TransactionAliasGuid
			INNER JOIN dbo.tblTransactionAliases d
			ON d.TransactionAliasGuid = c._MasterRecordGuid
			WHERE c.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] d
				INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] e
				ON e.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
				INNER JOIN dbo.tblTransactionAliases f
				ON f.TransactionAliasGuid = e.TransactionAliasGuid
				WHERE f.TransactionAliasGuid = c._MasterRecordGuid
				AND d.Value = a.Value
			)

			UPDATE a
			SET a.UserDataFieldTransactionAliasLineItemGuid = b.UserDataFieldTransactionAliasLineItemGuid
			FROM  @tblTargetUserDataListValueLineItemMasterRecord a
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			AND b.SiteGuid = a.SiteGuid
			AND b.DisplayName = a.DisplayName

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [dbo].[tblUserDataListValueTransactionAliasLineItem]	
			(UserDataFieldTransactionAliasLineItemGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT UserDataFieldTransactionAliasLineItemGuid, Value, GETDATE(), CreatedBy, GETDATE(), UpdatedBy
			FROM @tblTargetUserDataListValueLineItemMasterRecord
		END

		-- Process [UserGroups] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UserGroups') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child TransactionAlias record
			DELETE a FROM [map].[tblGroupToTransactionAlias] a
			INNER JOIN [dbo].tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE b.TransactionAliasGuid = @masterRecordGuid
			AND b.TransactionAliasGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblGroupToTransactionAlias] d
				INNER JOIN [dbo].tblTransactionAliases e
				ON e.TransactionAliasGuid = d.TransactionAliasGuid			
				WHERE d.TransactionAliasGuid = @SourceTransactionAliasGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.GroupGuid = a.GroupGuid
			)		
													
			--Update the master record version mappings that have been modified in the child TransactionAlias record
			UPDATE d
			SET d.LookupRightIndex = a.LookupRightIndex,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblGroupToTransactionAlias] a
			INNER JOIN [dbo].[tblTransactionAliases] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN [map].[tblGroupToTransactionAlias] d
			ON d.TransactionAliasGuid = b._MasterRecordGuid
			AND d.GroupGuid = a.GroupGuid
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblGroupToTransactionAlias]
			(TransactionAliasGuid, GroupGuid, LookupRightIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT b._MasterRecordGuid, a.GroupGuid, a.LookupRightIndex, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblGroupToTransactionAlias] a
			INNER JOIN dbo.tblTransactionAliases b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblGroupToTransactionAlias] d
				WHERE d.TransactionAliasGuid = b._MasterRecordGuid
				AND d.GroupGuid = a.GroupGuid
			)
		END

		-- Process [FieldOrder] External Field
		--FieldOrder is maintained through the DisplayOrder field that is located in three separate tables: [dbo].[tblTransactionAliasFields], [dbo].[tblUserDataFieldTransactionAlias], and0 [dbo].[tblUserDataFieldTransactionAliasLineItem].
		--The Insertion and Deletion propagation that has already been taken care of further up this Stored Procedure, would have also taken care of the Insert and Delete propagation needs of the FieldOrder external field.
		--Only the FieldOrder Update propagation is left to be handled separately below.
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'FieldOrder') > 0)
		BEGIN													
			UPDATE d
			SET d.DisplayOrder = a.DisplayOrder, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblTransactionAliasFields] a
			INNER JOIN [dbo].[tblTransactionAliases] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN [dbo].[tblTransactionAliasFields] d
			ON d.TransactionAliasGuid = b._MasterRecordGuid
			AND d.DbName = a.DbName	
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid

			UPDATE d
			SET d.DisplayOrder = a.DisplayOrder, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAlias] a
			INNER JOIN [dbo].[tblTransactionAliases] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] d
			ON d.TransactionAliasGuid = b._MasterRecordGuid
			AND d.DisplayName = a.DisplayName
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND d.SiteGuid = @masterSiteGuid

			UPDATE d
			SET d.DisplayOrder = a.DisplayOrder, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
			INNER JOIN [dbo].[tblTransactionAliases] b
			ON b.TransactionAliasGuid = a.TransactionAliasGuid
			INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] d
			ON d.TransactionAliasGuid = b._MasterRecordGuid
			AND d.DisplayName = a.DisplayName
			WHERE a.TransactionAliasGuid = @SourceTransactionAliasGuid
			AND d.SiteGuid = @masterSiteGuid
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
						+ 'Procedure Name: [erv].usp_ReplicateTransactionAliasGSChangesOnMaster' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END