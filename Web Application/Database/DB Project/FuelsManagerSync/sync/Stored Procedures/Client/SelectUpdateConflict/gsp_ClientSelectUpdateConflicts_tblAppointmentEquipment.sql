-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAppointmentEquipment
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAppointmentEquipment]
@AppointmentEquipmentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAppointmentEquipment].[AppointmentEquipmentGuid],[dbo].[tblAppointmentEquipment].[EquipmentGuid],[dbo].[tblAppointmentEquipment].[TestSetDefinitionGuid],[dbo].[tblAppointmentEquipment].[SiteGuid],[dbo].[tblAppointmentEquipment].[AssetText],[dbo].[tblAppointmentEquipment].[AppointmentCategory],[dbo].[tblAppointmentEquipment].[AppointmentIsSingle],[dbo].[tblAppointmentEquipment].[ScheduleOnWeekends],[dbo].[tblAppointmentEquipment].[ScheduleOnHolidays],[dbo].[tblAppointmentEquipment].[StartDate],[dbo].[tblAppointmentEquipment].[Duration],[dbo].[tblAppointmentEquipment].[AppointmentPeriod],[dbo].[tblAppointmentEquipment].[AppointmentPeriodText],[dbo].[tblAppointmentEquipment].[Description],[dbo].[tblAppointmentEquipment].[AppointmentTimeInterval],[dbo].[tblAppointmentEquipment].[AppointmentDayOfTheWeekText],[dbo].[tblAppointmentEquipment].[AppointmentDayOfTheWeek],[dbo].[tblAppointmentEquipment].[AppointmentReoccuranceInterval],[dbo].[tblAppointmentEquipment].[AppointmentOption2Selected],[dbo].[tblAppointmentEquipment].[AppointmentTimeOptionSelectionText],[dbo].[tblAppointmentEquipment].[AppointmentTimeOptionSelection],[dbo].[tblAppointmentEquipment].[AppointmentMonthSelectionText],[dbo].[tblAppointmentEquipment].[AppointmentMonthSelection],[dbo].[tblAppointmentEquipment].[AppointmentDayOfTheMonth],[dbo].[tblAppointmentEquipment].[CreatedDate],[dbo].[tblAppointmentEquipment].[CreatedBy],[dbo].[tblAppointmentEquipment].[UpdatedDate],[dbo].[tblAppointmentEquipment].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAppointmentEquipment]
            INNER JOIN [track].[tblAppointmentEquipment] CT
                ON CT.PK_AppointmentEquipmentGuid = [dbo].[tblAppointmentEquipment].[AppointmentEquipmentGuid]
        WHERE CT.PK_AppointmentEquipmentGuid = @AppointmentEquipmentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
