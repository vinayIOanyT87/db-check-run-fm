-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblWeightedAverageCosts
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblWeightedAverageCosts]
@WeightedAverageCostGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblWeightedAverageCosts].[WacValue],[dbo].[tblWeightedAverageCosts].[IsManualOverride],[dbo].[tblWeightedAverageCosts].[Source],[dbo].[tblWeightedAverageCosts].[Notes],[dbo].[tblWeightedAverageCosts].[CreatedBy],[dbo].[tblWeightedAverageCosts].[CreatedDate],[dbo].[tblWeightedAverageCosts].[UpdatedBy],[dbo].[tblWeightedAverageCosts].[UpdatedDate],CONVERT(CHAR(10), [dbo].[tblWeightedAverageCosts].[InventoryDate], 111) AS [InventoryDate],[dbo].[tblWeightedAverageCosts].[WeightedAverageCostGuid],[dbo].[tblWeightedAverageCosts].[SiteGuid],[dbo].[tblWeightedAverageCosts].[ProductGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblWeightedAverageCosts]
            INNER JOIN [track].[tblWeightedAverageCosts] CT
                ON CT.PK_WeightedAverageCostGuid = [dbo].[tblWeightedAverageCosts].[WeightedAverageCostGuid]
        WHERE CT.PK_WeightedAverageCostGuid = @WeightedAverageCostGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
