-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyCompanyToCompanyGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblCompanyCompanyToCompanyGroup]
@CompanyCompanyToCompanyGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyCompanyToCompanyGroup].[CompanyCompanyToCompanyGroupGuid],[map].[tblCompanyCompanyToCompanyGroup].[CompanyGuid],[map].[tblCompanyCompanyToCompanyGroup].[ApplicationStringGuid],[map].[tblCompanyCompanyToCompanyGroup].[SiteGuid],[map].[tblCompanyCompanyToCompanyGroup].[ID],[map].[tblCompanyCompanyToCompanyGroup].[CreatedDate],[map].[tblCompanyCompanyToCompanyGroup].[CreatedBy],[map].[tblCompanyCompanyToCompanyGroup].[UpdatedDate],[map].[tblCompanyCompanyToCompanyGroup].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyCompanyToCompanyGroup]
            INNER JOIN [track].[tblCompanyCompanyToCompanyGroup] CT
                ON CT.PK_CompanyCompanyToCompanyGroupGuid = [map].[tblCompanyCompanyToCompanyGroup].[CompanyCompanyToCompanyGroupGuid]
        WHERE CT.PK_CompanyCompanyToCompanyGroupGuid = @CompanyCompanyToCompanyGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
