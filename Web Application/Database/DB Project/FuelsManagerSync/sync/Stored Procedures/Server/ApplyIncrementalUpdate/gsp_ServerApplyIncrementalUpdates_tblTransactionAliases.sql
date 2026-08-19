-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionAliases
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblTransactionAliases]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AliasName nvarchar(32),
@MeterCloseout bit,
@BulkShipment bit,
@DistributedImpact bit,
@MultipleLineItems bit,
@LimitSelectionsBasedOnHierarchy bit,
@LineItemEditControl bit,
@MultipleWeightReadings bit,
@WeightReadingEditControl bit,
@AssociatedReport nvarchar(80),
@AssociatedPreloadReport nvarchar(80),
@DestinationEquipmentTypes1 bigint,
@DestinationEquipmentTypes2 bigint,
@DestinationEquipmentTypes3 bigint,
@SourceEquipmentTypes1 bigint,
@SourceEquipmentTypes2 bigint,
@SourceEquipmentTypes3 bigint,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@ShowCompanyName smallint,
@AggregateAssocTrans bit,
@EnableTotalQuantityExceededWarning bit,
@EnableQuantityToleranceExceededWarning bit,
@EnableTotalValueExceededWarning bit,
@EnableValueToleranceExceededWarning bit,
@LevelUnitIndex int,
@TemperatureUnitIndex int,
@DensityUnitIndex int,
@PressureUnitIndex int,
@FlowUnitIndex int,
@VolumeUnitIndex int,
@MassUnitIndex int,
@AdditiveVolumeUnitIndex int,
@AdditiveProfileCycleAmountUnitIndex int,
@AdditiveProfileRateUnitIndex int,
@LevelDecimalPlaces tinyint,
@TemperatureDecimalPlaces tinyint,
@DensityDecimalPlaces tinyint,
@PressureDecimalPlaces tinyint,
@FlowDecimalPlaces tinyint,
@VolumeDecimalPlaces tinyint,
@MassDecimalPlaces tinyint,
@AdditiveVolumeDecimalPlaces tinyint,
@UseComboBoxControls bit,
@MultipleTransportLineItems bit,
@TransactionAliasGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupTransTypeIndex smallint,
@LookupDefaultStatusIndex int,
@AssociatedTransactionAliasGuid uniqueidentifier,
@IncludeInDispatch bit,
@_MasterRecordGuid uniqueidentifier,
@EnableAutoCompleteControls bit,
@PermitNonReferenceData bit,
@UseTransactionDetailWithLayout bit,
@DefaultMeterToEquipmentID bit,
@LimitSourceEquipmentByProduct bit,
@RememberMeterEndForMeterID bit,
@PopulateCompaniesFromEquipment bit,
@PopulateGrossVolumeFromMeterValues bit,
@UseMeterAndCompressionFactorFromMeter bit,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTransactionAliases varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblTransactionAliases] CT
                        WHERE CT.PK_TransactionAliasGuid = @TransactionAliasGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblTransactionAliases].[AliasName],[dbo].[tblTransactionAliases].[MeterCloseout],[dbo].[tblTransactionAliases].[BulkShipment],[dbo].[tblTransactionAliases].[DistributedImpact],[dbo].[tblTransactionAliases].[MultipleLineItems],[dbo].[tblTransactionAliases].[LimitSelectionsBasedOnHierarchy],[dbo].[tblTransactionAliases].[LineItemEditControl],[dbo].[tblTransactionAliases].[MultipleWeightReadings],[dbo].[tblTransactionAliases].[WeightReadingEditControl],[dbo].[tblTransactionAliases].[AssociatedReport],[dbo].[tblTransactionAliases].[AssociatedPreloadReport],[dbo].[tblTransactionAliases].[DestinationEquipmentTypes1],[dbo].[tblTransactionAliases].[DestinationEquipmentTypes2],[dbo].[tblTransactionAliases].[DestinationEquipmentTypes3],[dbo].[tblTransactionAliases].[SourceEquipmentTypes1],[dbo].[tblTransactionAliases].[SourceEquipmentTypes2],[dbo].[tblTransactionAliases].[SourceEquipmentTypes3],[dbo].[tblTransactionAliases].[CreatedDate],[dbo].[tblTransactionAliases].[CreatedBy],[dbo].[tblTransactionAliases].[UpdatedDate],[dbo].[tblTransactionAliases].[UpdatedBy],[dbo].[tblTransactionAliases].[ShowCompanyName],[dbo].[tblTransactionAliases].[AggregateAssocTrans],[dbo].[tblTransactionAliases].[EnableTotalQuantityExceededWarning],[dbo].[tblTransactionAliases].[EnableQuantityToleranceExceededWarning],[dbo].[tblTransactionAliases].[EnableTotalValueExceededWarning],[dbo].[tblTransactionAliases].[EnableValueToleranceExceededWarning],[dbo].[tblTransactionAliases].[LevelUnitIndex],[dbo].[tblTransactionAliases].[TemperatureUnitIndex],[dbo].[tblTransactionAliases].[DensityUnitIndex],[dbo].[tblTransactionAliases].[PressureUnitIndex],[dbo].[tblTransactionAliases].[FlowUnitIndex],[dbo].[tblTransactionAliases].[VolumeUnitIndex],[dbo].[tblTransactionAliases].[MassUnitIndex],[dbo].[tblTransactionAliases].[AdditiveVolumeUnitIndex],[dbo].[tblTransactionAliases].[AdditiveProfileCycleAmountUnitIndex],[dbo].[tblTransactionAliases].[AdditiveProfileRateUnitIndex],[dbo].[tblTransactionAliases].[LevelDecimalPlaces],[dbo].[tblTransactionAliases].[TemperatureDecimalPlaces],[dbo].[tblTransactionAliases].[DensityDecimalPlaces],[dbo].[tblTransactionAliases].[PressureDecimalPlaces],[dbo].[tblTransactionAliases].[FlowDecimalPlaces],[dbo].[tblTransactionAliases].[VolumeDecimalPlaces],[dbo].[tblTransactionAliases].[MassDecimalPlaces],[dbo].[tblTransactionAliases].[AdditiveVolumeDecimalPlaces],[dbo].[tblTransactionAliases].[UseComboBoxControls],[dbo].[tblTransactionAliases].[MultipleTransportLineItems],[dbo].[tblTransactionAliases].[TransactionAliasGuid],[dbo].[tblTransactionAliases].[SiteGuid],[dbo].[tblTransactionAliases].[LookupTransTypeIndex],[dbo].[tblTransactionAliases].[LookupDefaultStatusIndex],[dbo].[tblTransactionAliases].[AssociatedTransactionAliasGuid],[dbo].[tblTransactionAliases].[IncludeInDispatch],[dbo].[tblTransactionAliases].[_MasterRecordGuid],[dbo].[tblTransactionAliases].[EnableAutoCompleteControls],[dbo].[tblTransactionAliases].[PermitNonReferenceData],[dbo].[tblTransactionAliases].[UseTransactionDetailWithLayout],[dbo].[tblTransactionAliases].[DefaultMeterToEquipmentID],[dbo].[tblTransactionAliases].[LimitSourceEquipmentByProduct],[dbo].[tblTransactionAliases].[RememberMeterEndForMeterID],[dbo].[tblTransactionAliases].[PopulateCompaniesFromEquipment],[dbo].[tblTransactionAliases].[PopulateGrossVolumeFromMeterValues],[dbo].[tblTransactionAliases].[UseMeterAndCompressionFactorFromMeter]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblTransactionAliases]
                        INNER JOIN [track].[tblTransactionAliases] CT
                            ON CT.PK_TransactionAliasGuid = [dbo].[tblTransactionAliases].[TransactionAliasGuid] 
                    WHERE CT.PK_TransactionAliasGuid = @TransactionAliasGuid
            ) MERGE existingData
            USING (SELECT @AliasName,@MeterCloseout,@BulkShipment,@DistributedImpact,@MultipleLineItems,@LimitSelectionsBasedOnHierarchy,@LineItemEditControl,@MultipleWeightReadings,@WeightReadingEditControl,@AssociatedReport,@AssociatedPreloadReport,@DestinationEquipmentTypes1,@DestinationEquipmentTypes2,@DestinationEquipmentTypes3,@SourceEquipmentTypes1,@SourceEquipmentTypes2,@SourceEquipmentTypes3,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@ShowCompanyName,@AggregateAssocTrans,@EnableTotalQuantityExceededWarning,@EnableQuantityToleranceExceededWarning,@EnableTotalValueExceededWarning,@EnableValueToleranceExceededWarning,@LevelUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@PressureUnitIndex,@FlowUnitIndex,@VolumeUnitIndex,@MassUnitIndex,@AdditiveVolumeUnitIndex,@AdditiveProfileCycleAmountUnitIndex,@AdditiveProfileRateUnitIndex,@LevelDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@PressureDecimalPlaces,@FlowDecimalPlaces,@VolumeDecimalPlaces,@MassDecimalPlaces,@AdditiveVolumeDecimalPlaces,@UseComboBoxControls,@MultipleTransportLineItems,@TransactionAliasGuid,@SiteGuid,@LookupTransTypeIndex,@LookupDefaultStatusIndex,@AssociatedTransactionAliasGuid,@IncludeInDispatch,@_MasterRecordGuid,@EnableAutoCompleteControls,@PermitNonReferenceData,@UseTransactionDetailWithLayout,@DefaultMeterToEquipmentID,@LimitSourceEquipmentByProduct,@RememberMeterEndForMeterID,@PopulateCompaniesFromEquipment,@PopulateGrossVolumeFromMeterValues,@UseMeterAndCompressionFactorFromMeter
                    ) AS remoteChanges ([AliasName],[MeterCloseout],[BulkShipment],[DistributedImpact],[MultipleLineItems],[LimitSelectionsBasedOnHierarchy],[LineItemEditControl],[MultipleWeightReadings],[WeightReadingEditControl],[AssociatedReport],[AssociatedPreloadReport],[DestinationEquipmentTypes1],[DestinationEquipmentTypes2],[DestinationEquipmentTypes3],[SourceEquipmentTypes1],[SourceEquipmentTypes2],[SourceEquipmentTypes3],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ShowCompanyName],[AggregateAssocTrans],[EnableTotalQuantityExceededWarning],[EnableQuantityToleranceExceededWarning],[EnableTotalValueExceededWarning],[EnableValueToleranceExceededWarning],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[UseComboBoxControls],[MultipleTransportLineItems],[TransactionAliasGuid],[SiteGuid],[LookupTransTypeIndex],[LookupDefaultStatusIndex],[AssociatedTransactionAliasGuid],[IncludeInDispatch],[_MasterRecordGuid],[EnableAutoCompleteControls],[PermitNonReferenceData],[UseTransactionDetailWithLayout],[DefaultMeterToEquipmentID],[LimitSourceEquipmentByProduct],[RememberMeterEndForMeterID],[PopulateCompaniesFromEquipment],[PopulateGrossVolumeFromMeterValues],[UseMeterAndCompressionFactorFromMeter])
            ON (existingData.[TransactionAliasGuid] = remoteChanges.[TransactionAliasGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [AliasName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AliasName'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[AliasName] ELSE remoteChanges.[AliasName] END
                       ,[MeterCloseout] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterCloseout'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[MeterCloseout] ELSE remoteChanges.[MeterCloseout] END
                       ,[BulkShipment] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BulkShipment'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[BulkShipment] ELSE remoteChanges.[BulkShipment] END
                       ,[DistributedImpact] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DistributedImpact'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[DistributedImpact] ELSE remoteChanges.[DistributedImpact] END
                       ,[MultipleLineItems] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MultipleLineItems'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[MultipleLineItems] ELSE remoteChanges.[MultipleLineItems] END
                       ,[LimitSelectionsBasedOnHierarchy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LimitSelectionsBasedOnHierarchy'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[LimitSelectionsBasedOnHierarchy] ELSE remoteChanges.[LimitSelectionsBasedOnHierarchy] END
                       ,[LineItemEditControl] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LineItemEditControl'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[LineItemEditControl] ELSE remoteChanges.[LineItemEditControl] END
                       ,[MultipleWeightReadings] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MultipleWeightReadings'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[MultipleWeightReadings] ELSE remoteChanges.[MultipleWeightReadings] END
                       ,[WeightReadingEditControl] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WeightReadingEditControl'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[WeightReadingEditControl] ELSE remoteChanges.[WeightReadingEditControl] END
                       ,[AssociatedReport] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssociatedReport'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[AssociatedReport] ELSE remoteChanges.[AssociatedReport] END
                       ,[AssociatedPreloadReport] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssociatedPreloadReport'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[AssociatedPreloadReport] ELSE remoteChanges.[AssociatedPreloadReport] END
                       ,[DestinationEquipmentTypes1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DestinationEquipmentTypes1'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[DestinationEquipmentTypes1] ELSE remoteChanges.[DestinationEquipmentTypes1] END
                       ,[DestinationEquipmentTypes2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DestinationEquipmentTypes2'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[DestinationEquipmentTypes2] ELSE remoteChanges.[DestinationEquipmentTypes2] END
                       ,[DestinationEquipmentTypes3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DestinationEquipmentTypes3'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[DestinationEquipmentTypes3] ELSE remoteChanges.[DestinationEquipmentTypes3] END
                       ,[SourceEquipmentTypes1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceEquipmentTypes1'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[SourceEquipmentTypes1] ELSE remoteChanges.[SourceEquipmentTypes1] END
                       ,[SourceEquipmentTypes2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceEquipmentTypes2'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[SourceEquipmentTypes2] ELSE remoteChanges.[SourceEquipmentTypes2] END
                       ,[SourceEquipmentTypes3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceEquipmentTypes3'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[SourceEquipmentTypes3] ELSE remoteChanges.[SourceEquipmentTypes3] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[ShowCompanyName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShowCompanyName'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[ShowCompanyName] ELSE remoteChanges.[ShowCompanyName] END
                       ,[AggregateAssocTrans] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AggregateAssocTrans'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[AggregateAssocTrans] ELSE remoteChanges.[AggregateAssocTrans] END
                       ,[EnableTotalQuantityExceededWarning] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableTotalQuantityExceededWarning'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[EnableTotalQuantityExceededWarning] ELSE remoteChanges.[EnableTotalQuantityExceededWarning] END
                       ,[EnableQuantityToleranceExceededWarning] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableQuantityToleranceExceededWarning'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[EnableQuantityToleranceExceededWarning] ELSE remoteChanges.[EnableQuantityToleranceExceededWarning] END
                       ,[EnableTotalValueExceededWarning] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableTotalValueExceededWarning'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[EnableTotalValueExceededWarning] ELSE remoteChanges.[EnableTotalValueExceededWarning] END
                       ,[EnableValueToleranceExceededWarning] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableValueToleranceExceededWarning'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[EnableValueToleranceExceededWarning] ELSE remoteChanges.[EnableValueToleranceExceededWarning] END
                       ,[LevelUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[LevelUnitIndex] ELSE remoteChanges.[LevelUnitIndex] END
                       ,[TemperatureUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[TemperatureUnitIndex] ELSE remoteChanges.[TemperatureUnitIndex] END
                       ,[DensityUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[DensityUnitIndex] ELSE remoteChanges.[DensityUnitIndex] END
                       ,[PressureUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[PressureUnitIndex] ELSE remoteChanges.[PressureUnitIndex] END
                       ,[FlowUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[FlowUnitIndex] ELSE remoteChanges.[FlowUnitIndex] END
                       ,[VolumeUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[VolumeUnitIndex] ELSE remoteChanges.[VolumeUnitIndex] END
                       ,[MassUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[MassUnitIndex] ELSE remoteChanges.[MassUnitIndex] END
                       ,[AdditiveVolumeUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveVolumeUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[AdditiveVolumeUnitIndex] ELSE remoteChanges.[AdditiveVolumeUnitIndex] END
                       ,[AdditiveProfileCycleAmountUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveProfileCycleAmountUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[AdditiveProfileCycleAmountUnitIndex] ELSE remoteChanges.[AdditiveProfileCycleAmountUnitIndex] END
                       ,[AdditiveProfileRateUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveProfileRateUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[AdditiveProfileRateUnitIndex] ELSE remoteChanges.[AdditiveProfileRateUnitIndex] END
                       ,[LevelDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[LevelDecimalPlaces] ELSE remoteChanges.[LevelDecimalPlaces] END
                       ,[TemperatureDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[TemperatureDecimalPlaces] ELSE remoteChanges.[TemperatureDecimalPlaces] END
                       ,[DensityDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[DensityDecimalPlaces] ELSE remoteChanges.[DensityDecimalPlaces] END
                       ,[PressureDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[PressureDecimalPlaces] ELSE remoteChanges.[PressureDecimalPlaces] END
                       ,[FlowDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[FlowDecimalPlaces] ELSE remoteChanges.[FlowDecimalPlaces] END
                       ,[VolumeDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[VolumeDecimalPlaces] ELSE remoteChanges.[VolumeDecimalPlaces] END
                       ,[MassDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[MassDecimalPlaces] ELSE remoteChanges.[MassDecimalPlaces] END
                       ,[AdditiveVolumeDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveVolumeDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[AdditiveVolumeDecimalPlaces] ELSE remoteChanges.[AdditiveVolumeDecimalPlaces] END
                       ,[UseComboBoxControls] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseComboBoxControls'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[UseComboBoxControls] ELSE remoteChanges.[UseComboBoxControls] END
                       ,[MultipleTransportLineItems] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MultipleTransportLineItems'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[MultipleTransportLineItems] ELSE remoteChanges.[MultipleTransportLineItems] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupTransTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupTransTypeIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[LookupTransTypeIndex] ELSE remoteChanges.[LookupTransTypeIndex] END
                       ,[LookupDefaultStatusIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupDefaultStatusIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[LookupDefaultStatusIndex] ELSE remoteChanges.[LookupDefaultStatusIndex] END
                       ,[AssociatedTransactionAliasGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssociatedTransactionAliasGuid'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[AssociatedTransactionAliasGuid] ELSE remoteChanges.[AssociatedTransactionAliasGuid] END
                       ,[IncludeInDispatch] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IncludeInDispatch'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[IncludeInDispatch] ELSE remoteChanges.[IncludeInDispatch] END
                       ,[_MasterRecordGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('_MasterRecordGuid'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[_MasterRecordGuid] ELSE remoteChanges.[_MasterRecordGuid] END
                       ,[EnableAutoCompleteControls] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableAutoCompleteControls'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[EnableAutoCompleteControls] ELSE remoteChanges.[EnableAutoCompleteControls] END
                       ,[PermitNonReferenceData] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PermitNonReferenceData'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[PermitNonReferenceData] ELSE remoteChanges.[PermitNonReferenceData] END
                       ,[UseTransactionDetailWithLayout] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseTransactionDetailWithLayout'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[UseTransactionDetailWithLayout] ELSE remoteChanges.[UseTransactionDetailWithLayout] END
                       ,[DefaultMeterToEquipmentID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DefaultMeterToEquipmentID'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[DefaultMeterToEquipmentID] ELSE remoteChanges.[DefaultMeterToEquipmentID] END
                       ,[LimitSourceEquipmentByProduct] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LimitSourceEquipmentByProduct'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[LimitSourceEquipmentByProduct] ELSE remoteChanges.[LimitSourceEquipmentByProduct] END
                       ,[RememberMeterEndForMeterID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RememberMeterEndForMeterID'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[RememberMeterEndForMeterID] ELSE remoteChanges.[RememberMeterEndForMeterID] END
                       ,[PopulateCompaniesFromEquipment] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PopulateCompaniesFromEquipment'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[PopulateCompaniesFromEquipment] ELSE remoteChanges.[PopulateCompaniesFromEquipment] END
                       ,[PopulateGrossVolumeFromMeterValues] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PopulateGrossVolumeFromMeterValues'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[PopulateGrossVolumeFromMeterValues] ELSE remoteChanges.[PopulateGrossVolumeFromMeterValues] END
                       ,[UseMeterAndCompressionFactorFromMeter] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseMeterAndCompressionFactorFromMeter'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN existingData.[UseMeterAndCompressionFactorFromMeter] ELSE remoteChanges.[UseMeterAndCompressionFactorFromMeter] END

            WHEN NOT MATCHED THEN
                INSERT ([AliasName],[MeterCloseout],[BulkShipment],[DistributedImpact],[MultipleLineItems],[LimitSelectionsBasedOnHierarchy],[LineItemEditControl],[MultipleWeightReadings],[WeightReadingEditControl],[AssociatedReport],[AssociatedPreloadReport],[DestinationEquipmentTypes1],[DestinationEquipmentTypes2],[DestinationEquipmentTypes3],[SourceEquipmentTypes1],[SourceEquipmentTypes2],[SourceEquipmentTypes3],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ShowCompanyName],[AggregateAssocTrans],[EnableTotalQuantityExceededWarning],[EnableQuantityToleranceExceededWarning],[EnableTotalValueExceededWarning],[EnableValueToleranceExceededWarning],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[UseComboBoxControls],[MultipleTransportLineItems],[TransactionAliasGuid],[SiteGuid],[LookupTransTypeIndex],[LookupDefaultStatusIndex],[AssociatedTransactionAliasGuid],[IncludeInDispatch],[_MasterRecordGuid],[EnableAutoCompleteControls],[PermitNonReferenceData],[UseTransactionDetailWithLayout],[DefaultMeterToEquipmentID],[LimitSourceEquipmentByProduct],[RememberMeterEndForMeterID],[PopulateCompaniesFromEquipment],[PopulateGrossVolumeFromMeterValues],[UseMeterAndCompressionFactorFromMeter])
                    VALUES (@AliasName,@MeterCloseout,@BulkShipment,@DistributedImpact,@MultipleLineItems,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LimitSelectionsBasedOnHierarchy'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @LimitSelectionsBasedOnHierarchy END),@LineItemEditControl,@MultipleWeightReadings,@WeightReadingEditControl,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssociatedReport'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @AssociatedReport END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssociatedPreloadReport'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @AssociatedPreloadReport END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DestinationEquipmentTypes1'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @DestinationEquipmentTypes1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DestinationEquipmentTypes2'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @DestinationEquipmentTypes2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DestinationEquipmentTypes3'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @DestinationEquipmentTypes3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceEquipmentTypes1'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @SourceEquipmentTypes1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceEquipmentTypes2'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @SourceEquipmentTypes2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SourceEquipmentTypes3'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @SourceEquipmentTypes3 END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShowCompanyName'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @ShowCompanyName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AggregateAssocTrans'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @AggregateAssocTrans END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableTotalQuantityExceededWarning'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @EnableTotalQuantityExceededWarning END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableQuantityToleranceExceededWarning'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @EnableQuantityToleranceExceededWarning END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableTotalValueExceededWarning'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @EnableTotalValueExceededWarning END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableValueToleranceExceededWarning'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @EnableValueToleranceExceededWarning END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @LevelUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @TemperatureUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @DensityUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @PressureUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @FlowUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @VolumeUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @MassUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveVolumeUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @AdditiveVolumeUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveProfileCycleAmountUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @AdditiveProfileCycleAmountUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveProfileRateUnitIndex'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @AdditiveProfileRateUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @LevelDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @TemperatureDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @DensityDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @PressureDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @FlowDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @VolumeDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @MassDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveVolumeDecimalPlaces'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @AdditiveVolumeDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseComboBoxControls'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @UseComboBoxControls END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MultipleTransportLineItems'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @MultipleTransportLineItems END),@TransactionAliasGuid,@SiteGuid,@LookupTransTypeIndex,@LookupDefaultStatusIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssociatedTransactionAliasGuid'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @AssociatedTransactionAliasGuid END),@IncludeInDispatch,@_MasterRecordGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableAutoCompleteControls'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @EnableAutoCompleteControls END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PermitNonReferenceData'), @sync_supported_columns_tblTransactionAliases)) WHEN 0 THEN NULL ELSE @PermitNonReferenceData END),@UseTransactionDetailWithLayout,@DefaultMeterToEquipmentID,@LimitSourceEquipmentByProduct,@RememberMeterEndForMeterID,@PopulateCompaniesFromEquipment,@PopulateGrossVolumeFromMeterValues,@UseMeterAndCompressionFactorFromMeter)
            ;
         SET @sync_row_count = @@rowcount;
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END

    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionAliasGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionAliasGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionAliasGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactionAliases] WHERE TransactionAliasGuid = @TransactionAliasGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
