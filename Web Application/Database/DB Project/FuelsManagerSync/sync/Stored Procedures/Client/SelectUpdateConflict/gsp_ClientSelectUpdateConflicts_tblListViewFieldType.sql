-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblListViewFieldType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblListViewFieldType]
@ListViewFieldTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblListViewFieldType].[ListViewFieldTypeIndex],[lookup].[tblListViewFieldType].[ListViewFieldTypeCode],[lookup].[tblListViewFieldType].[ListViewFieldTypeName],[lookup].[tblListViewFieldType].[ListViewFieldTypeGuid],[lookup].[tblListViewFieldType].[CreatedDate],[lookup].[tblListViewFieldType].[CreatedBy],[lookup].[tblListViewFieldType].[UpdatedDate],[lookup].[tblListViewFieldType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblListViewFieldType]
            INNER JOIN [track].[tblListViewFieldType] CT
                ON CT.PK_ListViewFieldTypeIndex = [lookup].[tblListViewFieldType].[ListViewFieldTypeIndex]
        WHERE CT.PK_ListViewFieldTypeIndex = @ListViewFieldTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
