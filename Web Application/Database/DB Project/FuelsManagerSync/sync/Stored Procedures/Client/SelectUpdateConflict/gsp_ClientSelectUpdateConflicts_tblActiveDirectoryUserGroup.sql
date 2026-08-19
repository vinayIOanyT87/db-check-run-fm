-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblActiveDirectoryUserGroup
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblActiveDirectoryUserGroup]
@ActiveDirectoryUserGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblActiveDirectoryUserGroup].[ActiveDirectoryUserGroupGuid],[dbo].[tblActiveDirectoryUserGroup].[Name],[dbo].[tblActiveDirectoryUserGroup].[Ssid],[dbo].[tblActiveDirectoryUserGroup].[CreatedBy],[dbo].[tblActiveDirectoryUserGroup].[CreatedDate],[dbo].[tblActiveDirectoryUserGroup].[UpdatedBy],[dbo].[tblActiveDirectoryUserGroup].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblActiveDirectoryUserGroup]
            INNER JOIN [track].[tblActiveDirectoryUserGroup] CT
                ON CT.PK_ActiveDirectoryUserGroupGuid = [dbo].[tblActiveDirectoryUserGroup].[ActiveDirectoryUserGroupGuid]
        WHERE CT.PK_ActiveDirectoryUserGroupGuid = @ActiveDirectoryUserGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
