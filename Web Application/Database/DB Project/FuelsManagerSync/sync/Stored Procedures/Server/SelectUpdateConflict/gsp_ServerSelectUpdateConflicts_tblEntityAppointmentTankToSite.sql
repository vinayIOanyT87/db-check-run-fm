-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAppointmentTankToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityAppointmentTankToSite]
@AppointmentTankToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAppointmentTankToSite].[AppointmentTankToSiteGuid],[map].[tblEntityAppointmentTankToSite].[AppointmentTankGuid],[map].[tblEntityAppointmentTankToSite].[SiteGuid],[map].[tblEntityAppointmentTankToSite].[CreatedDate],[map].[tblEntityAppointmentTankToSite].[CreatedBy],[map].[tblEntityAppointmentTankToSite].[UpdatedDate],[map].[tblEntityAppointmentTankToSite].[UpdatedBy],[map].[tblEntityAppointmentTankToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAppointmentTankToSite]
            INNER JOIN [track].[tblEntityAppointmentTankToSite] CT
                ON CT.PK_AppointmentTankToSiteGuid = [map].[tblEntityAppointmentTankToSite].[AppointmentTankToSiteGuid]
        WHERE CT.PK_AppointmentTankToSiteGuid = @AppointmentTankToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
