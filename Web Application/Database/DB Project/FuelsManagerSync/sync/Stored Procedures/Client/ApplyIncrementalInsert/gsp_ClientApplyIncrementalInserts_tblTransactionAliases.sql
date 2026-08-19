-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionAliases
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblTransactionAliases]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTransactionAliases] AS existingData
        USING (SELECT @AliasName 'AliasName',@MeterCloseout 'MeterCloseout',@BulkShipment 'BulkShipment',@DistributedImpact 'DistributedImpact',@MultipleLineItems 'MultipleLineItems',@LimitSelectionsBasedOnHierarchy 'LimitSelectionsBasedOnHierarchy',@LineItemEditControl 'LineItemEditControl',@MultipleWeightReadings 'MultipleWeightReadings',@WeightReadingEditControl 'WeightReadingEditControl',@AssociatedReport 'AssociatedReport',@AssociatedPreloadReport 'AssociatedPreloadReport',@DestinationEquipmentTypes1 'DestinationEquipmentTypes1',@DestinationEquipmentTypes2 'DestinationEquipmentTypes2',@DestinationEquipmentTypes3 'DestinationEquipmentTypes3',@SourceEquipmentTypes1 'SourceEquipmentTypes1',@SourceEquipmentTypes2 'SourceEquipmentTypes2',@SourceEquipmentTypes3 'SourceEquipmentTypes3',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@ShowCompanyName 'ShowCompanyName',@AggregateAssocTrans 'AggregateAssocTrans',@EnableTotalQuantityExceededWarning 'EnableTotalQuantityExceededWarning',@EnableQuantityToleranceExceededWarning 'EnableQuantityToleranceExceededWarning',@EnableTotalValueExceededWarning 'EnableTotalValueExceededWarning',@EnableValueToleranceExceededWarning 'EnableValueToleranceExceededWarning',@LevelUnitIndex 'LevelUnitIndex',@TemperatureUnitIndex 'TemperatureUnitIndex',@DensityUnitIndex 'DensityUnitIndex',@PressureUnitIndex 'PressureUnitIndex',@FlowUnitIndex 'FlowUnitIndex',@VolumeUnitIndex 'VolumeUnitIndex',@MassUnitIndex 'MassUnitIndex',@AdditiveVolumeUnitIndex 'AdditiveVolumeUnitIndex',@AdditiveProfileCycleAmountUnitIndex 'AdditiveProfileCycleAmountUnitIndex',@AdditiveProfileRateUnitIndex 'AdditiveProfileRateUnitIndex',@LevelDecimalPlaces 'LevelDecimalPlaces',@TemperatureDecimalPlaces 'TemperatureDecimalPlaces',@DensityDecimalPlaces 'DensityDecimalPlaces',@PressureDecimalPlaces 'PressureDecimalPlaces',@FlowDecimalPlaces 'FlowDecimalPlaces',@VolumeDecimalPlaces 'VolumeDecimalPlaces',@MassDecimalPlaces 'MassDecimalPlaces',@AdditiveVolumeDecimalPlaces 'AdditiveVolumeDecimalPlaces',@UseComboBoxControls 'UseComboBoxControls',@MultipleTransportLineItems 'MultipleTransportLineItems',@TransactionAliasGuid 'TransactionAliasGuid',@SiteGuid 'SiteGuid',@LookupTransTypeIndex 'LookupTransTypeIndex',@LookupDefaultStatusIndex 'LookupDefaultStatusIndex',@AssociatedTransactionAliasGuid 'AssociatedTransactionAliasGuid',@IncludeInDispatch 'IncludeInDispatch',@_MasterRecordGuid '_MasterRecordGuid',@EnableAutoCompleteControls 'EnableAutoCompleteControls',@PermitNonReferenceData 'PermitNonReferenceData',@UseTransactionDetailWithLayout 'UseTransactionDetailWithLayout',@DefaultMeterToEquipmentID 'DefaultMeterToEquipmentID',@LimitSourceEquipmentByProduct 'LimitSourceEquipmentByProduct',@RememberMeterEndForMeterID 'RememberMeterEndForMeterID',@PopulateCompaniesFromEquipment 'PopulateCompaniesFromEquipment',@PopulateGrossVolumeFromMeterValues 'PopulateGrossVolumeFromMeterValues',@UseMeterAndCompressionFactorFromMeter 'UseMeterAndCompressionFactorFromMeter'
                ) AS remoteChanges ([AliasName],[MeterCloseout],[BulkShipment],[DistributedImpact],[MultipleLineItems],[LimitSelectionsBasedOnHierarchy],[LineItemEditControl],[MultipleWeightReadings],[WeightReadingEditControl],[AssociatedReport],[AssociatedPreloadReport],[DestinationEquipmentTypes1],[DestinationEquipmentTypes2],[DestinationEquipmentTypes3],[SourceEquipmentTypes1],[SourceEquipmentTypes2],[SourceEquipmentTypes3],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ShowCompanyName],[AggregateAssocTrans],[EnableTotalQuantityExceededWarning],[EnableQuantityToleranceExceededWarning],[EnableTotalValueExceededWarning],[EnableValueToleranceExceededWarning],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[UseComboBoxControls],[MultipleTransportLineItems],[TransactionAliasGuid],[SiteGuid],[LookupTransTypeIndex],[LookupDefaultStatusIndex],[AssociatedTransactionAliasGuid],[IncludeInDispatch],[_MasterRecordGuid],[EnableAutoCompleteControls],[PermitNonReferenceData],[UseTransactionDetailWithLayout],[DefaultMeterToEquipmentID],[LimitSourceEquipmentByProduct],[RememberMeterEndForMeterID],[PopulateCompaniesFromEquipment],[PopulateGrossVolumeFromMeterValues],[UseMeterAndCompressionFactorFromMeter])
        ON (existingData.[TransactionAliasGuid] = remoteChanges.[TransactionAliasGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [AliasName] = remoteChanges.[AliasName]
                       ,[MeterCloseout] = remoteChanges.[MeterCloseout]
                       ,[BulkShipment] = remoteChanges.[BulkShipment]
                       ,[DistributedImpact] = remoteChanges.[DistributedImpact]
                       ,[MultipleLineItems] = remoteChanges.[MultipleLineItems]
                       ,[LimitSelectionsBasedOnHierarchy] = remoteChanges.[LimitSelectionsBasedOnHierarchy]
                       ,[LineItemEditControl] = remoteChanges.[LineItemEditControl]
                       ,[MultipleWeightReadings] = remoteChanges.[MultipleWeightReadings]
                       ,[WeightReadingEditControl] = remoteChanges.[WeightReadingEditControl]
                       ,[AssociatedReport] = remoteChanges.[AssociatedReport]
                       ,[AssociatedPreloadReport] = remoteChanges.[AssociatedPreloadReport]
                       ,[DestinationEquipmentTypes1] = remoteChanges.[DestinationEquipmentTypes1]
                       ,[DestinationEquipmentTypes2] = remoteChanges.[DestinationEquipmentTypes2]
                       ,[DestinationEquipmentTypes3] = remoteChanges.[DestinationEquipmentTypes3]
                       ,[SourceEquipmentTypes1] = remoteChanges.[SourceEquipmentTypes1]
                       ,[SourceEquipmentTypes2] = remoteChanges.[SourceEquipmentTypes2]
                       ,[SourceEquipmentTypes3] = remoteChanges.[SourceEquipmentTypes3]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[ShowCompanyName] = remoteChanges.[ShowCompanyName]
                       ,[AggregateAssocTrans] = remoteChanges.[AggregateAssocTrans]
                       ,[EnableTotalQuantityExceededWarning] = remoteChanges.[EnableTotalQuantityExceededWarning]
                       ,[EnableQuantityToleranceExceededWarning] = remoteChanges.[EnableQuantityToleranceExceededWarning]
                       ,[EnableTotalValueExceededWarning] = remoteChanges.[EnableTotalValueExceededWarning]
                       ,[EnableValueToleranceExceededWarning] = remoteChanges.[EnableValueToleranceExceededWarning]
                       ,[LevelUnitIndex] = remoteChanges.[LevelUnitIndex]
                       ,[TemperatureUnitIndex] = remoteChanges.[TemperatureUnitIndex]
                       ,[DensityUnitIndex] = remoteChanges.[DensityUnitIndex]
                       ,[PressureUnitIndex] = remoteChanges.[PressureUnitIndex]
                       ,[FlowUnitIndex] = remoteChanges.[FlowUnitIndex]
                       ,[VolumeUnitIndex] = remoteChanges.[VolumeUnitIndex]
                       ,[MassUnitIndex] = remoteChanges.[MassUnitIndex]
                       ,[AdditiveVolumeUnitIndex] = remoteChanges.[AdditiveVolumeUnitIndex]
                       ,[AdditiveProfileCycleAmountUnitIndex] = remoteChanges.[AdditiveProfileCycleAmountUnitIndex]
                       ,[AdditiveProfileRateUnitIndex] = remoteChanges.[AdditiveProfileRateUnitIndex]
                       ,[LevelDecimalPlaces] = remoteChanges.[LevelDecimalPlaces]
                       ,[TemperatureDecimalPlaces] = remoteChanges.[TemperatureDecimalPlaces]
                       ,[DensityDecimalPlaces] = remoteChanges.[DensityDecimalPlaces]
                       ,[PressureDecimalPlaces] = remoteChanges.[PressureDecimalPlaces]
                       ,[FlowDecimalPlaces] = remoteChanges.[FlowDecimalPlaces]
                       ,[VolumeDecimalPlaces] = remoteChanges.[VolumeDecimalPlaces]
                       ,[MassDecimalPlaces] = remoteChanges.[MassDecimalPlaces]
                       ,[AdditiveVolumeDecimalPlaces] = remoteChanges.[AdditiveVolumeDecimalPlaces]
                       ,[UseComboBoxControls] = remoteChanges.[UseComboBoxControls]
                       ,[MultipleTransportLineItems] = remoteChanges.[MultipleTransportLineItems]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[LookupTransTypeIndex] = remoteChanges.[LookupTransTypeIndex]
                       ,[LookupDefaultStatusIndex] = remoteChanges.[LookupDefaultStatusIndex]
                       ,[AssociatedTransactionAliasGuid] = remoteChanges.[AssociatedTransactionAliasGuid]
                       ,[IncludeInDispatch] = remoteChanges.[IncludeInDispatch]
                       ,[_MasterRecordGuid] = remoteChanges.[_MasterRecordGuid]
                       ,[EnableAutoCompleteControls] = remoteChanges.[EnableAutoCompleteControls]
                       ,[PermitNonReferenceData] = remoteChanges.[PermitNonReferenceData]
                       ,[UseTransactionDetailWithLayout] = remoteChanges.[UseTransactionDetailWithLayout]
                       ,[DefaultMeterToEquipmentID] = remoteChanges.[DefaultMeterToEquipmentID]
                       ,[LimitSourceEquipmentByProduct] = remoteChanges.[LimitSourceEquipmentByProduct]
                       ,[RememberMeterEndForMeterID] = remoteChanges.[RememberMeterEndForMeterID]
                       ,[PopulateCompaniesFromEquipment] = remoteChanges.[PopulateCompaniesFromEquipment]
                       ,[PopulateGrossVolumeFromMeterValues] = remoteChanges.[PopulateGrossVolumeFromMeterValues]
                       ,[UseMeterAndCompressionFactorFromMeter] = remoteChanges.[UseMeterAndCompressionFactorFromMeter]

        WHEN NOT MATCHED THEN
            INSERT ([AliasName],[MeterCloseout],[BulkShipment],[DistributedImpact],[MultipleLineItems],[LimitSelectionsBasedOnHierarchy],[LineItemEditControl],[MultipleWeightReadings],[WeightReadingEditControl],[AssociatedReport],[AssociatedPreloadReport],[DestinationEquipmentTypes1],[DestinationEquipmentTypes2],[DestinationEquipmentTypes3],[SourceEquipmentTypes1],[SourceEquipmentTypes2],[SourceEquipmentTypes3],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ShowCompanyName],[AggregateAssocTrans],[EnableTotalQuantityExceededWarning],[EnableQuantityToleranceExceededWarning],[EnableTotalValueExceededWarning],[EnableValueToleranceExceededWarning],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[UseComboBoxControls],[MultipleTransportLineItems],[TransactionAliasGuid],[SiteGuid],[LookupTransTypeIndex],[LookupDefaultStatusIndex],[AssociatedTransactionAliasGuid],[IncludeInDispatch],[_MasterRecordGuid],[EnableAutoCompleteControls],[PermitNonReferenceData],[UseTransactionDetailWithLayout],[DefaultMeterToEquipmentID],[LimitSourceEquipmentByProduct],[RememberMeterEndForMeterID],[PopulateCompaniesFromEquipment],[PopulateGrossVolumeFromMeterValues],[UseMeterAndCompressionFactorFromMeter])
                VALUES (@AliasName,@MeterCloseout,@BulkShipment,@DistributedImpact,@MultipleLineItems,@LimitSelectionsBasedOnHierarchy,@LineItemEditControl,@MultipleWeightReadings,@WeightReadingEditControl,@AssociatedReport,@AssociatedPreloadReport,@DestinationEquipmentTypes1,@DestinationEquipmentTypes2,@DestinationEquipmentTypes3,@SourceEquipmentTypes1,@SourceEquipmentTypes2,@SourceEquipmentTypes3,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@ShowCompanyName,@AggregateAssocTrans,@EnableTotalQuantityExceededWarning,@EnableQuantityToleranceExceededWarning,@EnableTotalValueExceededWarning,@EnableValueToleranceExceededWarning,@LevelUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@PressureUnitIndex,@FlowUnitIndex,@VolumeUnitIndex,@MassUnitIndex,@AdditiveVolumeUnitIndex,@AdditiveProfileCycleAmountUnitIndex,@AdditiveProfileRateUnitIndex,@LevelDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@PressureDecimalPlaces,@FlowDecimalPlaces,@VolumeDecimalPlaces,@MassDecimalPlaces,@AdditiveVolumeDecimalPlaces,@UseComboBoxControls,@MultipleTransportLineItems,@TransactionAliasGuid,@SiteGuid,@LookupTransTypeIndex,@LookupDefaultStatusIndex,@AssociatedTransactionAliasGuid,@IncludeInDispatch,@_MasterRecordGuid,@EnableAutoCompleteControls,@PermitNonReferenceData,@UseTransactionDetailWithLayout,@DefaultMeterToEquipmentID,@LimitSourceEquipmentByProduct,@RememberMeterEndForMeterID,@PopulateCompaniesFromEquipment,@PopulateGrossVolumeFromMeterValues,@UseMeterAndCompressionFactorFromMeter)
        ;
    
    SET @sync_row_count = @@rowcount;
    
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
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
