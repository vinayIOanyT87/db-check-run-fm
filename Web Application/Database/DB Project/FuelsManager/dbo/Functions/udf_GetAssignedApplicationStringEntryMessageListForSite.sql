CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringEntryMessageListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[EntryMessageToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityEntryMessageToSite].[EntryMessageToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityEntryMessageToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityEntryMessageToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityEntryMessageToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityEntryMessageToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END