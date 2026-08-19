-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblArchivedUsers
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblArchivedUsers]
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
@sync_batch_size_tblArchivedUsers int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblArchivedUsers int
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
        SELECT [dbo].[tblArchivedUsers].[UserID],[dbo].[tblArchivedUsers].[Password],[dbo].[tblArchivedUsers].[LastLoginDate],[dbo].[tblArchivedUsers].[LastLogoffDate],[dbo].[tblArchivedUsers].[ChangePassword],[dbo].[tblArchivedUsers].[PasswordTimeStamp],[dbo].[tblArchivedUsers].[Name],[dbo].[tblArchivedUsers].[EmailAddress],[dbo].[tblArchivedUsers].[CreatedDate],[dbo].[tblArchivedUsers].[CreatedBy],[dbo].[tblArchivedUsers].[UpdatedDate],[dbo].[tblArchivedUsers].[UpdatedBy],[dbo].[tblArchivedUsers].[PasswordHistory1],[dbo].[tblArchivedUsers].[PasswordHistory2],[dbo].[tblArchivedUsers].[PasswordHistory3],[dbo].[tblArchivedUsers].[PasswordHistory4],[dbo].[tblArchivedUsers].[PasswordHistory5],[dbo].[tblArchivedUsers].[PasswordHistory6],[dbo].[tblArchivedUsers].[PasswordHistory7],[dbo].[tblArchivedUsers].[PasswordHistory8],[dbo].[tblArchivedUsers].[PasswordHistory9],[dbo].[tblArchivedUsers].[PasswordHistory10],[dbo].[tblArchivedUsers].[PasswordHistory11],[dbo].[tblArchivedUsers].[PasswordHistory12],[dbo].[tblArchivedUsers].[PasswordHistory13],[dbo].[tblArchivedUsers].[PasswordHistory14],[dbo].[tblArchivedUsers].[PasswordHistory15],[dbo].[tblArchivedUsers].[PasswordHistory16],[dbo].[tblArchivedUsers].[PasswordHistory17],[dbo].[tblArchivedUsers].[PasswordHistory18],[dbo].[tblArchivedUsers].[PasswordHistory19],[dbo].[tblArchivedUsers].[PasswordHistory20],[dbo].[tblArchivedUsers].[PasswordHistory21],[dbo].[tblArchivedUsers].[PasswordHistory22],[dbo].[tblArchivedUsers].[PasswordHistory23],[dbo].[tblArchivedUsers].[PasswordHistory24],[dbo].[tblArchivedUsers].[PasswordLockoutCount],[dbo].[tblArchivedUsers].[InactivityLockout],[dbo].[tblArchivedUsers].[InactivityLockoutDate],[dbo].[tblArchivedUsers].[ArchivedDate],[dbo].[tblArchivedUsers].[ArchivedUserGuid],[dbo].[tblArchivedUsers].[SiteGuid],[dbo].[tblArchivedUsers].[UserGuid],[dbo].[tblArchivedUsers].[PasswordHint],[dbo].[tblArchivedUsers].[UserData1],[dbo].[tblArchivedUsers].[UserData2],[dbo].[tblArchivedUsers].[UserData3],[dbo].[tblArchivedUsers].[UserData4],[dbo].[tblArchivedUsers].[UserData5],[dbo].[tblArchivedUsers].[UserData6],[dbo].[tblArchivedUsers].[UserData7],[dbo].[tblArchivedUsers].[UserData8],[dbo].[tblArchivedUsers].[PhoneNumber],[dbo].[tblArchivedUsers].[AccountExpirationDate], [dbo].[tblArchivedUsers].[_RowVersion]
            FROM [dbo].[tblArchivedUsers]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblArchivedUsers IS NULL OR 
        (@sync_batch_size_tblArchivedUsers IS NOT NULL AND @sync_batch_size_tblArchivedUsers = 0))
    BEGIN
        SET @sync_batch_size_tblArchivedUsers = 2147483647;
    END


        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblArchivedUsers) WITH TIES [dbo].[tblArchivedUsers].[UserID],[dbo].[tblArchivedUsers].[Password],[dbo].[tblArchivedUsers].[LastLoginDate],[dbo].[tblArchivedUsers].[LastLogoffDate],[dbo].[tblArchivedUsers].[ChangePassword],[dbo].[tblArchivedUsers].[PasswordTimeStamp],[dbo].[tblArchivedUsers].[Name],[dbo].[tblArchivedUsers].[EmailAddress],[dbo].[tblArchivedUsers].[CreatedDate],[dbo].[tblArchivedUsers].[CreatedBy],[dbo].[tblArchivedUsers].[UpdatedDate],[dbo].[tblArchivedUsers].[UpdatedBy],[dbo].[tblArchivedUsers].[PasswordHistory1],[dbo].[tblArchivedUsers].[PasswordHistory2],[dbo].[tblArchivedUsers].[PasswordHistory3],[dbo].[tblArchivedUsers].[PasswordHistory4],[dbo].[tblArchivedUsers].[PasswordHistory5],[dbo].[tblArchivedUsers].[PasswordHistory6],[dbo].[tblArchivedUsers].[PasswordHistory7],[dbo].[tblArchivedUsers].[PasswordHistory8],[dbo].[tblArchivedUsers].[PasswordHistory9],[dbo].[tblArchivedUsers].[PasswordHistory10],[dbo].[tblArchivedUsers].[PasswordHistory11],[dbo].[tblArchivedUsers].[PasswordHistory12],[dbo].[tblArchivedUsers].[PasswordHistory13],[dbo].[tblArchivedUsers].[PasswordHistory14],[dbo].[tblArchivedUsers].[PasswordHistory15],[dbo].[tblArchivedUsers].[PasswordHistory16],[dbo].[tblArchivedUsers].[PasswordHistory17],[dbo].[tblArchivedUsers].[PasswordHistory18],[dbo].[tblArchivedUsers].[PasswordHistory19],[dbo].[tblArchivedUsers].[PasswordHistory20],[dbo].[tblArchivedUsers].[PasswordHistory21],[dbo].[tblArchivedUsers].[PasswordHistory22],[dbo].[tblArchivedUsers].[PasswordHistory23],[dbo].[tblArchivedUsers].[PasswordHistory24],[dbo].[tblArchivedUsers].[PasswordLockoutCount],[dbo].[tblArchivedUsers].[InactivityLockout],[dbo].[tblArchivedUsers].[InactivityLockoutDate],[dbo].[tblArchivedUsers].[ArchivedDate],[dbo].[tblArchivedUsers].[ArchivedUserGuid],[dbo].[tblArchivedUsers].[SiteGuid],[dbo].[tblArchivedUsers].[UserGuid],[dbo].[tblArchivedUsers].[PasswordHint],[dbo].[tblArchivedUsers].[UserData1],[dbo].[tblArchivedUsers].[UserData2],[dbo].[tblArchivedUsers].[UserData3],[dbo].[tblArchivedUsers].[UserData4],[dbo].[tblArchivedUsers].[UserData5],[dbo].[tblArchivedUsers].[UserData6],[dbo].[tblArchivedUsers].[UserData7],[dbo].[tblArchivedUsers].[UserData8],[dbo].[tblArchivedUsers].[PhoneNumber],[dbo].[tblArchivedUsers].[AccountExpirationDate],CT.InsertedRowVersion AS '_RowVersion'
            FROM [dbo].[tblArchivedUsers]
                INNER JOIN [track].[tblArchivedUsers] CT
                    ON CT.PK_ArchivedUserGuid = [dbo].[tblArchivedUsers].[ArchivedUserGuid] 
            WHERE ( [dbo].[tblArchivedUsers].[SiteGuid] = @sync_context_site_guid )
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
