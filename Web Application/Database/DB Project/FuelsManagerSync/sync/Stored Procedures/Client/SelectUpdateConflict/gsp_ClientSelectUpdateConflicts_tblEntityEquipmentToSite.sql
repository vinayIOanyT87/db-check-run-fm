-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityEquipmentToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityEquipmentToSite]
@EquipmentToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityEquipmentToSite].[EquipmentToSiteGuid],[map].[tblEntityEquipmentToSite].[EquipmentGuid],[map].[tblEntityEquipmentToSite].[SiteGuid],[map].[tblEntityEquipmentToSite].[CreatedDate],[map].[tblEntityEquipmentToSite].[CreatedBy],[map].[tblEntityEquipmentToSite].[UpdatedDate],[map].[tblEntityEquipmentToSite].[UpdatedBy],[map].[tblEntityEquipmentToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityEquipmentToSite]
            INNER JOIN [track].[tblEntityEquipmentToSite] CT
                ON CT.PK_EquipmentToSiteGuid = [map].[tblEntityEquipmentToSite].[EquipmentToSiteGuid]
        WHERE CT.PK_EquipmentToSiteGuid = @EquipmentToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
