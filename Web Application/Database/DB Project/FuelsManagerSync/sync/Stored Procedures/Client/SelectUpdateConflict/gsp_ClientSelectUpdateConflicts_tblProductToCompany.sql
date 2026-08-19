-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToCompany]
@ProductToCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToCompany].[ProductToCompanyGuid],[map].[tblProductToCompany].[ProductGuid],[map].[tblProductToCompany].[AssignedToCompanyGuid],[map].[tblProductToCompany].[Sequence],[map].[tblProductToCompany].[BlendPercentage],[map].[tblProductToCompany].[AdditiveRate],[map].[tblProductToCompany].[Ratio],[map].[tblProductToCompany].[AdditiveCycleVolume],[map].[tblProductToCompany].[Tolerance],[map].[tblProductToCompany].[PresetNumber],[map].[tblProductToCompany].[AdditiveProfileGuid],[map].[tblProductToCompany].[TankGuid],[map].[tblProductToCompany].[MeterID],[map].[tblProductToCompany].[ShipToProductID],[map].[tblProductToCompany].[ShipToProductCode],[map].[tblProductToCompany].[ShipToLoadRackDisplayText],[map].[tblProductToCompany].[UnavailableInventoryGross],[map].[tblProductToCompany].[UnavailableInventoryNet],[map].[tblProductToCompany].[CreatedDate],[map].[tblProductToCompany].[CreatedBy],[map].[tblProductToCompany].[UpdatedDate],[map].[tblProductToCompany].[UpdatedBy],[map].[tblProductToCompany].[SpecialInstructionNote], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToCompany]
            INNER JOIN [track].[tblProductToCompany] CT
                ON CT.PK_ProductToCompanyGuid = [map].[tblProductToCompany].[ProductToCompanyGuid]
        WHERE CT.PK_ProductToCompanyGuid = @ProductToCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
