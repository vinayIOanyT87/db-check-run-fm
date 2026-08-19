-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionAliases
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblTransactionAliases]
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
@sync_table_name nvarchar(512)
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
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
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
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
