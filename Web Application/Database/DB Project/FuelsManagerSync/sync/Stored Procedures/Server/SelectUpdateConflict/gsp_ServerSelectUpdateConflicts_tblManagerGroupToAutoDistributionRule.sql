-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblManagerGroupToAutoDistributionRule
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblManagerGroupToAutoDistributionRule]
@ManagerGroupToAutoDistributionRuleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblManagerGroupToAutoDistributionRule].[ManagerGroupToAutoDistributionRuleGuid],[map].[tblManagerGroupToAutoDistributionRule].[AutoDistributionRuleGuid],[map].[tblManagerGroupToAutoDistributionRule].[ManagerGroupGuid],[map].[tblManagerGroupToAutoDistributionRule].[CreatedDate],[map].[tblManagerGroupToAutoDistributionRule].[CreatedBy],[map].[tblManagerGroupToAutoDistributionRule].[UpdatedDate],[map].[tblManagerGroupToAutoDistributionRule].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblManagerGroupToAutoDistributionRule]
            INNER JOIN [track].[tblManagerGroupToAutoDistributionRule] CT
                ON CT.PK_ManagerGroupToAutoDistributionRuleGuid = [map].[tblManagerGroupToAutoDistributionRule].[ManagerGroupToAutoDistributionRuleGuid]
        WHERE CT.PK_ManagerGroupToAutoDistributionRuleGuid = @ManagerGroupToAutoDistributionRuleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
