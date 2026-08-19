-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblListViewType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblListViewType]
@ListViewTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblListViewType].[ListViewTypeIndex],[lookup].[tblListViewType].[ListViewTypeCode],[lookup].[tblListViewType].[ListViewTypeName],[lookup].[tblListViewType].[ListViewTypeGuid],[lookup].[tblListViewType].[CreatedDate],[lookup].[tblListViewType].[CreatedBy],[lookup].[tblListViewType].[UpdatedDate],[lookup].[tblListViewType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblListViewType]
            INNER JOIN [track].[tblListViewType] CT
                ON CT.PK_ListViewTypeIndex = [lookup].[tblListViewType].[ListViewTypeIndex]
        WHERE CT.PK_ListViewTypeIndex = @ListViewTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
