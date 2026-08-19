CREATE FUNCTION [dbo].[udf_GetAssignedPointTemplateTagAlarmStatusForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblPointTemplateList TABLE
(
	[PointTemplateToSiteGuid] [uniqueidentifier]
	,[PointTemplateTagAlarmStatusGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblPointTemplateList 
		SELECT [map].[tblEntityPointTemplateToSite].[PointTemplateToSiteGuid]
                , [dbo].[tblPointTemplateTagAlarmStatus].[PointTemplateTagAlarmStatusGuid]
                , [dbo].[tblPointTemplate].[SiteGuid] 'OwnerSiteGuid'
                , [map].[tblEntityPointTemplateToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityPointTemplateToSite]
			INNER JOIN [dbo].[tblPointTemplate]
				ON [map].[tblEntityPointTemplateToSite].[PointTemplateGuid] = [dbo].[tblPointTemplate].[PointTemplateGuid]
			INNER JOIN [dbo].[tblPointTemplateTag]
				ON [map].[tblEntityPointTemplateToSite].[PointTemplateGuid] = [dbo].[tblPointTemplateTag].[PointTemplateGuid]
			INNER JOIN [dbo].[tblAlarmTemplate]
				ON [dbo].[tblAlarmTemplate].[InputTemplateTagGuid] = [dbo].[tblPointTemplateTag].[PointTemplateTagGuid]
			INNER JOIN [dbo].[tblAlarmTestTemplate]
				ON [dbo].[tblAlarmTestTemplate].[AlarmTemplateGuid] = [dbo].[tblAlarmTemplate].[AlarmTemplateGuid]
			INNER JOIN [dbo].[tblPointTemplateTagAlarmStatus]
				ON [dbo].[tblPointTemplateTagAlarmStatus].[AlarmTestTemplateGuid] = [dbo].[tblAlarmTestTemplate].[AlarmTestTemplateGuid]

		WHERE ([map].[tblEntityPointTemplateToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END

GO


