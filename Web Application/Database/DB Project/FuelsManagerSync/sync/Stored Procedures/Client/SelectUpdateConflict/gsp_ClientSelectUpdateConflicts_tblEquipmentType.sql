-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblEquipmentType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEquipmentType]
@EquipmentTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblEquipmentType].[EquipmentTypeIndex],[lookup].[tblEquipmentType].[EquipmentTypeCode],[lookup].[tblEquipmentType].[EquipmentTypeName],[lookup].[tblEquipmentType].[EquipmentTypeGuid],[lookup].[tblEquipmentType].[CreatedDate],[lookup].[tblEquipmentType].[CreatedBy],[lookup].[tblEquipmentType].[UpdatedDate],[lookup].[tblEquipmentType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblEquipmentType]
            INNER JOIN [track].[tblEquipmentType] CT
                ON CT.PK_EquipmentTypeIndex = [lookup].[tblEquipmentType].[EquipmentTypeIndex]
        WHERE CT.PK_EquipmentTypeIndex = @EquipmentTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
