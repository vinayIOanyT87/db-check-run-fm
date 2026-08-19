-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblMeterToEquipment
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMeterToEquipment]
@MeterToEquipmentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblMeterToEquipment].[MeterToEquipmentGuid],[map].[tblMeterToEquipment].[MeterGuid],[map].[tblMeterToEquipment].[EquipmentGuid],[map].[tblMeterToEquipment].[CreatedDate],[map].[tblMeterToEquipment].[CreatedBy],[map].[tblMeterToEquipment].[UpdatedDate],[map].[tblMeterToEquipment].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblMeterToEquipment]
            INNER JOIN [track].[tblMeterToEquipment] CT
                ON CT.PK_MeterToEquipmentGuid = [map].[tblMeterToEquipment].[MeterToEquipmentGuid]
        WHERE CT.PK_MeterToEquipmentGuid = @MeterToEquipmentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
