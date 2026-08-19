-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUsers
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalUpdates_tblUsers]
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
@sync_batch_size_tblUsers int,
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
        SELECT [dbo].[tblUsers].[UserID],[dbo].[tblUsers].[Password],[dbo].[tblUsers].[LastLoginDate],[dbo].[tblUsers].[LastLogoffDate],[dbo].[tblUsers].[ChangePassword],[dbo].[tblUsers].[PasswordTimeStamp],[dbo].[tblUsers].[Name],[dbo].[tblUsers].[EmailAddress],[dbo].[tblUsers].[CreatedDate],[dbo].[tblUsers].[CreatedBy],[dbo].[tblUsers].[UpdatedDate],[dbo].[tblUsers].[UpdatedBy],[dbo].[tblUsers].[PasswordHistory1],[dbo].[tblUsers].[PasswordHistory2],[dbo].[tblUsers].[PasswordHistory3],[dbo].[tblUsers].[PasswordHistory4],[dbo].[tblUsers].[PasswordHistory5],[dbo].[tblUsers].[PasswordHistory6],[dbo].[tblUsers].[PasswordHistory7],[dbo].[tblUsers].[PasswordHistory8],[dbo].[tblUsers].[PasswordHistory9],[dbo].[tblUsers].[PasswordHistory10],[dbo].[tblUsers].[PasswordHistory11],[dbo].[tblUsers].[PasswordHistory12],[dbo].[tblUsers].[PasswordHistory13],[dbo].[tblUsers].[PasswordHistory14],[dbo].[tblUsers].[PasswordHistory15],[dbo].[tblUsers].[PasswordHistory16],[dbo].[tblUsers].[PasswordHistory17],[dbo].[tblUsers].[PasswordHistory18],[dbo].[tblUsers].[PasswordHistory19],[dbo].[tblUsers].[PasswordHistory20],[dbo].[tblUsers].[PasswordHistory21],[dbo].[tblUsers].[PasswordHistory22],[dbo].[tblUsers].[PasswordHistory23],[dbo].[tblUsers].[PasswordHistory24],[dbo].[tblUsers].[PasswordLockoutCount],[dbo].[tblUsers].[InactivityLockout],[dbo].[tblUsers].[InactivityLockoutDate],[dbo].[tblUsers].[UserGuid],[dbo].[tblUsers].[SiteGuid],[dbo].[tblUsers].[PasswordHint],[dbo].[tblUsers].[UserData1],[dbo].[tblUsers].[UserData2],[dbo].[tblUsers].[UserData3],[dbo].[tblUsers].[UserData4],[dbo].[tblUsers].[UserData5],[dbo].[tblUsers].[UserData6],[dbo].[tblUsers].[UserData7],[dbo].[tblUsers].[UserData8],[dbo].[tblUsers].[PhoneNumber],[dbo].[tblUsers].[AccountExpirationDate],[dbo].[tblUsers].[ActiveDirectoryUser], [dbo].[tblUsers].[_RowVersion]
            FROM [dbo].[tblUsers]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblUsers IS NULL OR 
        (@sync_batch_size_tblUsers IS NOT NULL AND @sync_batch_size_tblUsers = 0))
    BEGIN
        SET @sync_batch_size_tblUsers = 2147483647;
    END

            SELECT TOP(@sync_batch_size_tblUsers) WITH TIES [UserID],[Password],[LastLoginDate],[LastLogoffDate],[ChangePassword],[PasswordTimeStamp],[Name],[EmailAddress],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PasswordHistory1],[PasswordHistory2],[PasswordHistory3],[PasswordHistory4],[PasswordHistory5],[PasswordHistory6],[PasswordHistory7],[PasswordHistory8],[PasswordHistory9],[PasswordHistory10],[PasswordHistory11],[PasswordHistory12],[PasswordHistory13],[PasswordHistory14],[PasswordHistory15],[PasswordHistory16],[PasswordHistory17],[PasswordHistory18],[PasswordHistory19],[PasswordHistory20],[PasswordHistory21],[PasswordHistory22],[PasswordHistory23],[PasswordHistory24],[PasswordLockoutCount],[InactivityLockout],[InactivityLockoutDate],[UserGuid],[SiteGuid],[PasswordHint],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[PhoneNumber],[AccountExpirationDate],[ActiveDirectoryUser],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblUsers) WITH TIES [dbo].[tblUsers].[UserID],[dbo].[tblUsers].[Password],[dbo].[tblUsers].[LastLoginDate],[dbo].[tblUsers].[LastLogoffDate],[dbo].[tblUsers].[ChangePassword],[dbo].[tblUsers].[PasswordTimeStamp],[dbo].[tblUsers].[Name],[dbo].[tblUsers].[EmailAddress],[dbo].[tblUsers].[CreatedDate],[dbo].[tblUsers].[CreatedBy],[dbo].[tblUsers].[UpdatedDate],[dbo].[tblUsers].[UpdatedBy],[dbo].[tblUsers].[PasswordHistory1],[dbo].[tblUsers].[PasswordHistory2],[dbo].[tblUsers].[PasswordHistory3],[dbo].[tblUsers].[PasswordHistory4],[dbo].[tblUsers].[PasswordHistory5],[dbo].[tblUsers].[PasswordHistory6],[dbo].[tblUsers].[PasswordHistory7],[dbo].[tblUsers].[PasswordHistory8],[dbo].[tblUsers].[PasswordHistory9],[dbo].[tblUsers].[PasswordHistory10],[dbo].[tblUsers].[PasswordHistory11],[dbo].[tblUsers].[PasswordHistory12],[dbo].[tblUsers].[PasswordHistory13],[dbo].[tblUsers].[PasswordHistory14],[dbo].[tblUsers].[PasswordHistory15],[dbo].[tblUsers].[PasswordHistory16],[dbo].[tblUsers].[PasswordHistory17],[dbo].[tblUsers].[PasswordHistory18],[dbo].[tblUsers].[PasswordHistory19],[dbo].[tblUsers].[PasswordHistory20],[dbo].[tblUsers].[PasswordHistory21],[dbo].[tblUsers].[PasswordHistory22],[dbo].[tblUsers].[PasswordHistory23],[dbo].[tblUsers].[PasswordHistory24],[dbo].[tblUsers].[PasswordLockoutCount],[dbo].[tblUsers].[InactivityLockout],[dbo].[tblUsers].[InactivityLockoutDate],[dbo].[tblUsers].[UserGuid],[dbo].[tblUsers].[SiteGuid],[dbo].[tblUsers].[PasswordHint],[dbo].[tblUsers].[UserData1],[dbo].[tblUsers].[UserData2],[dbo].[tblUsers].[UserData3],[dbo].[tblUsers].[UserData4],[dbo].[tblUsers].[UserData5],[dbo].[tblUsers].[UserData6],[dbo].[tblUsers].[UserData7],[dbo].[tblUsers].[UserData8],[dbo].[tblUsers].[PhoneNumber],[dbo].[tblUsers].[AccountExpirationDate],[dbo].[tblUsers].[ActiveDirectoryUser],sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblUsers]
                        INNER JOIN (SELECT [UserToSiteGuid],[UserGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedUserListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblUsers].[UserGuid] <> '00000000-0000-0000-0000-000000000002' AND [dbo].[tblUsers].[UserGuid] = data.[UserGuid]
                        INNER JOIN [track].[tblUsers] CT
                            ON CT.PK_UserGuid = [dbo].[tblUsers].[UserGuid] 
                        INNER JOIN [track].[tblEntityUserToSite] MAPCT
                            ON MAPCT.PK_UserToSiteGuid = data.[UserToSiteGuid] 
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
