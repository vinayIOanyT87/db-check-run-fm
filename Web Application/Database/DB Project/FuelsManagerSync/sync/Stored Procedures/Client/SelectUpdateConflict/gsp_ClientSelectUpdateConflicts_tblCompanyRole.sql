-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblCompanyRole
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblCompanyRole]
@CompanyRoleIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblCompanyRole].[CompanyRoleIndex],[lookup].[tblCompanyRole].[CompanyRoleCode],[lookup].[tblCompanyRole].[CompanyRoleName],[lookup].[tblCompanyRole].[CompanyRoleGuid],[lookup].[tblCompanyRole].[CreatedDate],[lookup].[tblCompanyRole].[CreatedBy],[lookup].[tblCompanyRole].[UpdatedDate],[lookup].[tblCompanyRole].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblCompanyRole]
            INNER JOIN [track].[tblCompanyRole] CT
                ON CT.PK_CompanyRoleIndex = [lookup].[tblCompanyRole].[CompanyRoleIndex]
        WHERE CT.PK_CompanyRoleIndex = @CompanyRoleIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
