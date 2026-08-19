CREATE PROCEDURE [dbo].[gsp_TransactionAliasesUpdateByPK]
(
		@TransactionAliasGuid uniqueidentifier
	,	@AliasName nvarchar(32)=NULL
	,	@MeterCloseout bit=NULL
	,	@BulkShipment bit=NULL
	,	@DistributedImpact bit=NULL
	,	@MultipleLineItems bit=NULL
	,	@LimitSelectionsBasedOnHierarchy bit=NULL
	,	@LineItemEditControl bit=NULL
	,	@MultipleWeightReadings bit=NULL
	,	@WeightReadingEditControl bit=NULL
	,	@AssociatedReport nvarchar(80)=NULL
	,	@AssociatedPreloadReport nvarchar(80)=NULL
	,	@DestinationEquipmentTypes1 bigint=NULL
	,	@DestinationEquipmentTypes2 bigint=NULL
	,	@DestinationEquipmentTypes3 bigint=NULL
	,	@SourceEquipmentTypes1 bigint=NULL
	,	@SourceEquipmentTypes2 bigint=NULL
	,	@SourceEquipmentTypes3 bigint=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@ShowCompanyName smallint=NULL
	,	@AggregateAssocTrans bit=NULL
	,	@EnableTotalQuantityExceededWarning bit=NULL
	,	@EnableQuantityToleranceExceededWarning bit=NULL
	,	@EnableTotalValueExceededWarning bit=NULL
	,	@EnableValueToleranceExceededWarning bit=NULL
	,	@LevelUnitIndex int=NULL
	,	@TemperatureUnitIndex int=NULL
	,	@DensityUnitIndex int=NULL
	,	@PressureUnitIndex int=NULL
	,	@FlowUnitIndex int=NULL
	,	@VolumeUnitIndex int=NULL
	,	@MassUnitIndex int=NULL
	,	@AdditiveVolumeUnitIndex int=NULL
	,	@AdditiveProfileCycleAmountUnitIndex int=NULL
	,	@AdditiveProfileRateUnitIndex int=NULL
	,	@LevelDecimalPlaces tinyint=NULL
	,	@TemperatureDecimalPlaces tinyint=NULL
	,	@DensityDecimalPlaces tinyint=NULL
	,	@PressureDecimalPlaces tinyint=NULL
	,	@FlowDecimalPlaces tinyint=NULL
	,	@VolumeDecimalPlaces tinyint=NULL
	,	@MassDecimalPlaces tinyint=NULL
	,	@AdditiveVolumeDecimalPlaces tinyint=NULL
	,	@UseComboBoxControls bit=NULL
	,	@MultipleTransportLineItems bit=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupTransTypeIndex smallint=NULL
	,	@LookupDefaultStatusIndex int=NULL
	,	@AssociatedTransactionAliasGuid uniqueidentifier=NULL
	,	@IncludeInDispatch bit=NULL
	,	@_MasterRecordGuid uniqueidentifier=NULL
	,	@EnableAutoCompleteControls bit=NULL
	,	@PermitNonReferenceData bit=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
	,	@NullOverrideAliasName BIT=0 
	,	@NullOverrideMeterCloseout BIT=0 
	,	@NullOverrideBulkShipment BIT=0 
	,	@NullOverrideDistributedImpact BIT=0 
	,	@NullOverrideMultipleLineItems BIT=0 
	,	@NullOverrideLimitSelectionsBasedOnHierarchy BIT=0 
	,	@NullOverrideLineItemEditControl BIT=0 
	,	@NullOverrideMultipleWeightReadings BIT=0 
	,	@NullOverrideWeightReadingEditControl BIT=0 
	,	@NullOverrideAssociatedReport BIT=0 
	,	@NullOverrideAssociatedPreloadReport BIT=0 
	,	@NullOverrideDestinationEquipmentTypes1 BIT=0 
	,	@NullOverrideDestinationEquipmentTypes2 BIT=0 
	,	@NullOverrideDestinationEquipmentTypes3 BIT=0 
	,	@NullOverrideSourceEquipmentTypes1 BIT=0 
	,	@NullOverrideSourceEquipmentTypes2 BIT=0 
	,	@NullOverrideSourceEquipmentTypes3 BIT=0 
	,	@NullOverrideUpdatedDate BIT=0 
	,	@NullOverrideShowCompanyName BIT=0 
	,	@NullOverrideAggregateAssocTrans BIT=0 
	,	@NullOverrideEnableTotalQuantityExceededWarning BIT=0 
	,	@NullOverrideEnableQuantityToleranceExceededWarning BIT=0 
	,	@NullOverrideEnableTotalValueExceededWarning BIT=0 
	,	@NullOverrideEnableValueToleranceExceededWarning BIT=0 
	,	@NullOverrideLevelUnitIndex BIT=0 
	,	@NullOverrideTemperatureUnitIndex BIT=0 
	,	@NullOverrideDensityUnitIndex BIT=0 
	,	@NullOverridePressureUnitIndex BIT=0 
	,	@NullOverrideFlowUnitIndex BIT=0 
	,	@NullOverrideVolumeUnitIndex BIT=0 
	,	@NullOverrideMassUnitIndex BIT=0 
	,	@NullOverrideAdditiveVolumeUnitIndex BIT=0 
	,	@NullOverrideAdditiveProfileCycleAmountUnitIndex BIT=0 
	,	@NullOverrideAdditiveProfileRateUnitIndex BIT=0 
	,	@NullOverrideLevelDecimalPlaces BIT=0 
	,	@NullOverrideTemperatureDecimalPlaces BIT=0 
	,	@NullOverrideDensityDecimalPlaces BIT=0 
	,	@NullOverridePressureDecimalPlaces BIT=0 
	,	@NullOverrideFlowDecimalPlaces BIT=0 
	,	@NullOverrideVolumeDecimalPlaces BIT=0 
	,	@NullOverrideMassDecimalPlaces BIT=0 
	,	@NullOverrideAdditiveVolumeDecimalPlaces BIT=0 
	,	@NullOverrideUseComboBoxControls BIT=0 
	,	@NullOverrideMultipleTransportLineItems BIT=0 
	,	@NullOverrideSiteGuid BIT=0 
	,	@NullOverrideLookupTransTypeIndex BIT=0 
	,	@NullOverrideLookupDefaultStatusIndex BIT=0 
	,	@NullOverrideAssociatedTransactionAliasGuid BIT=0 
	,	@NullOverrideIncludeInDispatch BIT=0 
	,	@NullOverride_MasterRecordGuid BIT=0 
	,	@NullOverrideEnableAutoCompleteControls BIT=0 
	,	@NullOverridePermitNonReferenceData BIT=0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionAliasesUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.003 / 2014-02-05 16:52:29.8454317 -05:00
	-- Purpose: Update table [dbo].[tblTransactionAliases]
	-- Notes:
	-- 1. @TransactionAliasGuid and @UpdatedBy are required parameter.
	-- 2. If a value other than NULL is passed on @_RowVersion parameter then the stored procedure verifies whether _RowVersion of the record matches with the  
	--    @_RowVersion parameter and it will throw an exception if they don't match, otherwise it saves the parameters regardless.
	-- 3. The @_RowVersion output parameter will always be updated with new timestamp generated by the updating of the record.
	-- 4. To update a column with NULL then set the corresponding "@NullOverride..." parameter to 1 and either pass NULL through the correlated parameter 
	--    or do not include the parameter at all. 
	--    Example - Saving NULL to SiteGuid on tblEquipment:
	--            EXEC gsp_EquipmentUpdateByPK @EquipmentGuid='0000-...',@SiteGuid=NULL, @NullOverrideSiteGuid=1 
	--       or   EXEC gsp_EquipmentUpdateByPK @EquipmentGuid='0000-...', @NullOverrideSiteGuid=1 
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblTransactionAliases] WHERE TransactionAliasGuid=@TransactionAliasGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END
 
		UPDATE [dbo].[tblTransactionAliases] SET
			[AliasName]=(CASE ISNULL(@NullOverrideAliasName,0) WHEN 1 THEN @AliasName ELSE ISNULL(@AliasName,[AliasName]) END)
		,	[MeterCloseout]=(CASE ISNULL(@NullOverrideMeterCloseout,0) WHEN 1 THEN @MeterCloseout ELSE ISNULL(@MeterCloseout,[MeterCloseout]) END)
		,	[BulkShipment]=(CASE ISNULL(@NullOverrideBulkShipment,0) WHEN 1 THEN @BulkShipment ELSE ISNULL(@BulkShipment,[BulkShipment]) END)
		,	[DistributedImpact]=(CASE ISNULL(@NullOverrideDistributedImpact,0) WHEN 1 THEN @DistributedImpact ELSE ISNULL(@DistributedImpact,[DistributedImpact]) END)
		,	[MultipleLineItems]=(CASE ISNULL(@NullOverrideMultipleLineItems,0) WHEN 1 THEN @MultipleLineItems ELSE ISNULL(@MultipleLineItems,[MultipleLineItems]) END)
		,	[LimitSelectionsBasedOnHierarchy]=(CASE ISNULL(@NullOverrideLimitSelectionsBasedOnHierarchy,0) WHEN 1 THEN @LimitSelectionsBasedOnHierarchy ELSE ISNULL(@LimitSelectionsBasedOnHierarchy,[LimitSelectionsBasedOnHierarchy]) END)
		,	[LineItemEditControl]=(CASE ISNULL(@NullOverrideLineItemEditControl,0) WHEN 1 THEN @LineItemEditControl ELSE ISNULL(@LineItemEditControl,[LineItemEditControl]) END)
		,	[MultipleWeightReadings]=(CASE ISNULL(@NullOverrideMultipleWeightReadings,0) WHEN 1 THEN @MultipleWeightReadings ELSE ISNULL(@MultipleWeightReadings,[MultipleWeightReadings]) END)
		,	[WeightReadingEditControl]=(CASE ISNULL(@NullOverrideWeightReadingEditControl,0) WHEN 1 THEN @WeightReadingEditControl ELSE ISNULL(@WeightReadingEditControl,[WeightReadingEditControl]) END)
		,	[AssociatedReport]=(CASE ISNULL(@NullOverrideAssociatedReport,0) WHEN 1 THEN @AssociatedReport ELSE ISNULL(@AssociatedReport,[AssociatedReport]) END)
		,	[AssociatedPreloadReport]=(CASE ISNULL(@NullOverrideAssociatedPreloadReport,0) WHEN 1 THEN @AssociatedPreloadReport ELSE ISNULL(@AssociatedPreloadReport,[AssociatedPreloadReport]) END)
		,	[DestinationEquipmentTypes1]=(CASE ISNULL(@NullOverrideDestinationEquipmentTypes1,0) WHEN 1 THEN @DestinationEquipmentTypes1 ELSE ISNULL(@DestinationEquipmentTypes1,[DestinationEquipmentTypes1]) END)
		,	[DestinationEquipmentTypes2]=(CASE ISNULL(@NullOverrideDestinationEquipmentTypes2,0) WHEN 1 THEN @DestinationEquipmentTypes2 ELSE ISNULL(@DestinationEquipmentTypes2,[DestinationEquipmentTypes2]) END)
		,	[DestinationEquipmentTypes3]=(CASE ISNULL(@NullOverrideDestinationEquipmentTypes3,0) WHEN 1 THEN @DestinationEquipmentTypes3 ELSE ISNULL(@DestinationEquipmentTypes3,[DestinationEquipmentTypes3]) END)
		,	[SourceEquipmentTypes1]=(CASE ISNULL(@NullOverrideSourceEquipmentTypes1,0) WHEN 1 THEN @SourceEquipmentTypes1 ELSE ISNULL(@SourceEquipmentTypes1,[SourceEquipmentTypes1]) END)
		,	[SourceEquipmentTypes2]=(CASE ISNULL(@NullOverrideSourceEquipmentTypes2,0) WHEN 1 THEN @SourceEquipmentTypes2 ELSE ISNULL(@SourceEquipmentTypes2,[SourceEquipmentTypes2]) END)
		,	[SourceEquipmentTypes3]=(CASE ISNULL(@NullOverrideSourceEquipmentTypes3,0) WHEN 1 THEN @SourceEquipmentTypes3 ELSE ISNULL(@SourceEquipmentTypes3,[SourceEquipmentTypes3]) END)
		,	[UpdatedDate]=ISNULL(@UpdatedDate,SYSDATETIMEOFFSET())
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		,	[ShowCompanyName]=(CASE ISNULL(@NullOverrideShowCompanyName,0) WHEN 1 THEN @ShowCompanyName ELSE ISNULL(@ShowCompanyName,[ShowCompanyName]) END)
		,	[AggregateAssocTrans]=(CASE ISNULL(@NullOverrideAggregateAssocTrans,0) WHEN 1 THEN @AggregateAssocTrans ELSE ISNULL(@AggregateAssocTrans,[AggregateAssocTrans]) END)
		,	[EnableTotalQuantityExceededWarning]=(CASE ISNULL(@NullOverrideEnableTotalQuantityExceededWarning,0) WHEN 1 THEN @EnableTotalQuantityExceededWarning ELSE ISNULL(@EnableTotalQuantityExceededWarning,[EnableTotalQuantityExceededWarning]) END)
		,	[EnableQuantityToleranceExceededWarning]=(CASE ISNULL(@NullOverrideEnableQuantityToleranceExceededWarning,0) WHEN 1 THEN @EnableQuantityToleranceExceededWarning ELSE ISNULL(@EnableQuantityToleranceExceededWarning,[EnableQuantityToleranceExceededWarning]) END)
		,	[EnableTotalValueExceededWarning]=(CASE ISNULL(@NullOverrideEnableTotalValueExceededWarning,0) WHEN 1 THEN @EnableTotalValueExceededWarning ELSE ISNULL(@EnableTotalValueExceededWarning,[EnableTotalValueExceededWarning]) END)
		,	[EnableValueToleranceExceededWarning]=(CASE ISNULL(@NullOverrideEnableValueToleranceExceededWarning,0) WHEN 1 THEN @EnableValueToleranceExceededWarning ELSE ISNULL(@EnableValueToleranceExceededWarning,[EnableValueToleranceExceededWarning]) END)
		,	[LevelUnitIndex]=(CASE ISNULL(@NullOverrideLevelUnitIndex,0) WHEN 1 THEN @LevelUnitIndex ELSE ISNULL(@LevelUnitIndex,[LevelUnitIndex]) END)
		,	[TemperatureUnitIndex]=(CASE ISNULL(@NullOverrideTemperatureUnitIndex,0) WHEN 1 THEN @TemperatureUnitIndex ELSE ISNULL(@TemperatureUnitIndex,[TemperatureUnitIndex]) END)
		,	[DensityUnitIndex]=(CASE ISNULL(@NullOverrideDensityUnitIndex,0) WHEN 1 THEN @DensityUnitIndex ELSE ISNULL(@DensityUnitIndex,[DensityUnitIndex]) END)
		,	[PressureUnitIndex]=(CASE ISNULL(@NullOverridePressureUnitIndex,0) WHEN 1 THEN @PressureUnitIndex ELSE ISNULL(@PressureUnitIndex,[PressureUnitIndex]) END)
		,	[FlowUnitIndex]=(CASE ISNULL(@NullOverrideFlowUnitIndex,0) WHEN 1 THEN @FlowUnitIndex ELSE ISNULL(@FlowUnitIndex,[FlowUnitIndex]) END)
		,	[VolumeUnitIndex]=(CASE ISNULL(@NullOverrideVolumeUnitIndex,0) WHEN 1 THEN @VolumeUnitIndex ELSE ISNULL(@VolumeUnitIndex,[VolumeUnitIndex]) END)
		,	[MassUnitIndex]=(CASE ISNULL(@NullOverrideMassUnitIndex,0) WHEN 1 THEN @MassUnitIndex ELSE ISNULL(@MassUnitIndex,[MassUnitIndex]) END)
		,	[AdditiveVolumeUnitIndex]=(CASE ISNULL(@NullOverrideAdditiveVolumeUnitIndex,0) WHEN 1 THEN @AdditiveVolumeUnitIndex ELSE ISNULL(@AdditiveVolumeUnitIndex,[AdditiveVolumeUnitIndex]) END)
		,	[AdditiveProfileCycleAmountUnitIndex]=(CASE ISNULL(@NullOverrideAdditiveProfileCycleAmountUnitIndex,0) WHEN 1 THEN @AdditiveProfileCycleAmountUnitIndex ELSE ISNULL(@AdditiveProfileCycleAmountUnitIndex,[AdditiveProfileCycleAmountUnitIndex]) END)
		,	[AdditiveProfileRateUnitIndex]=(CASE ISNULL(@NullOverrideAdditiveProfileRateUnitIndex,0) WHEN 1 THEN @AdditiveProfileRateUnitIndex ELSE ISNULL(@AdditiveProfileRateUnitIndex,[AdditiveProfileRateUnitIndex]) END)
		,	[LevelDecimalPlaces]=(CASE ISNULL(@NullOverrideLevelDecimalPlaces,0) WHEN 1 THEN @LevelDecimalPlaces ELSE ISNULL(@LevelDecimalPlaces,[LevelDecimalPlaces]) END)
		,	[TemperatureDecimalPlaces]=(CASE ISNULL(@NullOverrideTemperatureDecimalPlaces,0) WHEN 1 THEN @TemperatureDecimalPlaces ELSE ISNULL(@TemperatureDecimalPlaces,[TemperatureDecimalPlaces]) END)
		,	[DensityDecimalPlaces]=(CASE ISNULL(@NullOverrideDensityDecimalPlaces,0) WHEN 1 THEN @DensityDecimalPlaces ELSE ISNULL(@DensityDecimalPlaces,[DensityDecimalPlaces]) END)
		,	[PressureDecimalPlaces]=(CASE ISNULL(@NullOverridePressureDecimalPlaces,0) WHEN 1 THEN @PressureDecimalPlaces ELSE ISNULL(@PressureDecimalPlaces,[PressureDecimalPlaces]) END)
		,	[FlowDecimalPlaces]=(CASE ISNULL(@NullOverrideFlowDecimalPlaces,0) WHEN 1 THEN @FlowDecimalPlaces ELSE ISNULL(@FlowDecimalPlaces,[FlowDecimalPlaces]) END)
		,	[VolumeDecimalPlaces]=(CASE ISNULL(@NullOverrideVolumeDecimalPlaces,0) WHEN 1 THEN @VolumeDecimalPlaces ELSE ISNULL(@VolumeDecimalPlaces,[VolumeDecimalPlaces]) END)
		,	[MassDecimalPlaces]=(CASE ISNULL(@NullOverrideMassDecimalPlaces,0) WHEN 1 THEN @MassDecimalPlaces ELSE ISNULL(@MassDecimalPlaces,[MassDecimalPlaces]) END)
		,	[AdditiveVolumeDecimalPlaces]=(CASE ISNULL(@NullOverrideAdditiveVolumeDecimalPlaces,0) WHEN 1 THEN @AdditiveVolumeDecimalPlaces ELSE ISNULL(@AdditiveVolumeDecimalPlaces,[AdditiveVolumeDecimalPlaces]) END)
		,	[UseComboBoxControls]=(CASE ISNULL(@NullOverrideUseComboBoxControls,0) WHEN 1 THEN @UseComboBoxControls ELSE ISNULL(@UseComboBoxControls,[UseComboBoxControls]) END)
		,	[MultipleTransportLineItems]=(CASE ISNULL(@NullOverrideMultipleTransportLineItems,0) WHEN 1 THEN @MultipleTransportLineItems ELSE ISNULL(@MultipleTransportLineItems,[MultipleTransportLineItems]) END)
		,	[SiteGuid]=(CASE ISNULL(@NullOverrideSiteGuid,0) WHEN 1 THEN @SiteGuid ELSE ISNULL(@SiteGuid,[SiteGuid]) END)
		,	[LookupTransTypeIndex]=(CASE ISNULL(@NullOverrideLookupTransTypeIndex,0) WHEN 1 THEN @LookupTransTypeIndex ELSE ISNULL(@LookupTransTypeIndex,[LookupTransTypeIndex]) END)
		,	[LookupDefaultStatusIndex]=(CASE ISNULL(@NullOverrideLookupDefaultStatusIndex,0) WHEN 1 THEN @LookupDefaultStatusIndex ELSE ISNULL(@LookupDefaultStatusIndex,[LookupDefaultStatusIndex]) END)
		,	[AssociatedTransactionAliasGuid]=(CASE ISNULL(@NullOverrideAssociatedTransactionAliasGuid,0) WHEN 1 THEN @AssociatedTransactionAliasGuid ELSE ISNULL(@AssociatedTransactionAliasGuid,[AssociatedTransactionAliasGuid]) END)
		,	[IncludeInDispatch]=(CASE ISNULL(@NullOverrideIncludeInDispatch,0) WHEN 1 THEN @IncludeInDispatch ELSE ISNULL(@IncludeInDispatch,[IncludeInDispatch]) END)
		,	[_MasterRecordGuid]=(CASE ISNULL(@NullOverride_MasterRecordGuid,0) WHEN 1 THEN @_MasterRecordGuid ELSE ISNULL(@_MasterRecordGuid,[_MasterRecordGuid]) END)
		,	[EnableAutoCompleteControls]=(CASE ISNULL(@NullOverrideEnableAutoCompleteControls,0) WHEN 1 THEN @EnableAutoCompleteControls ELSE ISNULL(@EnableAutoCompleteControls,[EnableAutoCompleteControls]) END)
		,	[PermitNonReferenceData]=(CASE ISNULL(@NullOverridePermitNonReferenceData,0) WHEN 1 THEN @PermitNonReferenceData ELSE ISNULL(@PermitNonReferenceData,[PermitNonReferenceData]) END)
		WHERE	TransactionAliasGuid=@TransactionAliasGuid;
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionAliases]           
		WHERE TransactionAliasGuid=@TransactionAliasGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionAliasesUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
