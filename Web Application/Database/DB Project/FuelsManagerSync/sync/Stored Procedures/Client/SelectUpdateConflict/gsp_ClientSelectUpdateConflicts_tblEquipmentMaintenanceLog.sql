-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEquipmentMaintenanceLog
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEquipmentMaintenanceLog]
@EquipmentMaintenanceLogGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblEquipmentMaintenanceLog].[EquipmentID],[dbo].[tblEquipmentMaintenanceLog].[EquipmentType],[dbo].[tblEquipmentMaintenanceLog].[OperatorID],[dbo].[tblEquipmentMaintenanceLog].[MaintenanceReason],[dbo].[tblEquipmentMaintenanceLog].[InServiceFlag],[dbo].[tblEquipmentMaintenanceLog].[ChangeDate],[dbo].[tblEquipmentMaintenanceLog].[EstReturnToServiceDate],[dbo].[tblEquipmentMaintenanceLog].[WorkOrder],[dbo].[tblEquipmentMaintenanceLog].[Memo],[dbo].[tblEquipmentMaintenanceLog].[CreatedDate],[dbo].[tblEquipmentMaintenanceLog].[CreatedBy],[dbo].[tblEquipmentMaintenanceLog].[UpdatedDate],[dbo].[tblEquipmentMaintenanceLog].[UpdatedBy],[dbo].[tblEquipmentMaintenanceLog].[EquipmentMaintenanceLogGuid],[dbo].[tblEquipmentMaintenanceLog].[SiteGuid],[dbo].[tblEquipmentMaintenanceLog].[EquipmentGuid],[dbo].[tblEquipmentMaintenanceLog].[MaintenanceReasonGuid],[dbo].[tblEquipmentMaintenanceLog].[OperatorPersonnelGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblEquipmentMaintenanceLog]
            INNER JOIN [track].[tblEquipmentMaintenanceLog] CT
                ON CT.PK_EquipmentMaintenanceLogGuid = [dbo].[tblEquipmentMaintenanceLog].[EquipmentMaintenanceLogGuid]
        WHERE CT.PK_EquipmentMaintenanceLogGuid = @EquipmentMaintenanceLogGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
