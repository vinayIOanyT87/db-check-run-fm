-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityMaintenanceReasonToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityMaintenanceReasonToSite]
@MaintenanceReasonToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityMaintenanceReasonToSite].[MaintenanceReasonToSiteGuid],[map].[tblEntityMaintenanceReasonToSite].[SiteGuid],[map].[tblEntityMaintenanceReasonToSite].[MaintenanceReasonGuid],[map].[tblEntityMaintenanceReasonToSite].[CreatedDate],[map].[tblEntityMaintenanceReasonToSite].[CreatedBy],[map].[tblEntityMaintenanceReasonToSite].[UpdatedDate],[map].[tblEntityMaintenanceReasonToSite].[UpdatedBy],[map].[tblEntityMaintenanceReasonToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityMaintenanceReasonToSite]
            INNER JOIN [track].[tblEntityMaintenanceReasonToSite] CT
                ON CT.PK_MaintenanceReasonToSiteGuid = [map].[tblEntityMaintenanceReasonToSite].[MaintenanceReasonToSiteGuid]
        WHERE CT.PK_MaintenanceReasonToSiteGuid = @MaintenanceReasonToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
