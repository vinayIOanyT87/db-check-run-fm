-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblActiveDirectorySiteGroup
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblActiveDirectorySiteGroup]
@ActiveDirectorySiteGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblActiveDirectorySiteGroup].[ActiveDirectorySiteGroupGuid],[dbo].[tblActiveDirectorySiteGroup].[Name],[dbo].[tblActiveDirectorySiteGroup].[Ssid],[dbo].[tblActiveDirectorySiteGroup].[CreatedBy],[dbo].[tblActiveDirectorySiteGroup].[CreatedDate],[dbo].[tblActiveDirectorySiteGroup].[UpdatedBy],[dbo].[tblActiveDirectorySiteGroup].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblActiveDirectorySiteGroup]
            INNER JOIN [track].[tblActiveDirectorySiteGroup] CT
                ON CT.PK_ActiveDirectorySiteGroupGuid = [dbo].[tblActiveDirectorySiteGroup].[ActiveDirectorySiteGroupGuid]
        WHERE CT.PK_ActiveDirectorySiteGroupGuid = @ActiveDirectorySiteGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
