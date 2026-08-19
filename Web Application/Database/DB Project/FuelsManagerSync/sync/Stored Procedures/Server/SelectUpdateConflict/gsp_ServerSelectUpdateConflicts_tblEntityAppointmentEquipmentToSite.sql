-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAppointmentEquipmentToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityAppointmentEquipmentToSite]
@AppointmentEquipmentToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAppointmentEquipmentToSite].[AppointmentEquipmentToSiteGuid],[map].[tblEntityAppointmentEquipmentToSite].[AppointmentEquipmentGuid],[map].[tblEntityAppointmentEquipmentToSite].[SiteGuid],[map].[tblEntityAppointmentEquipmentToSite].[CreatedDate],[map].[tblEntityAppointmentEquipmentToSite].[CreatedBy],[map].[tblEntityAppointmentEquipmentToSite].[UpdatedDate],[map].[tblEntityAppointmentEquipmentToSite].[UpdatedBy],[map].[tblEntityAppointmentEquipmentToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAppointmentEquipmentToSite]
            INNER JOIN [track].[tblEntityAppointmentEquipmentToSite] CT
                ON CT.PK_AppointmentEquipmentToSiteGuid = [map].[tblEntityAppointmentEquipmentToSite].[AppointmentEquipmentToSiteGuid]
        WHERE CT.PK_AppointmentEquipmentToSiteGuid = @AppointmentEquipmentToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
