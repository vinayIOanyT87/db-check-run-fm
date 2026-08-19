-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblListViewStandardType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblListViewStandardType]
@ListViewStandardTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblListViewStandardType].[ListViewStandardTypeIndex],[lookup].[tblListViewStandardType].[ListViewStandardTypeCode],[lookup].[tblListViewStandardType].[ListViewStandardTypeName],[lookup].[tblListViewStandardType].[ListViewStandardTypeGuid],[lookup].[tblListViewStandardType].[CreatedDate],[lookup].[tblListViewStandardType].[CreatedBy],[lookup].[tblListViewStandardType].[UpdatedDate],[lookup].[tblListViewStandardType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblListViewStandardType]
            INNER JOIN [track].[tblListViewStandardType] CT
                ON CT.PK_ListViewStandardTypeIndex = [lookup].[tblListViewStandardType].[ListViewStandardTypeIndex]
        WHERE CT.PK_ListViewStandardTypeIndex = @ListViewStandardTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
