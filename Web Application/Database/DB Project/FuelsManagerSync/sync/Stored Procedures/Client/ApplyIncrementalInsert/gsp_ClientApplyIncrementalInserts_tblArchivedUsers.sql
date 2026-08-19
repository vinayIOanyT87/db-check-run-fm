-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblArchivedUsers
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblArchivedUsers]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@UserID nvarchar(100),
@Password varbinary(256),
@LastLoginDate datetimeoffset(7),
@LastLogoffDate datetimeoffset(7),
@ChangePassword bit,
@PasswordTimeStamp datetimeoffset(7),
@Name nvarchar(50),
@EmailAddress nvarchar(50),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@PasswordHistory1 varbinary(256),
@PasswordHistory2 varbinary(256),
@PasswordHistory3 varbinary(256),
@PasswordHistory4 varbinary(256),
@PasswordHistory5 varbinary(256),
@PasswordHistory6 varbinary(256),
@PasswordHistory7 varbinary(256),
@PasswordHistory8 varbinary(256),
@PasswordHistory9 varbinary(256),
@PasswordHistory10 varbinary(256),
@PasswordHistory11 varbinary(256),
@PasswordHistory12 varbinary(256),
@PasswordHistory13 varbinary(256),
@PasswordHistory14 varbinary(256),
@PasswordHistory15 varbinary(256),
@PasswordHistory16 varbinary(256),
@PasswordHistory17 varbinary(256),
@PasswordHistory18 varbinary(256),
@PasswordHistory19 varbinary(256),
@PasswordHistory20 varbinary(256),
@PasswordHistory21 varbinary(256),
@PasswordHistory22 varbinary(256),
@PasswordHistory23 varbinary(256),
@PasswordHistory24 varbinary(256),
@PasswordLockoutCount int,
@InactivityLockout int,
@InactivityLockoutDate datetimeoffset(7),
@ArchivedDate datetimeoffset(7),
@ArchivedUserGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@UserGuid uniqueidentifier,
@PasswordHint varchar(80),
@UserData1 nvarchar(120),
@UserData2 nvarchar(120),
@UserData3 nvarchar(120),
@UserData4 nvarchar(120),
@UserData5 nvarchar(120),
@UserData6 nvarchar(120),
@UserData7 nvarchar(120),
@UserData8 nvarchar(120),
@PhoneNumber nvarchar(20),
@AccountExpirationDate datetime,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblArchivedUsers] AS existingData
        USING (SELECT @UserID 'UserID',@Password 'Password',@LastLoginDate 'LastLoginDate',@LastLogoffDate 'LastLogoffDate',@ChangePassword 'ChangePassword',@PasswordTimeStamp 'PasswordTimeStamp',@Name 'Name',@EmailAddress 'EmailAddress',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PasswordHistory1 'PasswordHistory1',@PasswordHistory2 'PasswordHistory2',@PasswordHistory3 'PasswordHistory3',@PasswordHistory4 'PasswordHistory4',@PasswordHistory5 'PasswordHistory5',@PasswordHistory6 'PasswordHistory6',@PasswordHistory7 'PasswordHistory7',@PasswordHistory8 'PasswordHistory8',@PasswordHistory9 'PasswordHistory9',@PasswordHistory10 'PasswordHistory10',@PasswordHistory11 'PasswordHistory11',@PasswordHistory12 'PasswordHistory12',@PasswordHistory13 'PasswordHistory13',@PasswordHistory14 'PasswordHistory14',@PasswordHistory15 'PasswordHistory15',@PasswordHistory16 'PasswordHistory16',@PasswordHistory17 'PasswordHistory17',@PasswordHistory18 'PasswordHistory18',@PasswordHistory19 'PasswordHistory19',@PasswordHistory20 'PasswordHistory20',@PasswordHistory21 'PasswordHistory21',@PasswordHistory22 'PasswordHistory22',@PasswordHistory23 'PasswordHistory23',@PasswordHistory24 'PasswordHistory24',@PasswordLockoutCount 'PasswordLockoutCount',@InactivityLockout 'InactivityLockout',@InactivityLockoutDate 'InactivityLockoutDate',@ArchivedDate 'ArchivedDate',@ArchivedUserGuid 'ArchivedUserGuid',@SiteGuid 'SiteGuid',@UserGuid 'UserGuid',@PasswordHint 'PasswordHint',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@PhoneNumber 'PhoneNumber',@AccountExpirationDate 'AccountExpirationDate'
                ) AS remoteChanges ([UserID],[Password],[LastLoginDate],[LastLogoffDate],[ChangePassword],[PasswordTimeStamp],[Name],[EmailAddress],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PasswordHistory1],[PasswordHistory2],[PasswordHistory3],[PasswordHistory4],[PasswordHistory5],[PasswordHistory6],[PasswordHistory7],[PasswordHistory8],[PasswordHistory9],[PasswordHistory10],[PasswordHistory11],[PasswordHistory12],[PasswordHistory13],[PasswordHistory14],[PasswordHistory15],[PasswordHistory16],[PasswordHistory17],[PasswordHistory18],[PasswordHistory19],[PasswordHistory20],[PasswordHistory21],[PasswordHistory22],[PasswordHistory23],[PasswordHistory24],[PasswordLockoutCount],[InactivityLockout],[InactivityLockoutDate],[ArchivedDate],[ArchivedUserGuid],[SiteGuid],[UserGuid],[PasswordHint],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[PhoneNumber],[AccountExpirationDate])
        ON (existingData.[ArchivedUserGuid] = remoteChanges.[ArchivedUserGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [UserID] = remoteChanges.[UserID]
                       ,[Password] = remoteChanges.[Password]
                       ,[LastLoginDate] = remoteChanges.[LastLoginDate]
                       ,[LastLogoffDate] = remoteChanges.[LastLogoffDate]
                       ,[ChangePassword] = remoteChanges.[ChangePassword]
                       ,[PasswordTimeStamp] = remoteChanges.[PasswordTimeStamp]
                       ,[Name] = remoteChanges.[Name]
                       ,[EmailAddress] = remoteChanges.[EmailAddress]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[PasswordHistory1] = remoteChanges.[PasswordHistory1]
                       ,[PasswordHistory2] = remoteChanges.[PasswordHistory2]
                       ,[PasswordHistory3] = remoteChanges.[PasswordHistory3]
                       ,[PasswordHistory4] = remoteChanges.[PasswordHistory4]
                       ,[PasswordHistory5] = remoteChanges.[PasswordHistory5]
                       ,[PasswordHistory6] = remoteChanges.[PasswordHistory6]
                       ,[PasswordHistory7] = remoteChanges.[PasswordHistory7]
                       ,[PasswordHistory8] = remoteChanges.[PasswordHistory8]
                       ,[PasswordHistory9] = remoteChanges.[PasswordHistory9]
                       ,[PasswordHistory10] = remoteChanges.[PasswordHistory10]
                       ,[PasswordHistory11] = remoteChanges.[PasswordHistory11]
                       ,[PasswordHistory12] = remoteChanges.[PasswordHistory12]
                       ,[PasswordHistory13] = remoteChanges.[PasswordHistory13]
                       ,[PasswordHistory14] = remoteChanges.[PasswordHistory14]
                       ,[PasswordHistory15] = remoteChanges.[PasswordHistory15]
                       ,[PasswordHistory16] = remoteChanges.[PasswordHistory16]
                       ,[PasswordHistory17] = remoteChanges.[PasswordHistory17]
                       ,[PasswordHistory18] = remoteChanges.[PasswordHistory18]
                       ,[PasswordHistory19] = remoteChanges.[PasswordHistory19]
                       ,[PasswordHistory20] = remoteChanges.[PasswordHistory20]
                       ,[PasswordHistory21] = remoteChanges.[PasswordHistory21]
                       ,[PasswordHistory22] = remoteChanges.[PasswordHistory22]
                       ,[PasswordHistory23] = remoteChanges.[PasswordHistory23]
                       ,[PasswordHistory24] = remoteChanges.[PasswordHistory24]
                       ,[PasswordLockoutCount] = remoteChanges.[PasswordLockoutCount]
                       ,[InactivityLockout] = remoteChanges.[InactivityLockout]
                       ,[InactivityLockoutDate] = remoteChanges.[InactivityLockoutDate]
                       ,[ArchivedDate] = remoteChanges.[ArchivedDate]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[UserGuid] = remoteChanges.[UserGuid]
                       ,[PasswordHint] = remoteChanges.[PasswordHint]
                       ,[UserData1] = remoteChanges.[UserData1]
                       ,[UserData2] = remoteChanges.[UserData2]
                       ,[UserData3] = remoteChanges.[UserData3]
                       ,[UserData4] = remoteChanges.[UserData4]
                       ,[UserData5] = remoteChanges.[UserData5]
                       ,[UserData6] = remoteChanges.[UserData6]
                       ,[UserData7] = remoteChanges.[UserData7]
                       ,[UserData8] = remoteChanges.[UserData8]
                       ,[PhoneNumber] = remoteChanges.[PhoneNumber]
                       ,[AccountExpirationDate] = remoteChanges.[AccountExpirationDate]

        WHEN NOT MATCHED THEN
            INSERT ([UserID],[Password],[LastLoginDate],[LastLogoffDate],[ChangePassword],[PasswordTimeStamp],[Name],[EmailAddress],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PasswordHistory1],[PasswordHistory2],[PasswordHistory3],[PasswordHistory4],[PasswordHistory5],[PasswordHistory6],[PasswordHistory7],[PasswordHistory8],[PasswordHistory9],[PasswordHistory10],[PasswordHistory11],[PasswordHistory12],[PasswordHistory13],[PasswordHistory14],[PasswordHistory15],[PasswordHistory16],[PasswordHistory17],[PasswordHistory18],[PasswordHistory19],[PasswordHistory20],[PasswordHistory21],[PasswordHistory22],[PasswordHistory23],[PasswordHistory24],[PasswordLockoutCount],[InactivityLockout],[InactivityLockoutDate],[ArchivedDate],[ArchivedUserGuid],[SiteGuid],[UserGuid],[PasswordHint],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[PhoneNumber],[AccountExpirationDate])
                VALUES (@UserID,@Password,@LastLoginDate,@LastLogoffDate,@ChangePassword,@PasswordTimeStamp,@Name,@EmailAddress,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@PasswordHistory1,@PasswordHistory2,@PasswordHistory3,@PasswordHistory4,@PasswordHistory5,@PasswordHistory6,@PasswordHistory7,@PasswordHistory8,@PasswordHistory9,@PasswordHistory10,@PasswordHistory11,@PasswordHistory12,@PasswordHistory13,@PasswordHistory14,@PasswordHistory15,@PasswordHistory16,@PasswordHistory17,@PasswordHistory18,@PasswordHistory19,@PasswordHistory20,@PasswordHistory21,@PasswordHistory22,@PasswordHistory23,@PasswordHistory24,@PasswordLockoutCount,@InactivityLockout,@InactivityLockoutDate,@ArchivedDate,@ArchivedUserGuid,@SiteGuid,@UserGuid,@PasswordHint,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@PhoneNumber,@AccountExpirationDate)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ArchivedUserGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ArchivedUserGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ArchivedUserGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblArchivedUsers] WHERE ArchivedUserGuid = @ArchivedUserGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
