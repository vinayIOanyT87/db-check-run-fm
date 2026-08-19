-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGroups
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGroups]
@GroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblGroups].[GroupID],[dbo].[tblGroups].[GroupDescription],[dbo].[tblGroups].[SessionTimeout],[dbo].[tblGroups].[CreatedDate],[dbo].[tblGroups].[CreatedBy],[dbo].[tblGroups].[UpdatedDate],[dbo].[tblGroups].[UpdatedBy],[dbo].[tblGroups].[GroupGuid],[dbo].[tblGroups].[SiteGuid],[dbo].[tblGroups].[ActiveDirectoryUserGroupGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblGroups]
            INNER JOIN [track].[tblGroups] CT
                ON CT.PK_GroupGuid = [dbo].[tblGroups].[GroupGuid]
        WHERE CT.PK_GroupGuid = @GroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
