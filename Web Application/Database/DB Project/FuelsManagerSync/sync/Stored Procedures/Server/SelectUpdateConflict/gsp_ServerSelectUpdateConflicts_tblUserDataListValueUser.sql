-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueUser
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataListValueUser]
@UserDataListValueUserGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValueUser].[UserDataListValueUserGuid],[dbo].[tblUserDataListValueUser].[UserDataFieldUserGuid],[dbo].[tblUserDataListValueUser].[Value],[dbo].[tblUserDataListValueUser].[CreatedDate],[dbo].[tblUserDataListValueUser].[CreatedBy],[dbo].[tblUserDataListValueUser].[UpdatedDate],[dbo].[tblUserDataListValueUser].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValueUser]
            INNER JOIN [track].[tblUserDataListValueUser] CT
                ON CT.PK_UserDataListValueUserGuid = [dbo].[tblUserDataListValueUser].[UserDataListValueUserGuid]
        WHERE CT.PK_UserDataListValueUserGuid = @UserDataListValueUserGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
