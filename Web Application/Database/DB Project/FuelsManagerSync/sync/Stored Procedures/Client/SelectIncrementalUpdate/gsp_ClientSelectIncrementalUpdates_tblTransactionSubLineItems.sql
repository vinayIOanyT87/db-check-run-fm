-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionSubLineItems
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblTransactionSubLineItems]
@sync_initialized bit,
@sync_last_received_anchor bigint,
@sync_new_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_server_id_binary binary(16),
@sync_context_site_guid uniqueidentifier,
@sync_context_site_id nvarchar(30),
@sync_context_site_guid_list nvarchar(1024),
@sync_context_site_id_list nvarchar(1024),
@sync_table_name nvarchar(512),
@sync_batch_size_tblTransactionSubLineItems int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    -- During an initial synchronization, we don't want to bring back any updates since we 
    -- should be picking them up with the select incremental inserts 
    --
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblTransactionSubLineItems].[SequenceID],[dbo].[tblTransactionSubLineItems].[Product],[dbo].[tblTransactionSubLineItems].[ProductCode],[dbo].[tblTransactionSubLineItems].[ProductType],[dbo].[tblTransactionSubLineItems].[GrossQuantity],[dbo].[tblTransactionSubLineItems].[DeliveredGrossQuantity],[dbo].[tblTransactionSubLineItems].[NetQuantity],[dbo].[tblTransactionSubLineItems].[DeliveredNetQuantity],[dbo].[tblTransactionSubLineItems].[Pressure],[dbo].[tblTransactionSubLineItems].[Vcf],[dbo].[tblTransactionSubLineItems].[Density],[dbo].[tblTransactionSubLineItems].[Temperature],[dbo].[tblTransactionSubLineItems].[Customs],[dbo].[tblTransactionSubLineItems].[ArmNumber],[dbo].[tblTransactionSubLineItems].[LineNumber],[dbo].[tblTransactionSubLineItems].[BatchNumber],[dbo].[tblTransactionSubLineItems].[LineFill],[dbo].[tblTransactionSubLineItems].[BottomVolume],[dbo].[tblTransactionSubLineItems].[NetCapacity],[dbo].[tblTransactionSubLineItems].[TankStatus],[dbo].[tblTransactionSubLineItems].[MeterFactor],[dbo].[tblTransactionSubLineItems].[MeterStart],[dbo].[tblTransactionSubLineItems].[MeterStop],[dbo].[tblTransactionSubLineItems].[MeterStopDateTime],[dbo].[tblTransactionSubLineItems].[MeterStartDateTime],[dbo].[tblTransactionSubLineItems].[FreezePoint],[dbo].[tblTransactionSubLineItems].[DifferentialPressure],[dbo].[tblTransactionSubLineItems].[DosageRate],[dbo].[tblTransactionSubLineItems].[DeleteFlag],[dbo].[tblTransactionSubLineItems].[PresetAmount],[dbo].[tblTransactionSubLineItems].[StorageLocationID],[dbo].[tblTransactionSubLineItems].[MeterID],[dbo].[tblTransactionSubLineItems].[COAID],[dbo].[tblTransactionSubLineItems].[CreatedBy],[dbo].[tblTransactionSubLineItems].[CreatedDate],[dbo].[tblTransactionSubLineItems].[UpdatedBy],[dbo].[tblTransactionSubLineItems].[UpdatedDate],CONVERT(CHAR(10), [dbo].[tblTransactionSubLineItems].[TransactionInventoryDate], 111) AS [TransactionInventoryDate],[dbo].[tblTransactionSubLineItems].[Tax1],[dbo].[tblTransactionSubLineItems].[Tax2],[dbo].[tblTransactionSubLineItems].[Tax3],[dbo].[tblTransactionSubLineItems].[Tax4],[dbo].[tblTransactionSubLineItems].[Tax5],[dbo].[tblTransactionSubLineItems].[TransVersion],[dbo].[tblTransactionSubLineItems].[ImproperAdditization],[dbo].[tblTransactionSubLineItems].[BrokenBlend],[dbo].[tblTransactionSubLineItems].[Flag01],[dbo].[tblTransactionSubLineItems].[Flag02],[dbo].[tblTransactionSubLineItems].[Flag03],[dbo].[tblTransactionSubLineItems].[Flag04],[dbo].[tblTransactionSubLineItems].[Flag05],[dbo].[tblTransactionSubLineItems].[Flag06],[dbo].[tblTransactionSubLineItems].[Number01],[dbo].[tblTransactionSubLineItems].[Number02],[dbo].[tblTransactionSubLineItems].[Number03],[dbo].[tblTransactionSubLineItems].[Number04],[dbo].[tblTransactionSubLineItems].[Number05],[dbo].[tblTransactionSubLineItems].[Number06],[dbo].[tblTransactionSubLineItems].[Date01],[dbo].[tblTransactionSubLineItems].[Date02],[dbo].[tblTransactionSubLineItems].[Date03],[dbo].[tblTransactionSubLineItems].[Date04],[dbo].[tblTransactionSubLineItems].[MassQuantity],[dbo].[tblTransactionSubLineItems].[NetManualValueFlag],[dbo].[tblTransactionSubLineItems].[MassManualValueFlag],[dbo].[tblTransactionSubLineItems].[GrossManualValueFlag],[dbo].[tblTransactionSubLineItems].[VcfManualValueFlag],[dbo].[tblTransactionSubLineItems].[DeliveredGrossManualValueFlag],[dbo].[tblTransactionSubLineItems].[DeliveredNetManualValueFlag],[dbo].[tblTransactionSubLineItems].[TransactionSubLineItemGuid],[dbo].[tblTransactionSubLineItems].[LookupTransactionStatusIndex],[dbo].[tblTransactionSubLineItems].[LookupQualityIndex],[dbo].[tblTransactionSubLineItems].[TransactionLineItemGuid],[dbo].[tblTransactionSubLineItems].[ProductGuid],[dbo].[tblTransactionSubLineItems].[TransactionGuid],[dbo].[tblTransactionSubLineItems].[StorageLocationTankGuid],[dbo].[tblTransactionSubLineItems].[MeterGuid],[dbo].[tblTransactionSubLineItems].[PackageManualValueFlag],[dbo].[tblTransactionSubLineItems].[CleanLineItem],[dbo].[tblTransactionSubLineItems].[CleanLineDeductItem],[dbo].[tblTransactionSubLineItems].[CleanLineDeductQuantity],[dbo].[tblTransactionSubLineItems].[CleanLinePackQuantity], [dbo].[tblTransactionSubLineItems].[_RowVersion]
            FROM [dbo].[tblTransactionSubLineItems]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblTransactionSubLineItems IS NULL OR 
        (@sync_batch_size_tblTransactionSubLineItems IS NOT NULL AND @sync_batch_size_tblTransactionSubLineItems = 0))
    BEGIN
        SET @sync_batch_size_tblTransactionSubLineItems = 2147483647;
    END

        -- Tables that are associated with tblTransactionSubLineItems are filtered through a temp #SyncTable based on the selected tblTransactionSubLineItems records
        -- and therefore are not limited by a TOP(n) clause
        -- 
        SELECT [dbo].[tblTransactionSubLineItems].[SequenceID],[dbo].[tblTransactionSubLineItems].[Product],[dbo].[tblTransactionSubLineItems].[ProductCode],[dbo].[tblTransactionSubLineItems].[ProductType],[dbo].[tblTransactionSubLineItems].[GrossQuantity],[dbo].[tblTransactionSubLineItems].[DeliveredGrossQuantity],[dbo].[tblTransactionSubLineItems].[NetQuantity],[dbo].[tblTransactionSubLineItems].[DeliveredNetQuantity],[dbo].[tblTransactionSubLineItems].[Pressure],[dbo].[tblTransactionSubLineItems].[Vcf],[dbo].[tblTransactionSubLineItems].[Density],[dbo].[tblTransactionSubLineItems].[Temperature],[dbo].[tblTransactionSubLineItems].[Customs],[dbo].[tblTransactionSubLineItems].[ArmNumber],[dbo].[tblTransactionSubLineItems].[LineNumber],[dbo].[tblTransactionSubLineItems].[BatchNumber],[dbo].[tblTransactionSubLineItems].[LineFill],[dbo].[tblTransactionSubLineItems].[BottomVolume],[dbo].[tblTransactionSubLineItems].[NetCapacity],[dbo].[tblTransactionSubLineItems].[TankStatus],[dbo].[tblTransactionSubLineItems].[MeterFactor],[dbo].[tblTransactionSubLineItems].[MeterStart],[dbo].[tblTransactionSubLineItems].[MeterStop],[dbo].[tblTransactionSubLineItems].[MeterStopDateTime],[dbo].[tblTransactionSubLineItems].[MeterStartDateTime],[dbo].[tblTransactionSubLineItems].[FreezePoint],[dbo].[tblTransactionSubLineItems].[DifferentialPressure],[dbo].[tblTransactionSubLineItems].[DosageRate],[dbo].[tblTransactionSubLineItems].[DeleteFlag],[dbo].[tblTransactionSubLineItems].[PresetAmount],[dbo].[tblTransactionSubLineItems].[StorageLocationID],[dbo].[tblTransactionSubLineItems].[MeterID],[dbo].[tblTransactionSubLineItems].[COAID],[dbo].[tblTransactionSubLineItems].[CreatedBy],[dbo].[tblTransactionSubLineItems].[CreatedDate],[dbo].[tblTransactionSubLineItems].[UpdatedBy],[dbo].[tblTransactionSubLineItems].[UpdatedDate],CONVERT(CHAR(10), [dbo].[tblTransactionSubLineItems].[TransactionInventoryDate], 111) AS [TransactionInventoryDate],[dbo].[tblTransactionSubLineItems].[Tax1],[dbo].[tblTransactionSubLineItems].[Tax2],[dbo].[tblTransactionSubLineItems].[Tax3],[dbo].[tblTransactionSubLineItems].[Tax4],[dbo].[tblTransactionSubLineItems].[Tax5],[dbo].[tblTransactionSubLineItems].[TransVersion],[dbo].[tblTransactionSubLineItems].[ImproperAdditization],[dbo].[tblTransactionSubLineItems].[BrokenBlend],[dbo].[tblTransactionSubLineItems].[Flag01],[dbo].[tblTransactionSubLineItems].[Flag02],[dbo].[tblTransactionSubLineItems].[Flag03],[dbo].[tblTransactionSubLineItems].[Flag04],[dbo].[tblTransactionSubLineItems].[Flag05],[dbo].[tblTransactionSubLineItems].[Flag06],[dbo].[tblTransactionSubLineItems].[Number01],[dbo].[tblTransactionSubLineItems].[Number02],[dbo].[tblTransactionSubLineItems].[Number03],[dbo].[tblTransactionSubLineItems].[Number04],[dbo].[tblTransactionSubLineItems].[Number05],[dbo].[tblTransactionSubLineItems].[Number06],[dbo].[tblTransactionSubLineItems].[Date01],[dbo].[tblTransactionSubLineItems].[Date02],[dbo].[tblTransactionSubLineItems].[Date03],[dbo].[tblTransactionSubLineItems].[Date04],[dbo].[tblTransactionSubLineItems].[MassQuantity],[dbo].[tblTransactionSubLineItems].[NetManualValueFlag],[dbo].[tblTransactionSubLineItems].[MassManualValueFlag],[dbo].[tblTransactionSubLineItems].[GrossManualValueFlag],[dbo].[tblTransactionSubLineItems].[VcfManualValueFlag],[dbo].[tblTransactionSubLineItems].[DeliveredGrossManualValueFlag],[dbo].[tblTransactionSubLineItems].[DeliveredNetManualValueFlag],[dbo].[tblTransactionSubLineItems].[TransactionSubLineItemGuid],[dbo].[tblTransactionSubLineItems].[LookupTransactionStatusIndex],[dbo].[tblTransactionSubLineItems].[LookupQualityIndex],[dbo].[tblTransactionSubLineItems].[TransactionLineItemGuid],[dbo].[tblTransactionSubLineItems].[ProductGuid],[dbo].[tblTransactionSubLineItems].[TransactionGuid],[dbo].[tblTransactionSubLineItems].[StorageLocationTankGuid],[dbo].[tblTransactionSubLineItems].[MeterGuid],[dbo].[tblTransactionSubLineItems].[PackageManualValueFlag],[dbo].[tblTransactionSubLineItems].[CleanLineItem],[dbo].[tblTransactionSubLineItems].[CleanLineDeductItem],[dbo].[tblTransactionSubLineItems].[CleanLineDeductQuantity],[dbo].[tblTransactionSubLineItems].[CleanLinePackQuantity],CT.UpdatedRowVersion AS '_RowVersion'
            FROM [dbo].[tblTransactionSubLineItems]
                INNER JOIN [dbo].[tblTransactionLineItems] ON [dbo].[tblTransactionSubLineItems].[TransactionLineItemGuid] = [dbo].[tblTransactionLineItems].[TransactionLineItemGuid] INNER JOIN #SyncTable ON #SyncTable.PK = [dbo].[tblTransactionLineItems].[TransactionGuid] 
                INNER JOIN [track].[tblTransactionSubLineItems] CT
                    ON CT.PK_TransactionSubLineItemGuid = [dbo].[tblTransactionSubLineItems].[TransactionSubLineItemGuid] 
            WHERE (#SyncTable.ChangeType = 'U')
                AND (CT.DeletedRowVersion IS NULL)
                AND ((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
            ORDER BY [_RowVersion] ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
