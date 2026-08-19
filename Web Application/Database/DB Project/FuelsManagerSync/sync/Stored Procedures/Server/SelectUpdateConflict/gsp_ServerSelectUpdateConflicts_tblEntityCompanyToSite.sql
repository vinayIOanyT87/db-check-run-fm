-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityCompanyToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityCompanyToSite]
@CompanyToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityCompanyToSite].[CompanyToSiteGuid],[map].[tblEntityCompanyToSite].[CompanyGuid],[map].[tblEntityCompanyToSite].[SiteGuid],[map].[tblEntityCompanyToSite].[CreatedDate],[map].[tblEntityCompanyToSite].[CreatedBy],[map].[tblEntityCompanyToSite].[UpdatedDate],[map].[tblEntityCompanyToSite].[UpdatedBy],[map].[tblEntityCompanyToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityCompanyToSite]
            INNER JOIN [track].[tblEntityCompanyToSite] CT
                ON CT.PK_CompanyToSiteGuid = [map].[tblEntityCompanyToSite].[CompanyToSiteGuid]
        WHERE CT.PK_CompanyToSiteGuid = @CompanyToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
