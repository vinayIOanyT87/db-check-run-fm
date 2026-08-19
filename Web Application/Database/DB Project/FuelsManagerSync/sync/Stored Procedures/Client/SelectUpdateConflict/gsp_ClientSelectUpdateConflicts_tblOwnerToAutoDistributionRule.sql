-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblOwnerToAutoDistributionRule
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblOwnerToAutoDistributionRule]
@OwnerToAutoDistributionRuleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblOwnerToAutoDistributionRule].[OwnerToAutoDistributionRuleGuid],[map].[tblOwnerToAutoDistributionRule].[AutoDistributionRuleGuid],[map].[tblOwnerToAutoDistributionRule].[OwnerGuid],[map].[tblOwnerToAutoDistributionRule].[CreatedDate],[map].[tblOwnerToAutoDistributionRule].[CreatedBy],[map].[tblOwnerToAutoDistributionRule].[UpdatedDate],[map].[tblOwnerToAutoDistributionRule].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblOwnerToAutoDistributionRule]
            INNER JOIN [track].[tblOwnerToAutoDistributionRule] CT
                ON CT.PK_OwnerToAutoDistributionRuleGuid = [map].[tblOwnerToAutoDistributionRule].[OwnerToAutoDistributionRuleGuid]
        WHERE CT.PK_OwnerToAutoDistributionRuleGuid = @OwnerToAutoDistributionRuleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
