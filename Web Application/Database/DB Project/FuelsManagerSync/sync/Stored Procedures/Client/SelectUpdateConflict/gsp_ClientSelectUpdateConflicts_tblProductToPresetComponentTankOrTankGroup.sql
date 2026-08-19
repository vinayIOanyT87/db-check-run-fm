-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToPresetComponentTankOrTankGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToPresetComponentTankOrTankGroup]
@ProductToPresetComponentTankOrTankGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToPresetComponentTankOrTankGroup].[ProductToPresetComponentTankOrTankGroupGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[ProductGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[AssignedToLoadArmGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[Sequence],[map].[tblProductToPresetComponentTankOrTankGroup].[BlendPercentage],[map].[tblProductToPresetComponentTankOrTankGroup].[AdditiveRate],[map].[tblProductToPresetComponentTankOrTankGroup].[Ratio],[map].[tblProductToPresetComponentTankOrTankGroup].[AdditiveCycleVolume],[map].[tblProductToPresetComponentTankOrTankGroup].[Tolerance],[map].[tblProductToPresetComponentTankOrTankGroup].[PresetNumber],[map].[tblProductToPresetComponentTankOrTankGroup].[AdditiveProfileGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[TankGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[TankGroupApplicationStringGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[MeterID],[map].[tblProductToPresetComponentTankOrTankGroup].[ShipToProductID],[map].[tblProductToPresetComponentTankOrTankGroup].[ShipToProductCode],[map].[tblProductToPresetComponentTankOrTankGroup].[ShipToLoadRackDisplayText],[map].[tblProductToPresetComponentTankOrTankGroup].[UnavailableInventoryGross],[map].[tblProductToPresetComponentTankOrTankGroup].[UnavailableInventoryNet],[map].[tblProductToPresetComponentTankOrTankGroup].[CreatedDate],[map].[tblProductToPresetComponentTankOrTankGroup].[CreatedBy],[map].[tblProductToPresetComponentTankOrTankGroup].[UpdatedDate],[map].[tblProductToPresetComponentTankOrTankGroup].[UpdatedBy],[map].[tblProductToPresetComponentTankOrTankGroup].[AssignedToMeterGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToPresetComponentTankOrTankGroup]
            INNER JOIN [track].[tblProductToPresetComponentTankOrTankGroup] CT
                ON CT.PK_ProductToPresetComponentTankOrTankGroupGuid = [map].[tblProductToPresetComponentTankOrTankGroup].[ProductToPresetComponentTankOrTankGroupGuid]
        WHERE CT.PK_ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
