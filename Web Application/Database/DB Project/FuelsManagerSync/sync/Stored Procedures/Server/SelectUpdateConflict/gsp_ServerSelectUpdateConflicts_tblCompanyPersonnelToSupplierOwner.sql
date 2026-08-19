-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyPersonnelToSupplierOwner
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCompanyPersonnelToSupplierOwner]
@CompanyPersonnelToSupplierOwnerGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyPersonnelToSupplierOwner].[CompanyPersonnelToSupplierOwnerGuid],[map].[tblCompanyPersonnelToSupplierOwner].[PersonnelGuid],[map].[tblCompanyPersonnelToSupplierOwner].[CompanySupplierToOwnerGuid],[map].[tblCompanyPersonnelToSupplierOwner].[SiteGuid],[map].[tblCompanyPersonnelToSupplierOwner].[ID],[map].[tblCompanyPersonnelToSupplierOwner].[CreatedDate],[map].[tblCompanyPersonnelToSupplierOwner].[CreatedBy],[map].[tblCompanyPersonnelToSupplierOwner].[UpdatedDate],[map].[tblCompanyPersonnelToSupplierOwner].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyPersonnelToSupplierOwner]
            INNER JOIN [track].[tblCompanyPersonnelToSupplierOwner] CT
                ON CT.PK_CompanyPersonnelToSupplierOwnerGuid = [map].[tblCompanyPersonnelToSupplierOwner].[CompanyPersonnelToSupplierOwnerGuid]
        WHERE CT.PK_CompanyPersonnelToSupplierOwnerGuid = @CompanyPersonnelToSupplierOwnerGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
