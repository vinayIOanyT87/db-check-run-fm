-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyEventErrorClassCode
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblGasboyEventErrorClassCode]
@sync_initialized bit,
@sync_last_received_anchor bigint,
@sync_new_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_server_id_binary binary(16),
@sync_context_site_guid_list nvarchar(1024),
@sync_context_site_id_list nvarchar(1024),
@sync_table_name nvarchar(512),
@sync_batch_size_tblGasboyEventErrorClassCode int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int
AS
BEGIN
    -- During an initial synchronization, we don't want to bring back any updates since we 
    -- should be picking them up with the select incremental inserts 
    --
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeIndex],[lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCode],[lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeName],[lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeGuid],[lookup].[tblGasboyEventErrorClassCode].[CreatedBy],[lookup].[tblGasboyEventErrorClassCode].[CreatedDate],[lookup].[tblGasboyEventErrorClassCode].[UpdatedBy],[lookup].[tblGasboyEventErrorClassCode].[UpdatedDate], [lookup].[tblGasboyEventErrorClassCode].[_RowVersion]
            FROM [lookup].[tblGasboyEventErrorClassCode]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblGasboyEventErrorClassCode IS NULL OR 
        (@sync_batch_size_tblGasboyEventErrorClassCode IS NOT NULL AND @sync_batch_size_tblGasboyEventErrorClassCode = 0))
    BEGIN
        SET @sync_batch_size_tblGasboyEventErrorClassCode = 2147483647;
    END

        SELECT TOP(@sync_batch_size_tblGasboyEventErrorClassCode) [lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeIndex],[lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCode],[lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeName],[lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeGuid],[lookup].[tblGasboyEventErrorClassCode].[CreatedBy],[lookup].[tblGasboyEventErrorClassCode].[CreatedDate],[lookup].[tblGasboyEventErrorClassCode].[UpdatedBy],[lookup].[tblGasboyEventErrorClassCode].[UpdatedDate],CT.UpdatedRowVersion AS '_RowVersion'
            FROM [lookup].[tblGasboyEventErrorClassCode]
                INNER JOIN [track].[tblGasboyEventErrorClassCode] CT
                    ON CT.PK_GasboyEventErrorClassCodeIndex = [lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeIndex]
            WHERE ((CT.UpdatedRowVersion > @sync_last_received_anchor)
                    AND (CT.UpdatedRowVersion <= @sync_new_received_anchor)
                    AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                    AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
            ORDER BY _RowVersion ASC

    DECLARE @minValidVersion BigInt 
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END