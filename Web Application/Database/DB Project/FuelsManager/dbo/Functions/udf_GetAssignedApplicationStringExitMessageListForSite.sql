CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringExitMessageListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[ExitMessageToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityExitMessageToSite].[ExitMessageToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityExitMessageToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityExitMessageToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityExitMessageToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityExitMessageToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END