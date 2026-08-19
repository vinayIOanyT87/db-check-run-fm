-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTrend
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTrend]
@TrendGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTrend].[ID],[dbo].[tblTrend].[Description],[dbo].[tblTrend].[Mode],[dbo].[tblTrend].[PeriodType],[dbo].[tblTrend].[Period],[dbo].[tblTrend].[Start],[dbo].[tblTrend].[End],[dbo].[tblTrend].[CreatedDate],[dbo].[tblTrend].[CreatedBy],[dbo].[tblTrend].[UpdatedDate],[dbo].[tblTrend].[UpdatedBy],[dbo].[tblTrend].[TrendGuid],[dbo].[tblTrend].[SiteGuid],[dbo].[tblTrend].[PointTemplateGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTrend]
            INNER JOIN [track].[tblTrend] CT
                ON CT.PK_TrendGuid = [dbo].[tblTrend].[TrendGuid]
        WHERE CT.PK_TrendGuid = @TrendGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
