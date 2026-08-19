-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAppointmentPersonnel
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAppointmentPersonnel]
@AppointmentPersonnelGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAppointmentPersonnel].[AppointmentPersonnelGuid],[dbo].[tblAppointmentPersonnel].[PersonnelGuid],[dbo].[tblAppointmentPersonnel].[TestSetDefinitionGuid],[dbo].[tblAppointmentPersonnel].[SiteGuid],[dbo].[tblAppointmentPersonnel].[AssetText],[dbo].[tblAppointmentPersonnel].[AppointmentCategory],[dbo].[tblAppointmentPersonnel].[AppointmentIsSingle],[dbo].[tblAppointmentPersonnel].[ScheduleOnWeekends],[dbo].[tblAppointmentPersonnel].[ScheduleOnHolidays],[dbo].[tblAppointmentPersonnel].[StartDate],[dbo].[tblAppointmentPersonnel].[Duration],[dbo].[tblAppointmentPersonnel].[AppointmentPeriod],[dbo].[tblAppointmentPersonnel].[AppointmentPeriodText],[dbo].[tblAppointmentPersonnel].[Description],[dbo].[tblAppointmentPersonnel].[AppointmentTimeInterval],[dbo].[tblAppointmentPersonnel].[AppointmentDayOfTheWeekText],[dbo].[tblAppointmentPersonnel].[AppointmentDayOfTheWeek],[dbo].[tblAppointmentPersonnel].[AppointmentReoccuranceInterval],[dbo].[tblAppointmentPersonnel].[AppointmentOption2Selected],[dbo].[tblAppointmentPersonnel].[AppointmentTimeOptionSelectionText],[dbo].[tblAppointmentPersonnel].[AppointmentTimeOptionSelection],[dbo].[tblAppointmentPersonnel].[AppointmentMonthSelectionText],[dbo].[tblAppointmentPersonnel].[AppointmentMonthSelection],[dbo].[tblAppointmentPersonnel].[AppointmentDayOfTheMonth],[dbo].[tblAppointmentPersonnel].[CreatedDate],[dbo].[tblAppointmentPersonnel].[CreatedBy],[dbo].[tblAppointmentPersonnel].[UpdatedDate],[dbo].[tblAppointmentPersonnel].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAppointmentPersonnel]
            INNER JOIN [track].[tblAppointmentPersonnel] CT
                ON CT.PK_AppointmentPersonnelGuid = [dbo].[tblAppointmentPersonnel].[AppointmentPersonnelGuid]
        WHERE CT.PK_AppointmentPersonnelGuid = @AppointmentPersonnelGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
