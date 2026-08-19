-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUsers
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblUsers]
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
@InactivityLockout bit,
@InactivityLockoutDate datetimeoffset(7),
@UserGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
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
@ActiveDirectoryUser bit,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblUsers varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblUsers] AS existingData
        USING (SELECT @UserID 'UserID',@Password 'Password',@LastLoginDate 'LastLoginDate',@LastLogoffDate 'LastLogoffDate',@ChangePassword 'ChangePassword',@PasswordTimeStamp 'PasswordTimeStamp',@Name 'Name',@EmailAddress 'EmailAddress',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PasswordHistory1 'PasswordHistory1',@PasswordHistory2 'PasswordHistory2',@PasswordHistory3 'PasswordHistory3',@PasswordHistory4 'PasswordHistory4',@PasswordHistory5 'PasswordHistory5',@PasswordHistory6 'PasswordHistory6',@PasswordHistory7 'PasswordHistory7',@PasswordHistory8 'PasswordHistory8',@PasswordHistory9 'PasswordHistory9',@PasswordHistory10 'PasswordHistory10',@PasswordHistory11 'PasswordHistory11',@PasswordHistory12 'PasswordHistory12',@PasswordHistory13 'PasswordHistory13',@PasswordHistory14 'PasswordHistory14',@PasswordHistory15 'PasswordHistory15',@PasswordHistory16 'PasswordHistory16',@PasswordHistory17 'PasswordHistory17',@PasswordHistory18 'PasswordHistory18',@PasswordHistory19 'PasswordHistory19',@PasswordHistory20 'PasswordHistory20',@PasswordHistory21 'PasswordHistory21',@PasswordHistory22 'PasswordHistory22',@PasswordHistory23 'PasswordHistory23',@PasswordHistory24 'PasswordHistory24',@PasswordLockoutCount 'PasswordLockoutCount',@InactivityLockout 'InactivityLockout',@InactivityLockoutDate 'InactivityLockoutDate',@UserGuid 'UserGuid',@SiteGuid 'SiteGuid',@PasswordHint 'PasswordHint',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@PhoneNumber 'PhoneNumber',@AccountExpirationDate 'AccountExpirationDate',@ActiveDirectoryUser 'ActiveDirectoryUser'
                ) AS remoteChanges ([UserID],[Password],[LastLoginDate],[LastLogoffDate],[ChangePassword],[PasswordTimeStamp],[Name],[EmailAddress],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PasswordHistory1],[PasswordHistory2],[PasswordHistory3],[PasswordHistory4],[PasswordHistory5],[PasswordHistory6],[PasswordHistory7],[PasswordHistory8],[PasswordHistory9],[PasswordHistory10],[PasswordHistory11],[PasswordHistory12],[PasswordHistory13],[PasswordHistory14],[PasswordHistory15],[PasswordHistory16],[PasswordHistory17],[PasswordHistory18],[PasswordHistory19],[PasswordHistory20],[PasswordHistory21],[PasswordHistory22],[PasswordHistory23],[PasswordHistory24],[PasswordLockoutCount],[InactivityLockout],[InactivityLockoutDate],[UserGuid],[SiteGuid],[PasswordHint],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[PhoneNumber],[AccountExpirationDate],[ActiveDirectoryUser])
        ON (existingData.[UserGuid] = remoteChanges.[UserGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [UserID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserID'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserID] ELSE remoteChanges.[UserID] END
                       ,[Password] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Password'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[Password] ELSE remoteChanges.[Password] END
                       ,[LastLoginDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastLoginDate'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[LastLoginDate] ELSE remoteChanges.[LastLoginDate] END
                       ,[LastLogoffDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastLogoffDate'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[LastLogoffDate] ELSE remoteChanges.[LastLogoffDate] END
                       ,[ChangePassword] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ChangePassword'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[ChangePassword] ELSE remoteChanges.[ChangePassword] END
                       ,[PasswordTimeStamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordTimeStamp'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordTimeStamp] ELSE remoteChanges.[PasswordTimeStamp] END
                       ,[Name] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Name'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[Name] ELSE remoteChanges.[Name] END
                       ,[EmailAddress] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EmailAddress'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[EmailAddress] ELSE remoteChanges.[EmailAddress] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[PasswordHistory1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory1'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory1] ELSE remoteChanges.[PasswordHistory1] END
                       ,[PasswordHistory2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory2'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory2] ELSE remoteChanges.[PasswordHistory2] END
                       ,[PasswordHistory3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory3'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory3] ELSE remoteChanges.[PasswordHistory3] END
                       ,[PasswordHistory4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory4'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory4] ELSE remoteChanges.[PasswordHistory4] END
                       ,[PasswordHistory5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory5'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory5] ELSE remoteChanges.[PasswordHistory5] END
                       ,[PasswordHistory6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory6'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory6] ELSE remoteChanges.[PasswordHistory6] END
                       ,[PasswordHistory7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory7'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory7] ELSE remoteChanges.[PasswordHistory7] END
                       ,[PasswordHistory8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory8'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory8] ELSE remoteChanges.[PasswordHistory8] END
                       ,[PasswordHistory9] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory9'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory9] ELSE remoteChanges.[PasswordHistory9] END
                       ,[PasswordHistory10] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory10'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory10] ELSE remoteChanges.[PasswordHistory10] END
                       ,[PasswordHistory11] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory11'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory11] ELSE remoteChanges.[PasswordHistory11] END
                       ,[PasswordHistory12] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory12'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory12] ELSE remoteChanges.[PasswordHistory12] END
                       ,[PasswordHistory13] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory13'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory13] ELSE remoteChanges.[PasswordHistory13] END
                       ,[PasswordHistory14] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory14'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory14] ELSE remoteChanges.[PasswordHistory14] END
                       ,[PasswordHistory15] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory15'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory15] ELSE remoteChanges.[PasswordHistory15] END
                       ,[PasswordHistory16] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory16'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory16] ELSE remoteChanges.[PasswordHistory16] END
                       ,[PasswordHistory17] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory17'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory17] ELSE remoteChanges.[PasswordHistory17] END
                       ,[PasswordHistory18] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory18'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory18] ELSE remoteChanges.[PasswordHistory18] END
                       ,[PasswordHistory19] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory19'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory19] ELSE remoteChanges.[PasswordHistory19] END
                       ,[PasswordHistory20] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory20'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory20] ELSE remoteChanges.[PasswordHistory20] END
                       ,[PasswordHistory21] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory21'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory21] ELSE remoteChanges.[PasswordHistory21] END
                       ,[PasswordHistory22] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory22'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory22] ELSE remoteChanges.[PasswordHistory22] END
                       ,[PasswordHistory23] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory23'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory23] ELSE remoteChanges.[PasswordHistory23] END
                       ,[PasswordHistory24] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory24'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHistory24] ELSE remoteChanges.[PasswordHistory24] END
                       ,[PasswordLockoutCount] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordLockoutCount'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordLockoutCount] ELSE remoteChanges.[PasswordLockoutCount] END
                       ,[InactivityLockout] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InactivityLockout'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[InactivityLockout] ELSE remoteChanges.[InactivityLockout] END
                       ,[InactivityLockoutDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InactivityLockoutDate'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[InactivityLockoutDate] ELSE remoteChanges.[InactivityLockoutDate] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[PasswordHint] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHint'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordHint] ELSE remoteChanges.[PasswordHint] END
                       ,[UserData1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserData1] ELSE remoteChanges.[UserData1] END
                       ,[UserData2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserData2] ELSE remoteChanges.[UserData2] END
                       ,[UserData3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserData3] ELSE remoteChanges.[UserData3] END
                       ,[UserData4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserData4] ELSE remoteChanges.[UserData4] END
                       ,[UserData5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserData5] ELSE remoteChanges.[UserData5] END
                       ,[UserData6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserData6] ELSE remoteChanges.[UserData6] END
                       ,[UserData7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserData7] ELSE remoteChanges.[UserData7] END
                       ,[UserData8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserData8] ELSE remoteChanges.[UserData8] END
                       ,[PhoneNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PhoneNumber'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PhoneNumber] ELSE remoteChanges.[PhoneNumber] END
                       ,[AccountExpirationDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AccountExpirationDate'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[AccountExpirationDate] ELSE remoteChanges.[AccountExpirationDate] END
                       ,[ActiveDirectoryUser] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ActiveDirectoryUser'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[ActiveDirectoryUser] ELSE remoteChanges.[ActiveDirectoryUser] END

        WHEN NOT MATCHED THEN
            INSERT ([UserID],[Password],[LastLoginDate],[LastLogoffDate],[ChangePassword],[PasswordTimeStamp],[Name],[EmailAddress],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PasswordHistory1],[PasswordHistory2],[PasswordHistory3],[PasswordHistory4],[PasswordHistory5],[PasswordHistory6],[PasswordHistory7],[PasswordHistory8],[PasswordHistory9],[PasswordHistory10],[PasswordHistory11],[PasswordHistory12],[PasswordHistory13],[PasswordHistory14],[PasswordHistory15],[PasswordHistory16],[PasswordHistory17],[PasswordHistory18],[PasswordHistory19],[PasswordHistory20],[PasswordHistory21],[PasswordHistory22],[PasswordHistory23],[PasswordHistory24],[PasswordLockoutCount],[InactivityLockout],[InactivityLockoutDate],[UserGuid],[SiteGuid],[PasswordHint],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[PhoneNumber],[AccountExpirationDate],[ActiveDirectoryUser])
                VALUES (@UserID,@Password,@LastLoginDate,@LastLogoffDate,@ChangePassword,@PasswordTimeStamp,@Name,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EmailAddress'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @EmailAddress END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory1'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory2'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory3'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory4'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory5'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory6'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory7'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory8'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory8 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory9'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory9 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory10'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory10 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory11'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory11 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory12'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory12 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory13'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory13 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory14'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory14 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory15'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory15 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory16'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory16 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory17'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory17 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory18'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory18 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory19'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory19 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory20'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory20 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory21'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory21 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory22'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory22 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory23'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory23 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHistory24'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHistory24 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordLockoutCount'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordLockoutCount END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InactivityLockout'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @InactivityLockout END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InactivityLockoutDate'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @InactivityLockoutDate END),@UserGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordHint'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PasswordHint END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @UserData1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @UserData2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @UserData3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @UserData4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @UserData5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @UserData6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @UserData7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @UserData8 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PhoneNumber'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @PhoneNumber END),@AccountExpirationDate,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ActiveDirectoryUser'), @sync_supported_columns_tblUsers)) WHEN 0 THEN NULL ELSE @ActiveDirectoryUser END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @UserGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @UserGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @UserGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblUsers] WHERE UserGuid = @UserGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

