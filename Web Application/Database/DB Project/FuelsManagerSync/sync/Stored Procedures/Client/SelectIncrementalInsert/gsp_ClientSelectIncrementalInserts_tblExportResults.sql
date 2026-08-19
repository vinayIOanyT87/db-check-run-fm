-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExportResults
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblExportResults]
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
@sync_batch_size_tblExportResults int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblExportResults int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    -- The FuelsManager Client selection for inserts is not coded to support a default SELECT ALL in order to push into the Enterprise.  This is by design.
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblExportResults].[InterfaceName],[dbo].[tblExportResults].[TransVersion],[dbo].[tblExportResults].[FailedCount],[dbo].[tblExportResults].[SuccessCount],[dbo].[tblExportResults].[TransDateTime],[dbo].[tblExportResults].[CreatedDate],[dbo].[tblExportResults].[CreatedBy],[dbo].[tblExportResults].[UpdatedDate],[dbo].[tblExportResults].[UpdatedBy],[dbo].[tblExportResults].[BatchID],[dbo].[tblExportResults].[ExportResultGuid],[dbo].[tblExportResults].[SiteGuid],[dbo].[tblExportResults].[LookupExportResultTypeIndex],[dbo].[tblExportResults].[ArchiveFileName], [dbo].[tblExportResults].[_RowVersion]
            FROM [dbo].[tblExportResults]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblExportResults IS NULL OR 
        (@sync_batch_size_tblExportResults IS NOT NULL AND @sync_batch_size_tblExportResults = 0))
    BEGIN
        SET @sync_batch_size_tblExportResults = 2147483647;
    END

        -- We only do this on tblExportResults so we know what which tblExportResults records are being processed.
        -- Synchronization will only synchronize other dependent records that are associated with the tblExportResults records
        -- that were included.
    INSERT INTO #SyncTable 
        SELECT TOP(@sync_batch_size_tblExportResults) WITH TIES [dbo].[tblExportResults].[ExportResultGuid] AS 'PK', 'I' AS 'ChangeType' 
            FROM [dbo].[tblExportResults]
                INNER JOIN [track].[tblExportResults] CT
                    ON CT.PK_ExportResultGuid = [dbo].[tblExportResults].[ExportResultGuid]
            WHERE ( [dbo].[tblExportResults].[SiteGuid] = @sync_context_site_guid )
                    AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
            ORDER BY CT.InsertedRowVersion ASC

    IF (@sync_request_type <> 4) -- This replaced sync_initialized since we can't control it when performing batch synchronization
    BEGIN
        INSERT INTO #SyncTable
            SELECT TOP(@sync_batch_size_tblExportResults) WITH TIES CT.PK_ExportResultGuid AS 'PK', 'D' AS 'ChangeType' 
                FROM [track].[tblExportResults] CT
                WHERE (CT.CurrentSiteGuid = @sync_context_site_guid)
                    AND ((CT.DeletedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.DeletedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.DeletedContext IS NULL OR CT.DeletedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    AND CT.PK_ExportResultGuid NOT IN (SELECT PK FROM #SyncTable)
                ORDER BY CT.DeletedRowVersion ASC

        INSERT INTO #SyncTable
            SELECT TOP(@sync_batch_size_tblExportResults) WITH TIES [dbo].[tblExportResults].[ExportResultGuid] AS 'PK', 'U' AS 'ChangeType' 
                FROM [dbo].[tblExportResults]
                    INNER JOIN [track].[tblExportResults] CT
                        ON CT.PK_ExportResultGuid = [dbo].[tblExportResults].[ExportResultGuid]
                WHERE ( [dbo].[tblExportResults].[SiteGuid] = @sync_context_site_guid )
                        AND ((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        AND [dbo].[tblExportResults].[ExportResultGuid] NOT IN (SELECT PK FROM #SyncTable)
                ORDER BY CT.UpdatedRowVersion ASC
    END


        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblExportResults) WITH TIES [dbo].[tblExportResults].[InterfaceName],[dbo].[tblExportResults].[TransVersion],[dbo].[tblExportResults].[FailedCount],[dbo].[tblExportResults].[SuccessCount],[dbo].[tblExportResults].[TransDateTime],[dbo].[tblExportResults].[CreatedDate],[dbo].[tblExportResults].[CreatedBy],[dbo].[tblExportResults].[UpdatedDate],[dbo].[tblExportResults].[UpdatedBy],[dbo].[tblExportResults].[BatchID],[dbo].[tblExportResults].[ExportResultGuid],[dbo].[tblExportResults].[SiteGuid],[dbo].[tblExportResults].[LookupExportResultTypeIndex],[dbo].[tblExportResults].[ArchiveFileName],CT.InsertedRowVersion AS '_RowVersion'
            FROM [dbo].[tblExportResults]
                 INNER JOIN #SyncTable ON #SyncTable.PK = [dbo].[tblExportResults].[ExportResultGuid]
                INNER JOIN [track].[tblExportResults] CT
                    ON CT.PK_ExportResultGuid = [dbo].[tblExportResults].[ExportResultGuid] 
            WHERE (#SyncTable.ChangeType = 'I')
                AND (CT.DeletedRowVersion IS NULL)
                AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
