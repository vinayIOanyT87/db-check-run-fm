-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityUserGroupToSite
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblEntityUserGroupToSite]
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
@sync_batch_size_tblEntityUserGroupToSite int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblEntityUserGroupToSite int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblEntityUserGroupToSite IS NOT NULL AND @sync_first_time_sync_option_tblEntityUserGroupToSite = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [map].[tblEntityUserGroupToSite].[UserGroupToSiteGuid],[map].[tblEntityUserGroupToSite].[GroupGuid],[map].[tblEntityUserGroupToSite].[SiteGuid],[map].[tblEntityUserGroupToSite].[CreatedDate],[map].[tblEntityUserGroupToSite].[CreatedBy],[map].[tblEntityUserGroupToSite].[UpdatedDate],[map].[tblEntityUserGroupToSite].[UpdatedBy],[map].[tblEntityUserGroupToSite].[AssignedFromSiteGuid], [map].[tblEntityUserGroupToSite].[_RowVersion]
            FROM [map].[tblEntityUserGroupToSite]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblEntityUserGroupToSite IS NULL OR 
        (@sync_batch_size_tblEntityUserGroupToSite IS NOT NULL AND @sync_batch_size_tblEntityUserGroupToSite = 0))
    BEGIN
        SET @sync_batch_size_tblEntityUserGroupToSite = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
    SELECT TOP(@sync_batch_size_tblEntityUserGroupToSite) WITH TIES [map].[tblEntityUserGroupToSite].[UserGroupToSiteGuid],[map].[tblEntityUserGroupToSite].[GroupGuid],[map].[tblEntityUserGroupToSite].[SiteGuid],[map].[tblEntityUserGroupToSite].[CreatedDate],[map].[tblEntityUserGroupToSite].[CreatedBy],[map].[tblEntityUserGroupToSite].[UpdatedDate],[map].[tblEntityUserGroupToSite].[UpdatedBy],[map].[tblEntityUserGroupToSite].[AssignedFromSiteGuid],CT.InsertedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityUserGroupToSite]
             INNER JOIN [dbo].[tblGroups] ON [map].[tblEntityUserGroupToSite].[GroupGuid] = [dbo].[tblGroups].[GroupGuid] 
            INNER JOIN [track].[tblEntityUserGroupToSite] CT
                ON CT.PK_UserGroupToSiteGuid = [map].[tblEntityUserGroupToSite].[UserGroupToSiteGuid] 
        WHERE ( [map].[tblEntityUserGroupToSite].[SiteGuid] = @sync_context_site_guid )
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
