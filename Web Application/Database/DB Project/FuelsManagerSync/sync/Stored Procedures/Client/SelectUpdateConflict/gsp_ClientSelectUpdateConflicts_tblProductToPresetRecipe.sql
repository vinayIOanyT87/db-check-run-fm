-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToPresetRecipe
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToPresetRecipe]
@ProductToPresetRecipeGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToPresetRecipe].[ProductToPresetRecipeGuid],[map].[tblProductToPresetRecipe].[ProductGuid],[map].[tblProductToPresetRecipe].[AssignedToLoadArmGuid],[map].[tblProductToPresetRecipe].[Sequence],[map].[tblProductToPresetRecipe].[BlendPercentage],[map].[tblProductToPresetRecipe].[AdditiveRate],[map].[tblProductToPresetRecipe].[Ratio],[map].[tblProductToPresetRecipe].[AdditiveCycleVolume],[map].[tblProductToPresetRecipe].[Tolerance],[map].[tblProductToPresetRecipe].[PresetNumber],[map].[tblProductToPresetRecipe].[AdditiveProfileGuid],[map].[tblProductToPresetRecipe].[TankGuid],[map].[tblProductToPresetRecipe].[MeterID],[map].[tblProductToPresetRecipe].[ShipToProductID],[map].[tblProductToPresetRecipe].[ShipToProductCode],[map].[tblProductToPresetRecipe].[ShipToLoadRackDisplayText],[map].[tblProductToPresetRecipe].[UnavailableInventoryGross],[map].[tblProductToPresetRecipe].[UnavailableInventoryNet],[map].[tblProductToPresetRecipe].[CreatedDate],[map].[tblProductToPresetRecipe].[CreatedBy],[map].[tblProductToPresetRecipe].[UpdatedDate],[map].[tblProductToPresetRecipe].[UpdatedBy],[map].[tblProductToPresetRecipe].[EnableRecipe], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToPresetRecipe]
            INNER JOIN [track].[tblProductToPresetRecipe] CT
                ON CT.PK_ProductToPresetRecipeGuid = [map].[tblProductToPresetRecipe].[ProductToPresetRecipeGuid]
        WHERE CT.PK_ProductToPresetRecipeGuid = @ProductToPresetRecipeGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
