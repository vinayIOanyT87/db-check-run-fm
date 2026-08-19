-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityEquipmentTestAndInspectionToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityEquipmentTestAndInspectionToSite]
@EquipmentTestAndInspectionToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityEquipmentTestAndInspectionToSite].[EquipmentTestAndInspectionToSiteGuid],[map].[tblEntityEquipmentTestAndInspectionToSite].[QualificationGuid],[map].[tblEntityEquipmentTestAndInspectionToSite].[SiteGuid],[map].[tblEntityEquipmentTestAndInspectionToSite].[CreatedDate],[map].[tblEntityEquipmentTestAndInspectionToSite].[CreatedBy],[map].[tblEntityEquipmentTestAndInspectionToSite].[UpdatedDate],[map].[tblEntityEquipmentTestAndInspectionToSite].[UpdatedBy],[map].[tblEntityEquipmentTestAndInspectionToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityEquipmentTestAndInspectionToSite]
            INNER JOIN [track].[tblEntityEquipmentTestAndInspectionToSite] CT
                ON CT.PK_EquipmentTestAndInspectionToSiteGuid = [map].[tblEntityEquipmentTestAndInspectionToSite].[EquipmentTestAndInspectionToSiteGuid]
        WHERE CT.PK_EquipmentTestAndInspectionToSiteGuid = @EquipmentTestAndInspectionToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
