-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToPresetFlowControlledAdditive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProductToPresetFlowControlledAdditive]
@ProductToPresetFlowControlledAdditiveGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToPresetFlowControlledAdditive].[ProductToPresetFlowControlledAdditiveGuid],[map].[tblProductToPresetFlowControlledAdditive].[ProductGuid],[map].[tblProductToPresetFlowControlledAdditive].[AssignedToLoadArmGuid],[map].[tblProductToPresetFlowControlledAdditive].[Sequence],[map].[tblProductToPresetFlowControlledAdditive].[BlendPercentage],[map].[tblProductToPresetFlowControlledAdditive].[AdditiveRate],[map].[tblProductToPresetFlowControlledAdditive].[Ratio],[map].[tblProductToPresetFlowControlledAdditive].[AdditiveCycleVolume],[map].[tblProductToPresetFlowControlledAdditive].[Tolerance],[map].[tblProductToPresetFlowControlledAdditive].[PresetNumber],[map].[tblProductToPresetFlowControlledAdditive].[AdditiveProfileGuid],[map].[tblProductToPresetFlowControlledAdditive].[TankGuid],[map].[tblProductToPresetFlowControlledAdditive].[MeterID],[map].[tblProductToPresetFlowControlledAdditive].[ShipToProductID],[map].[tblProductToPresetFlowControlledAdditive].[ShipToProductCode],[map].[tblProductToPresetFlowControlledAdditive].[ShipToLoadRackDisplayText],[map].[tblProductToPresetFlowControlledAdditive].[UnavailableInventoryGross],[map].[tblProductToPresetFlowControlledAdditive].[UnavailableInventoryNet],[map].[tblProductToPresetFlowControlledAdditive].[CreatedDate],[map].[tblProductToPresetFlowControlledAdditive].[CreatedBy],[map].[tblProductToPresetFlowControlledAdditive].[UpdatedDate],[map].[tblProductToPresetFlowControlledAdditive].[UpdatedBy],[map].[tblProductToPresetFlowControlledAdditive].[AssignedToMeterGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToPresetFlowControlledAdditive]
            INNER JOIN [track].[tblProductToPresetFlowControlledAdditive] CT
                ON CT.PK_ProductToPresetFlowControlledAdditiveGuid = [map].[tblProductToPresetFlowControlledAdditive].[ProductToPresetFlowControlledAdditiveGuid]
        WHERE CT.PK_ProductToPresetFlowControlledAdditiveGuid = @ProductToPresetFlowControlledAdditiveGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
