-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToLedgerView
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProductToLedgerView]
@ProductToLedgerViewGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToLedgerView].[ProductToLedgerViewGuid],[map].[tblProductToLedgerView].[ProductGuid],[map].[tblProductToLedgerView].[AssignedToListViewGuid],[map].[tblProductToLedgerView].[Sequence],[map].[tblProductToLedgerView].[BlendPercentage],[map].[tblProductToLedgerView].[AdditiveRate],[map].[tblProductToLedgerView].[Ratio],[map].[tblProductToLedgerView].[AdditiveCycleVolume],[map].[tblProductToLedgerView].[Tolerance],[map].[tblProductToLedgerView].[PresetNumber],[map].[tblProductToLedgerView].[AdditiveProfileGuid],[map].[tblProductToLedgerView].[TankGuid],[map].[tblProductToLedgerView].[MeterID],[map].[tblProductToLedgerView].[ShipToProductID],[map].[tblProductToLedgerView].[ShipToProductCode],[map].[tblProductToLedgerView].[ShipToLoadRackDisplayText],[map].[tblProductToLedgerView].[UnavailableInventoryGross],[map].[tblProductToLedgerView].[UnavailableInventoryNet],[map].[tblProductToLedgerView].[CreatedDate],[map].[tblProductToLedgerView].[CreatedBy],[map].[tblProductToLedgerView].[UpdatedDate],[map].[tblProductToLedgerView].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToLedgerView]
            INNER JOIN [track].[tblProductToLedgerView] CT
                ON CT.PK_ProductToLedgerViewGuid = [map].[tblProductToLedgerView].[ProductToLedgerViewGuid]
        WHERE CT.PK_ProductToLedgerViewGuid = @ProductToLedgerViewGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
