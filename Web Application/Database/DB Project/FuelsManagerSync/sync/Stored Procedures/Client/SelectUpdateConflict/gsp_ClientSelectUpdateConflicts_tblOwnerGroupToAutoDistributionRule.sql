-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblOwnerGroupToAutoDistributionRule
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblOwnerGroupToAutoDistributionRule]
@OwnerGroupToAutoDistributionRuleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblOwnerGroupToAutoDistributionRule].[OwnerGroupToAutoDistributionRuleGuid],[map].[tblOwnerGroupToAutoDistributionRule].[AutoDistributionRuleGuid],[map].[tblOwnerGroupToAutoDistributionRule].[OwnerGroupGuid],[map].[tblOwnerGroupToAutoDistributionRule].[CreatedDate],[map].[tblOwnerGroupToAutoDistributionRule].[CreatedBy],[map].[tblOwnerGroupToAutoDistributionRule].[UpdatedDate],[map].[tblOwnerGroupToAutoDistributionRule].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblOwnerGroupToAutoDistributionRule]
            INNER JOIN [track].[tblOwnerGroupToAutoDistributionRule] CT
                ON CT.PK_OwnerGroupToAutoDistributionRuleGuid = [map].[tblOwnerGroupToAutoDistributionRule].[OwnerGroupToAutoDistributionRuleGuid]
        WHERE CT.PK_OwnerGroupToAutoDistributionRuleGuid = @OwnerGroupToAutoDistributionRuleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
