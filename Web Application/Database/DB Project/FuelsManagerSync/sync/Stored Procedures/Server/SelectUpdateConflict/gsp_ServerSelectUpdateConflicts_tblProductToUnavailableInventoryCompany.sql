-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToUnavailableInventoryCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProductToUnavailableInventoryCompany]
@ProductToUnavailableInventoryCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToUnavailableInventoryCompany].[ProductToUnavailableInventoryCompanyGuid],[map].[tblProductToUnavailableInventoryCompany].[ProductGuid],[map].[tblProductToUnavailableInventoryCompany].[AssignedToCompanyGuid],[map].[tblProductToUnavailableInventoryCompany].[Sequence],[map].[tblProductToUnavailableInventoryCompany].[BlendPercentage],[map].[tblProductToUnavailableInventoryCompany].[AdditiveRate],[map].[tblProductToUnavailableInventoryCompany].[Ratio],[map].[tblProductToUnavailableInventoryCompany].[AdditiveCycleVolume],[map].[tblProductToUnavailableInventoryCompany].[Tolerance],[map].[tblProductToUnavailableInventoryCompany].[PresetNumber],[map].[tblProductToUnavailableInventoryCompany].[AdditiveProfileGuid],[map].[tblProductToUnavailableInventoryCompany].[TankGuid],[map].[tblProductToUnavailableInventoryCompany].[MeterID],[map].[tblProductToUnavailableInventoryCompany].[ShipToProductID],[map].[tblProductToUnavailableInventoryCompany].[ShipToProductCode],[map].[tblProductToUnavailableInventoryCompany].[ShipToLoadRackDisplayText],[map].[tblProductToUnavailableInventoryCompany].[UnavailableInventoryGross],[map].[tblProductToUnavailableInventoryCompany].[UnavailableInventoryNet],[map].[tblProductToUnavailableInventoryCompany].[CreatedDate],[map].[tblProductToUnavailableInventoryCompany].[CreatedBy],[map].[tblProductToUnavailableInventoryCompany].[UpdatedDate],[map].[tblProductToUnavailableInventoryCompany].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToUnavailableInventoryCompany]
            INNER JOIN [track].[tblProductToUnavailableInventoryCompany] CT
                ON CT.PK_ProductToUnavailableInventoryCompanyGuid = [map].[tblProductToUnavailableInventoryCompany].[ProductToUnavailableInventoryCompanyGuid]
        WHERE CT.PK_ProductToUnavailableInventoryCompanyGuid = @ProductToUnavailableInventoryCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
