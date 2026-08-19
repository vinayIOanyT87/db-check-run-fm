-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanySupplierToOwner
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCompanySupplierToOwner]
@CompanySupplierToOwnerGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanySupplierToOwner].[CompanySupplierToOwnerGuid],[map].[tblCompanySupplierToOwner].[CompanyGuid],[map].[tblCompanySupplierToOwner].[CompanyOffLoadOwnerToManagerGuid],[map].[tblCompanySupplierToOwner].[SiteGuid],[map].[tblCompanySupplierToOwner].[ID],[map].[tblCompanySupplierToOwner].[CreatedDate],[map].[tblCompanySupplierToOwner].[CreatedBy],[map].[tblCompanySupplierToOwner].[UpdatedDate],[map].[tblCompanySupplierToOwner].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanySupplierToOwner]
            INNER JOIN [track].[tblCompanySupplierToOwner] CT
                ON CT.PK_CompanySupplierToOwnerGuid = [map].[tblCompanySupplierToOwner].[CompanySupplierToOwnerGuid]
        WHERE CT.PK_CompanySupplierToOwnerGuid = @CompanySupplierToOwnerGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
