-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyToRole
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblCompanyToRole]
@CompanyToRoleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyToRole].[CompanyToRoleGuid],[map].[tblCompanyToRole].[CompanyGuid],[map].[tblCompanyToRole].[LookupCompanyRoleIndex],[map].[tblCompanyToRole].[SiteGuid],[map].[tblCompanyToRole].[CreatedDate],[map].[tblCompanyToRole].[CreatedBy],[map].[tblCompanyToRole].[UpdatedDate],[map].[tblCompanyToRole].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyToRole]
            INNER JOIN [track].[tblCompanyToRole] CT
                ON CT.PK_CompanyToRoleGuid = [map].[tblCompanyToRole].[CompanyToRoleGuid]
        WHERE CT.PK_CompanyToRoleGuid = @CompanyToRoleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
