CREATE FUNCTION [dbo].[udf_GetAssignedAlarmPriorityListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblAlarmPriorityList TABLE
(
	[AlarmPriorityToSiteGuid] [uniqueidentifier]
	,[AlarmPriorityGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblAlarmPriorityList 
		SELECT [map].[tblEntityAlarmPriorityToSite].[AlarmPriorityToSiteGuid], [dbo].[tblAlarmPriorities].[AlarmPriorityGuid],[dbo].[tblAlarmPriorities].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAlarmPriorityToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityAlarmPriorityToSite]
			INNER JOIN [dbo].[tblAlarmPriorities]
				ON [map].[tblEntityAlarmPriorityToSite].[AlarmPriorityGuid] = [dbo].[tblAlarmPriorities].[AlarmPriorityGuid]
		WHERE ([map].[tblEntityAlarmPriorityToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END