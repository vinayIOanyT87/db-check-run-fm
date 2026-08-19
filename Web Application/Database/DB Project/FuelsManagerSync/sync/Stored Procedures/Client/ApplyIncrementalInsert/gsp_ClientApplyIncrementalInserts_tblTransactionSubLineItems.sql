-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionSubLineItems
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblTransactionSubLineItems]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@SequenceID int,
@Product nvarchar(30),
@ProductCode nvarchar(50),
@ProductType nvarchar(20),
@GrossQuantity float,
@DeliveredGrossQuantity float,
@NetQuantity float,
@DeliveredNetQuantity float,
@Pressure float,
@Vcf float,
@Density float,
@Temperature float,
@Customs nvarchar(20),
@ArmNumber int,
@LineNumber int,
@BatchNumber nvarchar(20),
@LineFill float,
@BottomVolume float,
@NetCapacity float,
@TankStatus nvarchar(30),
@MeterFactor float,
@MeterStart float,
@MeterStop float,
@MeterStopDateTime datetimeoffset(7),
@MeterStartDateTime datetimeoffset(7),
@FreezePoint float,
@DifferentialPressure float,
@DosageRate float,
@DeleteFlag bit,
@PresetAmount float,
@StorageLocationID nvarchar(50),
@MeterID nvarchar(50),
@COAID nvarchar(40),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@TransactionInventoryDate date,
@Tax1 float,
@Tax2 float,
@Tax3 float,
@Tax4 float,
@Tax5 float,
@TransVersion bigint,
@ImproperAdditization bit,
@BrokenBlend bit,
@Flag01 bit,
@Flag02 bit,
@Flag03 bit,
@Flag04 bit,
@Flag05 bit,
@Flag06 bit,
@Number01 float,
@Number02 float,
@Number03 float,
@Number04 float,
@Number05 float,
@Number06 float,
@Date01 datetimeoffset(7),
@Date02 datetimeoffset(7),
@Date03 datetimeoffset(7),
@Date04 datetimeoffset(7),
@MassQuantity float,
@NetManualValueFlag bit,
@MassManualValueFlag bit,
@GrossManualValueFlag bit,
@VcfManualValueFlag bit,
@DeliveredGrossManualValueFlag bit,
@DeliveredNetManualValueFlag bit,
@TransactionSubLineItemGuid uniqueidentifier,
@LookupTransactionStatusIndex int,
@LookupQualityIndex int,
@TransactionLineItemGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@TransactionGuid uniqueidentifier,
@StorageLocationTankGuid uniqueidentifier,
@MeterGuid uniqueidentifier,
@PackageManualValueFlag bit,
@CleanLineItem bit,
@CleanLineDeductItem bit,
@CleanLineDeductQuantity float,
@CleanLinePackQuantity float,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    ;   MERGE [dbo].[tblTransactionSubLineItems] AS existingData
        USING (SELECT @SequenceID 'SequenceID',@Product 'Product',@ProductCode 'ProductCode',@ProductType 'ProductType',@GrossQuantity 'GrossQuantity',@DeliveredGrossQuantity 'DeliveredGrossQuantity',@NetQuantity 'NetQuantity',@DeliveredNetQuantity 'DeliveredNetQuantity',@Pressure 'Pressure',@Vcf 'Vcf',@Density 'Density',@Temperature 'Temperature',@Customs 'Customs',@ArmNumber 'ArmNumber',@LineNumber 'LineNumber',@BatchNumber 'BatchNumber',@LineFill 'LineFill',@BottomVolume 'BottomVolume',@NetCapacity 'NetCapacity',@TankStatus 'TankStatus',@MeterFactor 'MeterFactor',@MeterStart 'MeterStart',@MeterStop 'MeterStop',@MeterStopDateTime 'MeterStopDateTime',@MeterStartDateTime 'MeterStartDateTime',@FreezePoint 'FreezePoint',@DifferentialPressure 'DifferentialPressure',@DosageRate 'DosageRate',@DeleteFlag 'DeleteFlag',@PresetAmount 'PresetAmount',@StorageLocationID 'StorageLocationID',@MeterID 'MeterID',@COAID 'COAID',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@TransactionInventoryDate 'TransactionInventoryDate',@Tax1 'Tax1',@Tax2 'Tax2',@Tax3 'Tax3',@Tax4 'Tax4',@Tax5 'Tax5',@TransVersion 'TransVersion',@ImproperAdditization 'ImproperAdditization',@BrokenBlend 'BrokenBlend',@Flag01 'Flag01',@Flag02 'Flag02',@Flag03 'Flag03',@Flag04 'Flag04',@Flag05 'Flag05',@Flag06 'Flag06',@Number01 'Number01',@Number02 'Number02',@Number03 'Number03',@Number04 'Number04',@Number05 'Number05',@Number06 'Number06',@Date01 'Date01',@Date02 'Date02',@Date03 'Date03',@Date04 'Date04',@MassQuantity 'MassQuantity',@NetManualValueFlag 'NetManualValueFlag',@MassManualValueFlag 'MassManualValueFlag',@GrossManualValueFlag 'GrossManualValueFlag',@VcfManualValueFlag 'VcfManualValueFlag',@DeliveredGrossManualValueFlag 'DeliveredGrossManualValueFlag',@DeliveredNetManualValueFlag 'DeliveredNetManualValueFlag',@TransactionSubLineItemGuid 'TransactionSubLineItemGuid',@LookupTransactionStatusIndex 'LookupTransactionStatusIndex',@LookupQualityIndex 'LookupQualityIndex',@TransactionLineItemGuid 'TransactionLineItemGuid',@ProductGuid 'ProductGuid',@TransactionGuid 'TransactionGuid',@StorageLocationTankGuid 'StorageLocationTankGuid',@MeterGuid 'MeterGuid',@PackageManualValueFlag 'PackageManualValueFlag',@CleanLineItem 'CleanLineItem',@CleanLineDeductItem 'CleanLineDeductItem',@CleanLineDeductQuantity 'CleanLineDeductQuantity',@CleanLinePackQuantity 'CleanLinePackQuantity'
                ) AS remoteChanges ([SequenceID],[Product],[ProductCode],[ProductType],[GrossQuantity],[DeliveredGrossQuantity],[NetQuantity],[DeliveredNetQuantity],[Pressure],[Vcf],[Density],[Temperature],[Customs],[ArmNumber],[LineNumber],[BatchNumber],[LineFill],[BottomVolume],[NetCapacity],[TankStatus],[MeterFactor],[MeterStart],[MeterStop],[MeterStopDateTime],[MeterStartDateTime],[FreezePoint],[DifferentialPressure],[DosageRate],[DeleteFlag],[PresetAmount],[StorageLocationID],[MeterID],[COAID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionInventoryDate],[Tax1],[Tax2],[Tax3],[Tax4],[Tax5],[TransVersion],[ImproperAdditization],[BrokenBlend],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[Date01],[Date02],[Date03],[Date04],[MassQuantity],[NetManualValueFlag],[MassManualValueFlag],[GrossManualValueFlag],[VcfManualValueFlag],[DeliveredGrossManualValueFlag],[DeliveredNetManualValueFlag],[TransactionSubLineItemGuid],[LookupTransactionStatusIndex],[LookupQualityIndex],[TransactionLineItemGuid],[ProductGuid],[TransactionGuid],[StorageLocationTankGuid],[MeterGuid],[PackageManualValueFlag],[CleanLineItem],[CleanLineDeductItem],[CleanLineDeductQuantity],[CleanLinePackQuantity])
        ON (existingData.[TransactionSubLineItemGuid] = remoteChanges.[TransactionSubLineItemGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [SequenceID] = remoteChanges.[SequenceID]
                       ,[Product] = remoteChanges.[Product]
                       ,[ProductCode] = remoteChanges.[ProductCode]
                       ,[ProductType] = remoteChanges.[ProductType]
                       ,[GrossQuantity] = remoteChanges.[GrossQuantity]
                       ,[DeliveredGrossQuantity] = remoteChanges.[DeliveredGrossQuantity]
                       ,[NetQuantity] = remoteChanges.[NetQuantity]
                       ,[DeliveredNetQuantity] = remoteChanges.[DeliveredNetQuantity]
                       ,[Pressure] = remoteChanges.[Pressure]
                       ,[Vcf] = remoteChanges.[Vcf]
                       ,[Density] = remoteChanges.[Density]
                       ,[Temperature] = remoteChanges.[Temperature]
                       ,[Customs] = remoteChanges.[Customs]
                       ,[ArmNumber] = remoteChanges.[ArmNumber]
                       ,[LineNumber] = remoteChanges.[LineNumber]
                       ,[BatchNumber] = remoteChanges.[BatchNumber]
                       ,[LineFill] = remoteChanges.[LineFill]
                       ,[BottomVolume] = remoteChanges.[BottomVolume]
                       ,[NetCapacity] = remoteChanges.[NetCapacity]
                       ,[TankStatus] = remoteChanges.[TankStatus]
                       ,[MeterFactor] = remoteChanges.[MeterFactor]
                       ,[MeterStart] = remoteChanges.[MeterStart]
                       ,[MeterStop] = remoteChanges.[MeterStop]
                       ,[MeterStopDateTime] = remoteChanges.[MeterStopDateTime]
                       ,[MeterStartDateTime] = remoteChanges.[MeterStartDateTime]
                       ,[FreezePoint] = remoteChanges.[FreezePoint]
                       ,[DifferentialPressure] = remoteChanges.[DifferentialPressure]
                       ,[DosageRate] = remoteChanges.[DosageRate]
                       ,[DeleteFlag] = remoteChanges.[DeleteFlag]
                       ,[PresetAmount] = remoteChanges.[PresetAmount]
                       ,[StorageLocationID] = remoteChanges.[StorageLocationID]
                       ,[MeterID] = remoteChanges.[MeterID]
                       ,[COAID] = remoteChanges.[COAID]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[TransactionInventoryDate] = remoteChanges.[TransactionInventoryDate]
                       ,[Tax1] = remoteChanges.[Tax1]
                       ,[Tax2] = remoteChanges.[Tax2]
                       ,[Tax3] = remoteChanges.[Tax3]
                       ,[Tax4] = remoteChanges.[Tax4]
                       ,[Tax5] = remoteChanges.[Tax5]
                       ,[TransVersion] = remoteChanges.[TransVersion]
                       ,[ImproperAdditization] = remoteChanges.[ImproperAdditization]
                       ,[BrokenBlend] = remoteChanges.[BrokenBlend]
                       ,[Flag01] = remoteChanges.[Flag01]
                       ,[Flag02] = remoteChanges.[Flag02]
                       ,[Flag03] = remoteChanges.[Flag03]
                       ,[Flag04] = remoteChanges.[Flag04]
                       ,[Flag05] = remoteChanges.[Flag05]
                       ,[Flag06] = remoteChanges.[Flag06]
                       ,[Number01] = remoteChanges.[Number01]
                       ,[Number02] = remoteChanges.[Number02]
                       ,[Number03] = remoteChanges.[Number03]
                       ,[Number04] = remoteChanges.[Number04]
                       ,[Number05] = remoteChanges.[Number05]
                       ,[Number06] = remoteChanges.[Number06]
                       ,[Date01] = remoteChanges.[Date01]
                       ,[Date02] = remoteChanges.[Date02]
                       ,[Date03] = remoteChanges.[Date03]
                       ,[Date04] = remoteChanges.[Date04]
                       ,[MassQuantity] = remoteChanges.[MassQuantity]
                       ,[NetManualValueFlag] = remoteChanges.[NetManualValueFlag]
                       ,[MassManualValueFlag] = remoteChanges.[MassManualValueFlag]
                       ,[GrossManualValueFlag] = remoteChanges.[GrossManualValueFlag]
                       ,[VcfManualValueFlag] = remoteChanges.[VcfManualValueFlag]
                       ,[DeliveredGrossManualValueFlag] = remoteChanges.[DeliveredGrossManualValueFlag]
                       ,[DeliveredNetManualValueFlag] = remoteChanges.[DeliveredNetManualValueFlag]
                       ,[LookupTransactionStatusIndex] = remoteChanges.[LookupTransactionStatusIndex]
                       ,[LookupQualityIndex] = remoteChanges.[LookupQualityIndex]
                       ,[TransactionLineItemGuid] = remoteChanges.[TransactionLineItemGuid]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]
                       ,[TransactionGuid] = remoteChanges.[TransactionGuid]
                       ,[StorageLocationTankGuid] = remoteChanges.[StorageLocationTankGuid]
                       ,[MeterGuid] = remoteChanges.[MeterGuid]
                       ,[PackageManualValueFlag] = remoteChanges.[PackageManualValueFlag]
                       ,[CleanLineItem] = remoteChanges.[CleanLineItem]
                       ,[CleanLineDeductItem] = remoteChanges.[CleanLineDeductItem]
                       ,[CleanLineDeductQuantity] = remoteChanges.[CleanLineDeductQuantity]
                       ,[CleanLinePackQuantity] = remoteChanges.[CleanLinePackQuantity]

        WHEN NOT MATCHED THEN
            INSERT ([SequenceID],[Product],[ProductCode],[ProductType],[GrossQuantity],[DeliveredGrossQuantity],[NetQuantity],[DeliveredNetQuantity],[Pressure],[Vcf],[Density],[Temperature],[Customs],[ArmNumber],[LineNumber],[BatchNumber],[LineFill],[BottomVolume],[NetCapacity],[TankStatus],[MeterFactor],[MeterStart],[MeterStop],[MeterStopDateTime],[MeterStartDateTime],[FreezePoint],[DifferentialPressure],[DosageRate],[DeleteFlag],[PresetAmount],[StorageLocationID],[MeterID],[COAID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionInventoryDate],[Tax1],[Tax2],[Tax3],[Tax4],[Tax5],[TransVersion],[ImproperAdditization],[BrokenBlend],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[Date01],[Date02],[Date03],[Date04],[MassQuantity],[NetManualValueFlag],[MassManualValueFlag],[GrossManualValueFlag],[VcfManualValueFlag],[DeliveredGrossManualValueFlag],[DeliveredNetManualValueFlag],[TransactionSubLineItemGuid],[LookupTransactionStatusIndex],[LookupQualityIndex],[TransactionLineItemGuid],[ProductGuid],[TransactionGuid],[StorageLocationTankGuid],[MeterGuid],[PackageManualValueFlag],[CleanLineItem],[CleanLineDeductItem],[CleanLineDeductQuantity],[CleanLinePackQuantity])
                VALUES (@SequenceID,@Product,@ProductCode,@ProductType,@GrossQuantity,@DeliveredGrossQuantity,@NetQuantity,@DeliveredNetQuantity,@Pressure,@Vcf,@Density,@Temperature,@Customs,@ArmNumber,@LineNumber,@BatchNumber,@LineFill,@BottomVolume,@NetCapacity,@TankStatus,@MeterFactor,@MeterStart,@MeterStop,@MeterStopDateTime,@MeterStartDateTime,@FreezePoint,@DifferentialPressure,@DosageRate,@DeleteFlag,@PresetAmount,@StorageLocationID,@MeterID,@COAID,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@TransactionInventoryDate,@Tax1,@Tax2,@Tax3,@Tax4,@Tax5,@TransVersion,@ImproperAdditization,@BrokenBlend,@Flag01,@Flag02,@Flag03,@Flag04,@Flag05,@Flag06,@Number01,@Number02,@Number03,@Number04,@Number05,@Number06,@Date01,@Date02,@Date03,@Date04,@MassQuantity,@NetManualValueFlag,@MassManualValueFlag,@GrossManualValueFlag,@VcfManualValueFlag,@DeliveredGrossManualValueFlag,@DeliveredNetManualValueFlag,@TransactionSubLineItemGuid,@LookupTransactionStatusIndex,@LookupQualityIndex,@TransactionLineItemGuid,@ProductGuid,@TransactionGuid,@StorageLocationTankGuid,@MeterGuid,@PackageManualValueFlag,@CleanLineItem,@CleanLineDeductItem,@CleanLineDeductQuantity,@CleanLinePackQuantity)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionSubLineItemGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionSubLineItemGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionSubLineItemGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactionSubLineItems] WHERE TransactionSubLineItemGuid = @TransactionSubLineItemGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
