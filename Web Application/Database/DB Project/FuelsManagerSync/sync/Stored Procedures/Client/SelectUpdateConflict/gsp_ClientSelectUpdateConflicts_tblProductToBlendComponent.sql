-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToBlendComponent
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToBlendComponent]
@ProductToBlendComponentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToBlendComponent].[ProductToBlendComponentGuid],[map].[tblProductToBlendComponent].[ProductGuid],[map].[tblProductToBlendComponent].[AssignedToProductGuid],[map].[tblProductToBlendComponent].[Sequence],[map].[tblProductToBlendComponent].[BlendPercentage],[map].[tblProductToBlendComponent].[AdditiveRate],[map].[tblProductToBlendComponent].[Ratio],[map].[tblProductToBlendComponent].[AdditiveCycleVolume],[map].[tblProductToBlendComponent].[Tolerance],[map].[tblProductToBlendComponent].[PresetNumber],[map].[tblProductToBlendComponent].[AdditiveProfileGuid],[map].[tblProductToBlendComponent].[TankGuid],[map].[tblProductToBlendComponent].[MeterID],[map].[tblProductToBlendComponent].[ShipToProductID],[map].[tblProductToBlendComponent].[ShipToProductCode],[map].[tblProductToBlendComponent].[ShipToLoadRackDisplayText],[map].[tblProductToBlendComponent].[UnavailableInventoryGross],[map].[tblProductToBlendComponent].[UnavailableInventoryNet],[map].[tblProductToBlendComponent].[CreatedDate],[map].[tblProductToBlendComponent].[CreatedBy],[map].[tblProductToBlendComponent].[UpdatedDate],[map].[tblProductToBlendComponent].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToBlendComponent]
            INNER JOIN [track].[tblProductToBlendComponent] CT
                ON CT.PK_ProductToBlendComponentGuid = [map].[tblProductToBlendComponent].[ProductToBlendComponentGuid]
        WHERE CT.PK_ProductToBlendComponentGuid = @ProductToBlendComponentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
