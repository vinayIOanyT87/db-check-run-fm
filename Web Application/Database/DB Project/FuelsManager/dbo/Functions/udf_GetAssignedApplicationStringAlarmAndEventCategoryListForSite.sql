CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringAlarmAndEventCategoryListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[AlarmAndEventCategoryToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityAlarmAndEventCategoryToSite].[AlarmAndEventCategoryToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAlarmAndEventCategoryToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityAlarmAndEventCategoryToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityAlarmAndEventCategoryToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityAlarmAndEventCategoryToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END