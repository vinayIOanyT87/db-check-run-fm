-- ********************** CREATE SYNCHRONIZATION METHODS FOR tblUserDataListValueIata **********************
-- 
-- 
-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueIata
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblUserDataListValueIata]
@sync_initialized BIT,
@sync_last_received_anchor BIGINT,
@sync_new_received_anchor BIGINT,
@sync_start_daterange DATETIMEOFFSET(7),
@sync_end_daterange DATETIMEOFFSET(7),
@sync_filter_by_daterange BIT,
@sync_client_id_binary BINARY(16),
@sync_client_id UNIQUEIDENTIFIER,
@sync_server_id_binary BINARY(16),
@sync_context_site_guid UNIQUEIDENTIFIER,
@sync_context_site_id NVARCHAR(30),
@sync_context_site_guid_list NVARCHAR(1024),
@sync_context_site_id_list NVARCHAR(1024),
@sync_table_name NVARCHAR(512),
@sync_batch_size_tblUserDataListValueIata INT,
@sync_bypass_insert_update_extraction BIT,
@sync_request_type INT,
@sync_first_time_sync_option_tblUserDataListValueIata INT
AS
BEGIN
    DECLARE @minValidVersion BIGINT 

    DECLARE @sync_last_received_anchor_varbinary VARBINARY(8)
    DECLARE @sync_new_received_anchor_varbinary VARBINARY(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(VARBINARY(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(VARBINARY(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblUserDataListValueIata IS NOT NULL AND @sync_first_time_sync_option_tblUserDataListValueIata = 1))
		OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblUserDataListValueIata].[UserDataListValueIataGuid],[dbo].[tblUserDataListValueIata].[UserDataFieldIataGuid],[dbo].[tblUserDataListValueIata].[Value],[dbo].[tblUserDataListValueIata].[CreatedDate],[dbo].[tblUserDataListValueIata].[CreatedBy],[dbo].[tblUserDataListValueIata].[UpdatedDate],[dbo].[tblUserDataListValueIata].[UpdatedBy], [dbo].[tblUserDataListValueIata].[_RowVersion]
            FROM [dbo].[tblUserDataListValueIata]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblUserDataListValueIata IS NULL OR 
        (@sync_batch_size_tblUserDataListValueIata IS NOT NULL AND @sync_batch_size_tblUserDataListValueIata = 0))
    BEGIN
        SET @sync_batch_size_tblUserDataListValueIata = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
    SELECT TOP(@sync_batch_size_tblUserDataListValueIata) WITH TIES [UserDataListValueIataGuid],[UserDataFieldIataGuid],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
    FROM (
        SELECT [UserDataListValueIataGuid],[UserDataFieldIataGuid],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblUserDataListValueIata) WITH TIES [dbo].[tblUserDataListValueIata].[UserDataListValueIataGuid],[dbo].[tblUserDataListValueIata].[UserDataFieldIataGuid],[dbo].[tblUserDataListValueIata].[Value],[dbo].[tblUserDataListValueIata].[CreatedDate],[dbo].[tblUserDataListValueIata].[CreatedBy],[dbo].[tblUserDataListValueIata].[UpdatedDate],[dbo].[tblUserDataListValueIata].[UpdatedBy],CT.InsertedRowVersion AS '_RowVersion'
                FROM [dbo].[tblUserDataListValueIata]
                    INNER JOIN [dbo].[tblUserDataFieldIata] data
                        ON [dbo].[tblUserDataListValueIata].[UserDataFieldIataGuid] = DATA.[UserDataFieldIataGuid]
                    INNER JOIN [track].[tblUserDataListValueIata] CT
                        ON CT.PK_UserDataListValueIataGuid = [dbo].[tblUserDataListValueIata].[UserDataListValueIataGuid]
                WHERE (data.[SiteGuid] = @sync_context_site_guid)
                        AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion
            ) rs
        UNION
        SELECT [UserDataListValueIataGuid],[UserDataFieldIataGuid],[Value],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblUserDataListValueIata) WITH TIES [dbo].[tblUserDataListValueIata].[UserDataListValueIataGuid],[dbo].[tblUserDataListValueIata].[UserDataFieldIataGuid],[dbo].[tblUserDataListValueIata].[Value],[dbo].[tblUserDataListValueIata].[CreatedDate],[dbo].[tblUserDataListValueIata].[CreatedBy],[dbo].[tblUserDataListValueIata].[UpdatedDate],[dbo].[tblUserDataListValueIata].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                FROM [dbo].[tblUserDataListValueIata]
                    INNER JOIN (SELECT [UserDataToSiteGuid],[UserDataFieldIataGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedUserDataFieldIataListForSite](@sync_context_site_guid)) data
                        ON [dbo].[tblUserDataListValueIata].[UserDataFieldIataGuid] = DATA.[UserDataFieldIataGuid]
                    INNER JOIN [track].[tblUserDataListValueIata] CT
                        ON CT.PK_UserDataListValueIataGuid = [dbo].[tblUserDataListValueIata].[UserDataListValueIataGuid] 
                    INNER JOIN [track].[tblEntityUserDataToSite] MAPCT
                        ON MAPCT.PK_UserDataToSiteGuid = data.[UserDataToSiteGuid]
                    INNER JOIN [track].[tblUserDataFieldIata] MAPCT2
                        ON MAPCT2.PK_UserDataFieldIataGuid = data.[UserDataFieldIataGuid]
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT2.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT2.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT2.InsertedContext IS NULL OR MAPCT2.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1   -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ) mainRs
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
