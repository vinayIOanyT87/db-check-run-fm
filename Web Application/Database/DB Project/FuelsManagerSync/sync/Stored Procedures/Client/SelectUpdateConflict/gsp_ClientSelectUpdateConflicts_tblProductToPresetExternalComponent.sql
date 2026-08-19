-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToPresetExternalComponent
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToPresetExternalComponent]
@ProductToPresetExternalComponentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToPresetExternalComponent].[ProductToPresetExternalComponentGuid],[map].[tblProductToPresetExternalComponent].[TankGroupApplicationStringGuid],[map].[tblProductToPresetExternalComponent].[ProductGuid],[map].[tblProductToPresetExternalComponent].[AssignedToLoadArmGuid],[map].[tblProductToPresetExternalComponent].[Sequence],[map].[tblProductToPresetExternalComponent].[BlendPercentage],[map].[tblProductToPresetExternalComponent].[AdditiveRate],[map].[tblProductToPresetExternalComponent].[Ratio],[map].[tblProductToPresetExternalComponent].[AdditiveCycleVolume],[map].[tblProductToPresetExternalComponent].[Tolerance],[map].[tblProductToPresetExternalComponent].[PresetNumber],[map].[tblProductToPresetExternalComponent].[AdditiveProfileGuid],[map].[tblProductToPresetExternalComponent].[TankGuid],[map].[tblProductToPresetExternalComponent].[MeterID],[map].[tblProductToPresetExternalComponent].[ShipToProductID],[map].[tblProductToPresetExternalComponent].[ShipToProductCode],[map].[tblProductToPresetExternalComponent].[ShipToLoadRackDisplayText],[map].[tblProductToPresetExternalComponent].[UnavailableInventoryGross],[map].[tblProductToPresetExternalComponent].[UnavailableInventoryNet],[map].[tblProductToPresetExternalComponent].[CreatedDate],[map].[tblProductToPresetExternalComponent].[CreatedBy],[map].[tblProductToPresetExternalComponent].[UpdatedDate],[map].[tblProductToPresetExternalComponent].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToPresetExternalComponent]
            INNER JOIN [track].[tblProductToPresetExternalComponent] CT
                ON CT.PK_ProductToPresetExternalComponentGuid = [map].[tblProductToPresetExternalComponent].[ProductToPresetExternalComponentGuid]
        WHERE CT.PK_ProductToPresetExternalComponentGuid = @ProductToPresetExternalComponentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
