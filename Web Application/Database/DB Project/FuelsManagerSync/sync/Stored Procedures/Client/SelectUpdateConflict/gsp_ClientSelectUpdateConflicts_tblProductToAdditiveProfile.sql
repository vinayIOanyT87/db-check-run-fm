-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToAdditiveProfile
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToAdditiveProfile]
@ProductToAdditiveProfileGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToAdditiveProfile].[ProductToAdditiveProfileGuid],[map].[tblProductToAdditiveProfile].[ProductGuid],[map].[tblProductToAdditiveProfile].[AssignedToAdditiveProfileGuid],[map].[tblProductToAdditiveProfile].[Sequence],[map].[tblProductToAdditiveProfile].[BlendPercentage],[map].[tblProductToAdditiveProfile].[AdditiveRate],[map].[tblProductToAdditiveProfile].[Ratio],[map].[tblProductToAdditiveProfile].[AdditiveCycleVolume],[map].[tblProductToAdditiveProfile].[Tolerance],[map].[tblProductToAdditiveProfile].[PresetNumber],[map].[tblProductToAdditiveProfile].[AdditiveProfileGuid],[map].[tblProductToAdditiveProfile].[TankGuid],[map].[tblProductToAdditiveProfile].[MeterID],[map].[tblProductToAdditiveProfile].[ShipToProductID],[map].[tblProductToAdditiveProfile].[ShipToProductCode],[map].[tblProductToAdditiveProfile].[ShipToLoadRackDisplayText],[map].[tblProductToAdditiveProfile].[UnavailableInventoryGross],[map].[tblProductToAdditiveProfile].[UnavailableInventoryNet],[map].[tblProductToAdditiveProfile].[DesiredTreatRate],[map].[tblProductToAdditiveProfile].[EnableRecipe],[map].[tblProductToAdditiveProfile].[CreatedDate],[map].[tblProductToAdditiveProfile].[CreatedBy],[map].[tblProductToAdditiveProfile].[UpdatedDate],[map].[tblProductToAdditiveProfile].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToAdditiveProfile]
            INNER JOIN [track].[tblProductToAdditiveProfile] CT
                ON CT.PK_ProductToAdditiveProfileGuid = [map].[tblProductToAdditiveProfile].[ProductToAdditiveProfileGuid]
        WHERE CT.PK_ProductToAdditiveProfileGuid = @ProductToAdditiveProfileGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
