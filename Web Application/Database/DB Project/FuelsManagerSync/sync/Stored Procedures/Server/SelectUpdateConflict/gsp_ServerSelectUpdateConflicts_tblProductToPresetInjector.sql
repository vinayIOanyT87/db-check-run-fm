-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToPresetInjector
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProductToPresetInjector]
@ProductToPresetInjectorGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToPresetInjector].[ProductToPresetInjectorGuid],[map].[tblProductToPresetInjector].[ProductGuid],[map].[tblProductToPresetInjector].[AssignedToLoadArmGuid],[map].[tblProductToPresetInjector].[Sequence],[map].[tblProductToPresetInjector].[BlendPercentage],[map].[tblProductToPresetInjector].[AdditiveRate],[map].[tblProductToPresetInjector].[Ratio],[map].[tblProductToPresetInjector].[AdditiveCycleVolume],[map].[tblProductToPresetInjector].[Tolerance],[map].[tblProductToPresetInjector].[PresetNumber],[map].[tblProductToPresetInjector].[AdditiveProfileGuid],[map].[tblProductToPresetInjector].[TankGuid],[map].[tblProductToPresetInjector].[MeterID],[map].[tblProductToPresetInjector].[ShipToProductID],[map].[tblProductToPresetInjector].[ShipToProductCode],[map].[tblProductToPresetInjector].[ShipToLoadRackDisplayText],[map].[tblProductToPresetInjector].[UnavailableInventoryGross],[map].[tblProductToPresetInjector].[UnavailableInventoryNet],[map].[tblProductToPresetInjector].[CreatedDate],[map].[tblProductToPresetInjector].[CreatedBy],[map].[tblProductToPresetInjector].[UpdatedDate],[map].[tblProductToPresetInjector].[UpdatedBy],[map].[tblProductToPresetInjector].[AssignedToMeterGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToPresetInjector]
            INNER JOIN [track].[tblProductToPresetInjector] CT
                ON CT.PK_ProductToPresetInjectorGuid = [map].[tblProductToPresetInjector].[ProductToPresetInjectorGuid]
        WHERE CT.PK_ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
