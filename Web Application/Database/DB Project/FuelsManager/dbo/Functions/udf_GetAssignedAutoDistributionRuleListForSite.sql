CREATE FUNCTION [dbo].[udf_GetAssignedAutoDistributionRuleListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblAutoDistributionRuleList TABLE
(
	[AutoDistributionRuleToSiteGuid] [uniqueidentifier]
	,[AutoDistributionRuleGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblAutoDistributionRuleList 
		SELECT [map].[tblEntityAutoDistributionRuleToSite].[AutoDistributionRuleToSiteGuid], [dbo].[tblAutoDistributionRule].[AutoDistributionRuleGuid],[dbo].[tblAutoDistributionRule].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAutoDistributionRuleToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityAutoDistributionRuleToSite]
			INNER JOIN [dbo].[tblAutoDistributionRule]
				ON [map].[tblEntityAutoDistributionRuleToSite].[AutoDistributionRuleGuid] = [dbo].[tblAutoDistributionRule].[AutoDistributionRuleGuid]
		WHERE ([map].[tblEntityAutoDistributionRuleToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END