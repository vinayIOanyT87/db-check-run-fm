CREATE FUNCTION [dbo].[udf_GetAssignedPointDetailListPerSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblPointDetailList TABLE
(
	[DrawingGuid] [uniqueidentifier],
	[PointTemplateToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	INSERT INTO @tblPointDetailList 
		SELECT DISTINCT d.[DrawingGuid], eptts.[PointTemplateToSiteGuid] FROM [dbo].[tblDrawings] d
		INNER JOIN [map].[tblEntityPointTemplateToSite] eptts ON eptts.PointTemplateGuid = d.PointTemplateGuid
	   INNER JOIN [dbo].[udf_GetSiteToSiteHierarchyListForSiteGuid](@sync_context_site_guid, 0, 0, 0, 1, 0, 0) h ON h.SiteGuid = d.SiteGuid
		WHERE eptts.SiteGuid = @sync_context_site_guid
	RETURN;
END