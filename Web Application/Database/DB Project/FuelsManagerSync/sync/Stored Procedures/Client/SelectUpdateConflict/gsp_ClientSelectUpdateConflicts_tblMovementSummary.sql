-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMovementSummary
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMovementSummary]
@MovementSummaryGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblMovementSummary].[MovementSummaryGuid],[dbo].[tblMovementSummary].[ID],[dbo].[tblMovementSummary].[Description],[dbo].[tblMovementSummary].[MovementSummaryType],[dbo].[tblMovementSummary].[ColumnsDefinition],[dbo].[tblMovementSummary].[FontSize],[dbo].[tblMovementSummary].[RowsDefinition],[dbo].[tblMovementSummary].[OwnerUserGuid],[dbo].[tblMovementSummary].[SiteGuid],[dbo].[tblMovementSummary].[CreatedDate],[dbo].[tblMovementSummary].[CreatedBy],[dbo].[tblMovementSummary].[UpdatedDate],[dbo].[tblMovementSummary].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblMovementSummary]
            INNER JOIN [track].[tblMovementSummary] CT
                ON CT.PK_MovementSummaryGuid = [dbo].[tblMovementSummary].[MovementSummaryGuid]
        WHERE CT.PK_MovementSummaryGuid = @MovementSummaryGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
