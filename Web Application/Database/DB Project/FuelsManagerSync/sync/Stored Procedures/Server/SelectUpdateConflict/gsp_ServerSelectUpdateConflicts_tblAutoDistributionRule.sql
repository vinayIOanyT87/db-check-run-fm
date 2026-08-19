-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAutoDistributionRule
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAutoDistributionRule]
@AutoDistributionRuleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAutoDistributionRule].[AutoDistributionRuleGuid],[dbo].[tblAutoDistributionRule].[SiteGuid],[dbo].[tblAutoDistributionRule].[RuleID],[dbo].[tblAutoDistributionRule].[RuleDescription],[dbo].[tblAutoDistributionRule].[RuleEnabled],[dbo].[tblAutoDistributionRule].[DefaultEOM],[dbo].[tblAutoDistributionRule].[TransactionAliasGuid],[dbo].[tblAutoDistributionRule].[DefaultReasonCodeGuid],[dbo].[tblAutoDistributionRule].[DefaultNotes],[dbo].[tblAutoDistributionRule].[CreatedDate],[dbo].[tblAutoDistributionRule].[CreatedBy],[dbo].[tblAutoDistributionRule].[UpdatedDate],[dbo].[tblAutoDistributionRule].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAutoDistributionRule]
            INNER JOIN [track].[tblAutoDistributionRule] CT
                ON CT.PK_AutoDistributionRuleGuid = [dbo].[tblAutoDistributionRule].[AutoDistributionRuleGuid]
        WHERE CT.PK_AutoDistributionRuleGuid = @AutoDistributionRuleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
