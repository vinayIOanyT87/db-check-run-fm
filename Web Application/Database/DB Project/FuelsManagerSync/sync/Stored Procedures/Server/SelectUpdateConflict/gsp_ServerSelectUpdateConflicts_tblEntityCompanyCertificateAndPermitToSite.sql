-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityCompanyCertificateAndPermitToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityCompanyCertificateAndPermitToSite]
@CompanyCertificateAndPermitToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityCompanyCertificateAndPermitToSite].[CompanyCertificateAndPermitToSiteGuid],[map].[tblEntityCompanyCertificateAndPermitToSite].[QualificationGuid],[map].[tblEntityCompanyCertificateAndPermitToSite].[SiteGuid],[map].[tblEntityCompanyCertificateAndPermitToSite].[CreatedDate],[map].[tblEntityCompanyCertificateAndPermitToSite].[CreatedBy],[map].[tblEntityCompanyCertificateAndPermitToSite].[UpdatedDate],[map].[tblEntityCompanyCertificateAndPermitToSite].[UpdatedBy],[map].[tblEntityCompanyCertificateAndPermitToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityCompanyCertificateAndPermitToSite]
            INNER JOIN [track].[tblEntityCompanyCertificateAndPermitToSite] CT
                ON CT.PK_CompanyCertificateAndPermitToSiteGuid = [map].[tblEntityCompanyCertificateAndPermitToSite].[CompanyCertificateAndPermitToSiteGuid]
        WHERE CT.PK_CompanyCertificateAndPermitToSiteGuid = @CompanyCertificateAndPermitToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
