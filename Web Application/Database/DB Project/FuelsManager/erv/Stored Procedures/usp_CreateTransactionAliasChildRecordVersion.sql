/*
	DROP PROCEDURE [erv].[usp_CreateTransactionAliasChildRecordVersion]

	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	--EXEC [erv].[usp_CreateTransactionAliasChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '0F7228B9-D8E4-41C8-A862-B71FB3F38763', @dt, 'HB'
	EXEC [erv].[usp_CreateTransactionAliasChildRecordVersion] '012D8DD3-E6FA-4B78-A81A-C84F1C360558', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', @dt, 'HB'

	SELECT TransactionAliasGuid, Id, _MasterRecordGuid, SiteGuid, * FROM tblTransactionAlias WHERE _MasterRecordGuid = 'F5EA57B8-2CFB-4605-9B55-8850199671C7'	
*/

CREATE PROCEDURE [erv].[usp_CreateTransactionAliasChildRecordVersion]
(
	@ParentEntityGuid uniqueidentifier, @TargetSiteIndex uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreateTransactionAliasChildRecordVersion] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new TransactionAlias record version for a target site/sitegroup, off a parent record version 
	-- Notes:
	-- 1. @ParentEntityGuid: Entity Guid of the record to be cloned.
	-- 2. @TargetSiteIndex: Site/SiteGroup for which the new clone needs to be created.
	-- 3. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	-- 4. For external relationships, usually the entity under record versioning that owns the relationship is referenced using its specifc record version guid, whereas the external entity 
	--	  is referenced using its master record guid to account for cases where Record Versioning might later be turned off on the external entity type.
	--	  E.g. Equipment maintains a foreign relationship with Companies. The relationship is owned and maintained directly in tblEquipment itself. Therefore in this relationship, the 
	--	  equipment is referenced using the specifc Equipment record version guid, and the Company record is referenced using its master record guid.
	--	  However, when both entity types in a relationship supports Record Versioning, and this relationship is not owned by either one, but is owned by both, and is maintained in a 
	--	  separate mapping table that is configurable from either entity type (symmetry configurations), then both entities are referenced by their specific record version guid. This
	--	  also holds true for mappings between the same entity types (e.g. Company-To-Company mappings).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @childRecordVersionGuid uniqueidentifier
		SET @childRecordVersionGuid = NEWID()

		DECLARE @masterRecGuid uniqueidentifier
		DECLARE @sourceSite uniqueidentifier
		SELECT @masterRecGuid = _MasterRecordGuid, @sourceSite = SiteGuid FROM tblTransactionAliases
		WHERE TransactionAliasGuid = @ParentEntityGuid

		IF NOT EXISTS
		(
			SELECT * FROM map.tblEntityTransactionAliasToSite
			WHERE TransactionAliasGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteIndex
		)
		BEGIN
			RETURN
		END

		IF EXISTS
		(
			SELECT * FROM tblTransactionAliases
			WHERE _MasterRecordGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteIndex
		)
		BEGIN
			RETURN
		END

		--Create the child record version by cloning the internal fields of the parent record version
		INSERT INTO tblTransactionAliases
		(TransactionAliasGuid,AliasName,SiteGuid,_MasterRecordGuid,MeterCloseout,BulkShipment,DistributedImpact,MultipleLineItems,LimitSelectionsBasedOnHierarchy,LineItemEditControl,MultipleWeightReadings,WeightReadingEditControl,AssociatedReport,AssociatedPreloadReport,DestinationEquipmentTypes1,DestinationEquipmentTypes2,DestinationEquipmentTypes3,SourceEquipmentTypes1,SourceEquipmentTypes2,SourceEquipmentTypes3,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,ShowCompanyName,AggregateAssocTrans,EnableTotalQuantityExceededWarning,EnableQuantityToleranceExceededWarning,EnableTotalValueExceededWarning,EnableValueToleranceExceededWarning,LevelUnitIndex,TemperatureUnitIndex,DensityUnitIndex,PressureUnitIndex,FlowUnitIndex,VolumeUnitIndex,MassUnitIndex,AdditiveVolumeUnitIndex,AdditiveProfileCycleAmountUnitIndex,AdditiveProfileRateUnitIndex,LevelDecimalPlaces,TemperatureDecimalPlaces,DensityDecimalPlaces,PressureDecimalPlaces,FlowDecimalPlaces,VolumeDecimalPlaces,MassDecimalPlaces,AdditiveVolumeDecimalPlaces,UseComboBoxControls,MultipleTransportLineItems,LookupTransTypeIndex,LookupDefaultStatusIndex,AssociatedTransactionAliasGuid,IncludeInDispatch,EnableAutoCompleteControls,PermitNonReferenceData)
		SELECT @childRecordVersionGuid,AliasName,@TargetSiteIndex,_MasterRecordGuid,MeterCloseout,BulkShipment,DistributedImpact,MultipleLineItems,LimitSelectionsBasedOnHierarchy,LineItemEditControl,MultipleWeightReadings,WeightReadingEditControl,AssociatedReport,AssociatedPreloadReport,DestinationEquipmentTypes1,DestinationEquipmentTypes2,DestinationEquipmentTypes3,SourceEquipmentTypes1,SourceEquipmentTypes2,SourceEquipmentTypes3,@CreatedDate,@CreatedBy,@CreatedDate,@CreatedBy,ShowCompanyName,AggregateAssocTrans,EnableTotalQuantityExceededWarning,EnableQuantityToleranceExceededWarning,EnableTotalValueExceededWarning,EnableValueToleranceExceededWarning,LevelUnitIndex,TemperatureUnitIndex,DensityUnitIndex,PressureUnitIndex,FlowUnitIndex,VolumeUnitIndex,MassUnitIndex,AdditiveVolumeUnitIndex,AdditiveProfileCycleAmountUnitIndex,AdditiveProfileRateUnitIndex,LevelDecimalPlaces,TemperatureDecimalPlaces,DensityDecimalPlaces,PressureDecimalPlaces,FlowDecimalPlaces,VolumeDecimalPlaces,MassDecimalPlaces,AdditiveVolumeDecimalPlaces,UseComboBoxControls,MultipleTransportLineItems,LookupTransTypeIndex,LookupDefaultStatusIndex,AssociatedTransactionAliasGuid,IncludeInDispatch,EnableAutoCompleteControls,PermitNonReferenceData
		FROM tblTransactionAliases
		WHERE TransactionAliasGuid = @ParentEntityGuid

		--Clone the external attributes of the parent record version
		--Associations
		UPDATE a 
		SET a.ParentTransactionAliasGuid = @childRecordVersionGuid
		FROM [map].[tblAssociatedTransactionAliases] a
		INNER JOIN tblTransactionAliases b
		ON b.TransactionAliasGuid = a.ParentTransactionAliasGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.ParentTransactionAliasGuid = @ParentEntityGuid		

		INSERT INTO [map].[tblAssociatedTransactionAliases]
		(ParentTransactionAliasGuid, ChildTransactionAliasGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Transaction_Alias', b._MasterRecordGuid, @TargetSiteIndex), a.ChildTransactionAliasGuid), --Clone the mapping even if the ChildTransactionAliasGuid is not assigned to the target site, so that the invalid mapping is available when/if the ChildTransactionAliasGuid is eventually mapped to the site.
		@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblAssociatedTransactionAliases] a
		INNER JOIN tblTransactionAliases b
		ON b.TransactionAliasGuid = a.ChildTransactionAliasGuid
		WHERE a.ParentTransactionAliasGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblAssociatedTransactionAliases]  c
		  WHERE c.ParentTransactionAliasGuid = @childRecordVersionGuid
		  AND c.ChildTransactionAliasGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Transaction_Alias', b._MasterRecordGuid, @TargetSiteIndex), a.ChildTransactionAliasGuid)
		)

		--Both Fields and FieldOrder
		INSERT INTO [dbo].[tblTransactionAliasFields]
		(TransactionAliasGuid, AliasId, DbName, DisplayOrder, DisplayName, Required, Virtual, LookupTransactionFieldTypeIndex, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		a.AliasID, a.DbName, a.DisplayOrder, a.DisplayName, a.Required, a.Virtual, a.LookupTransactionFieldTypeIndex, a.UserGroupGuid, a.DispatchField, a.ClearOnNew, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblTransactionAliasFields] a		
		WHERE a.TransactionAliasGuid = @ParentEntityGuid

		--Products		
		UPDATE a 
		SET a.AssignedToTransactionAliasGuid = @childRecordVersionGuid
		FROM [map].[tblProductToTransactionAliasExclusion] a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.AssignedToTransactionAliasGuid = @ParentEntityGuid

		INSERT INTO [map].[tblProductToTransactionAliasExclusion]
		(AssignedToTransactionAliasGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', b.MasterRecordGuid, @TargetSiteIndex), a.ProductGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Product is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToTransactionAliasExclusion] a
		INNER JOIN [erv].[udf_GetProductRecordVersions](@sourceSite) b  --Only clone those TransactionAlias mappings that are pertinent to the parent/source site. This filter covers the case where the child record version mappings were originally built when TransactionAlias RecordVersioning was Off (and Product RecordVersioning was On).
		ON b.ProductGuid = a.ProductGuid
		WHERE a.AssignedToTransactionAliasGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblProductToTransactionAliasExclusion] c
		  WHERE c.AssignedToTransactionAliasGuid = @childRecordVersionGuid
		  AND c.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', b.MasterRecordGuid, @TargetSiteIndex), a.ProductGuid)
		)

		--Statuses
		INSERT INTO [map].[tblTransactionAliasToStatus] 
		(TransactionAliasGuid, LookupTransactionStatusIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, a.LookupTransactionStatusIndex, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblTransactionAliasToStatus] a
		WHERE a.TransactionAliasGuid = @ParentEntityGuid


		--UserData
		--UserData  -- [dbo].[tblUserDataFieldTransactionAlias] and [dbo].[tblUserDataListValueTransactionAlias]
		INSERT INTO [dbo].[tblUserDataFieldTransactionAlias]
		(TransactionAliasGuid, SiteGuid, Number, DisplayOrder, DisplayName, LookupUserDataTypeIndex, Required, UserGroupGuid, DispatchField, ClearOnNew, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, @TargetSiteIndex, 
		a.Number, a.DisplayOrder, a.DisplayName, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblUserDataFieldTransactionAlias] a
		WHERE a.TransactionAliasGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [dbo].[tblUserDataFieldTransactionAlias] c
		  WHERE c.TransactionAliasGuid = @childRecordVersionGuid
		  AND c.DisplayName = a.DisplayName
		)

		-- Insert a UserData ListValue record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData ListValue items.		
		INSERT INTO [dbo].[tblUserDataListValueTransactionAlias]
		(UserDataFieldTransactionAliasGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT e.UserDataFieldTransactionAliasGuid, a.Value, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblUserDataListValueTransactionAlias] a
		INNER JOIN [dbo].[tblUserDataFieldTransactionAlias] b
		ON b.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid
		INNER JOIN tblTransactionAliases d
		ON d._MasterRecordGuid = @masterRecGuid
		AND d.SiteGuid = @TargetSiteIndex
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
		SELECT @childRecordVersionGuid, @TargetSiteIndex, 
		a.Number, a.DisplayOrder, a.DisplayName, a.LookupUserDataTypeIndex, a.Required, a.UserGroupGuid, a.DispatchField, a.ClearOnNew, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] a
		WHERE a.TransactionAliasGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [dbo].[tblUserDataFieldTransactionAliasLineItem] c
		  WHERE c.TransactionAliasGuid = @childRecordVersionGuid
		  AND c.DisplayName = a.DisplayName
		)

		-- Insert a UserData LineItem ListValue record for each child record version mapping updated or inserted, for which the corresponding parent mapping has newly added UserData ListValue items.		
		INSERT INTO [dbo].[tblUserDataListValueTransactionAliasLineItem] 
		(UserDataFieldTransactionAliasLineItemGuid, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT e.UserDataFieldTransactionAliasLineItemGuid, a.Value, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblUserDataListValueTransactionAliasLineItem] a
		INNER JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] b
		ON b.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid
		INNER JOIN tblTransactionAliases d
		ON d._MasterRecordGuid = @masterRecGuid
		AND d.SiteGuid = @TargetSiteIndex
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
		SET a.TransactionAliasGuid = @childRecordVersionGuid
		FROM [map].[tblGroupToTransactionAlias] a
		INNER JOIN [dbo].[tblGroups] b
		ON b.GroupGuid = a.GroupGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.TransactionAliasGuid = @ParentEntityGuid

		INSERT INTO [map].[tblGroupToTransactionAlias]
		(TransactionAliasGuid, GroupGuid, LookupRightIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, a.GroupGuid, a.LookupRightIndex, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblGroupToTransactionAlias] a
		WHERE a.TransactionAliasGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblGroupToTransactionAlias] c
		  WHERE c.TransactionAliasGuid = @childRecordVersionGuid
		  AND c.GroupGuid = a.GroupGuid
		)

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
						+ 'Procedure Name: [erv].usp_CreateTransactionAliasChildRecordVersion' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
