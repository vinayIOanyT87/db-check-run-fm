-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblUserToGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserToGroup]
@UserToGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblUserToGroup].[UserToGroupGuid],[map].[tblUserToGroup].[UserGuid],[map].[tblUserToGroup].[GroupGuid],[map].[tblUserToGroup].[ExpirationDate],[map].[tblUserToGroup].[CreatedDate],[map].[tblUserToGroup].[CreatedBy],[map].[tblUserToGroup].[UpdatedDate],[map].[tblUserToGroup].[UpdatedBy],[map].[tblUserToGroup].[SiteGuid],[map].[tblUserToGroup].[DenyADPermission], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblUserToGroup]
            INNER JOIN [track].[tblUserToGroup] CT
                ON CT.PK_UserToGroupGuid = [map].[tblUserToGroup].[UserToGroupGuid]
        WHERE CT.PK_UserToGroupGuid = @UserToGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
