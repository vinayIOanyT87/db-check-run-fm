-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionLinks
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblTransactionLinks]
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
@sync_batch_size_tblTransactionLinks int,
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
        SELECT [dbo].[tblTransactionLinks].[OriginalTransID],[dbo].[tblTransactionLinks].[LinkedTransID],[dbo].[tblTransactionLinks].[Level],[dbo].[tblTransactionLinks].[CreatedBy],[dbo].[tblTransactionLinks].[CreatedDate],[dbo].[tblTransactionLinks].[UpdatedBy],[dbo].[tblTransactionLinks].[UpdatedDate],[dbo].[tblTransactionLinks].[TransactionLinkGuid],[dbo].[tblTransactionLinks].[SiteGuid],[dbo].[tblTransactionLinks].[LinkedTransactionLineItemGuid],[dbo].[tblTransactionLinks].[TransactionLineItemGuid], [dbo].[tblTransactionLinks].[_RowVersion]
            FROM [dbo].[tblTransactionLinks]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblTransactionLinks IS NULL OR 
        (@sync_batch_size_tblTransactionLinks IS NOT NULL AND @sync_batch_size_tblTransactionLinks = 0))
    BEGIN
        SET @sync_batch_size_tblTransactionLinks = 2147483647;
    END

        -- Tables that are associated with tblTransactionLinks are filtered through a temp #SyncTable based on the selected tblTransactionLinks records
        -- and therefore are not limited by a TOP(n) clause
        -- 
        SELECT [dbo].[tblTransactionLinks].[OriginalTransID],[dbo].[tblTransactionLinks].[LinkedTransID],[dbo].[tblTransactionLinks].[Level],[dbo].[tblTransactionLinks].[CreatedBy],[dbo].[tblTransactionLinks].[CreatedDate],[dbo].[tblTransactionLinks].[UpdatedBy],[dbo].[tblTransactionLinks].[UpdatedDate],[dbo].[tblTransactionLinks].[TransactionLinkGuid],[dbo].[tblTransactionLinks].[SiteGuid],[dbo].[tblTransactionLinks].[LinkedTransactionLineItemGuid],[dbo].[tblTransactionLinks].[TransactionLineItemGuid],CT.UpdatedRowVersion AS '_RowVersion'
            FROM [dbo].[tblTransactionLinks]
                INNER JOIN [dbo].[tblTransactionLineItems] ON [dbo].[tblTransactionLinks].[TransactionLineItemGuid] = [dbo].[tblTransactionLineItems].[TransactionLineItemGuid] INNER JOIN #SyncTable ON #SyncTable.PK = [dbo].[tblTransactionLineItems].[TransactionGuid] 
                INNER JOIN [track].[tblTransactionLinks] CT
                    ON CT.PK_TransactionLinkGuid = [dbo].[tblTransactionLinks].[TransactionLinkGuid] 
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
