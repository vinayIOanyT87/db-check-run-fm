-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityEquipmentTagAndLicenseToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityEquipmentTagAndLicenseToSite]
@EquipmentTagAndLicenseToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityEquipmentTagAndLicenseToSite].[EquipmentTagAndLicenseToSiteGuid],[map].[tblEntityEquipmentTagAndLicenseToSite].[QualificationGuid],[map].[tblEntityEquipmentTagAndLicenseToSite].[SiteGuid],[map].[tblEntityEquipmentTagAndLicenseToSite].[CreatedDate],[map].[tblEntityEquipmentTagAndLicenseToSite].[CreatedBy],[map].[tblEntityEquipmentTagAndLicenseToSite].[UpdatedDate],[map].[tblEntityEquipmentTagAndLicenseToSite].[UpdatedBy],[map].[tblEntityEquipmentTagAndLicenseToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityEquipmentTagAndLicenseToSite]
            INNER JOIN [track].[tblEntityEquipmentTagAndLicenseToSite] CT
                ON CT.PK_EquipmentTagAndLicenseToSiteGuid = [map].[tblEntityEquipmentTagAndLicenseToSite].[EquipmentTagAndLicenseToSiteGuid]
        WHERE CT.PK_EquipmentTagAndLicenseToSiteGuid = @EquipmentTagAndLicenseToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
