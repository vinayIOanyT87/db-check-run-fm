-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToOffloadExternalMeter
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToOffloadExternalMeter]
@ProductToOffloadExternalMeterGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToOffloadExternalMeter].[ProductToOffloadExternalMeterGuid],[map].[tblProductToOffloadExternalMeter].[ProductGuid],[map].[tblProductToOffloadExternalMeter].[AssignedToLoadArmGuid],[map].[tblProductToOffloadExternalMeter].[Sequence],[map].[tblProductToOffloadExternalMeter].[BlendPercentage],[map].[tblProductToOffloadExternalMeter].[AdditiveRate],[map].[tblProductToOffloadExternalMeter].[Ratio],[map].[tblProductToOffloadExternalMeter].[AdditiveCycleVolume],[map].[tblProductToOffloadExternalMeter].[Tolerance],[map].[tblProductToOffloadExternalMeter].[PresetNumber],[map].[tblProductToOffloadExternalMeter].[AdditiveProfileGuid],[map].[tblProductToOffloadExternalMeter].[TankGuid],[map].[tblProductToOffloadExternalMeter].[MeterID],[map].[tblProductToOffloadExternalMeter].[ShipToProductID],[map].[tblProductToOffloadExternalMeter].[ShipToProductCode],[map].[tblProductToOffloadExternalMeter].[ShipToLoadRackDisplayText],[map].[tblProductToOffloadExternalMeter].[UnavailableInventoryGross],[map].[tblProductToOffloadExternalMeter].[UnavailableInventoryNet],[map].[tblProductToOffloadExternalMeter].[CreatedDate],[map].[tblProductToOffloadExternalMeter].[CreatedBy],[map].[tblProductToOffloadExternalMeter].[UpdatedDate],[map].[tblProductToOffloadExternalMeter].[UpdatedBy],[map].[tblProductToOffloadExternalMeter].[AssignedToMeterGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToOffloadExternalMeter]
            INNER JOIN [track].[tblProductToOffloadExternalMeter] CT
                ON CT.PK_ProductToOffloadExternalMeterGuid = [map].[tblProductToOffloadExternalMeter].[ProductToOffloadExternalMeterGuid]
        WHERE CT.PK_ProductToOffloadExternalMeterGuid = @ProductToOffloadExternalMeterGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
