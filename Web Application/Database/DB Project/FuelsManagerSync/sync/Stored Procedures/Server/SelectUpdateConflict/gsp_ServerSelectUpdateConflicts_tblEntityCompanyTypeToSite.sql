-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityCompanyTypeToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityCompanyTypeToSite]
@CompanyTypeToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityCompanyTypeToSite].[CompanyTypeToSiteGuid],[map].[tblEntityCompanyTypeToSite].[ApplicationStringGuid],[map].[tblEntityCompanyTypeToSite].[SiteGuid],[map].[tblEntityCompanyTypeToSite].[CreatedDate],[map].[tblEntityCompanyTypeToSite].[CreatedBy],[map].[tblEntityCompanyTypeToSite].[UpdatedDate],[map].[tblEntityCompanyTypeToSite].[UpdatedBy],[map].[tblEntityCompanyTypeToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityCompanyTypeToSite]
            INNER JOIN [track].[tblEntityCompanyTypeToSite] CT
                ON CT.PK_CompanyTypeToSiteGuid = [map].[tblEntityCompanyTypeToSite].[CompanyTypeToSiteGuid]
        WHERE CT.PK_CompanyTypeToSiteGuid = @CompanyTypeToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
