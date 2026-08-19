-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToProductGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProductToProductGroup]
@ProductToProductGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToProductGroup].[ProductToProductGroupGuid],[map].[tblProductToProductGroup].[ProductGuid],[map].[tblProductToProductGroup].[AssignedToApplicationStringGuid],[map].[tblProductToProductGroup].[Sequence],[map].[tblProductToProductGroup].[BlendPercentage],[map].[tblProductToProductGroup].[AdditiveRate],[map].[tblProductToProductGroup].[Ratio],[map].[tblProductToProductGroup].[AdditiveCycleVolume],[map].[tblProductToProductGroup].[Tolerance],[map].[tblProductToProductGroup].[PresetNumber],[map].[tblProductToProductGroup].[AdditiveProfileGuid],[map].[tblProductToProductGroup].[TankGuid],[map].[tblProductToProductGroup].[MeterID],[map].[tblProductToProductGroup].[ShipToProductID],[map].[tblProductToProductGroup].[ShipToProductCode],[map].[tblProductToProductGroup].[ShipToLoadRackDisplayText],[map].[tblProductToProductGroup].[UnavailableInventoryGross],[map].[tblProductToProductGroup].[UnavailableInventoryNet],[map].[tblProductToProductGroup].[CreatedDate],[map].[tblProductToProductGroup].[CreatedBy],[map].[tblProductToProductGroup].[UpdatedDate],[map].[tblProductToProductGroup].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToProductGroup]
            INNER JOIN [track].[tblProductToProductGroup] CT
                ON CT.PK_ProductToProductGroupGuid = [map].[tblProductToProductGroup].[ProductToProductGroupGuid]
        WHERE CT.PK_ProductToProductGroupGuid = @ProductToProductGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
