-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityCompanyGroupToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityCompanyGroupToSite]
@CompanyGroupToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityCompanyGroupToSite].[CompanyGroupToSiteGuid],[map].[tblEntityCompanyGroupToSite].[ApplicationStringGuid],[map].[tblEntityCompanyGroupToSite].[SiteGuid],[map].[tblEntityCompanyGroupToSite].[CreatedDate],[map].[tblEntityCompanyGroupToSite].[CreatedBy],[map].[tblEntityCompanyGroupToSite].[UpdatedDate],[map].[tblEntityCompanyGroupToSite].[UpdatedBy],[map].[tblEntityCompanyGroupToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityCompanyGroupToSite]
            INNER JOIN [track].[tblEntityCompanyGroupToSite] CT
                ON CT.PK_CompanyGroupToSiteGuid = [map].[tblEntityCompanyGroupToSite].[CompanyGroupToSiteGuid]
        WHERE CT.PK_CompanyGroupToSiteGuid = @CompanyGroupToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
