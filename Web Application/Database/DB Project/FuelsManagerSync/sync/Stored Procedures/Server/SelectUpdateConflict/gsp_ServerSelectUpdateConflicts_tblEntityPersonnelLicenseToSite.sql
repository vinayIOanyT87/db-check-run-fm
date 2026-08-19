-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityPersonnelLicenseToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityPersonnelLicenseToSite]
@PersonnelLicenseToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityPersonnelLicenseToSite].[QualificationGuid],[map].[tblEntityPersonnelLicenseToSite].[SiteGuid],[map].[tblEntityPersonnelLicenseToSite].[CreatedDate],[map].[tblEntityPersonnelLicenseToSite].[CreatedBy],[map].[tblEntityPersonnelLicenseToSite].[UpdatedDate],[map].[tblEntityPersonnelLicenseToSite].[UpdatedBy],[map].[tblEntityPersonnelLicenseToSite].[PersonnelLicenseToSiteGuid],[map].[tblEntityPersonnelLicenseToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityPersonnelLicenseToSite]
            INNER JOIN [track].[tblEntityPersonnelLicenseToSite] CT
                ON CT.PK_PersonnelLicenseToSiteGuid = [map].[tblEntityPersonnelLicenseToSite].[PersonnelLicenseToSiteGuid]
        WHERE CT.PK_PersonnelLicenseToSiteGuid = @PersonnelLicenseToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
