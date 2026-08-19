-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductGroupToAutoDistributionRule
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProductGroupToAutoDistributionRule]
@ProductGroupToAutoDistributionRuleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblProductGroupToAutoDistributionRule].[ProductGroupToAutoDistributionRuleGuid],[map].[tblProductGroupToAutoDistributionRule].[AutoDistributionRuleGuid],[map].[tblProductGroupToAutoDistributionRule].[ProductGroupGuid],[map].[tblProductGroupToAutoDistributionRule].[CreatedDate],[map].[tblProductGroupToAutoDistributionRule].[CreatedBy],[map].[tblProductGroupToAutoDistributionRule].[UpdatedDate],[map].[tblProductGroupToAutoDistributionRule].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblProductGroupToAutoDistributionRule]
            INNER JOIN [track].[tblProductGroupToAutoDistributionRule] CT
                ON CT.PK_ProductGroupToAutoDistributionRuleGuid = [map].[tblProductGroupToAutoDistributionRule].[ProductGroupToAutoDistributionRuleGuid]
        WHERE CT.PK_ProductGroupToAutoDistributionRuleGuid = @ProductGroupToAutoDistributionRuleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
