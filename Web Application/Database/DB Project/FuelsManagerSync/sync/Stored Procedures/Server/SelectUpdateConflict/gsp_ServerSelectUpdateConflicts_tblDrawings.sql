-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblDrawings
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblDrawings]
@DrawingGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblDrawings].[DrawingGuid],[dbo].[tblDrawings].[ID],[dbo].[tblDrawings].[Description],[dbo].[tblDrawings].[Image],[dbo].[tblDrawings].[SiteGuid],[dbo].[tblDrawings].[PanelType],[dbo].[tblDrawings].[PointTemplateGuid],[dbo].[tblDrawings].[Published],[dbo].[tblDrawings].[CreatedDate],[dbo].[tblDrawings].[CreatedBy],[dbo].[tblDrawings].[UpdatedDate],[dbo].[tblDrawings].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblDrawings]
            INNER JOIN [track].[tblDrawings] CT
                ON CT.PK_DrawingGuid = [dbo].[tblDrawings].[DrawingGuid]
        WHERE CT.PK_DrawingGuid = @DrawingGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
