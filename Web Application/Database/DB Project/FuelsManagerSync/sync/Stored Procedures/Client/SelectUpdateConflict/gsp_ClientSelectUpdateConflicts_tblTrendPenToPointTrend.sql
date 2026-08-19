-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblTrendPenToPointTrend
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTrendPenToPointTrend]
@TrendPenToPointTrendGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblTrendPenToPointTrend].[TrendPenToPointTrendGuid],[map].[tblTrendPenToPointTrend].[PointTagGuid],[map].[tblTrendPenToPointTrend].[TrendGuid],[map].[tblTrendPenToPointTrend].[PenColor],[map].[tblTrendPenToPointTrend].[CreatedDate],[map].[tblTrendPenToPointTrend].[CreatedBy],[map].[tblTrendPenToPointTrend].[UpdatedDate],[map].[tblTrendPenToPointTrend].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblTrendPenToPointTrend]
            INNER JOIN [track].[tblTrendPenToPointTrend] CT
                ON CT.PK_TrendPenToPointTrendGuid = [map].[tblTrendPenToPointTrend].[TrendPenToPointTrendGuid]
        WHERE CT.PK_TrendPenToPointTrendGuid = @TrendPenToPointTrendGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
