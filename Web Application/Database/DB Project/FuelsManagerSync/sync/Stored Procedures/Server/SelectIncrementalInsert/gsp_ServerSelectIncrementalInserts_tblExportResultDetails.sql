-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExportResultDetails
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblExportResultDetails]
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
@sync_batch_size_tblExportResultDetails int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblExportResultDetails int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblExportResultDetails IS NOT NULL AND @sync_first_time_sync_option_tblExportResultDetails = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblExportResultDetails].[RecordID],[dbo].[tblExportResultDetails].[Fail],[dbo].[tblExportResultDetails].[TransVersion],[dbo].[tblExportResultDetails].[CreatedDate],[dbo].[tblExportResultDetails].[CreatedBy],[dbo].[tblExportResultDetails].[UpdatedDate],[dbo].[tblExportResultDetails].[UpdatedBy],[dbo].[tblExportResultDetails].[Error],[dbo].[tblExportResultDetails].[ExportResultDetailGuid],[dbo].[tblExportResultDetails].[ExportResultGuid],[dbo].[tblExportResultDetails].[InterfaceData01],[dbo].[tblExportResultDetails].[InterfaceData02],[dbo].[tblExportResultDetails].[InterfaceData03],[dbo].[tblExportResultDetails].[InterfaceData04],[dbo].[tblExportResultDetails].[InterfaceData05],[dbo].[tblExportResultDetails].[InterfaceData06],[dbo].[tblExportResultDetails].[InterfaceData07],[dbo].[tblExportResultDetails].[InterfaceData08], [dbo].[tblExportResultDetails].[_RowVersion]
            FROM [dbo].[tblExportResultDetails]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblExportResultDetails IS NULL OR 
        (@sync_batch_size_tblExportResultDetails IS NOT NULL AND @sync_batch_size_tblExportResultDetails = 0))
    BEGIN
        SET @sync_batch_size_tblExportResultDetails = 2147483647;
    END

        -- We only do this on tblExportResultDetails so we know what which tblExportResultDetails records are being processed.
        -- Synchronization will only synchronize other dependent records that are associated with the tblExportResultDetails records
        -- that were included.
    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
    -- Tables that are associated with tblExportResultDetails are filtered through a temp #SyncTable based on the selected tblExportResultDetails records
    -- and therefore are not limited by a TOP(n) clause
    -- 
    SELECT [dbo].[tblExportResultDetails].[RecordID],[dbo].[tblExportResultDetails].[Fail],[dbo].[tblExportResultDetails].[TransVersion],[dbo].[tblExportResultDetails].[CreatedDate],[dbo].[tblExportResultDetails].[CreatedBy],[dbo].[tblExportResultDetails].[UpdatedDate],[dbo].[tblExportResultDetails].[UpdatedBy],[dbo].[tblExportResultDetails].[Error],[dbo].[tblExportResultDetails].[ExportResultDetailGuid],[dbo].[tblExportResultDetails].[ExportResultGuid],[dbo].[tblExportResultDetails].[InterfaceData01],[dbo].[tblExportResultDetails].[InterfaceData02],[dbo].[tblExportResultDetails].[InterfaceData03],[dbo].[tblExportResultDetails].[InterfaceData04],[dbo].[tblExportResultDetails].[InterfaceData05],[dbo].[tblExportResultDetails].[InterfaceData06],[dbo].[tblExportResultDetails].[InterfaceData07],[dbo].[tblExportResultDetails].[InterfaceData08],CT.InsertedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExportResultDetails]
             INNER JOIN #SyncTable ON #SyncTable.PK = [dbo].[tblExportResultDetails].[ExportResultGuid]
            INNER JOIN [track].[tblExportResultDetails] CT
                ON CT.PK_ExportResultDetailGuid = [dbo].[tblExportResultDetails].[ExportResultDetailGuid] 
            WHERE (#SyncTable.ChangeType = 'I')
                AND (CT.DeletedRowVersion IS NULL)
                AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
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
