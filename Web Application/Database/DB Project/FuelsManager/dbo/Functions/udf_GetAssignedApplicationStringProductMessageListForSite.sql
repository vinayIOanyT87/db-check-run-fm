CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringProductMessageListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[ProductMessageToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityProductMessageToSite].[ProductMessageToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityProductMessageToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityProductMessageToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityProductMessageToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityProductMessageToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END