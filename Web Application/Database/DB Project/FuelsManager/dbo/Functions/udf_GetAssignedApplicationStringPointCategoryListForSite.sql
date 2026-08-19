CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringPointTemplateTypeListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[PointTemplateTypeToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityPointTemplateTypeToSite].[PointTemplateTypeToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityPointTemplateTypeToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityPointTemplateTypeToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityPointTemplateTypeToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityPointTemplateTypeToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END