-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEmailGroups
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEmailGroups]
@EmailGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblEmailGroups].[ID],[dbo].[tblEmailGroups].[AlwaysEnabled],[dbo].[tblEmailGroups].[StartTime],[dbo].[tblEmailGroups].[EndTime],[dbo].[tblEmailGroups].[CategoriesAndPriorities],[dbo].[tblEmailGroups].[CreatedDate],[dbo].[tblEmailGroups].[CreatedBy],[dbo].[tblEmailGroups].[UpdatedDate],[dbo].[tblEmailGroups].[UpdatedBy],[dbo].[tblEmailGroups].[EmailGroupGuid],[dbo].[tblEmailGroups].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblEmailGroups]
            INNER JOIN [track].[tblEmailGroups] CT
                ON CT.PK_EmailGroupGuid = [dbo].[tblEmailGroups].[EmailGroupGuid]
        WHERE CT.PK_EmailGroupGuid = @EmailGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
