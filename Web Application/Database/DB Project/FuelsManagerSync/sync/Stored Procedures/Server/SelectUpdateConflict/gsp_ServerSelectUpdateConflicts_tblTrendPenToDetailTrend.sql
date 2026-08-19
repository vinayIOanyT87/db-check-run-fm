-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblTrendPenToDetailTrend
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTrendPenToDetailTrend]
@TrendPenToDetailTrendGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblTrendPenToDetailTrend].[TrendPenToDetailTrendGuid],[map].[tblTrendPenToDetailTrend].[PointTemplateTagGuid],[map].[tblTrendPenToDetailTrend].[TrendGuid],[map].[tblTrendPenToDetailTrend].[PenColor],[map].[tblTrendPenToDetailTrend].[CreatedDate],[map].[tblTrendPenToDetailTrend].[CreatedBy],[map].[tblTrendPenToDetailTrend].[UpdatedDate],[map].[tblTrendPenToDetailTrend].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblTrendPenToDetailTrend]
            INNER JOIN [track].[tblTrendPenToDetailTrend] CT
                ON CT.PK_TrendPenToDetailTrendGuid = [map].[tblTrendPenToDetailTrend].[TrendPenToDetailTrendGuid]
        WHERE CT.PK_TrendPenToDetailTrendGuid = @TrendPenToDetailTrendGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
