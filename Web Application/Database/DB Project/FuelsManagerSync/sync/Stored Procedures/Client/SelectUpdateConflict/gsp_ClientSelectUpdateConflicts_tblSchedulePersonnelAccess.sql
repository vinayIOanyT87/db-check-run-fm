-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblSchedulePersonnelAccess
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblSchedulePersonnelAccess]
@SchedulePersonnelAccessGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblSchedulePersonnelAccess].[SchedulePersonnelAccessGuid],[dbo].[tblSchedulePersonnelAccess].[PersonnelGuid],[dbo].[tblSchedulePersonnelAccess].[LookupDayOfWeekIndex],[dbo].[tblSchedulePersonnelAccess].[Enabled],[dbo].[tblSchedulePersonnelAccess].[OpeningTime],[dbo].[tblSchedulePersonnelAccess].[ClosingTime],[dbo].[tblSchedulePersonnelAccess].[EndOfDayEnabled],[dbo].[tblSchedulePersonnelAccess].[EndOfDayTime],[dbo].[tblSchedulePersonnelAccess].[CreatedDate],[dbo].[tblSchedulePersonnelAccess].[CreatedBy],[dbo].[tblSchedulePersonnelAccess].[UpdatedDate],[dbo].[tblSchedulePersonnelAccess].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblSchedulePersonnelAccess]
            INNER JOIN [track].[tblSchedulePersonnelAccess] CT
                ON CT.PK_SchedulePersonnelAccessGuid = [dbo].[tblSchedulePersonnelAccess].[SchedulePersonnelAccessGuid]
        WHERE CT.PK_SchedulePersonnelAccessGuid = @SchedulePersonnelAccessGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
