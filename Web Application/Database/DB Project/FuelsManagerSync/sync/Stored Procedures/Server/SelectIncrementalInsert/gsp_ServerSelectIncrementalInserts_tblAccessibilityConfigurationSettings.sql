-- ********************** CREATE SYNCHRONIZATION METHODS FOR tblAccessibilityConfigurationSettings **********************
-- 
-- 
-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAccessibilityConfigurationSettings
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblAccessibilityConfigurationSettings]
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
@sync_batch_size_tblAccessibilityConfigurationSettings int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblAccessibilityConfigurationSettings int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblAccessibilityConfigurationSettings IS NOT NULL AND @sync_first_time_sync_option_tblAccessibilityConfigurationSettings = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblAccessibilityConfigurationSettings].[AccessibilityConfigurationSettingGuid],[dbo].[tblAccessibilityConfigurationSettings].[AccessibilityGuid],[dbo].[tblAccessibilityConfigurationSettings].[UserGuid],[dbo].[tblAccessibilityConfigurationSettings].[SettingValue],[dbo].[tblAccessibilityConfigurationSettings].[CreatedDate],[dbo].[tblAccessibilityConfigurationSettings].[CreatedBy],[dbo].[tblAccessibilityConfigurationSettings].[UpdatedDate],[dbo].[tblAccessibilityConfigurationSettings].[UpdatedBy], [dbo].[tblAccessibilityConfigurationSettings].[_RowVersion]
            FROM [dbo].[tblAccessibilityConfigurationSettings]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblAccessibilityConfigurationSettings IS NULL OR 
        (@sync_batch_size_tblAccessibilityConfigurationSettings IS NOT NULL AND @sync_batch_size_tblAccessibilityConfigurationSettings = 0))
    BEGIN
        SET @sync_batch_size_tblAccessibilityConfigurationSettings = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblAccessibilityConfigurationSettings) WITH TIES [AccessibilityConfigurationSettingGuid],[AccessibilityGuid],[UserGuid],[SettingValue],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblAccessibilityConfigurationSettings) WITH TIES [dbo].[tblAccessibilityConfigurationSettings].[AccessibilityConfigurationSettingGuid],[dbo].[tblAccessibilityConfigurationSettings].[AccessibilityGuid],[dbo].[tblAccessibilityConfigurationSettings].[UserGuid],[dbo].[tblAccessibilityConfigurationSettings].[SettingValue],[dbo].[tblAccessibilityConfigurationSettings].[CreatedDate],[dbo].[tblAccessibilityConfigurationSettings].[CreatedBy],[dbo].[tblAccessibilityConfigurationSettings].[UpdatedDate],[dbo].[tblAccessibilityConfigurationSettings].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                FROM [dbo].[tblAccessibilityConfigurationSettings]
                    INNER JOIN (SELECT [UserToSiteGuid],[UserGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedUserListForSite](@sync_context_site_guid)) data
                        ON [dbo].[tblAccessibilityConfigurationSettings].[UserGuid] = data.[UserGuid]
                    INNER JOIN [track].[tblAccessibilityConfigurationSettings] CT
                        ON CT.PK_AccessibilityConfigurationSettingGuid = [dbo].[tblAccessibilityConfigurationSettings].[AccessibilityConfigurationSettingGuid] 
                    INNER JOIN [track].[tblEntityUserToSite] MAPCT
                        ON MAPCT.PK_UserToSiteGuid = data.[UserToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ORDER BY _RowVersion ASC


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
