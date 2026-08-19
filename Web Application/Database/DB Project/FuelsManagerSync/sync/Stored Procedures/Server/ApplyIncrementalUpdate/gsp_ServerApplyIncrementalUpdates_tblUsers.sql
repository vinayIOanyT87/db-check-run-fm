-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUsers
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblUsers]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);


    DECLARE @existingData TABLE
    (
        [UserID] [dbo].[udtUserID] NOT NULL,
        [Password] [varbinary](256) NOT NULL,
        [LastLoginDate] [datetimeoffset](7) NOT NULL,
        [LastLogoffDate] [datetimeoffset](7) NOT NULL,
        [ChangePassword] [bit] NOT NULL,
        [PasswordTimeStamp] [datetimeoffset](7) NOT NULL,
        [Name] [nvarchar](50) NOT NULL,
        [EmailAddress] [nvarchar](50) NULL,
        [CreatedDate] [datetimeoffset](7) NOT NULL,
        [CreatedBy] [dbo].[udtUserID] NOT NULL,
        [UpdatedDate] [datetimeoffset](7) NOT NULL,
        [UpdatedBy] [dbo].[udtUserID] NOT NULL,
        [PasswordHistory1] [varbinary](256) NULL,
        [PasswordHistory2] [varbinary](256) NULL,
        [PasswordHistory3] [varbinary](256) NULL,
        [PasswordHistory4] [varbinary](256) NULL,
        [PasswordHistory5] [varbinary](256) NULL,
        [PasswordHistory6] [varbinary](256) NULL,
        [PasswordHistory7] [varbinary](256) NULL,
        [PasswordHistory8] [varbinary](256) NULL,
        [PasswordHistory9] [varbinary](256) NULL,
        [PasswordHistory10] [varbinary](256) NULL,
        [PasswordHistory11] [varbinary](256) NULL,
        [PasswordHistory12] [varbinary](256) NULL,
        [PasswordHistory13] [varbinary](256) NULL,
        [PasswordHistory14] [varbinary](256) NULL,
        [PasswordHistory15] [varbinary](256) NULL,
        [PasswordHistory16] [varbinary](256) NULL,
        [PasswordHistory17] [varbinary](256) NULL,
        [PasswordHistory18] [varbinary](256) NULL,
        [PasswordHistory19] [varbinary](256) NULL,
        [PasswordHistory20] [varbinary](256) NULL,
        [PasswordHistory21] [varbinary](256) NULL,
        [PasswordHistory22] [varbinary](256) NULL,
        [PasswordHistory23] [varbinary](256) NULL,
        [PasswordHistory24] [varbinary](256) NULL,
        [PasswordLockoutCount] [int] NULL,
        [InactivityLockout] [bit] NULL,
        [InactivityLockoutDate] [datetimeoffset](7) NULL,
        [UserGuid] [uniqueidentifier] NOT NULL,
        [SiteGuid] [uniqueidentifier] NOT NULL,
        [PasswordHint] [varchar](80) NULL,
        [UserData1] [nvarchar](120) NULL,
        [UserData2] [nvarchar](120) NULL,
        [UserData3] [nvarchar](120) NULL,
        [UserData4] [nvarchar](120) NULL,
        [UserData5] [nvarchar](120) NULL,
        [UserData6] [nvarchar](120) NULL,
        [UserData7] [nvarchar](120) NULL,
        [UserData8] [nvarchar](120) NULL,
        [PhoneNumber] [nvarchar] (20) NULL,
        [AccountExpirationDate] [datetime] NULL,        
        [CT_UpdatedContext] [varbinary](128) NULL,
        [CT_UpdatedRowVersion] [varbinary](8) NULL,
        [CT_UpdatedDate] [datetimeoffset](7) NULL
    )

    SET NOCOUNT ON;
    INSERT INTO @existingData 
        SELECT [dbo].[tblUsers].[UserID],[dbo].[tblUsers].[Password],[dbo].[tblUsers].[LastLoginDate],[dbo].[tblUsers].[LastLogoffDate],[dbo].[tblUsers].[ChangePassword],[dbo].[tblUsers].[PasswordTimeStamp],[dbo].[tblUsers].[Name],[dbo].[tblUsers].[EmailAddress],[dbo].[tblUsers].[CreatedDate],[dbo].[tblUsers].[CreatedBy],[dbo].[tblUsers].[UpdatedDate],[dbo].[tblUsers].[UpdatedBy],[dbo].[tblUsers].[PasswordHistory1],[dbo].[tblUsers].[PasswordHistory2],[dbo].[tblUsers].[PasswordHistory3],[dbo].[tblUsers].[PasswordHistory4],[dbo].[tblUsers].[PasswordHistory5],[dbo].[tblUsers].[PasswordHistory6],[dbo].[tblUsers].[PasswordHistory7],[dbo].[tblUsers].[PasswordHistory8],[dbo].[tblUsers].[PasswordHistory9],[dbo].[tblUsers].[PasswordHistory10],[dbo].[tblUsers].[PasswordHistory11],[dbo].[tblUsers].[PasswordHistory12],[dbo].[tblUsers].[PasswordHistory13],[dbo].[tblUsers].[PasswordHistory14],[dbo].[tblUsers].[PasswordHistory15],[dbo].[tblUsers].[PasswordHistory16],[dbo].[tblUsers].[PasswordHistory17],[dbo].[tblUsers].[PasswordHistory18],[dbo].[tblUsers].[PasswordHistory19],[dbo].[tblUsers].[PasswordHistory20],[dbo].[tblUsers].[PasswordHistory21],[dbo].[tblUsers].[PasswordHistory22],[dbo].[tblUsers].[PasswordHistory23],[dbo].[tblUsers].[PasswordHistory24],[dbo].[tblUsers].[PasswordLockoutCount],[dbo].[tblUsers].[InactivityLockout],[dbo].[tblUsers].[InactivityLockoutDate],[dbo].[tblUsers].[UserGuid],[dbo].[tblUsers].[SiteGuid],[dbo].[tblUsers].[PasswordHint], [dbo].[tblUsers].[UserData1],[dbo].[tblUsers].[UserData2],[dbo].[tblUsers].[UserData3],[dbo].[tblUsers].[UserData4],[dbo].[tblUsers].[UserData5],[dbo].[tblUsers].[UserData6],[dbo].[tblUsers].[UserData7],[dbo].[tblUsers].[UserData8], [dbo].[tblUsers].[PhoneNumber], [dbo].[tblUsers].[AccountExpirationDate], CT.[UpdatedContext] 'CT_UpdatedContext',CT.[UpdatedRowVersion] 'CT_UpdatedRowVersion',CT.[UpdatedDate] 'CT_UpdatedDate'
            FROM [dbo].[tblUsers]
                INNER JOIN [track].[tblUsers] CT
                    ON CT.PK_UserGuid = [dbo].[tblUsers].[UserGuid] 
            WHERE CT.PK_UserGuid = @UserGuid
                AND @UserGuid <> '00000000-0000-0000-0000-000000000002' --admin user guid,don't want to sync admin user

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblUsers] CT
                        WHERE CT.PK_UserGuid = @UserGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    SET NOCOUNT OFF;

    -- Make sure we were able to locate the user record and that it meets our update criteria
    -- The server has a more recent copy of the record and the password may or may not have changed.
    IF (@wasDeleted = 0 AND EXISTS (SELECT 1 FROM @existingData WHERE UserGuid = @UserGuid 
                                                                        AND PasswordTimeStamp <= @PasswordTimeStamp
                                                                        AND (@sync_force_write = 1 
                                                                            OR (CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                                                                            OR (CT_UpdatedRowVersion IS NOT NULL AND CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                                                                            OR (@UpdatedDate > CT_UpdatedDate AND (CT_UpdatedContext IS NULL OR CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                                                                            OR (@UpdatedDate >= CT_UpdatedDate AND CT_UpdatedContext IS NOT NULL AND CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                                                                        ))
    BEGIN
        UPDATE [dbo].[tblUsers] SET [UserID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserID'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserID] ELSE remoteChanges.[UserID] END
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
                FROM [dbo].[tblUsers] existingData
                    INNER JOIN
                    (
                        SELECT @UserID 'UserID',@Password 'Password',@LastLoginDate 'LastLoginDate',@LastLogoffDate 'LastLogoffDate',@ChangePassword 'ChangePassword',@PasswordTimeStamp 'PasswordTimeStamp',@Name 'Name',@EmailAddress 'EmailAddress',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PasswordHistory1 'PasswordHistory1',@PasswordHistory2 'PasswordHistory2',@PasswordHistory3 'PasswordHistory3',@PasswordHistory4 'PasswordHistory4',@PasswordHistory5 'PasswordHistory5',@PasswordHistory6 'PasswordHistory6',@PasswordHistory7 'PasswordHistory7',@PasswordHistory8 'PasswordHistory8',@PasswordHistory9 'PasswordHistory9',@PasswordHistory10 'PasswordHistory10',@PasswordHistory11 'PasswordHistory11',@PasswordHistory12 'PasswordHistory12',@PasswordHistory13 'PasswordHistory13',@PasswordHistory14 'PasswordHistory14',@PasswordHistory15 'PasswordHistory15',@PasswordHistory16 'PasswordHistory16',@PasswordHistory17 'PasswordHistory17',@PasswordHistory18 'PasswordHistory18',@PasswordHistory19 'PasswordHistory19',@PasswordHistory20 'PasswordHistory20',@PasswordHistory21 'PasswordHistory21',@PasswordHistory22 'PasswordHistory22',@PasswordHistory23 'PasswordHistory23',@PasswordHistory24 'PasswordHistory24',@PasswordLockoutCount 'PasswordLockoutCount',@InactivityLockout 'InactivityLockout',@InactivityLockoutDate 'InactivityLockoutDate',@UserGuid 'UserGuid',@SiteGuid 'SiteGuid',@PasswordHint 'PasswordHint',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8', @PhoneNumber 'PhoneNumber', @AccountExpirationDate 'AccountExpirationDate'
                    ) remoteChanges
                    ON existingData.[UserGuid] = remoteChanges.[UserGuid]
                    AND @UserGuid <> '00000000-0000-0000-0000-000000000002' --admin user guid,don't want to sync admin user
		SET @sync_row_count = @@rowcount;
	END
    ELSE IF (@wasDeleted = 0 AND EXISTS (SELECT 1 FROM @existingData WHERE UserGuid = @UserGuid 
                                                                            AND PasswordTimeStamp > @PasswordTimeStamp
                                                                            AND (@sync_force_write = 1 
                                                                                    OR (CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                                                                                    OR (CT_UpdatedRowVersion IS NOT NULL AND CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                                                                                    OR (@UpdatedDate > CT_UpdatedDate AND (CT_UpdatedContext IS NULL OR CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                                                                                    OR (@UpdatedDate >= CT_UpdatedDate AND CT_UpdatedContext IS NOT NULL AND CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                                                                            ))
    BEGIN
        UPDATE [dbo].[tblUsers] SET [UserID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserID'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UserID] ELSE remoteChanges.[UserID] END
                                    ,[LastLoginDate] = (CASE 
                                                            WHEN (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastLoginDate'), @sync_supported_columns_tblUsers)) = 0 THEN existingData.[LastLoginDate] 
                                                            WHEN remoteChanges.[LastLoginDate] > existingData.[LastLoginDate] THEN remoteChanges.[LastLoginDate] 
                                                            ELSE existingData.[LastLoginDate] 
                                                        END)
                                    ,[LastLogoffDate] = (CASE 
                                                            WHEN (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastLogoffDate'), @sync_supported_columns_tblUsers)) = 0 THEN existingData.[LastLogoffDate] 
                                                            WHEN remoteChanges.[LastLogoffDate] > existingData.[LastLogoffDate] THEN remoteChanges.[LastLogoffDate]
                                                            ELSE existingData.[LastLogoffDate] 
                                                        END)
                                    ,[ChangePassword] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ChangePassword'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[ChangePassword] ELSE remoteChanges.[ChangePassword] END
                                    ,[Name] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Name'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[Name] ELSE remoteChanges.[Name] END
                                    ,[EmailAddress] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EmailAddress'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[EmailAddress] ELSE remoteChanges.[EmailAddress] END
                                    ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                                    ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                                    ,[PasswordLockoutCount] = (CASE 
                                                                    WHEN (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordLockoutCount'), @sync_supported_columns_tblUsers)) = 0 THEN existingData.[PasswordLockoutCount] 
                                                                    WHEN remoteChanges.[LastLoginDate] > existingData.[LastLoginDate] THEN remoteChanges.[PasswordLockoutCount]
                                                                    ELSE existingData.[PasswordLockoutCount] 
                                                                END)
                                    ,[InactivityLockout] = (CASE 
                                                                WHEN (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InactivityLockout'), @sync_supported_columns_tblUsers)) = 0 THEN existingData.[InactivityLockout] 
                                                                WHEN remoteChanges.[LastLoginDate] > existingData.[LastLoginDate] THEN remoteChanges.[InactivityLockout]
                                                                ELSE existingData.[InactivityLockout]
                                                            END)
                                    ,[InactivityLockoutDate] = (CASE 
                                                                    WHEN (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InactivityLockoutDate'), @sync_supported_columns_tblUsers)) = 0 THEN existingData.[InactivityLockoutDate] 
                                                                    WHEN remoteChanges.[LastLoginDate] > existingData.[LastLoginDate] THEN remoteChanges.[InactivityLockoutDate]
                                                                    ELSE existingData.[InactivityLockoutDate]
                                                                END)
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
                FROM [dbo].[tblUsers] existingData
                    INNER JOIN
                    (
                        SELECT @UserID 'UserID',@Password 'Password',@LastLoginDate 'LastLoginDate',@LastLogoffDate 'LastLogoffDate',@ChangePassword 'ChangePassword',@PasswordTimeStamp 'PasswordTimeStamp',@Name 'Name',@EmailAddress 'EmailAddress',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PasswordHistory1 'PasswordHistory1',@PasswordHistory2 'PasswordHistory2',@PasswordHistory3 'PasswordHistory3',@PasswordHistory4 'PasswordHistory4',@PasswordHistory5 'PasswordHistory5',@PasswordHistory6 'PasswordHistory6',@PasswordHistory7 'PasswordHistory7',@PasswordHistory8 'PasswordHistory8',@PasswordHistory9 'PasswordHistory9',@PasswordHistory10 'PasswordHistory10',@PasswordHistory11 'PasswordHistory11',@PasswordHistory12 'PasswordHistory12',@PasswordHistory13 'PasswordHistory13',@PasswordHistory14 'PasswordHistory14',@PasswordHistory15 'PasswordHistory15',@PasswordHistory16 'PasswordHistory16',@PasswordHistory17 'PasswordHistory17',@PasswordHistory18 'PasswordHistory18',@PasswordHistory19 'PasswordHistory19',@PasswordHistory20 'PasswordHistory20',@PasswordHistory21 'PasswordHistory21',@PasswordHistory22 'PasswordHistory22',@PasswordHistory23 'PasswordHistory23',@PasswordHistory24 'PasswordHistory24',@PasswordLockoutCount 'PasswordLockoutCount',@InactivityLockout 'InactivityLockout',@InactivityLockoutDate 'InactivityLockoutDate',@UserGuid 'UserGuid',@SiteGuid 'SiteGuid',@PasswordHint 'PasswordHint',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8', @PhoneNumber 'PhoneNumber', @AccountExpirationDate 'AccountExpirationDate'
                    ) remoteChanges
                    ON existingData.[UserGuid] = remoteChanges.[UserGuid]
                    AND @UserGuid <> '00000000-0000-0000-0000-000000000002' --admin user guid,don't want to sync admin user
		SET @sync_row_count = @@rowcount;
	END
    ELSE IF (@wasDeleted = 0 AND EXISTS (SELECT 1 FROM @existingData WHERE UserGuid = @UserGuid 
                                                                            AND (InactivityLockoutDate IS NULL OR (@InactivityLockoutDate IS NOT NULL AND @InactivityLockoutDate > InactivityLockoutDate))
                                                                            AND (@sync_force_write = 1 
                                                                                    OR (CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                                                                                    OR (CT_UpdatedRowVersion IS NOT NULL AND CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                                                                                    OR (@UpdatedDate > CT_UpdatedDate AND (CT_UpdatedContext IS NULL OR CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                                                                                    OR (@UpdatedDate >= CT_UpdatedDate AND CT_UpdatedContext IS NOT NULL AND CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                                                                            ))
    BEGIN
        UPDATE [dbo].[tblUsers] SET [Password] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Password'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[Password] ELSE remoteChanges.[Password] END
                                    ,[ChangePassword] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ChangePassword'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[ChangePassword] ELSE remoteChanges.[ChangePassword] END
                                    ,[PasswordTimeStamp] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PasswordTimeStamp'), @sync_supported_columns_tblUsers)) WHEN 0 THEN existingData.[PasswordTimeStamp] ELSE remoteChanges.[PasswordTimeStamp] END
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
                FROM [dbo].[tblUsers] existingData
                    INNER JOIN
                    (
                        SELECT @UserID 'UserID',@Password 'Password',@LastLoginDate 'LastLoginDate',@LastLogoffDate 'LastLogoffDate',@ChangePassword 'ChangePassword',@PasswordTimeStamp 'PasswordTimeStamp',@Name 'Name',@EmailAddress 'EmailAddress',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PasswordHistory1 'PasswordHistory1',@PasswordHistory2 'PasswordHistory2',@PasswordHistory3 'PasswordHistory3',@PasswordHistory4 'PasswordHistory4',@PasswordHistory5 'PasswordHistory5',@PasswordHistory6 'PasswordHistory6',@PasswordHistory7 'PasswordHistory7',@PasswordHistory8 'PasswordHistory8',@PasswordHistory9 'PasswordHistory9',@PasswordHistory10 'PasswordHistory10',@PasswordHistory11 'PasswordHistory11',@PasswordHistory12 'PasswordHistory12',@PasswordHistory13 'PasswordHistory13',@PasswordHistory14 'PasswordHistory14',@PasswordHistory15 'PasswordHistory15',@PasswordHistory16 'PasswordHistory16',@PasswordHistory17 'PasswordHistory17',@PasswordHistory18 'PasswordHistory18',@PasswordHistory19 'PasswordHistory19',@PasswordHistory20 'PasswordHistory20',@PasswordHistory21 'PasswordHistory21',@PasswordHistory22 'PasswordHistory22',@PasswordHistory23 'PasswordHistory23',@PasswordHistory24 'PasswordHistory24',@PasswordLockoutCount 'PasswordLockoutCount',@InactivityLockout 'InactivityLockout',@InactivityLockoutDate 'InactivityLockoutDate',@UserGuid 'UserGuid',@SiteGuid 'SiteGuid',@PasswordHint 'PasswordHint',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8', @PhoneNumber 'PhoneNumber', @AccountExpirationDate 'AccountExpirationDate'
                    ) remoteChanges
                    ON existingData.[UserGuid] = remoteChanges.[UserGuid]
                    AND @UserGuid <> '00000000-0000-0000-0000-000000000002' --admin user guid,don't want to sync admin user
         SET @sync_row_count = @@rowcount;
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END

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

    -- If nothing was updated and it was because we couldn't find a record that we could update
    --
    IF (@sync_row_count = 0 AND @UserGuid = '00000000-0000-0000-0000-000000000002')
    BEGIN
        SET @sync_row_count = 1;
    END
    IF (@sync_row_count = 0 AND EXISTS (SELECT 1 FROM @existingData))
    BEGIN
        SET @sync_row_count = 1;
    END
    ELSE IF (@sync_row_count = 0 AND EXISTS (SELECT 1 FROM tblUsers WHERE UserGuid = @UserGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate)
    AND (@InactivityLockoutDate IS NULL
    OR (@InactivityLockoutDate IS NOT NULL AND @InactivityLockoutDate <= InactivityLockoutDate))))
    BEGIN
        SET @sync_row_count = 1;
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
