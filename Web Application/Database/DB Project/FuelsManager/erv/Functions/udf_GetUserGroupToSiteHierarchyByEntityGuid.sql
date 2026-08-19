/*
	DROP FUNCTION [erv].[udf_GetUserGroupToSiteHierarchyByEntityGuid]

	SELECT * FROM [erv].[udf_GetUserGroupToSiteHierarchyByEntityGuid] ('6045F07E-956F-43A6-B604-676679CFE91E') ORDER BY HierarchyLevel, SiteId
	SELECT * FROM [erv].[udf_GetUserGroupToSiteHierarchyByEntityGuid] ('B4E4B396-1366-4BEA-BDD6-D08F35863E87') ORDER BY HierarchyLevel, SiteId	
*/



/*
------------------------------------------------------------------------------------------------------
-- Function: [erv].[udf_GetUserGroupToSiteHierarchyByEntityGuid] 
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Function to return the site hierarchy below a given sitegroup based on the assignments of a given UserGroup record from its owner sitegroup. 
--          The owner sitegroup is included in the result set and is set with a hierarchy level of 0.
-- Notes:
-- 1. @EntityGuid: Guid of the UserGroup entity record version for which the assignment tree is to be queried. 
-- 2. 
------------------------------------------------------------------------------------------------------
*/
CREATE FUNCTION [erv].[udf_GetUserGroupToSiteHierarchyByEntityGuid]
(
	@EntityGuid uniqueidentifier
)
RETURNS @tblSiteGroupTree TABLE
(
	SiteGuid uniqueidentifier
	, SiteId nvarchar(30)
	, SiteGroupFlag bit
	, AssignedFromSiteGuid uniqueidentifier
	, AssignedFromSiteId nvarchar(30)
	, HierarchyLevel int
)
AS
BEGIN

	DECLARE @startSiteGuid uniqueidentifier
	SELECT @startSiteGuid = SiteGuid FROM tblGroups WHERE GroupGuid = @EntityGuid;

	WITH SiteHierarchy (SiteGuid, ParentSiteGuid, lvl)
	AS 
	(
		--Anchor
		SELECT TOP(1)SiteGuid, AssignedFromSiteGuid, 0
		FROM map.tblEntityUserGroupToSite
		WHERE GroupGuid = @EntityGuid
		AND SiteGuid = @StartSiteGuid -- node for which the tree structure underneath it needs to be retrieved.
		-- Recursive Call
		UNION ALL
		SELECT a.SiteGuid, a.AssignedFromSiteGuid, lvl + 1
		FROM map.tblEntityUserGroupToSite a 
		INNER JOIN SiteHierarchy b
		ON a.AssignedFromSiteGuid = b.SiteGuid
		WHERE GroupGuid = @EntityGuid
		AND a.SiteGuid <> a.AssignedFromSiteGuid
	)
		
	INSERT INTO @tblSiteGroupTree
	(SiteGuid, SiteId, SiteGroupFlag, AssignedFromSiteGuid, AssignedFromSiteId, HierarchyLevel)
	SELECT a.SiteGuid, b.id SiteId, b.SiteGroupFlag, a.ParentSiteGuid, c.Id, a.lvl
	FROM SiteHierarchy a
	INNER JOIN tblSites b
	ON b.SiteGuid = a.SiteGuid
	LEFT OUTER JOIN tblSites c
	ON c.SiteGuid = a.ParentSiteGuid
		
	--Note: No need to group on SiteGuid because, unlike the site hierarchy where a site/sitegroup can be the child to more than one parent sitegroup, 
	--in the case of entity-to-site hierarchy, an entity can only be mapped to a site/sitegroup from one parent sitegroup.

	OPTION (MAXRECURSION 10000);

	RETURN;
END
