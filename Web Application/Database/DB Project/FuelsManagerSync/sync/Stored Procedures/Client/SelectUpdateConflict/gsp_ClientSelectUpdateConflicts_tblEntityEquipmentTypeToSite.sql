-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityEquipmentTypeToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityEquipmentTypeToSite]
@EquipmentTypeToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityEquipmentTypeToSite].[EquipmentTypeToSiteGuid],[map].[tblEntityEquipmentTypeToSite].[EquipmentTypeGuid],[map].[tblEntityEquipmentTypeToSite].[SiteGuid],[map].[tblEntityEquipmentTypeToSite].[CreatedDate],[map].[tblEntityEquipmentTypeToSite].[CreatedBy],[map].[tblEntityEquipmentTypeToSite].[UpdatedDate],[map].[tblEntityEquipmentTypeToSite].[UpdatedBy],[map].[tblEntityEquipmentTypeToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityEquipmentTypeToSite]
            INNER JOIN [track].[tblEntityEquipmentTypeToSite] CT
                ON CT.PK_EquipmentTypeToSiteGuid = [map].[tblEntityEquipmentTypeToSite].[EquipmentTypeToSiteGuid]
        WHERE CT.PK_EquipmentTypeToSiteGuid = @EquipmentTypeToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
