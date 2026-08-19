-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEquipmentQualityTagLog
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEquipmentQualityTagLog]
@EquipmentQualityTagLogGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblEquipmentQualityTagLog].[QualityTagName],[dbo].[tblEquipmentQualityTagLog].[EquipmentID],[dbo].[tblEquipmentQualityTagLog].[EquipmentType],[dbo].[tblEquipmentQualityTagLog].[TaggedDate],[dbo].[tblEquipmentQualityTagLog].[TaggedBy],[dbo].[tblEquipmentQualityTagLog].[Memo],[dbo].[tblEquipmentQualityTagLog].[RemovedDate],[dbo].[tblEquipmentQualityTagLog].[RemovedBy],[dbo].[tblEquipmentQualityTagLog].[DeleteFlag],[dbo].[tblEquipmentQualityTagLog].[CreatedDate],[dbo].[tblEquipmentQualityTagLog].[CreatedBy],[dbo].[tblEquipmentQualityTagLog].[UpdatedDate],[dbo].[tblEquipmentQualityTagLog].[UpdatedBy],[dbo].[tblEquipmentQualityTagLog].[TagNumber],[dbo].[tblEquipmentQualityTagLog].[EquipmentQualityTagLogGuid],[dbo].[tblEquipmentQualityTagLog].[SiteGuid],[dbo].[tblEquipmentQualityTagLog].[EquipmentGuid],[dbo].[tblEquipmentQualityTagLog].[QualityTagGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblEquipmentQualityTagLog]
            INNER JOIN [track].[tblEquipmentQualityTagLog] CT
                ON CT.PK_EquipmentQualityTagLogGuid = [dbo].[tblEquipmentQualityTagLog].[EquipmentQualityTagLogGuid]
        WHERE CT.PK_EquipmentQualityTagLogGuid = @EquipmentQualityTagLogGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
