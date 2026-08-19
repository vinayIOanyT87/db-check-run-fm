-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblScheduleHoliday
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblScheduleHoliday]
@ScheduleHolidayGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblScheduleHoliday].[ScheduleHolidayGuid],[dbo].[tblScheduleHoliday].[SiteGuid],[dbo].[tblScheduleHoliday].[Enabled],[dbo].[tblScheduleHoliday].[OpeningTime],[dbo].[tblScheduleHoliday].[ClosingTime],[dbo].[tblScheduleHoliday].[EndOfDayEnabled],[dbo].[tblScheduleHoliday].[EndOfDayTime],[dbo].[tblScheduleHoliday].[CreatedDate],[dbo].[tblScheduleHoliday].[CreatedBy],[dbo].[tblScheduleHoliday].[UpdatedDate],[dbo].[tblScheduleHoliday].[UpdatedBy],[dbo].[tblScheduleHoliday].[HolidayDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblScheduleHoliday]
            INNER JOIN [track].[tblScheduleHoliday] CT
                ON CT.PK_ScheduleHolidayGuid = [dbo].[tblScheduleHoliday].[ScheduleHolidayGuid]
        WHERE CT.PK_ScheduleHolidayGuid = @ScheduleHolidayGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
