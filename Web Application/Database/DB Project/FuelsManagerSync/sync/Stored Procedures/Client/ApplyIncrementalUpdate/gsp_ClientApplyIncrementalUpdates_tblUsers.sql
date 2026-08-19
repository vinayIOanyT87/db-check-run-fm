-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUsers
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblUsers]
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
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);


    DECLARE @existingLastLoginDate [datetimeoffset](7)
    DECLARE @existingLastLogoffDate [datetimeoffset](7)
    DECLARE @existingPasswordTimeStamp [datetimeoffset](7)
    DECLARE @existingCreatedDate [datetimeoffset](7)
    DECLARE @existingUpdatedDate [datetimeoffset](7)
    DECLARE @existingPasswordLockoutCount [int]
    DECLARE @existingInactivityLockout [bit]
    DECLARE @existingInactivityLockoutDate [datetimeoffset](7)
    DECLARE @existingUserGuid [uniqueidentifier]
    DECLARE @CT_UpdatedContext [varbinary](128)
    DECLARE @CT_UpdatedRowVersion [varbinary](8)
    DECLARE @CT_UpdatedDate [datetimeoffset](7)

    SET NOCOUNT ON;
    SELECT @existingLastLoginDate = [dbo].[tblUsers].[LastLoginDate]
            ,@existingLastLogoffDate = [dbo].[tblUsers].[LastLogoffDate]
            ,@existingPasswordTimeStamp = [dbo].[tblUsers].[PasswordTimeStamp]
            ,@existingCreatedDate = [dbo].[tblUsers].[CreatedDate]
            ,@existingUpdatedDate = [dbo].[tblUsers].[UpdatedDate]
            ,@existingPasswordLockoutCount = [dbo].[tblUsers].[PasswordLockoutCount]
            ,@existingInactivityLockout = [dbo].[tblUsers].[InactivityLockout]
            ,@existingInactivityLockoutDate = [dbo].[tblUsers].[InactivityLockoutDate]
            ,@existingUserGuid = [dbo].[tblUsers].[UserGuid]
            ,@CT_UpdatedContext = CT.[UpdatedContext]
            ,@CT_UpdatedRowVersion = CT.[UpdatedRowVersion]
            ,@CT_UpdatedDate = CT.[UpdatedDate]
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
    IF (@wasDeleted = 0 AND (@existingUserGuid IS NOT NULL))
    BEGIN
        -- If the server was the last one to update this record and the updated date hasn't change and the password timestamp hasn't changed then leave it
        IF (@PasswordTimeStamp >= @existingPasswordTimeStamp
            AND (@sync_force_write = 1 -- THE ENGINE IS FORCING US TO ATTEMPT TO UPDATE IT
                OR (@CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                OR (@CT_UpdatedRowVersion IS NOT NULL AND @CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- (INTERNALLY, THIS ANCHOR HAS BEEN SWAPPED FOR THE LAST SENT ANCHOR), IF THE CURRENT RECORD HAS NEVER BEEN MODIFIED OR IS OLDER THAN OUR LAST SYNC ANCHOR
                OR (@UpdatedDate > @CT_UpdatedDate AND (@CT_UpdatedContext IS NULL OR @CT_UpdatedContext <> @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), Incoming changes are newer than changes made locally or by another client via sync
                OR (@UpdatedDate >= @CT_UpdatedDate AND @CT_UpdatedContext IS NOT NULL AND @CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
            )
        BEGIN
            -- The server has a more recent copy of the record and the password may or may not have changed.
            UPDATE [dbo].[tblUsers] SET [UserID] = @UserID
                                       ,[Password] = @Password
                                       ,[LastLoginDate] = @LastLoginDate
                                       ,[LastLogoffDate] = @LastLogoffDate
                                       ,[ChangePassword] = @ChangePassword
                                       ,[PasswordTimeStamp] = @PasswordTimeStamp
                                       ,[Name] = @Name
                                       ,[EmailAddress] = @EmailAddress
                                       ,[CreatedDate] = @CreatedDate
                                       ,[CreatedBy] = @CreatedBy
                                       ,[UpdatedDate] = @UpdatedDate
                                       ,[UpdatedBy] = @UpdatedBy
                                       ,[PasswordHistory1] = @PasswordHistory1
                                       ,[PasswordHistory2] = @PasswordHistory2
                                       ,[PasswordHistory3] = @PasswordHistory3
                                       ,[PasswordHistory4] = @PasswordHistory4
                                       ,[PasswordHistory5] = @PasswordHistory5
                                       ,[PasswordHistory6] = @PasswordHistory6
                                       ,[PasswordHistory7] = @PasswordHistory7
                                       ,[PasswordHistory8] = @PasswordHistory8
                                       ,[PasswordHistory9] = @PasswordHistory9
                                       ,[PasswordHistory10] = @PasswordHistory10
                                       ,[PasswordHistory11] = @PasswordHistory11
                                       ,[PasswordHistory12] = @PasswordHistory12
                                       ,[PasswordHistory13] = @PasswordHistory13
                                       ,[PasswordHistory14] = @PasswordHistory14
                                       ,[PasswordHistory15] = @PasswordHistory15
                                       ,[PasswordHistory16] = @PasswordHistory16
                                       ,[PasswordHistory17] = @PasswordHistory17
                                       ,[PasswordHistory18] = @PasswordHistory18
                                       ,[PasswordHistory19] = @PasswordHistory19
                                       ,[PasswordHistory20] = @PasswordHistory20
                                       ,[PasswordHistory21] = @PasswordHistory21
                                       ,[PasswordHistory22] = @PasswordHistory22
                                       ,[PasswordHistory23] = @PasswordHistory23
                                       ,[PasswordHistory24] = @PasswordHistory24
                                       ,[PasswordLockoutCount] = @PasswordLockoutCount
                                       ,[InactivityLockout] = @InactivityLockout
                                       ,[InactivityLockoutDate] = @InactivityLockoutDate
                                       ,[SiteGuid] = @SiteGuid
                                       ,[PasswordHint] = @PasswordHint
                                       ,[UserData1] = @UserData1
                                       ,[UserData2] = @UserData2
                                       ,[UserData3] = @UserData3
                                       ,[UserData4] = @UserData4
                                       ,[UserData5] = @UserData5
                                       ,[UserData6] = @UserData6
                                       ,[UserData7] = @UserData7
                                       ,[UserData8] = @UserData8
                                       ,[PhoneNumber] = @PhoneNumber 
                                       ,[AccountExpirationDate] = @AccountExpirationDate
                                       
                            WHERE [dbo].[tblUsers].[UserGuid] = @UserGuid
                                AND @UserGuid <> '00000000-0000-0000-0000-000000000002' --admin user guid,don't want to sync admin user
        END
        ELSE IF (@wasDeleted = 0 AND (@PasswordTimeStamp < @existingPasswordTimeStamp
                                        AND (@sync_force_write = 1 -- THE ENGINE IS FORCING US TO ATTEMPT TO UPDATE IT
                                            OR (@CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                                            OR (@CT_UpdatedRowVersion IS NOT NULL AND @CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- (INTERNALLY, THIS ANCHOR HAS BEEN SWAPPED FOR THE LAST SENT ANCHOR), IF THE CURRENT RECORD HAS NEVER BEEN MODIFIED OR IS OLDER THAN OUR LAST SYNC ANCHOR
                                            OR (@UpdatedDate > @CT_UpdatedDate AND (@CT_UpdatedContext IS NULL OR @CT_UpdatedContext <> @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), Incoming changes are newer than changes made locally or by another client via sync
                                            OR (@UpdatedDate >= @CT_UpdatedDate AND @CT_UpdatedContext IS NOT NULL AND @CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
        
        ))
        BEGIN
            UPDATE [dbo].[tblUsers] SET [UserID] = @UserID
                                       ,[LastLoginDate] = CASE WHEN @LastLoginDate > @existingLastLoginDate THEN @LastLoginDate ELSE @existingLastLoginDate END
                                       ,[LastLogoffDate] = CASE WHEN @LastLogoffDate > @existingLastLogoffDate THEN @LastLogoffDate ELSE @existingLastLogoffDate END
                                       ,[ChangePassword] = @ChangePassword
                                       ,[Name] = @Name
                                       ,[EmailAddress] = @EmailAddress
                                       ,[UpdatedDate] = @UpdatedDate
                                       ,[UpdatedBy] = @UpdatedBy
                                       ,[PasswordLockoutCount] = CASE WHEN @LastLoginDate > @existingLastLoginDate THEN @PasswordLockoutCount ELSE @existingPasswordLockoutCount END
                                       ,[InactivityLockout] = CASE WHEN @LastLoginDate > @existingLastLoginDate THEN @InactivityLockout ELSE @existingInactivityLockout END
                                       ,[InactivityLockoutDate] = CASE WHEN @LastLoginDate > @existingLastLoginDate THEN @InactivityLockoutDate ELSE @existingInactivityLockoutDate END
                                       ,[SiteGuid] = @SiteGuid
                                       ,[PasswordHint] = @PasswordHint
                                       ,[UserData1] = @UserData1
                                       ,[UserData2] = @UserData2
                                       ,[UserData3] = @UserData3
                                       ,[UserData4] = @UserData4
                                       ,[UserData5] = @UserData5
                                       ,[UserData6] = @UserData6
                                       ,[UserData7] = @UserData7
                                       ,[UserData8] = @UserData8
                                       ,[PhoneNumber] = @PhoneNumber 
                                       ,[AccountExpirationDate] = @AccountExpirationDate
                            WHERE [dbo].[tblUsers].[UserGuid] = @UserGuid
                                AND @UserGuid <> '00000000-0000-0000-0000-000000000002' --admin user guid,don't want to sync admin user
        END
        ELSE IF (@wasDeleted = 0 AND ((@InactivityLockoutDate IS NOT NULL
                                        AND (@existingInactivityLockoutDate IS NULL OR @InactivityLockoutDate > @existingInactivityLockoutDate))
                                        AND (@sync_force_write = 1 -- THE ENGINE IS FORCING US TO ATTEMPT TO UPDATE IT
                                                OR (@CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                                                OR (@CT_UpdatedRowVersion IS NOT NULL AND @CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- (INTERNALLY, THIS ANCHOR HAS BEEN SWAPPED FOR THE LAST SENT ANCHOR), IF THE CURRENT RECORD HAS NEVER BEEN MODIFIED OR IS OLDER THAN OUR LAST SYNC ANCHOR
                                                OR (@UpdatedDate > @CT_UpdatedDate AND (@CT_UpdatedContext IS NULL OR @CT_UpdatedContext <> @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), Incoming changes are newer than changes made locally or by another client via sync
                                                OR (@UpdatedDate >= @CT_UpdatedDate AND @CT_UpdatedContext IS NOT NULL AND @CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                                    ))
        BEGIN
            UPDATE [dbo].[tblUsers] SET [Password] = @Password
                                       ,[ChangePassword] = @ChangePassword
                                       ,[PasswordTimeStamp] = @PasswordTimeStamp
                                       ,[PasswordHistory1] = @PasswordHistory1
                                       ,[PasswordHistory2] = @PasswordHistory2
                                       ,[PasswordHistory3] = @PasswordHistory3
                                       ,[PasswordHistory4] = @PasswordHistory4
                                       ,[PasswordHistory5] = @PasswordHistory5
                                       ,[PasswordHistory6] = @PasswordHistory6
                                       ,[PasswordHistory7] = @PasswordHistory7
                                       ,[PasswordHistory8] = @PasswordHistory8
                                       ,[PasswordHistory9] = @PasswordHistory9
                                       ,[PasswordHistory10] = @PasswordHistory10
                                       ,[PasswordHistory11] = @PasswordHistory11
                                       ,[PasswordHistory12] = @PasswordHistory12
                                       ,[PasswordHistory13] = @PasswordHistory13
                                       ,[PasswordHistory14] = @PasswordHistory14
                                       ,[PasswordHistory15] = @PasswordHistory15
                                       ,[PasswordHistory16] = @PasswordHistory16
                                       ,[PasswordHistory17] = @PasswordHistory17
                                       ,[PasswordHistory18] = @PasswordHistory18
                                       ,[PasswordHistory19] = @PasswordHistory19
                                       ,[PasswordHistory20] = @PasswordHistory20
                                       ,[PasswordHistory21] = @PasswordHistory21
                                       ,[PasswordHistory22] = @PasswordHistory22
                                       ,[PasswordHistory23] = @PasswordHistory23
                                       ,[PasswordHistory24] = @PasswordHistory24
                                       ,[PasswordLockoutCount] = @PasswordLockoutCount
                                       ,[InactivityLockout] = @InactivityLockout
                                       ,[InactivityLockoutDate] = @InactivityLockoutDate
                                       ,[PasswordHint] = @PasswordHint
                                       ,[UpdatedBy] = @UpdatedBy
                                       ,[UserData1] = @UserData1
                                       ,[UserData2] = @UserData2
                                       ,[UserData3] = @UserData3
                                       ,[UserData4] = @UserData4
                                       ,[UserData5] = @UserData5
                                       ,[UserData6] = @UserData6
                                       ,[UserData7] = @UserData7
                                       ,[UserData8] = @UserData8
                                       ,[PhoneNumber] = @PhoneNumber 
                                       ,[AccountExpirationDate] = @AccountExpirationDate
                            WHERE [dbo].[tblUsers].[UserGuid] = @UserGuid
                                AND @UserGuid <> '00000000-0000-0000-0000-000000000002' --admin user guid,don't want to sync admin user
        END
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
    IF (@sync_row_count = 0 AND @existingUserGuid IS NULL)
    BEGIN
        SET @sync_row_count = 1;
    END
    ELSE IF (@sync_row_count = 0 AND EXISTS (SELECT 1 FROM tblUsers WHERE UserGuid = @UserGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate)
    OR (@InactivityLockoutDate IS NULL
    OR (@InactivityLockoutDate IS NOT NULL AND @InactivityLockoutDate <= InactivityLockoutDate))))
    BEGIN

            SET @sync_row_count = 1;
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
