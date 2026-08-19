-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblDrawings
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblDrawings]
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
@sync_batch_size_tblDrawings int,
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
        SELECT [dbo].[tblDrawings].[DrawingGuid],[dbo].[tblDrawings].[ID],[dbo].[tblDrawings].[Description],[dbo].[tblDrawings].[Image],[dbo].[tblDrawings].[SiteGuid],[dbo].[tblDrawings].[PanelType],[dbo].[tblDrawings].[PointTemplateGuid],[dbo].[tblDrawings].[Published],[dbo].[tblDrawings].[CreatedDate],[dbo].[tblDrawings].[CreatedBy],[dbo].[tblDrawings].[UpdatedDate],[dbo].[tblDrawings].[UpdatedBy], [dbo].[tblDrawings].[_RowVersion]
            FROM [dbo].[tblDrawings]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblDrawings IS NULL OR 
        (@sync_batch_size_tblDrawings IS NOT NULL AND @sync_batch_size_tblDrawings = 0))
    BEGIN
        SET @sync_batch_size_tblDrawings = 2147483647;
    END

        SELECT TOP(@sync_batch_size_tblDrawings) WITH TIES [DrawingGuid],[ID],[Description],[Image],[SiteGuid],[PanelType],[PointTemplateGuid],[Published],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
        FROM (
            SELECT [DrawingGuid],[ID],[Description],[Image],[SiteGuid],[PanelType],[PointTemplateGuid],[Published],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblDrawings) WITH TIES [dbo].[tblDrawings].[DrawingGuid],[dbo].[tblDrawings].[ID],[dbo].[tblDrawings].[Description],[dbo].[tblDrawings].[Image],[dbo].[tblDrawings].[SiteGuid],[dbo].[tblDrawings].[PanelType],[dbo].[tblDrawings].[PointTemplateGuid],[dbo].[tblDrawings].[Published],[dbo].[tblDrawings].[CreatedDate],[dbo].[tblDrawings].[CreatedBy],[dbo].[tblDrawings].[UpdatedDate],[dbo].[tblDrawings].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblDrawings]
                        INNER JOIN (SELECT [DrawingGuid],[PointTemplateToSiteGuid] FROM [dbo].[udf_GetAssignedPointDetailListPerSite](@sync_context_site_guid)) data
                            ON [dbo].[tblDrawings].[DrawingGuid] = data.[DrawingGuid]
                        INNER JOIN [track].[tblDrawings] CT
                            ON CT.PK_DrawingGuid = [dbo].[tblDrawings].[DrawingGuid] 
                        INNER JOIN [track].[tblEntityPointTemplateToSite] MAPCT
                            ON MAPCT.PK_PointTemplateToSiteGuid = data.[PointTemplateToSiteGuid] 
                WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs1
            UNION
            SELECT [DrawingGuid],[ID],[Description],[Image],[SiteGuid],[PanelType],[PointTemplateGuid],[Published],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblDrawings) WITH TIES [dbo].[tblDrawings].[DrawingGuid],[dbo].[tblDrawings].[ID],[dbo].[tblDrawings].[Description],[dbo].[tblDrawings].[Image],[dbo].[tblDrawings].[SiteGuid],[dbo].[tblDrawings].[PanelType],[dbo].[tblDrawings].[PointTemplateGuid],[dbo].[tblDrawings].[Published],[dbo].[tblDrawings].[CreatedDate],[dbo].[tblDrawings].[CreatedBy],[dbo].[tblDrawings].[UpdatedDate],[dbo].[tblDrawings].[UpdatedBy],CT.UpdatedRowVersion AS '_RowVersion'
                    FROM [dbo].[tblDrawings]
                        INNER JOIN (SELECT [DrawingGuid] FROM [dbo].[udf_GetAssignedDrawingListPerSite](@sync_context_site_guid)) data1
                            ON [dbo].[tblDrawings].[DrawingGuid] = data1.[DrawingGuid]
                        INNER JOIN [track].[tblDrawings] CT
                            ON CT.PK_DrawingGuid = [dbo].[tblDrawings].[DrawingGuid] 
                WHERE ((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY [_RowVersion] ASC
            ) rs2
        ) mainRs
        ORDER BY [_RowVersion] ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
