-- ********************** CREATE SYNCHRONIZATION METHODS FOR tblExternalStationLog **********************
-- 
-- 
-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExternalStationLog
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblExternalStationLog]
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
@sync_batch_size_tblExternalStationLog int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int
AS
BEGIN
    IF (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1)
    BEGIN
        SELECT [dbo].[tblExternalStationLog].[ExternalStationLogGuid],[dbo].[tblExternalStationLog].[SiteGuid],[dbo].[tblExternalStationLog].[ExternalStationGuid],[dbo].[tblExternalStationLog].[LogText],[dbo].[tblExternalStationLog].[LookupExternalStationLogTypeIndex],[dbo].[tblExternalStationLog].[LogDate],[dbo].[tblExternalStationLog].[CreatedBy],[dbo].[tblExternalStationLog].[CreatedDate],[dbo].[tblExternalStationLog].[UpdatedBy],[dbo].[tblExternalStationLog].[UpdatedDate], [dbo].[tblExternalStationLog].[_RowVersion]
            FROM [dbo].[tblExternalStationLog]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblExternalStationLog IS NULL OR 
        (@sync_batch_size_tblExternalStationLog IS NOT NULL AND @sync_batch_size_tblExternalStationLog = 0))
    BEGIN
        SET @sync_batch_size_tblExternalStationLog = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
    SELECT TOP(@sync_batch_size_tblExternalStationLog) [dbo].[tblExternalStationLog].[ExternalStationLogGuid],[dbo].[tblExternalStationLog].[SiteGuid],[dbo].[tblExternalStationLog].[ExternalStationGuid],[dbo].[tblExternalStationLog].[LogText],[dbo].[tblExternalStationLog].[LookupExternalStationLogTypeIndex],[dbo].[tblExternalStationLog].[LogDate],[dbo].[tblExternalStationLog].[CreatedBy],[dbo].[tblExternalStationLog].[CreatedDate],[dbo].[tblExternalStationLog].[UpdatedBy],[dbo].[tblExternalStationLog].[UpdatedDate],CT.InsertedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExternalStationLog]
            INNER JOIN [track].[tblExternalStationLog] CT
                ON CT.PK_ExternalStationLogGuid = [dbo].[tblExternalStationLog].[ExternalStationLogGuid] 
            WHERE ( [dbo].[tblExternalStationLog].[SiteGuid] = @sync_context_site_guid)
                    AND ((CT.InsertedRowVersion > @sync_last_received_anchor)
                AND (CT.InsertedRowVersion <= @sync_new_received_anchor)
                AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
            ORDER BY _RowVersion ASC

    DECLARE @minValidVersion BigInt 
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END