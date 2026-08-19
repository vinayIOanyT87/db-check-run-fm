-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAutoDistributionRuleToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityAutoDistributionRuleToSite]
@AutoDistributionRuleToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAutoDistributionRuleToSite].[AutoDistributionRuleToSiteGuid],[map].[tblEntityAutoDistributionRuleToSite].[SiteGuid],[map].[tblEntityAutoDistributionRuleToSite].[AutoDistributionRuleGuid],[map].[tblEntityAutoDistributionRuleToSite].[CreatedDate],[map].[tblEntityAutoDistributionRuleToSite].[CreatedBy],[map].[tblEntityAutoDistributionRuleToSite].[UpdatedDate],[map].[tblEntityAutoDistributionRuleToSite].[UpdatedBy],[map].[tblEntityAutoDistributionRuleToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAutoDistributionRuleToSite]
            INNER JOIN [track].[tblEntityAutoDistributionRuleToSite] CT
                ON CT.PK_AutoDistributionRuleToSiteGuid = [map].[tblEntityAutoDistributionRuleToSite].[AutoDistributionRuleToSiteGuid]
        WHERE CT.PK_AutoDistributionRuleToSiteGuid = @AutoDistributionRuleToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
