-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblCompanyBillToToShipper
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblCompanyBillToToShipper]
@CompanyBillToToShipperGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblCompanyBillToToShipper].[CompanyBillToToShipperGuid],[map].[tblCompanyBillToToShipper].[CompanyGuid],[map].[tblCompanyBillToToShipper].[CompanyShipperToOwnerGuid],[map].[tblCompanyBillToToShipper].[SiteGuid],[map].[tblCompanyBillToToShipper].[ID],[map].[tblCompanyBillToToShipper].[CreatedDate],[map].[tblCompanyBillToToShipper].[CreatedBy],[map].[tblCompanyBillToToShipper].[UpdatedDate],[map].[tblCompanyBillToToShipper].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblCompanyBillToToShipper]
            INNER JOIN [track].[tblCompanyBillToToShipper] CT
                ON CT.PK_CompanyBillToToShipperGuid = [map].[tblCompanyBillToToShipper].[CompanyBillToToShipperGuid]
        WHERE CT.PK_CompanyBillToToShipperGuid = @CompanyBillToToShipperGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
