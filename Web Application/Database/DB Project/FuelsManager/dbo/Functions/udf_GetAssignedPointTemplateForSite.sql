CREATE FUNCTION [dbo].[udf_GetAssignedPointTemplateForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblPointTemplateList TABLE
(
	[PointTemplateToSiteGuid] [uniqueidentifier]
	,[PointTemplateGuid] [uniqueidentifier]
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
                , [dbo].[tblPointTemplate].[PointTemplateGuid]
                , [dbo].[tblPointTemplate].[SiteGuid] 'OwnerSiteGuid'
                , [map].[tblEntityPointTemplateToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityPointTemplateToSite]
			INNER JOIN [dbo].[tblPointTemplate]
				ON [map].[tblEntityPointTemplateToSite].[PointTemplateGuid] = [dbo].[tblPointTemplate].[PointTemplateGuid]
		WHERE ([map].[tblEntityPointTemplateToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END