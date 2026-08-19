-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblManagerToAutoDistributionRule
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblManagerToAutoDistributionRule]
@ManagerToAutoDistributionRuleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblManagerToAutoDistributionRule].[ManagerToAutoDistributionRuleGuid],[map].[tblManagerToAutoDistributionRule].[AutoDistributionRuleGuid],[map].[tblManagerToAutoDistributionRule].[ManagerGuid],[map].[tblManagerToAutoDistributionRule].[CreatedDate],[map].[tblManagerToAutoDistributionRule].[CreatedBy],[map].[tblManagerToAutoDistributionRule].[UpdatedDate],[map].[tblManagerToAutoDistributionRule].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblManagerToAutoDistributionRule]
            INNER JOIN [track].[tblManagerToAutoDistributionRule] CT
                ON CT.PK_ManagerToAutoDistributionRuleGuid = [map].[tblManagerToAutoDistributionRule].[ManagerToAutoDistributionRuleGuid]
        WHERE CT.PK_ManagerToAutoDistributionRuleGuid = @ManagerToAutoDistributionRuleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
