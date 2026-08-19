-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToAutoDistributionRule
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductToAutoDistributionRule]
@ProductToAutoDistributionRuleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductToAutoDistributionRule].[ProductToAutoDistributionRuleGuid],[map].[tblProductToAutoDistributionRule].[AutoDistributionRuleGuid],[map].[tblProductToAutoDistributionRule].[ProductGuid],[map].[tblProductToAutoDistributionRule].[CreatedDate],[map].[tblProductToAutoDistributionRule].[CreatedBy],[map].[tblProductToAutoDistributionRule].[UpdatedDate],[map].[tblProductToAutoDistributionRule].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductToAutoDistributionRule]
            INNER JOIN [track].[tblProductToAutoDistributionRule] CT
                ON CT.PK_ProductToAutoDistributionRuleGuid = [map].[tblProductToAutoDistributionRule].[ProductToAutoDistributionRuleGuid]
        WHERE CT.PK_ProductToAutoDistributionRuleGuid = @ProductToAutoDistributionRuleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
