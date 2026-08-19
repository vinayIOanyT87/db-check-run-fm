-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExportResultType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExportResultType]
@ExportResultTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblExportResultType].[ExportResultTypeIndex],[lookup].[tblExportResultType].[ExportResultTypeCode],[lookup].[tblExportResultType].[ExportResultTypeName],[lookup].[tblExportResultType].[ExportResultTypeGuid],[lookup].[tblExportResultType].[CreatedDate],[lookup].[tblExportResultType].[CreatedBy],[lookup].[tblExportResultType].[UpdatedDate],[lookup].[tblExportResultType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblExportResultType]
            INNER JOIN [track].[tblExportResultType] CT
                ON CT.PK_ExportResultTypeIndex = [lookup].[tblExportResultType].[ExportResultTypeIndex]
        WHERE CT.PK_ExportResultTypeIndex = @ExportResultTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
