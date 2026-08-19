-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblListViews
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblListViews]
@ListViewGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblListViews].[CreatedDate],[dbo].[tblListViews].[CreatedBy],[dbo].[tblListViews].[UpdatedDate],[dbo].[tblListViews].[UpdatedBy],[dbo].[tblListViews].[ID],[dbo].[tblListViews].[ListViewGuid],[dbo].[tblListViews].[SiteGuid],[dbo].[tblListViews].[LookupListViewTypeIndex],[dbo].[tblListViews].[LookupListViewStandardTypeIndex],[dbo].[tblListViews].[LedgerAggregateColumnGuid],[dbo].[tblListViews].[TransactionAliasGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblListViews]
            INNER JOIN [track].[tblListViews] CT
                ON CT.PK_ListViewGuid = [dbo].[tblListViews].[ListViewGuid]
        WHERE CT.PK_ListViewGuid = @ListViewGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
