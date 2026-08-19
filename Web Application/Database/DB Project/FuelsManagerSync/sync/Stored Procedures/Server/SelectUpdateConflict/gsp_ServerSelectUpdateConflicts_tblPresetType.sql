-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblPresetType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPresetType]
@PresetTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblPresetType].[PresetTypeIndex],[lookup].[tblPresetType].[PresetTypeCode],[lookup].[tblPresetType].[PresetTypeName],[lookup].[tblPresetType].[PresetTypeGuid],[lookup].[tblPresetType].[CreatedDate],[lookup].[tblPresetType].[CreatedBy],[lookup].[tblPresetType].[UpdatedDate],[lookup].[tblPresetType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblPresetType]
            INNER JOIN [track].[tblPresetType] CT
                ON CT.PK_PresetTypeIndex = [lookup].[tblPresetType].[PresetTypeIndex]
        WHERE CT.PK_PresetTypeIndex = @PresetTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
