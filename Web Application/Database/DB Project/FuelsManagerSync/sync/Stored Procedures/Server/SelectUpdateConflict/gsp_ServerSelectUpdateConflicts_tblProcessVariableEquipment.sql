-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableEquipment
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableEquipment]
@ProcessVariableEquipmentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableEquipment].[ProcessVariableEquipmentGuid],[dbo].[tblProcessVariableEquipment].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableEquipment].[InstanceNumber],[dbo].[tblProcessVariableEquipment].[EquipmentGuid],[dbo].[tblProcessVariableEquipment].[OPCConnectionGuid],[dbo].[tblProcessVariableEquipment].[OPCItemID],[dbo].[tblProcessVariableEquipment].[DataType],[dbo].[tblProcessVariableEquipment].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableEquipment].[Quality],[dbo].[tblProcessVariableEquipment].[SIValue],[dbo].[tblProcessVariableEquipment].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableEquipment].[DateTimeStamp],[dbo].[tblProcessVariableEquipment].[Maximum],[dbo].[tblProcessVariableEquipment].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableEquipment].[Minimum],[dbo].[tblProcessVariableEquipment].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableEquipment].[DataTypeEnabled],[dbo].[tblProcessVariableEquipment].[Input],[dbo].[tblProcessVariableEquipment].[InputEnabled],[dbo].[tblProcessVariableEquipment].[MessageApplicationStringGuid],[dbo].[tblProcessVariableEquipment].[CreatedDate],[dbo].[tblProcessVariableEquipment].[CreatedBy],[dbo].[tblProcessVariableEquipment].[UpdatedDate],[dbo].[tblProcessVariableEquipment].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableEquipment]
            INNER JOIN [track].[tblProcessVariableEquipment] CT
                ON CT.PK_ProcessVariableEquipmentGuid = [dbo].[tblProcessVariableEquipment].[ProcessVariableEquipmentGuid]
        WHERE CT.PK_ProcessVariableEquipmentGuid = @ProcessVariableEquipmentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
