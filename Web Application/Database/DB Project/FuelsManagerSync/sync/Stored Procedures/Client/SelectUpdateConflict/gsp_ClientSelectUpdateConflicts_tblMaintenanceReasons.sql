-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMaintenanceReasons
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMaintenanceReasons]
@MaintenanceReasonGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblMaintenanceReasons].[ID],[dbo].[tblMaintenanceReasons].[Description],[dbo].[tblMaintenanceReasons].[CreatedDate],[dbo].[tblMaintenanceReasons].[CreatedBy],[dbo].[tblMaintenanceReasons].[UpdatedDate],[dbo].[tblMaintenanceReasons].[UpdatedBy],[dbo].[tblMaintenanceReasons].[DeletedFlag],[dbo].[tblMaintenanceReasons].[MaintenanceReasonGuid],[dbo].[tblMaintenanceReasons].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblMaintenanceReasons]
            INNER JOIN [track].[tblMaintenanceReasons] CT
                ON CT.PK_MaintenanceReasonGuid = [dbo].[tblMaintenanceReasons].[MaintenanceReasonGuid]
        WHERE CT.PK_MaintenanceReasonGuid = @MaintenanceReasonGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
