-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyAuthorizedCarrierToCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCompanyAuthorizedCarrierToCompany]
@CompanyAuthorizedCarrierToCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyAuthorizedCarrierToCompany].[CompanyAuthorizedCarrierToCompanyGuid],[map].[tblCompanyAuthorizedCarrierToCompany].[CompanyGuid],[map].[tblCompanyAuthorizedCarrierToCompany].[AssignedToCompanyGuid],[map].[tblCompanyAuthorizedCarrierToCompany].[SiteGuid],[map].[tblCompanyAuthorizedCarrierToCompany].[ID],[map].[tblCompanyAuthorizedCarrierToCompany].[CreatedDate],[map].[tblCompanyAuthorizedCarrierToCompany].[CreatedBy],[map].[tblCompanyAuthorizedCarrierToCompany].[UpdatedDate],[map].[tblCompanyAuthorizedCarrierToCompany].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyAuthorizedCarrierToCompany]
            INNER JOIN [track].[tblCompanyAuthorizedCarrierToCompany] CT
                ON CT.PK_CompanyAuthorizedCarrierToCompanyGuid = [map].[tblCompanyAuthorizedCarrierToCompany].[CompanyAuthorizedCarrierToCompanyGuid]
        WHERE CT.PK_CompanyAuthorizedCarrierToCompanyGuid = @CompanyAuthorizedCarrierToCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
