-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExportResults
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblExportResults]
@ExportResultGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblExportResults].[InterfaceName],[dbo].[tblExportResults].[TransVersion],[dbo].[tblExportResults].[FailedCount],[dbo].[tblExportResults].[SuccessCount],[dbo].[tblExportResults].[TransDateTime],[dbo].[tblExportResults].[CreatedDate],[dbo].[tblExportResults].[CreatedBy],[dbo].[tblExportResults].[UpdatedDate],[dbo].[tblExportResults].[UpdatedBy],[dbo].[tblExportResults].[BatchID],[dbo].[tblExportResults].[ExportResultGuid],[dbo].[tblExportResults].[SiteGuid],[dbo].[tblExportResults].[LookupExportResultTypeIndex],[dbo].[tblExportResults].[ArchiveFileName], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExportResults]
            INNER JOIN [track].[tblExportResults] CT
                ON CT.PK_ExportResultGuid = [dbo].[tblExportResults].[ExportResultGuid]
        WHERE CT.PK_ExportResultGuid = @ExportResultGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
