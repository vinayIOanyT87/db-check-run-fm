/*
	DROP FUNCTION [map].[udf_GetSiteHierarchy]

	SELECT * FROM [map].[udf_GetSiteHierarchy] (575, 1) ORDER BY HierarchyLevel, SiteId
	
*/

	CREATE FUNCTION [map].[udf_GetSiteHierarchy]
	(
		@StartSiteSKey int, @IncludeSites bit = 0
	)
	RETURNS @tblSiteGroupTree TABLE
	(
		SiteSKey int
		, SiteId nvarchar(30)
		, SiteGroupFlag bit
		, HierarchyLevel int
		, RecordUpdatedDate DatetimeOffset(7)
		, IsRecordDeleted bit
	)
	AS
	BEGIN
	/*
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetSiteHierarchy] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to return the site hieararchy below a given sitegroup. The given sitegroup is included in the result set and is set with a hierarchy level of 0.
	-- Notes:
	-- 1. @StartSiteSKey Site or sitegroup SKey (from dimSite) below which the hierarchy needs to be fetched. If a site SKey (instead of a sitegroup SKey) is provided, then the resultset only contains one entry for the given site SKey.
	   2. @IncludeSites: 0: The resultset exclude the leaft site nodes. 1: The resultset includes the leaf site nodes.
	-- 3. In a multi-level site hierarchy, there might be more than one path to get from a sitegroup to a site. 
	--	  The recursive operation below will generate a record for each path. By grouping on SiteSKey, SiteId, and SitegroupFlag the resultset is
	--	  collapsed to a single entry for each site.
	--	  The correct level indicator for each site will correspond to the deepest path encountered in the recursive operation for that site. This is returned using MAX(a.lvl).
	--    Each path to the same site has its own RecordUpdatedDate and IsRecordDeleted value. Since the results have been collapsed to a single record for each site, the 
	--    RecordUpdatedDate is taken as the earliest change noted for the mapping to the site, and the IsRecodDeleted flag is set to 1 if it was set for any of the paths to the site.
	------------------------------------------------------------------------------------------------------
	*/		

		IF (@StartSiteSKey IS NULL)
		BEGIN
			RETURN;
		END;

		WITH SiteHierarchy (SiteSKey, ParentSiteSKey, lvl, RecordUpdatedDate, IsRecordDeleted)
		AS 
		(
			--Anchor
			SELECT TOP(1) ChildSiteSKey, ParentSiteSKey, 0, _RecordUpdatedDate, _DeletedFlag
			FROM map.tblSiteToSite
			WHERE ChildSiteSKey = @StartSiteSKey -- node for which the tree structure underneath it needs to be retrieved.
			-- Recursive Call
			UNION ALL
			SELECT a.ChildSiteSKey, a.ParentSiteSKey, lvl + 1, a._RecordUpdatedDate, a._DeletedFlag
			FROM map.tblSiteToSite a 
			INNER JOIN SiteHierarchy b
			ON a.ParentSiteSKey = b.SiteSKey
			AND a.ChildSiteSKey <> a.ParentSiteSKey
		)
		INSERT INTO @tblSiteGroupTree
		(SiteSKey, SiteId, SiteGroupFlag, HierarchyLevel, RecordUpdatedDate, IsRecordDeleted)
		SELECT a.SiteSKey, b.SiteId, b.SiteGroupFlag, MAX(a.lvl), MIN(a.RecordUpdatedDate), MAX(CONVERT(int, a.IsRecordDeleted))
		FROM SiteHierarchy a
		INNER JOIN dbo.DimSite b
		ON b.SKey = a.SiteSKey
		LEFT OUTER JOIN dbo.DimSite c
		ON c.SKey = a.ParentSiteSKey
		WHERE (((b.SiteGroupFlag = 1) AND (@IncludeSites = 0)) OR (@IncludeSites = 1))
		GROUP BY a.SiteSKey, b.SiteId, b.SiteGroupFlag
		OPTION (MAXRECURSION 10000);	

		RETURN;
	END