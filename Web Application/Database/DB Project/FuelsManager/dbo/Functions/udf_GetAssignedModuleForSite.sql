
CREATE FUNCTION [dbo].[udf_GetAssignedModuleForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblModuleList TABLE
(
	  [ModuleToSiteGuid] [Uniqueidentifier]
	, [PointTemplateToSiteGuid] [uniqueidentifier]
	, [ModuleToPointTemplateGuid] [uniqueidentifier]
	, [ModuleGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblModuleList 
		SELECT DISTINCT ModuleToSiteGuid, PointTemplateToSiteGuid, ModuleToPointTemplateGuid, ModuleGuid FROM
		(SELECT NULL AS ModuleToSiteGuid, [map].[tblEntityPointTemplateToSite].PointTemplateToSiteGuid AS PointTemplateToSiteGuid, [map].[tblModuleToPointTemplate].[ModuleToPointTemplateGuid] AS ModuleToPointTemplateGuid, [map].[tblModuleToPointTemplate].ModuleGuid AS ModuleGuid
 		FROM [map].[tblEntityPointTemplateToSite]
			INNER JOIN [dbo].[tblPointTemplate]	ON [map].[tblEntityPointTemplateToSite].[PointTemplateGuid] = [dbo].[tblPointTemplate].[PointTemplateGuid]
			INNER JOIN [map].[tblModuleToPointTemplate]	ON [map].[tblModuleToPointTemplate].PointTemplateGuid = [dbo].[tblPointTemplate].[PointTemplateGuid]
		WHERE ([map].[tblEntityPointTemplateToSite].[SiteGuid] = @sync_context_site_guid)
		UNION
			SELECT [map].[tblEntityModuleToSite].ModuleToSiteGuid AS ModuleToSiteGuid, NULL as PointTemplateToSiteGuid, NULL AS ModuleToPointTemplateGuide, [map].[tblEntityModuleToSite].ModuleGuid AS ModuleGuid
			FROM [map].[tblEntityModuleToSite]
			WHERE ([map].[tblEntityModuleToSite].[SiteGuid] = @sync_context_site_guid)
		) ModuleGuids
	RETURN;
END

