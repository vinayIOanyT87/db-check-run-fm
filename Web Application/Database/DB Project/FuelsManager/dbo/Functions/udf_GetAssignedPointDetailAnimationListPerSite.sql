CREATE FUNCTION [dbo].[udf_GetAssignedPointDetailAnimationListPerSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblPointDetailAnimationList TABLE
(
	[animationGuid] [uniqueidentifier],
	[PointTemplateToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	INSERT INTO @tblPointDetailAnimationList 
		SELECT DISTINCT a.[AnimationGuid], eptts.[PointTemplateToSiteGuid] FROM [dbo].[tblAnimation] a
		INNER JOIN [dbo].[tblDrawings] d ON d.SiteGuid = a.SiteGuid
		INNER JOIN [map].[tblEntityPointTemplateToSite] eptts ON eptts.PointTemplateGuid = d.PointTemplateGuid
	   INNER JOIN [dbo].[udf_GetSiteToSiteHierarchyListForSiteGuid](@sync_context_site_guid, 0, 0, 0, 1, 0, 0) h ON h.SiteGuid = d.SiteGuid
		INNER JOIN [map].[tblAnimationToDrawing] atd ON atd.AnimationGuid = a.AnimationGuid AND atd.DrawingGuid = d.DrawingGuid 
		WHERE eptts.SiteGuid = @sync_context_site_guid
	RETURN;
END