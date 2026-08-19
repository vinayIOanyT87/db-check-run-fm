/*
	SELECT * FROM [erv].[udf_GetSiteHierarchy] (NULL, 1) ORDER BY HierarchyLevel, SiteId
	SELECT * FROM [erv].[udf_GetSiteHierarchy] (NULL, 0) ORDER BY HierarchyLevel, SiteId
	SELECT * FROM [erv].[udf_GetSiteHierarchy] ('F4761A16-AB2F-41EE-B6FA-D17658DF2602', 1) ORDER BY HierarchyLevel, SiteId
	SELECT * FROM [erv].[udf_GetSiteHierarchy] ('72B0AA54-3969-4EA6-A581-5A8BCAEEBACE', 1) ORDER BY HierarchyLevel, SiteId
	SELECT * FROM [erv].[udf_GetSiteHierarchy] ('00000000-0000-0000-0000-000000000001', 0) ORDER BY HierarchyLevel, SiteId
	
*/

	CREATE FUNCTION [erv].[udf_GetSiteHierarchy]
	(
		@StartSiteGuid uniqueidentifier, @IncludeSites bit = 0
	)
	RETURNS @tblSiteGroupTree TABLE
	(
		SiteGuid uniqueidentifier
		, SiteId nvarchar(30)
		, SiteGroupFlag bit
		, HierarchyLevel int
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
	-- 1. @StartSiteGuid Site or sitegroup guid below which the hierarchy needs to be fetched. If a site guid (instead of a sitegroup guid) is provided, then the resultset only contains one entry for the given site guid.
	   2. @IncludeSites: 0: The resultset exclude the leaft site nodes. 1: The resultset includes the leaf site nodes.
	-- 3. 
	------------------------------------------------------------------------------------------------------
	*/
		DECLARE @siteAdminGuid uniqueidentifier
		SET @siteAdminGuid = '00000000-0000-0000-0000-000000000001'

		IF (@StartSiteGuid IS NULL)
		BEGIN
			IF (EXISTS (SELECT * FROM dbo.tblSites WHERE SiteGuid = @siteAdminGuid))
				SET @StartSiteGuid = @siteAdminGuid
			ELSE
			BEGIN
				RETURN;
			END
		END;

		WITH SiteHierarchy (SiteGuid, ParentSiteGuid, lvl)
		AS 
		(
			--Anchor
			SELECT TOP(1) ChildSiteGuid, ParentSiteGuid, 0
			FROM map.tblSiteToSite
			WHERE ChildSiteGuid = @StartSiteGuid -- node for which the tree structure underneath it needs to be retrieved.
			-- Recursive Call
			UNION ALL
			SELECT a.ChildSiteGuid, a.ParentSiteGuid, lvl + 1
			FROM map.tblSiteToSite a 
			INNER JOIN SiteHierarchy b
			ON a.ParentSiteGuid = b.SiteGuid
			AND a.ChildSiteGuid <> a.ParentSiteGuid
		)
		INSERT INTO @tblSiteGroupTree
		(SiteGuid, SiteId, SiteGroupFlag, HierarchyLevel)
		SELECT a.SiteGuid, b.ID SiteId, b.SiteGroupFlag, MAX(a.lvl)
		FROM SiteHierarchy a
		INNER JOIN tblSites b
		ON b.SiteGuid = a.SiteGuid
		LEFT OUTER JOIN tblSites c
		ON c.SiteGuid = a.ParentSiteGuid
		WHERE (((b.SiteGroupFlag = 1) AND (@IncludeSites = 0)) OR (@IncludeSites = 1))
		GROUP BY a.SiteGuid, b.ID, b.SiteGroupFlag
		OPTION (MAXRECURSION 10000);


		RETURN;
	END 