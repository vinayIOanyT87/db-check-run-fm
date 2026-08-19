-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblQualityTags
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalUpdates_tblQualityTags]
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
@sync_batch_size_tblQualityTags int,
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
        SELECT [dbo].[tblQualityTags].[Name],[dbo].[tblQualityTags].[Severity],[dbo].[tblQualityTags].[Active],[dbo].[tblQualityTags].[CreatedDate],[dbo].[tblQualityTags].[CreatedBy],[dbo].[tblQualityTags].[UpdatedDate],[dbo].[tblQualityTags].[UpdatedBy],[dbo].[tblQualityTags].[QualityTagGuid],[dbo].[tblQualityTags].[SiteGuid], [dbo].[tblQualityTags].[_RowVersion]
            FROM [dbo].[tblQualityTags]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblQualityTags IS NULL OR 
        (@sync_batch_size_tblQualityTags IS NOT NULL AND @sync_batch_size_tblQualityTags = 0))
    BEGIN
        SET @sync_batch_size_tblQualityTags = 2147483647;
    END

            SELECT TOP(@sync_batch_size_tblQualityTags) WITH TIES [Name],[Severity],[Active],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[QualityTagGuid],[SiteGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblQualityTags) WITH TIES [dbo].[tblQualityTags].[Name],[dbo].[tblQualityTags].[Severity],[dbo].[tblQualityTags].[Active],[dbo].[tblQualityTags].[CreatedDate],[dbo].[tblQualityTags].[CreatedBy],[dbo].[tblQualityTags].[UpdatedDate],[dbo].[tblQualityTags].[UpdatedBy],[dbo].[tblQualityTags].[QualityTagGuid],[dbo].[tblQualityTags].[SiteGuid],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblQualityTags]
                        INNER JOIN (SELECT [QualityTagToSiteGuid],[QualityTagGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedQualityTagListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblQualityTags].[QualityTagGuid] = data.[QualityTagGuid]
                        INNER JOIN [track].[tblQualityTags] CT
                            ON CT.PK_QualityTagGuid = [dbo].[tblQualityTags].[QualityTagGuid] 
                        INNER JOIN [track].[tblEntityQualityTagToSite] MAPCT
                            ON MAPCT.PK_QualityTagToSiteGuid = data.[QualityTagToSiteGuid] 
                    WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                            AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                            AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1
            ORDER BY _RowVersion ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
