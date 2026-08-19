-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExportResultDetails
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExportResultDetails]
@ExportResultDetailGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblExportResultDetails].[RecordID],[dbo].[tblExportResultDetails].[Fail],[dbo].[tblExportResultDetails].[TransVersion],[dbo].[tblExportResultDetails].[CreatedDate],[dbo].[tblExportResultDetails].[CreatedBy],[dbo].[tblExportResultDetails].[UpdatedDate],[dbo].[tblExportResultDetails].[UpdatedBy],[dbo].[tblExportResultDetails].[Error],[dbo].[tblExportResultDetails].[ExportResultDetailGuid],[dbo].[tblExportResultDetails].[ExportResultGuid],[dbo].[tblExportResultDetails].[InterfaceData01],[dbo].[tblExportResultDetails].[InterfaceData02],[dbo].[tblExportResultDetails].[InterfaceData03],[dbo].[tblExportResultDetails].[InterfaceData04],[dbo].[tblExportResultDetails].[InterfaceData05],[dbo].[tblExportResultDetails].[InterfaceData06],[dbo].[tblExportResultDetails].[InterfaceData07],[dbo].[tblExportResultDetails].[InterfaceData08], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExportResultDetails]
            INNER JOIN [track].[tblExportResultDetails] CT
                ON CT.PK_ExportResultDetailGuid = [dbo].[tblExportResultDetails].[ExportResultDetailGuid]
        WHERE CT.PK_ExportResultDetailGuid = @ExportResultDetailGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
