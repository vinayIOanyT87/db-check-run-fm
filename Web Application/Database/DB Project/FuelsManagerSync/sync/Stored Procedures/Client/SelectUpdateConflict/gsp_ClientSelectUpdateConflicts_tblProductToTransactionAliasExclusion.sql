-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToTransactionAliasExclusion
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToTransactionAliasExclusion]
@ProductToTransactionAliasExclusionGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToTransactionAliasExclusion].[ProductToTransactionAliasExclusionGuid],[map].[tblProductToTransactionAliasExclusion].[ProductGuid],[map].[tblProductToTransactionAliasExclusion].[AssignedToTransactionAliasGuid],[map].[tblProductToTransactionAliasExclusion].[Sequence],[map].[tblProductToTransactionAliasExclusion].[BlendPercentage],[map].[tblProductToTransactionAliasExclusion].[AdditiveRate],[map].[tblProductToTransactionAliasExclusion].[Ratio],[map].[tblProductToTransactionAliasExclusion].[AdditiveCycleVolume],[map].[tblProductToTransactionAliasExclusion].[Tolerance],[map].[tblProductToTransactionAliasExclusion].[PresetNumber],[map].[tblProductToTransactionAliasExclusion].[AdditiveProfileGuid],[map].[tblProductToTransactionAliasExclusion].[TankGuid],[map].[tblProductToTransactionAliasExclusion].[MeterID],[map].[tblProductToTransactionAliasExclusion].[ShipToProductID],[map].[tblProductToTransactionAliasExclusion].[ShipToProductCode],[map].[tblProductToTransactionAliasExclusion].[ShipToLoadRackDisplayText],[map].[tblProductToTransactionAliasExclusion].[UnavailableInventoryGross],[map].[tblProductToTransactionAliasExclusion].[UnavailableInventoryNet],[map].[tblProductToTransactionAliasExclusion].[CreatedDate],[map].[tblProductToTransactionAliasExclusion].[CreatedBy],[map].[tblProductToTransactionAliasExclusion].[UpdatedDate],[map].[tblProductToTransactionAliasExclusion].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToTransactionAliasExclusion]
            INNER JOIN [track].[tblProductToTransactionAliasExclusion] CT
                ON CT.PK_ProductToTransactionAliasExclusionGuid = [map].[tblProductToTransactionAliasExclusion].[ProductToTransactionAliasExclusionGuid]
        WHERE CT.PK_ProductToTransactionAliasExclusionGuid = @ProductToTransactionAliasExclusionGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
