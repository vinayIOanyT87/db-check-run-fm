
/*
	DROP FUNCTION [erv].[udf_GetReverseSiteHierarchy]

	SELECT * FROM [erv].[udf_GetReverseSiteHierarchy] ('7E868C04-682B-4DDB-8089-7193B0D503B6', '3DB9DF78-DE73-4D19-8E9F-56CF8BEF6470') ORDER BY HierarchyLevel
	SELECT * FROM [erv].[udf_GetReverseSiteHierarchy] ('7E868C04-682B-4DDB-8089-7193B0D503B6', NULL) ORDER BY HierarchyLevel
	SELECT * FROM [erv].[udf_GetReverseSiteHierarchy] ('7E868C04-682B-4DDB-8089-7193B0D503B6', '292C9F0B-6FD6-4816-9838-34557EF69E12') ORDER BY HierarchyLevel
	SELECT * FROM [erv].[udf_GetReverseSiteHierarchy] ('9675BE6E-BD82-4BAE-842F-CC632E9D61F5', NULL) ORDER BY HierarchyLevel
	
*/

--SELECT * FROM map.tblSiteToSite WHERE ChildSiteGuid = '7E868C04-682B-4DDB-8089-7193B0D503B6'

CREATE FUNCTION [erv].[udf_GetReverseSiteHierarchy]
(
	@StartSiteGuid uniqueidentifier, @ParentSitegroupGuid uniqueidentifier
)
RETURNS @tblSiteHierarchyResult TABLE
(
	ParentSiteGuid uniqueidentifier
	, ChildSiteGuid uniqueidentifier
	, ParentSiteId nvarchar(30)
	, ChildSiteId nvarchar(30)
	, HierarchyLevel int
)
AS
BEGIN
/*
------------------------------------------------------------------------------------------------------
-- Function: [erv].[udf_GetReverseSiteHierarchy] 
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Function to return the site hieararchy in reverse, starting from a site up to a given sitegroup.
-- Notes:
-- 1. @StartSiteGuid Site or sitegroup guid from which to start climbing the site hierarchy.
	2. @ParentSitegroupGuid: Guid of the target sitegroup point at which to stop climbing the site hierarchy.
-- 3. 
------------------------------------------------------------------------------------------------------
*/
	DECLARE @siteAdminGuid uniqueidentifier
	SET @siteAdminGuid = '00000000-0000-0000-0000-000000000001'

	DECLARE @targetParentSiteGuid uniqueidentifier
	DECLARE @childSiteGuid uniqueidentifier
	DECLARE @hierarchylevel int
	DECLARE @maxHierarchylevel int
	DECLARE @tblSiteHierarchy TABLE
	(
		ParentSiteGuid uniqueidentifier
		, ChildSiteGuid uniqueidentifier
		, ReverseHierarchyLevel int
	)

	SET @targetParentSiteGuid = @ParentSitegroupGuid
	IF (@ParentSitegroupGuid IS NULL)
	BEGIN
		SET @targetParentSiteGuid = @siteAdminGuid
	END
		
	SET @childSiteGuid = @StartSiteGuid
	SET @hierarchylevel = 0
	WHILE ((SELECT COUNT(*) FROM map.tblSiteToSite WHERE ChildSiteGuid = @childSiteGuid) > 0)
	BEGIN
		
		INSERT INTO @tblSiteHierarchy
		(ParentSiteGuid, ChildSiteGuid, ReverseHierarchyLevel)
		SELECT ParentSiteGuid, ChildSiteGuid, @hierarchylevel FROM map.tblSiteToSite
		WHERE ChildSiteGuid = @childSiteGuid
		AND ParentSiteGuid <> ChildSiteGuid
			
		IF ((SELECT COUNT(*) FROM @tblSiteHierarchy WHERE ISNULL(ParentSiteGuid, @targetParentSiteGuid) = @targetParentSiteGuid) > 0)
		BEGIN
			BREAK
		END

		SET @childSiteGuid = (SELECT TOP(1) ParentSiteGuid FROM @tblSiteHierarchy WHERE ReverseHierarchyLevel = @hierarchyLevel)
		SET @hierarchylevel = @hierarchylevel + 1

	END

	SELECT @maxHierarchylevel = MAX(ReverseHierarchyLevel) FROM @tblSiteHierarchy	

	IF ((SELECT COUNT(*) FROM @tblSiteHierarchy WHERE ParentSiteGuid = @targetParentSiteGuid) > 0)
	BEGIN
		INSERT INTO @tblSiteHierarchyResult
		(ParentSiteGuid, ChildSiteGuid, ParentSiteId, ChildSiteId, HierarchyLevel)
		SELECT a.ParentSiteGuid, a.ChildSiteGuid, b.Id, c.Id, (ISNULL(@maxHierarchyLevel, 0) - a.ReverseHierarchyLevel) FROM @tblSiteHierarchy a
		INNER JOIN tblSites b ON b.SiteGuid = a.ParentSiteGuid
		INNER JOIN tblSites c ON c.SiteGuid = a.ChildSiteGuid
	END

	RETURN;
END
GO


