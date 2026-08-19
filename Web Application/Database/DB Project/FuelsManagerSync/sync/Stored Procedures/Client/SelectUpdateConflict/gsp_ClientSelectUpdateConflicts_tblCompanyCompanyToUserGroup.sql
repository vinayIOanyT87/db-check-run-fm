-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyCompanyToUserGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblCompanyCompanyToUserGroup]
@CompanyCompanyToUserGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyCompanyToUserGroup].[CompanyCompanyToUserGroupGuid],[map].[tblCompanyCompanyToUserGroup].[CompanyGuid],[map].[tblCompanyCompanyToUserGroup].[GroupGuid],[map].[tblCompanyCompanyToUserGroup].[SiteGuid],[map].[tblCompanyCompanyToUserGroup].[ID],[map].[tblCompanyCompanyToUserGroup].[CreatedDate],[map].[tblCompanyCompanyToUserGroup].[CreatedBy],[map].[tblCompanyCompanyToUserGroup].[UpdatedDate],[map].[tblCompanyCompanyToUserGroup].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyCompanyToUserGroup]
            INNER JOIN [track].[tblCompanyCompanyToUserGroup] CT
                ON CT.PK_CompanyCompanyToUserGroupGuid = [map].[tblCompanyCompanyToUserGroup].[CompanyCompanyToUserGroupGuid]
        WHERE CT.PK_CompanyCompanyToUserGroupGuid = @CompanyCompanyToUserGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
