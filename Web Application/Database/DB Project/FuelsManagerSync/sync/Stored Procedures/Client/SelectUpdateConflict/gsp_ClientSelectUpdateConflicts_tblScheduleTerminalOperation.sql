-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblScheduleTerminalOperation
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblScheduleTerminalOperation]
@ScheduleTerminalOperationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblScheduleTerminalOperation].[ScheduleTerminalOperationGuid],[dbo].[tblScheduleTerminalOperation].[SiteGuid],[dbo].[tblScheduleTerminalOperation].[LookupDayOfWeekIndex],[dbo].[tblScheduleTerminalOperation].[Enabled],[dbo].[tblScheduleTerminalOperation].[OpeningTime],[dbo].[tblScheduleTerminalOperation].[ClosingTime],[dbo].[tblScheduleTerminalOperation].[EndOfDayEnabled],[dbo].[tblScheduleTerminalOperation].[EndOfDayTime],[dbo].[tblScheduleTerminalOperation].[CreatedDate],[dbo].[tblScheduleTerminalOperation].[CreatedBy],[dbo].[tblScheduleTerminalOperation].[UpdatedDate],[dbo].[tblScheduleTerminalOperation].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblScheduleTerminalOperation]
            INNER JOIN [track].[tblScheduleTerminalOperation] CT
                ON CT.PK_ScheduleTerminalOperationGuid = [dbo].[tblScheduleTerminalOperation].[ScheduleTerminalOperationGuid]
        WHERE CT.PK_ScheduleTerminalOperationGuid = @ScheduleTerminalOperationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
