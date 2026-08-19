-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblArchivedUsers
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblArchivedUsers]
@ArchivedUserGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblArchivedUsers].[UserID],[dbo].[tblArchivedUsers].[Password],[dbo].[tblArchivedUsers].[LastLoginDate],[dbo].[tblArchivedUsers].[LastLogoffDate],[dbo].[tblArchivedUsers].[ChangePassword],[dbo].[tblArchivedUsers].[PasswordTimeStamp],[dbo].[tblArchivedUsers].[Name],[dbo].[tblArchivedUsers].[EmailAddress],[dbo].[tblArchivedUsers].[CreatedDate],[dbo].[tblArchivedUsers].[CreatedBy],[dbo].[tblArchivedUsers].[UpdatedDate],[dbo].[tblArchivedUsers].[UpdatedBy],[dbo].[tblArchivedUsers].[PasswordHistory1],[dbo].[tblArchivedUsers].[PasswordHistory2],[dbo].[tblArchivedUsers].[PasswordHistory3],[dbo].[tblArchivedUsers].[PasswordHistory4],[dbo].[tblArchivedUsers].[PasswordHistory5],[dbo].[tblArchivedUsers].[PasswordHistory6],[dbo].[tblArchivedUsers].[PasswordHistory7],[dbo].[tblArchivedUsers].[PasswordHistory8],[dbo].[tblArchivedUsers].[PasswordHistory9],[dbo].[tblArchivedUsers].[PasswordHistory10],[dbo].[tblArchivedUsers].[PasswordHistory11],[dbo].[tblArchivedUsers].[PasswordHistory12],[dbo].[tblArchivedUsers].[PasswordHistory13],[dbo].[tblArchivedUsers].[PasswordHistory14],[dbo].[tblArchivedUsers].[PasswordHistory15],[dbo].[tblArchivedUsers].[PasswordHistory16],[dbo].[tblArchivedUsers].[PasswordHistory17],[dbo].[tblArchivedUsers].[PasswordHistory18],[dbo].[tblArchivedUsers].[PasswordHistory19],[dbo].[tblArchivedUsers].[PasswordHistory20],[dbo].[tblArchivedUsers].[PasswordHistory21],[dbo].[tblArchivedUsers].[PasswordHistory22],[dbo].[tblArchivedUsers].[PasswordHistory23],[dbo].[tblArchivedUsers].[PasswordHistory24],[dbo].[tblArchivedUsers].[PasswordLockoutCount],[dbo].[tblArchivedUsers].[InactivityLockout],[dbo].[tblArchivedUsers].[InactivityLockoutDate],[dbo].[tblArchivedUsers].[ArchivedDate],[dbo].[tblArchivedUsers].[ArchivedUserGuid],[dbo].[tblArchivedUsers].[SiteGuid],[dbo].[tblArchivedUsers].[UserGuid],[dbo].[tblArchivedUsers].[PasswordHint],[dbo].[tblArchivedUsers].[UserData1],[dbo].[tblArchivedUsers].[UserData2],[dbo].[tblArchivedUsers].[UserData3],[dbo].[tblArchivedUsers].[UserData4],[dbo].[tblArchivedUsers].[UserData5],[dbo].[tblArchivedUsers].[UserData6],[dbo].[tblArchivedUsers].[UserData7],[dbo].[tblArchivedUsers].[UserData8],[dbo].[tblArchivedUsers].[PhoneNumber],[dbo].[tblArchivedUsers].[AccountExpirationDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblArchivedUsers]
            INNER JOIN [track].[tblArchivedUsers] CT
                ON CT.PK_ArchivedUserGuid = [dbo].[tblArchivedUsers].[ArchivedUserGuid]
        WHERE CT.PK_ArchivedUserGuid = @ArchivedUserGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
