-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityProductToSite
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblEntityProductToSite]
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
@sync_batch_size_tblEntityProductToSite int,
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
        SELECT [map].[tblEntityProductToSite].[ProductToSiteGuid],[map].[tblEntityProductToSite].[ProductGuid],[map].[tblEntityProductToSite].[SiteGuid],[map].[tblEntityProductToSite].[CreatedDate],[map].[tblEntityProductToSite].[CreatedBy],[map].[tblEntityProductToSite].[UpdatedDate],[map].[tblEntityProductToSite].[UpdatedBy],[map].[tblEntityProductToSite].[AssignedFromSiteGuid], [map].[tblEntityProductToSite].[_RowVersion]
            FROM [map].[tblEntityProductToSite]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblEntityProductToSite IS NULL OR 
        (@sync_batch_size_tblEntityProductToSite IS NOT NULL AND @sync_batch_size_tblEntityProductToSite = 0))
    BEGIN
        SET @sync_batch_size_tblEntityProductToSite = 2147483647;
    END

        -- This is a list of entity assignments that meet the "change tracking" filter criteria and should be synchronized.
        -- We need to use this list to identify the other "entity assignments" that do not belong to the site but that are
        -- part of the entity assignment tree starting from the owning site.  We need these intermediate entity assignments to sync
        -- in order for the record versioning queries to work correctly and display the assigned entities in the GUI.
        ; WITH EntityAssignmentTree_CTE AS 
        (
            SELECT [ProductToSiteGuid],[TrackedProductToSiteGuid],[IncludeChangeTrackingFlag] FROM (SELECT [ProductToSiteGuid],[TrackedProductToSiteGuid],[IncludeChangeTrackingFlag] FROM [dbo].[udf_GetAssignmentTreeForProductListForSite](@sync_context_site_guid)) data

        ), ChangedEntityToSiteRecordList_CTE AS 
        (
            SELECT data.[ProductToSiteGuid],data.[TrackedProductToSiteGuid],data.[IncludeChangeTrackingFlag],CT.UpdatedRowVersion AS 'UpdatedRowVersion' 
                FROM [map].[tblEntityProductToSite]
                    INNER JOIN EntityAssignmentTree_CTE data
                        ON [map].[tblEntityProductToSite].[ProductToSiteGuid] = data.[ProductToSiteGuid]
                    INNER JOIN [track].[tblEntityProductToSite] CT
                        ON CT.PK_ProductToSiteGuid = [map].[tblEntityProductToSite].[ProductToSiteGuid] 
                WHERE data.[IncludeChangeTrackingFlag] = 1
                        AND ((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                        AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ) 
        SELECT TOP(@sync_batch_size_tblEntityProductToSite) WITH TIES [map].[tblEntityProductToSite].[ProductToSiteGuid],[map].[tblEntityProductToSite].[ProductGuid],[map].[tblEntityProductToSite].[SiteGuid],[map].[tblEntityProductToSite].[CreatedDate],[map].[tblEntityProductToSite].[CreatedBy],[map].[tblEntityProductToSite].[UpdatedDate],[map].[tblEntityProductToSite].[UpdatedBy],[map].[tblEntityProductToSite].[AssignedFromSiteGuid], data.UpdatedRowVersion AS '_RowVersion'
            FROM [map].[tblEntityProductToSite]
                INNER JOIN (SELECT data.[ProductToSiteGuid], data1.UpdatedRowVersion
                                FROM EntityAssignmentTree_CTE data
                                    LEFT OUTER JOIN ChangedEntityToSiteRecordList_CTE data1
                                        ON data.[TrackedProductToSiteGuid] = data1.[ProductToSiteGuid]
                                WHERE data1.[TrackedProductToSiteGuid] IS NOT NULL) data
                        ON data.ProductToSiteGuid = [map].[tblEntityProductToSite].[ProductToSiteGuid]
        ORDER BY [_RowVersion] ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
