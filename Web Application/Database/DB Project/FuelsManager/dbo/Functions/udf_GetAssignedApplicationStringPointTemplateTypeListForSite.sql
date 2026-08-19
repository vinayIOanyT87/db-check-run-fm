CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringPointCategoryListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[PointCategoryToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityPointCategoryToSite].[PointCategoryToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityPointCategoryToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityPointCategoryToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityPointCategoryToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityPointCategoryToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END