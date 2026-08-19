-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAppointmentTank
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAppointmentTank]
@AppointmentTankGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAppointmentTank].[AppointmentTankGuid],[dbo].[tblAppointmentTank].[TankGuid],[dbo].[tblAppointmentTank].[TestSetDefinitionGuid],[dbo].[tblAppointmentTank].[SiteGuid],[dbo].[tblAppointmentTank].[AssetText],[dbo].[tblAppointmentTank].[AppointmentCategory],[dbo].[tblAppointmentTank].[AppointmentIsSingle],[dbo].[tblAppointmentTank].[ScheduleOnWeekends],[dbo].[tblAppointmentTank].[ScheduleOnHolidays],[dbo].[tblAppointmentTank].[StartDate],[dbo].[tblAppointmentTank].[Duration],[dbo].[tblAppointmentTank].[AppointmentPeriod],[dbo].[tblAppointmentTank].[AppointmentPeriodText],[dbo].[tblAppointmentTank].[Description],[dbo].[tblAppointmentTank].[AppointmentTimeInterval],[dbo].[tblAppointmentTank].[AppointmentDayOfTheWeekText],[dbo].[tblAppointmentTank].[AppointmentDayOfTheWeek],[dbo].[tblAppointmentTank].[AppointmentReoccuranceInterval],[dbo].[tblAppointmentTank].[AppointmentOption2Selected],[dbo].[tblAppointmentTank].[AppointmentTimeOptionSelectionText],[dbo].[tblAppointmentTank].[AppointmentTimeOptionSelection],[dbo].[tblAppointmentTank].[AppointmentMonthSelectionText],[dbo].[tblAppointmentTank].[AppointmentMonthSelection],[dbo].[tblAppointmentTank].[AppointmentDayOfTheMonth],[dbo].[tblAppointmentTank].[CreatedDate],[dbo].[tblAppointmentTank].[CreatedBy],[dbo].[tblAppointmentTank].[UpdatedDate],[dbo].[tblAppointmentTank].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAppointmentTank]
            INNER JOIN [track].[tblAppointmentTank] CT
                ON CT.PK_AppointmentTankGuid = [dbo].[tblAppointmentTank].[AppointmentTankGuid]
        WHERE CT.PK_AppointmentTankGuid = @AppointmentTankGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
