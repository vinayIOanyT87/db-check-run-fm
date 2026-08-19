-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityMobileDeviceProfileToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityMobileDeviceProfileToSite]
@MobileDeviceProfileToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileToSiteGuid],[map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileGuid],[map].[tblEntityMobileDeviceProfileToSite].[SiteGuid],[map].[tblEntityMobileDeviceProfileToSite].[CreatedDate],[map].[tblEntityMobileDeviceProfileToSite].[CreatedBy],[map].[tblEntityMobileDeviceProfileToSite].[UpdatedDate],[map].[tblEntityMobileDeviceProfileToSite].[UpdatedBy],[map].[tblEntityMobileDeviceProfileToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityMobileDeviceProfileToSite]
            INNER JOIN [track].[tblEntityMobileDeviceProfileToSite] CT
                ON CT.PK_MobileDeviceProfileToSiteGuid = [map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileToSiteGuid]
        WHERE CT.PK_MobileDeviceProfileToSiteGuid = @MobileDeviceProfileToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
