-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyPersonnelAssignedToCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCompanyPersonnelAssignedToCompany]
@CompanyPersonnelAssignedToCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyPersonnelAssignedToCompany].[CompanyPersonnelAssignedToCompanyGuid],[map].[tblCompanyPersonnelAssignedToCompany].[CompanyGuid],[map].[tblCompanyPersonnelAssignedToCompany].[PersonnelGuid],[map].[tblCompanyPersonnelAssignedToCompany].[SiteGuid],[map].[tblCompanyPersonnelAssignedToCompany].[ID],[map].[tblCompanyPersonnelAssignedToCompany].[CreatedDate],[map].[tblCompanyPersonnelAssignedToCompany].[CreatedBy],[map].[tblCompanyPersonnelAssignedToCompany].[UpdatedDate],[map].[tblCompanyPersonnelAssignedToCompany].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyPersonnelAssignedToCompany]
            INNER JOIN [track].[tblCompanyPersonnelAssignedToCompany] CT
                ON CT.PK_CompanyPersonnelAssignedToCompanyGuid = [map].[tblCompanyPersonnelAssignedToCompany].[CompanyPersonnelAssignedToCompanyGuid]
        WHERE CT.PK_CompanyPersonnelAssignedToCompanyGuid = @CompanyPersonnelAssignedToCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
