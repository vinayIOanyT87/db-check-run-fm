-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblTestSetStatus
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblTestSetStatus]
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
@sync_batch_size_tblTestSetStatus int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblTestSetStatus int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblTestSetStatus IS NOT NULL AND @sync_first_time_sync_option_tblTestSetStatus = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [lookup].[tblTestSetStatus].[TestSetStatusIndex],[lookup].[tblTestSetStatus].[TestSetStatusCode],[lookup].[tblTestSetStatus].[TestSetStatusName],[lookup].[tblTestSetStatus].[TestSetStatusGuid],[lookup].[tblTestSetStatus].[CreatedDate],[lookup].[tblTestSetStatus].[CreatedBy],[lookup].[tblTestSetStatus].[UpdatedDate],[lookup].[tblTestSetStatus].[UpdatedBy], [lookup].[tblTestSetStatus].[_RowVersion]
            FROM [lookup].[tblTestSetStatus]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblTestSetStatus IS NULL OR 
        (@sync_batch_size_tblTestSetStatus IS NOT NULL AND @sync_batch_size_tblTestSetStatus = 0))
    BEGIN
        SET @sync_batch_size_tblTestSetStatus = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
    SELECT TOP(@sync_batch_size_tblTestSetStatus) WITH TIES [lookup].[tblTestSetStatus].[TestSetStatusIndex],[lookup].[tblTestSetStatus].[TestSetStatusCode],[lookup].[tblTestSetStatus].[TestSetStatusName],[lookup].[tblTestSetStatus].[TestSetStatusGuid],[lookup].[tblTestSetStatus].[CreatedDate],[lookup].[tblTestSetStatus].[CreatedBy],[lookup].[tblTestSetStatus].[UpdatedDate],[lookup].[tblTestSetStatus].[UpdatedBy],CT.InsertedRowVersion AS '_RowVersion'
        FROM [lookup].[tblTestSetStatus]
            INNER JOIN [track].[tblTestSetStatus] CT
                ON CT.PK_TestSetStatusIndex = [lookup].[tblTestSetStatus].[TestSetStatusIndex]
            WHERE ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY _RowVersion ASC


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
