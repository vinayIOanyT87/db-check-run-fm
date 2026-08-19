-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTankMaintenanceLog
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTankMaintenanceLog]
@TankMaintenanceLogGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTankMaintenanceLog].[TankID],[dbo].[tblTankMaintenanceLog].[VesselType],[dbo].[tblTankMaintenanceLog].[OperatorID],[dbo].[tblTankMaintenanceLog].[MaintenanceReason],[dbo].[tblTankMaintenanceLog].[InServiceFlag],[dbo].[tblTankMaintenanceLog].[ChangeDate],[dbo].[tblTankMaintenanceLog].[EstReturnToServiceDate],[dbo].[tblTankMaintenanceLog].[WorkOrder],[dbo].[tblTankMaintenanceLog].[Memo],[dbo].[tblTankMaintenanceLog].[CreatedDate],[dbo].[tblTankMaintenanceLog].[CreatedBy],[dbo].[tblTankMaintenanceLog].[UpdatedDate],[dbo].[tblTankMaintenanceLog].[UpdatedBy],[dbo].[tblTankMaintenanceLog].[TankMaintenanceLogGuid],[dbo].[tblTankMaintenanceLog].[SiteGuid],[dbo].[tblTankMaintenanceLog].[LookupVesselTypeIndex],[dbo].[tblTankMaintenanceLog].[MaintenanceReasonGuid],[dbo].[tblTankMaintenanceLog].[OperatorPersonnelGuid],[dbo].[tblTankMaintenanceLog].[TankGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTankMaintenanceLog]
            INNER JOIN [track].[tblTankMaintenanceLog] CT
                ON CT.PK_TankMaintenanceLogGuid = [dbo].[tblTankMaintenanceLog].[TankMaintenanceLogGuid]
        WHERE CT.PK_TankMaintenanceLogGuid = @TankMaintenanceLogGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
