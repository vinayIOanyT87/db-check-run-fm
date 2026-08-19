-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblTransactionAliasToAutoDistributionRule
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTransactionAliasToAutoDistributionRule]
@TransactionAliasToAutoDistributionRuleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblTransactionAliasToAutoDistributionRule].[TransactionAliasToAutoDistributionRuleGuid],[map].[tblTransactionAliasToAutoDistributionRule].[AutoDistributionRuleGuid],[map].[tblTransactionAliasToAutoDistributionRule].[TransactionAliasGuid],[map].[tblTransactionAliasToAutoDistributionRule].[CreatedDate],[map].[tblTransactionAliasToAutoDistributionRule].[CreatedBy],[map].[tblTransactionAliasToAutoDistributionRule].[UpdatedDate],[map].[tblTransactionAliasToAutoDistributionRule].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblTransactionAliasToAutoDistributionRule]
            INNER JOIN [track].[tblTransactionAliasToAutoDistributionRule] CT
                ON CT.PK_TransactionAliasToAutoDistributionRuleGuid = [map].[tblTransactionAliasToAutoDistributionRule].[TransactionAliasToAutoDistributionRuleGuid]
        WHERE CT.PK_TransactionAliasToAutoDistributionRuleGuid = @TransactionAliasToAutoDistributionRuleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
