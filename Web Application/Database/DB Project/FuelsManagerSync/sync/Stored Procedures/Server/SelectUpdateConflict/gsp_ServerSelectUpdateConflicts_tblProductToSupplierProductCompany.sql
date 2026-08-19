-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToSupplierProductCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProductToSupplierProductCompany]
@ProductToSupplierProductCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToSupplierProductCompany].[ProductToSupplierProductCompanyGuid],[map].[tblProductToSupplierProductCompany].[ProductGuid],[map].[tblProductToSupplierProductCompany].[AssignedToCompanyGuid],[map].[tblProductToSupplierProductCompany].[Sequence],[map].[tblProductToSupplierProductCompany].[BlendPercentage],[map].[tblProductToSupplierProductCompany].[AdditiveRate],[map].[tblProductToSupplierProductCompany].[Ratio],[map].[tblProductToSupplierProductCompany].[AdditiveCycleVolume],[map].[tblProductToSupplierProductCompany].[Tolerance],[map].[tblProductToSupplierProductCompany].[PresetNumber],[map].[tblProductToSupplierProductCompany].[AdditiveProfileGuid],[map].[tblProductToSupplierProductCompany].[TankGuid],[map].[tblProductToSupplierProductCompany].[MeterID],[map].[tblProductToSupplierProductCompany].[ShipToProductID],[map].[tblProductToSupplierProductCompany].[ShipToProductCode],[map].[tblProductToSupplierProductCompany].[ShipToLoadRackDisplayText],[map].[tblProductToSupplierProductCompany].[UnavailableInventoryGross],[map].[tblProductToSupplierProductCompany].[UnavailableInventoryNet],[map].[tblProductToSupplierProductCompany].[CreatedDate],[map].[tblProductToSupplierProductCompany].[CreatedBy],[map].[tblProductToSupplierProductCompany].[UpdatedDate],[map].[tblProductToSupplierProductCompany].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToSupplierProductCompany]
            INNER JOIN [track].[tblProductToSupplierProductCompany] CT
                ON CT.PK_ProductToSupplierProductCompanyGuid = [map].[tblProductToSupplierProductCompany].[ProductToSupplierProductCompanyGuid]
        WHERE CT.PK_ProductToSupplierProductCompanyGuid = @ProductToSupplierProductCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
