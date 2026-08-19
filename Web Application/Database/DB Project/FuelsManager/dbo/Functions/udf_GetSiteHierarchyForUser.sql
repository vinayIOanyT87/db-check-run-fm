-- ==================================================================================================================
-- Author:		Hans Bapoo
-- Create date:	2022-05-03
-- Description:	Returns a site hierarchy based on the User and current Site.
--
-- Testing:
-- SELECT * FROM [dbo].[udf_GetSiteHierarchyForUser] (NULL, NULL) ORDER BY HierarchyLevel, SiteId
-- SELECT * FROM [dbo].[udf_GetSiteHierarchyForUser] (NULL, '850E070F-AC91-417B-A087-373BECE8A9D3') ORDER BY HierarchyLevel, SiteId
-- SELECT * FROM [dbo].[udf_GetSiteHierarchyForUser] ('7C150A16-F8A0-4D51-809E-C07CDF3D7A46', '850E070F-AC91-417B-A087-373BECE8A9D3') ORDER BY HierarchyLevel, SiteId
-- SELECT * FROM [dbo].[udf_GetSiteHierarchyForUser] ('00000000-0000-0000-0000-000000000001', '850E070F-AC91-417B-A087-373BECE8A9D3') ORDER BY HierarchyLevel, SiteId
-- ==================================================================================================================
CREATE FUNCTION [dbo].[udf_GetSiteHierarchyForUser]
(
	@StartSiteGuid uniqueidentifier, @UserGuid uniqueidentifier
)
RETURNS @tblSiteHierarchy TABLE
(
	SiteGuid uniqueidentifier
	, SiteId nvarchar(30)
	, SiteGroupFlag bit
	, HierarchyLevel int
)
AS
BEGIN
	INSERT INTO @tblSiteHierarchy
	(SiteGuid, SiteId, HierarchyLevel, SiteGroupFlag)
	SELECT DISTINCT a.SiteGuid, a.SiteId, a.HierarchyLevel, a.SiteGroupFlag
	FROM [erv].[udf_GetSiteHierarchy](@StartSiteGuid, 1) a
	INNER JOIN map.tblEntityUserToSite b ON b.SiteGuid = a.SiteGuid
	INNER JOIN map.tblUserToGroup c ON c.SiteGuid = a.SiteGuid AND c.UserGuid = b.UserGuid
	WHERE b.UserGuid = @UserGuid
	ORDER BY a.HierarchyLevel, a.SiteGuid

	RETURN;
END
