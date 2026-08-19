CREATE FUNCTION [dbo].[udf_GetAssignedAlarmAndEventListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblAlarmAndEventList TABLE
(
	[AlarmAndEventToSiteGuid] [uniqueidentifier]
	,[AlarmAndEventGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblAlarmAndEventList
		SELECT [map].[tblEntityAlarmAndEventToSite].[AlarmAndEventToSiteGuid], [dbo].[tblAlarmAndEvents].[AlarmAndEventGuid],[dbo].[tblAlarmAndEvents].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAlarmAndEventToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityAlarmAndEventToSite]
			INNER JOIN [dbo].[tblAlarmAndEvents]
				ON [map].[tblEntityAlarmAndEventToSite].[OwnerSiteGuid] = [dbo].[tblAlarmAndEvents].[SiteGuid]
		WHERE ([map].[tblEntityAlarmAndEventToSite].[MapToSiteGuid] = @sync_context_site_guid)

	RETURN;
END