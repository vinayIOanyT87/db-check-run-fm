-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUsers
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUsers]
@UserGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUsers].[UserID],[dbo].[tblUsers].[Password],[dbo].[tblUsers].[LastLoginDate],[dbo].[tblUsers].[LastLogoffDate],[dbo].[tblUsers].[ChangePassword],[dbo].[tblUsers].[PasswordTimeStamp],[dbo].[tblUsers].[Name],[dbo].[tblUsers].[EmailAddress],[dbo].[tblUsers].[CreatedDate],[dbo].[tblUsers].[CreatedBy],[dbo].[tblUsers].[UpdatedDate],[dbo].[tblUsers].[UpdatedBy],[dbo].[tblUsers].[PasswordHistory1],[dbo].[tblUsers].[PasswordHistory2],[dbo].[tblUsers].[PasswordHistory3],[dbo].[tblUsers].[PasswordHistory4],[dbo].[tblUsers].[PasswordHistory5],[dbo].[tblUsers].[PasswordHistory6],[dbo].[tblUsers].[PasswordHistory7],[dbo].[tblUsers].[PasswordHistory8],[dbo].[tblUsers].[PasswordHistory9],[dbo].[tblUsers].[PasswordHistory10],[dbo].[tblUsers].[PasswordHistory11],[dbo].[tblUsers].[PasswordHistory12],[dbo].[tblUsers].[PasswordHistory13],[dbo].[tblUsers].[PasswordHistory14],[dbo].[tblUsers].[PasswordHistory15],[dbo].[tblUsers].[PasswordHistory16],[dbo].[tblUsers].[PasswordHistory17],[dbo].[tblUsers].[PasswordHistory18],[dbo].[tblUsers].[PasswordHistory19],[dbo].[tblUsers].[PasswordHistory20],[dbo].[tblUsers].[PasswordHistory21],[dbo].[tblUsers].[PasswordHistory22],[dbo].[tblUsers].[PasswordHistory23],[dbo].[tblUsers].[PasswordHistory24],[dbo].[tblUsers].[PasswordLockoutCount],[dbo].[tblUsers].[InactivityLockout],[dbo].[tblUsers].[InactivityLockoutDate],[dbo].[tblUsers].[UserGuid],[dbo].[tblUsers].[SiteGuid],[dbo].[tblUsers].[PasswordHint],[dbo].[tblUsers].[UserData1],[dbo].[tblUsers].[UserData2],[dbo].[tblUsers].[UserData3],[dbo].[tblUsers].[UserData4],[dbo].[tblUsers].[UserData5],[dbo].[tblUsers].[UserData6],[dbo].[tblUsers].[UserData7],[dbo].[tblUsers].[UserData8],[dbo].[tblUsers].[PhoneNumber],[dbo].[tblUsers].[AccountExpirationDate],[dbo].[tblUsers].[ActiveDirectoryUser], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUsers]
            INNER JOIN [track].[tblUsers] CT
                ON CT.PK_UserGuid = [dbo].[tblUsers].[UserGuid]
        WHERE CT.PK_UserGuid = @UserGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
