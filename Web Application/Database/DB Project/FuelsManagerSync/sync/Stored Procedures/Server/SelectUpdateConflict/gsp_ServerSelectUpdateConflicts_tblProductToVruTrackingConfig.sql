-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToVruTrackingConfig
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProductToVruTrackingConfig]
@ProductToVruTrackingConfigGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToVruTrackingConfig].[ProductToVruTrackingConfigGuid],[map].[tblProductToVruTrackingConfig].[ProductGuid],[map].[tblProductToVruTrackingConfig].[AssignedToSiteGuid],[map].[tblProductToVruTrackingConfig].[Sequence],[map].[tblProductToVruTrackingConfig].[BlendPercentage],[map].[tblProductToVruTrackingConfig].[AdditiveRate],[map].[tblProductToVruTrackingConfig].[Ratio],[map].[tblProductToVruTrackingConfig].[AdditiveCycleVolume],[map].[tblProductToVruTrackingConfig].[Tolerance],[map].[tblProductToVruTrackingConfig].[PresetNumber],[map].[tblProductToVruTrackingConfig].[AdditiveProfileGuid],[map].[tblProductToVruTrackingConfig].[TankGuid],[map].[tblProductToVruTrackingConfig].[MeterID],[map].[tblProductToVruTrackingConfig].[ShipToProductID],[map].[tblProductToVruTrackingConfig].[ShipToProductCode],[map].[tblProductToVruTrackingConfig].[ShipToLoadRackDisplayText],[map].[tblProductToVruTrackingConfig].[UnavailableInventoryGross],[map].[tblProductToVruTrackingConfig].[UnavailableInventoryNet],[map].[tblProductToVruTrackingConfig].[CreatedDate],[map].[tblProductToVruTrackingConfig].[CreatedBy],[map].[tblProductToVruTrackingConfig].[UpdatedDate],[map].[tblProductToVruTrackingConfig].[UpdatedBy],[map].[tblProductToVruTrackingConfig].[SpecialInstructionNote], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToVruTrackingConfig]
            INNER JOIN [track].[tblProductToVruTrackingConfig] CT
                ON CT.PK_ProductToVruTrackingConfigGuid = [map].[tblProductToVruTrackingConfig].[ProductToVruTrackingConfigGuid]
        WHERE CT.PK_ProductToVruTrackingConfigGuid = @ProductToVruTrackingConfigGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
