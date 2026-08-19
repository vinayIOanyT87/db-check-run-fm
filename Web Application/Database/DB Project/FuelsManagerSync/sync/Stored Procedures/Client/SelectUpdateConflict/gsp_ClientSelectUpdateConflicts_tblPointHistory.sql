-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointHistory
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointHistory]
@PointHistoryGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointHistory].[PointHistoryGuid],[dbo].[tblPointHistory].[UserGuid],[dbo].[tblPointHistory].[SiteGuid],[dbo].[tblPointHistory].[StartDate],[dbo].[tblPointHistory].[IntervalQuantity],[dbo].[tblPointHistory].[IntervalType],[dbo].[tblPointHistory].[RangeQuantity],[dbo].[tblPointHistory].[RangeType],[dbo].[tblPointHistory].[ColumnsDefinition],[dbo].[tblPointHistory].[CreatedDate],[dbo].[tblPointHistory].[CreatedBy],[dbo].[tblPointHistory].[UpdatedDate],[dbo].[tblPointHistory].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointHistory]
            INNER JOIN [track].[tblPointHistory] CT
                ON CT.PK_PointHistoryGuid = [dbo].[tblPointHistory].[PointHistoryGuid]
        WHERE CT.PK_PointHistoryGuid = @PointHistoryGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
