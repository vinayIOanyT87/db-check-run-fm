-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblFilterViews
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblFilterViews]
@FilterViewGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblFilterViews].[CreatedDate],[dbo].[tblFilterViews].[CreatedBy],[dbo].[tblFilterViews].[UpdatedDate],[dbo].[tblFilterViews].[UpdatedBy],[dbo].[tblFilterViews].[FilterViewGuid],[dbo].[tblFilterViews].[LookupTransTypeIndex],[dbo].[tblFilterViews].[LookupFilterFieldIndex], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblFilterViews]
            INNER JOIN [track].[tblFilterViews] CT
                ON CT.PK_FilterViewGuid = [dbo].[tblFilterViews].[FilterViewGuid]
        WHERE CT.PK_FilterViewGuid = @FilterViewGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
