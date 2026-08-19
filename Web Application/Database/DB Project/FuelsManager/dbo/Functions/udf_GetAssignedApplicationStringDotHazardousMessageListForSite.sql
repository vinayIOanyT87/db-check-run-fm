CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringDotHazardousMessageListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[DotHazardousMessagesToSiteGuid] [uniqueidentifier]
	,[ApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblApplicationStringList 
		SELECT [map].[tblEntityDotHazardousMessagesToSite].[DotHazardousMessagesToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityDotHazardousMessagesToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityDotHazardousMessagesToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityDotHazardousMessagesToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityDotHazardousMessagesToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END