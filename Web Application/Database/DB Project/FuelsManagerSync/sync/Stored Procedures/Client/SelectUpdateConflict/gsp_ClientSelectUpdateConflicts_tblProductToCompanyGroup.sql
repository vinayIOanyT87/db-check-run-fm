-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToCompanyGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToCompanyGroup]
@ProductToCompanyGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToCompanyGroup].[ProductToCompanyGroupGuid],[map].[tblProductToCompanyGroup].[ProductGuid],[map].[tblProductToCompanyGroup].[AssignedToApplicationStringGuid],[map].[tblProductToCompanyGroup].[Sequence],[map].[tblProductToCompanyGroup].[BlendPercentage],[map].[tblProductToCompanyGroup].[AdditiveRate],[map].[tblProductToCompanyGroup].[Ratio],[map].[tblProductToCompanyGroup].[AdditiveCycleVolume],[map].[tblProductToCompanyGroup].[Tolerance],[map].[tblProductToCompanyGroup].[PresetNumber],[map].[tblProductToCompanyGroup].[AdditiveProfileGuid],[map].[tblProductToCompanyGroup].[TankGuid],[map].[tblProductToCompanyGroup].[MeterID],[map].[tblProductToCompanyGroup].[ShipToProductID],[map].[tblProductToCompanyGroup].[ShipToProductCode],[map].[tblProductToCompanyGroup].[ShipToLoadRackDisplayText],[map].[tblProductToCompanyGroup].[UnavailableInventoryGross],[map].[tblProductToCompanyGroup].[UnavailableInventoryNet],[map].[tblProductToCompanyGroup].[CreatedDate],[map].[tblProductToCompanyGroup].[CreatedBy],[map].[tblProductToCompanyGroup].[UpdatedDate],[map].[tblProductToCompanyGroup].[UpdatedBy],[map].[tblProductToCompanyGroup].[SpecialInstructionNote], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToCompanyGroup]
            INNER JOIN [track].[tblProductToCompanyGroup] CT
                ON CT.PK_ProductToCompanyGroupGuid = [map].[tblProductToCompanyGroup].[ProductToCompanyGroupGuid]
        WHERE CT.PK_ProductToCompanyGroupGuid = @ProductToCompanyGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
