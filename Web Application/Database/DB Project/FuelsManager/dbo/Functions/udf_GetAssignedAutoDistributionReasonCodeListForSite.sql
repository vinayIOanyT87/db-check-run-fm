CREATE FUNCTION [dbo].[udf_GetAssignedAutoDistributionReasonCodeListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblAutoDistributionReasonCodeList TABLE
(
	[AutoDistributionReasonCodeToSiteGuid] [uniqueidentifier]
	,[AutoDistributionReasonCodeGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblAutoDistributionReasonCodeList 
		SELECT [map].[tblEntityAutoDistributionReasonCodeToSite].[AutoDistributionReasonCodeToSiteGuid], [dbo].[tblAutoDistributionReasonCodes].[AutoDistributionReasonCodeGuid],[dbo].[tblAutoDistributionReasonCodes].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAutoDistributionReasonCodeToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityAutoDistributionReasonCodeToSite]
			INNER JOIN [dbo].[tblAutoDistributionReasonCodes]
				ON [map].[tblEntityAutoDistributionReasonCodeToSite].[AutoDistributionReasonCodeGuid] = [dbo].[tblAutoDistributionReasonCodes].[AutoDistributionReasonCodeGuid]
		WHERE ([map].[tblEntityAutoDistributionReasonCodeToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END