/*
	DROP PROCEDURE [erv].[usp_CreateTransactionAliasChildRecordVersionBySegment]

	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	EXEC [erv].[usp_CreateTransactionAliasChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', '00000000-0000-0000-0000-000000000001', @dt, 'HB'
	--EXEC [erv].[usp_CreateTransactionAliasChildRecordVersionBySegment] '23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', @dt, 'HB'

*/

CREATE PROCEDURE [erv].[usp_CreateTransactionAliasChildRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreateTransactionAliasChildRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new TransactionAlias record version for each of the existing entity assignments of a given TransactionAlias segment from a given SiteGroup.
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
			TransactionAliasGuid uniqueidentifier  -- The child record version TransactionAliasGuid is not initially available since the process will be creating the new TransactionAlias child record versions, but it is populated and used further down the process when handling the external attributes.
		)

		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@entityTypeId = 'Transaction_Alias')
		BEGIN
			INSERT INTO @tblTargetEntitySite
			(SiteGuid, MasterRecordGuid, ParentEntityGuid)
			SELECT b.SiteGuid, b.TransactionAliasGuid, a.TransactionAliasGuid
			FROM tblTransactionAliases a
			INNER JOIN map.tblEntityTransactionAliasToSite b
			ON b.TransactionAliasGuid = a._MasterRecordGuid
			AND b.AssignedFromSiteGuid = a.SiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about creating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no need to create a child record version in any case.
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND NOT EXISTS
			(SELECT * FROM tblTransactionAliases c
			WHERE c._MasterRecordGuid = a._MasterRecordGuid
			AND c.SiteGuid = b.SiteGuid)
			AND b.SiteGuid <> b.AssignedFromSiteGuid
		END
				

		--Create the child record versions by cloning the internal fields of the parent record version
		INSERT INTO tblTransactionAliases
		(AliasName,SiteGuid,_MasterRecordGuid,MeterCloseout,BulkShipment,DistributedImpact,MultipleLineItems,LimitSelectionsBasedOnHierarchy,LineItemEditControl,MultipleWeightReadings,WeightReadingEditControl,AssociatedReport,AssociatedPreloadReport,DestinationEquipmentTypes1,DestinationEquipmentTypes2,DestinationEquipmentTypes3,SourceEquipmentTypes1,SourceEquipmentTypes2,SourceEquipmentTypes3,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ShowCompanyName,AggregateAssocTrans,EnableTotalQuantityExceededWarning,EnableQuantityToleranceExceededWarning,EnableTotalValueExceededWarning,EnableValueToleranceExceededWarning,LevelUnitIndex,TemperatureUnitIndex,DensityUnitIndex,PressureUnitIndex,FlowUnitIndex,VolumeUnitIndex,MassUnitIndex,AdditiveVolumeUnitIndex,AdditiveProfileCycleAmountUnitIndex,AdditiveProfileRateUnitIndex,LevelDecimalPlaces,TemperatureDecimalPlaces,DensityDecimalPlaces,PressureDecimalPlaces,FlowDecimalPlaces,VolumeDecimalPlaces,MassDecimalPlaces,AdditiveVolumeDecimalPlaces,UseComboBoxControls,MultipleTransportLineItems,LookupTransTypeIndex,LookupDefaultStatusIndex,AssociatedTransactionAliasGuid,IncludeInDispatch,EnableAutoCompleteControls,PermitNonReferenceData)
		SELECT a.AliasName, b.SiteGuid, a._MasterRecordGuid, a.MeterCloseout, a.BulkShipment, a.DistributedImpact, a.MultipleLineItems, a.LimitSelectionsBasedOnHierarchy, a.LineItemEditControl, a.MultipleWeightReadings, a.WeightReadingEditControl, a.AssociatedReport, a.AssociatedPreloadReport, a.DestinationEquipmentTypes1, a.DestinationEquipmentTypes2, a.DestinationEquipmentTypes3, a.SourceEquipmentTypes1, a.SourceEquipmentTypes2, a.SourceEquipmentTypes3, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy, a.ShowCompanyName, a.AggregateAssocTrans, a.EnableTotalQuantityExceededWarning, a.EnableQuantityToleranceExceededWarning, a.EnableTotalValueExceededWarning, a.EnableValueToleranceExceededWarning, a.LevelUnitIndex, a.TemperatureUnitIndex, a.DensityUnitIndex, a.PressureUnitIndex, a.FlowUnitIndex, a.VolumeUnitIndex, a.MassUnitIndex, a.AdditiveVolumeUnitIndex, a.AdditiveProfileCycleAmountUnitIndex, a.AdditiveProfileRateUnitIndex, a.LevelDecimalPlaces, a.TemperatureDecimalPlaces, a.DensityDecimalPlaces, a.PressureDecimalPlaces, a.FlowDecimalPlaces, a.VolumeDecimalPlaces, a.MassDecimalPlaces, a.AdditiveVolumeDecimalPlaces, a.UseComboBoxControls, a.MultipleTransportLineItems, a.LookupTransTypeIndex, a.LookupDefaultStatusIndex, a.AssociatedTransactionAliasGuid, a.IncludeInDispatch, a.EnableAutoCompleteControls,a.PermitNonReferenceData
		FROM tblTransactionAliases a
		INNER JOIN @tblTargetEntitySite b
		ON b.MasterRecordGuid = a._MasterRecordGuid
		AND b.ParentEntityGuid = a.TransactionAliasGuid


		--Clone the external attributes of the parent record version


		--Retrieve the first available Transaction Alias record version applicable for all Transaction Alias mappings to @SourceSiteGroupGuid
		--Note: This basically implements the [erv].[udf_GetFirstParentRecordVersionGuid] functionality for bulk processing, with the exception that it just updates the AssignedFromSiteGuid and the EntityGuid of the initial mapping record to reflect the actual parent record.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempEntityMappingHierarchy
		(EntityMasterGuid, EntityGuid, AssignedToSiteGuid, MappingLevel, _CallingReferenceGuid)		
		SELECT a.TransactionAliasGuid, b.TransactionAliasGuid, a.SiteGuid, 0, @callingRef1Guid
		FROM map.tblEntityTransactionAliasToSite a
		LEFT OUTER JOIN tblTransactionaliases b
		ON b._MasterRecordGuid = a.TransactionAliasGuid
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
			SET a.AssignedFromSiteGuid = b.SiteGuid, a.EntityGuid = c.TransactionAliasGuid
			FROM erv.tblTempEntityMappingHierarchy a
			INNER JOIN map.tblEntityTransactionAliasToSite b
			ON b.TransactionAliasGuid = a.EntityMasterGuid
			AND b.SiteGuid = a.AssignedFromSiteGuid
			LEFT OUTER JOIN tblTransactionAliases c
			ON c._MasterRecordGuid = b.TransactionAliasGuid
			AND c.SiteGuid = b.SiteGuid
			WHERE a._CallingReferenceGuid = @callingRef1Guid
			AND a.EntityGuid IS NULL
		END				
		
		----Associations
		UPDATE a 
		SET a.ParentTransactionAliasGuid = e.TransactionAliasGuid
		FROM [map].[tblAssociatedTransactionAliases] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.ParentTransactionAliasGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblTransactionAliases d
		ON d.TransactionAliasGuid = a.ChildTransactionAliasGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblTransactionAliases e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblTransactionAliases f
		ON f.TransactionAliasGuid = a.ParentTransactionAliasGuid
		WHERE e._MasterRecordGuid <> e.TransactionAliasGuid
		AND f.SiteGuid <> c.SiteGuid	
		AND b._CallingReferenceGuid = @callingRef1Guid

		INSERT INTO [map].[tblAssociatedTransactionAliases]
		(ParentTransactionAliasGuid, ChildTransactionAliasGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.TransactionAliasGuid,
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Transaction_Alias', d._MasterRecordGuid, b.SiteGuid), a.ChildTransactionAliasGuid), --Clone the mapping even if the ChildTransactionAliasGuid is not assigned to the target site, so that the invalid mapping is available when/if the ChildTransactionAliasGuid is eventually mapped to the site.
		@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblAssociatedTransactionAliases] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.ParentTransactionAliasGuid
		INNER JOIN tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblTransactionAliases d
		ON d.TransactionAliasGuid = a.ChildTransactionAliasGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblAssociatedTransactionAliases] e 
			WHERE e.ParentTransactionAliasGuid = c.TransactionAliasGuid
			AND e.ChildTransactionAliasGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Transaction_Alias', d._MasterRecordGuid, b.SiteGuid), a.ChildTransactionAliasGuid)
		)


		--Fields and FieldOrder
		INSERT INTO [dbo].[tblTransactionAliasFields]
		(TransactionAliasGuid, AliasId, DbName, DisplayOrder, DisplayName, Required, Virtual, LookupTransactionFieldTypeIndex, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.TransactionAliasGuid,
		a.AliasID, a.DbName, a.DisplayOrder, a.DisplayName, a.Required, a.Virtual, a.LookupTransactionFieldTypeIndex, a.UserGroupGuid, a.DispatchField, a.ClearOnNew, 
		@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblTransactionAliasFields] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.TransactionAliasGuid
		INNER JOIN tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblTransactionAliasFields] e 
			WHERE e.TransactionAliasGuid = c.TransactionAliasGuid
			AND e.LookupTransactionFieldTypeIndex = a.LookupTransactionFieldTypeIndex
			AND e.DbName = a.DbName
		)


		--Products
		-- For all the ProductToTransactionAlias mappings that reference a Parent TransactionAlias record version instead of the actual TransactionAlias child record version, because Record Versioning 
		-- was previously OFF for TransactionAlias for that site, update the TransactionAlias field of the mapping to point to the newly created TransactionAlias child record versions.			
		UPDATE a 
		SET a.AssignedToTransactionAliasGuid = e.TransactionAliasGuid
		FROM [map].[tblProductToTransactionAliasExclusion] a
		INNER JOIN erv.tblTempEntityMappingHierarchy b
		ON b.EntityMasterGuid = a.AssignedToTransactionAliasGuid
		INNER JOIN @tblTargetEntitySite c
		ON c.ParentEntityGuid = b.EntityGuid
		INNER JOIN dbo.tblProducts d
		ON d.ProductGuid = a.ProductGuid
		AND d.SiteGuid = c.SiteGuid
		INNER JOIN tblTransactionAliases e
		ON e._MasterRecordGuid = c.MasterRecordGuid
		AND e.SiteGuid = c.SiteGuid
		INNER JOIN tblTransactionAliases f
		ON f.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
		WHERE e._MasterRecordGuid <> e.TransactionAliasGuid
		AND f.SiteGuid <> c.SiteGuid	
		AND b._CallingReferenceGuid = @callingRef1Guid

		--Clone the ProductToTransactionAlias mappings, making sure to ignore:
		-- (i) Those mappings that might have already been introduced through the mapping Update statement above.
		-- (ii) Mappings against a Product owned by a sitegroup/site lower than the SourceSiteGroup. Product is also an External Client of TransactionAlias, which allows a Product at a lower site/sitegroup 
		--      to establish a relationship with a TransactionAlias assigned to the site/sitegroup from the upper SourceSiteGroup. This mapping will automatically be updated to reference the right TransactionAlias 
		--      record version when this SP is eventually run (through Record Versioning/FLC propagation) for the site/sitegroup owner of the Product (through the Update statement above). Cloning 
		--      this type of mapping can lead to incorrect Product-to-TransactionAlias relationships, i.e Product-to-TransactionAlias relationships that did not exist prior to turning TransactionAlias Record Versioning ON.
		-- Note: Mappings against a Product not assigned to the target site/sitegroup are not filtered out, so as not to dictate that all necessary entity assignments have to take place before turning Record Versioning ON.
		INSERT INTO [map].[tblProductToTransactionAliasExclusion]
		(AssignedToTransactionAliasGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.TransactionAliasGuid,
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', d._MasterRecordGuid, b.SiteGuid), a.ProductGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Product is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToTransactionAliasExclusion] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.AssignedToTransactionAliasGuid
		INNER JOIN tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblProducts d
		ON d.ProductGuid = a.ProductGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND NOT EXISTS
		(
			SELECT * FROM [erv].[udf_GetSiteHierarchy] (@SourceSiteGroupGuid, 1) e
			WHERE e.HierarchyLevel > 0
			AND e.SiteGuid = d.SiteGuid
		)
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToTransactionAliasExclusion] f
			WHERE f.AssignedToTransactionAliasGuid = c.TransactionAliasGuid
			AND f.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', d._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
		)


		--Statuses
		INSERT INTO [map].[tblTransactionAliasToStatus]
		(TransactionAliasGuid, LookupTransactionStatusIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.TransactionAliasGuid,
		a.LookupTransactionStatusIndex, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblTransactionAliasToStatus] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.TransactionAliasGuid
		INNER JOIN tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblTransactionAliasToStatus] e 
			WHERE e.TransactionAliasGuid = c.TransactionAliasGuid
			AND e.LookupTransactionStatusIndex = a.LookupTransactionStatusIndex
		)


		--UserData  -- [dbo].[tblUserDataFieldTransactionAlias] and [dbo].[tblUserDataListValueTransactionAlias]
		INSERT INTO [dbo].[tblUserDataFieldTransactionAlias]
		(TransactionAliasGuid, SiteGuid, Number, DisplayOrder, DisplayName, LookupUserDataTypeIndex, Required, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.TransactionAliasGuid, b.SiteGuid,
		a.Number, a.DisplayOrder, a.DisplayName, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblUserDataFieldTransactionAlias] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.TransactionAliasGuid
		INNER JOIN tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblUserDataFieldTransactionAlias] e 
			WHERE e.TransactionAliasGuid = c.TransactionAliasGuid
			AND e.DisplayName = a.DisplayName
		)

		-- Insert a UserData ListValue record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData ListValue items.
		INSERT INTO [dbo].[tblUserDataListValueTransactionAlias] 
		(UserDataFieldTransactionAliasGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT e.UserDataFieldTransactionAliasGuid, a.Value, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblUserDataListValueTransactionAlias] a
		INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
		ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
		INNER JOIN @tblTargetEntitySite c
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
		


		--UserData  -- [dbo].[tblUserDataFieldTransactionAliasLineItem] and [dbo].[tblUserDataListValueTransactionAliasLineItem]
		INSERT INTO [dbo].[tblUserDataFieldTransactionAliasLineItem]
		(TransactionAliasGuid, SiteGuid, Number, DisplayOrder, DisplayName, LookupUserDataTypeIndex, Required, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.TransactionAliasGuid, b.SiteGuid,
		a.Number, a.DisplayOrder, a.DisplayName, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.TransactionAliasGuid
		INNER JOIN tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] e 
			WHERE e.TransactionAliasGuid = c.TransactionAliasGuid
			AND e.DisplayName = a.DisplayName
		)

		-- Insert a UserData LineItem ListValue record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData ListValue items.
		INSERT INTO [dbo].[tblUserDataListValueTransactionAliasLineItem] 
		(UserDataFieldTransactionAliasLineItemGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT e.UserDataFieldTransactionAliasLineItemGuid, a.Value, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
		INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
		ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
		INNER JOIN @tblTargetEntitySite c
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


		--UserGroups
		UPDATE a 
		SET a.TransactionAliasGuid = d.TransactionAliasGuid
		FROM [map].[tblGroupToTransactionAlias] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = erv.udf_GetFirstParentRecordVersionGuid('Transaction_Alias', a.TransactionAliasGuid, @SourceSiteGroupGuid)
		INNER JOIN dbo.tblGroups  c
		ON c.GroupGuid = a.GroupGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblTransactionAliases d
		ON d._MasterRecordGuid = b.MasterRecordGuid
		AND d.SiteGuid = b.SiteGuid
		INNER JOIN tblTransactionAliases e
		ON e.TransactionAliasGuid = a.TransactionAliasGuid
		WHERE d._MasterRecordGuid <> d.TransactionAliasGuid
		AND e.SiteGuid <> b.SiteGuid	

		INSERT INTO [map].[tblGroupToTransactionAlias]
		(TransactionAliasGuid, GroupGuid, LookupRightIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT c.TransactionAliasGuid, a.GroupGuid, a.LookupRightIndex, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblGroupToTransactionAlias] a
		INNER JOIN @tblTargetEntitySite b
		ON b.ParentEntityGuid = a.TransactionAliasGuid
		INNER JOIN tblTransactionAliases c
		ON c._MasterRecordGuid = b.MasterRecordGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN tblGroups d
		ON d.GroupGuid = a.GroupGuid
		WHERE c._MasterRecordGuid <> c.TransactionAliasGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblGroupToTransactionAlias] e 
			WHERE e.TransactionAliasGuid = c.TransactionAliasGuid
			AND e.GroupGuid = a.GroupGuid
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
						+ 'Procedure Name: [erv].usp_CreateTransactionAliasChildRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
