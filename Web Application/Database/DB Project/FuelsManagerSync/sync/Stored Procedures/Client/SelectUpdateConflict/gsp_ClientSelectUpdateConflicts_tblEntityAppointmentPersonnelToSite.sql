-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAppointmentPersonnelToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityAppointmentPersonnelToSite]
@AppointmentPersonnelToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAppointmentPersonnelToSite].[AppointmentPersonnelToSiteGuid],[map].[tblEntityAppointmentPersonnelToSite].[AppointmentPersonnelGuid],[map].[tblEntityAppointmentPersonnelToSite].[SiteGuid],[map].[tblEntityAppointmentPersonnelToSite].[CreatedDate],[map].[tblEntityAppointmentPersonnelToSite].[CreatedBy],[map].[tblEntityAppointmentPersonnelToSite].[UpdatedDate],[map].[tblEntityAppointmentPersonnelToSite].[UpdatedBy],[map].[tblEntityAppointmentPersonnelToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAppointmentPersonnelToSite]
            INNER JOIN [track].[tblEntityAppointmentPersonnelToSite] CT
                ON CT.PK_AppointmentPersonnelToSiteGuid = [map].[tblEntityAppointmentPersonnelToSite].[AppointmentPersonnelToSiteGuid]
        WHERE CT.PK_AppointmentPersonnelToSiteGuid = @AppointmentPersonnelToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
