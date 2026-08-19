CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringProcessVariableMessageListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[ProcessVariableMessageToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityProcessVariableMessageToSite].[ProcessVariableMessageToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityProcessVariableMessageToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityProcessVariableMessageToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityProcessVariableMessageToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityProcessVariableMessageToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END