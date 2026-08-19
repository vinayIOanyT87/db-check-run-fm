-- ===========================================================================================
-- Author:		<Author,,Peters George C>
-- Create Date:	<Create Date,,03-18-2016>
-- Description:	<Description,,This function creates a Site Dependency Graph for the
-- SiteGuid passed in.  Ancestors represent the SiteGroups that the SiteGuid is a member of OR 
-- is a descendant of.  If the passed in SiteGuid is for a SiteGroup, then the descendants 
-- represent the SiteGroups or Sites that are members of the SiteGroup.  Typically there will
-- not be any Descendants if the passed in SiteGuid represents a Site.
--
-- The SiteGuid will always have a SiteTreeLevel of 0.  Any Ancestors will have a negative (-)
-- SiteTreeLevel and any Descendants will have a positive (+) SiteTreeLevel.
-- ===========================================================================================

/* {CheckPoint: CREATING FUNCTION: udf_GetSiteToSiteHierarchyListForSiteGuid } */
CREATE FUNCTION [dbo].[udf_GetSiteToSiteHierarchyListForSiteGuid](
    @SiteGuid uniqueidentifier = NULL,
	@ExcludeSiteGroups bit = 1,
	@ExcludeSites bit = 1,
	@ExcludeParentTree bit = 0,
	@ExcludeChildTree bit = 0,
	@OnlyImmediateParents bit = 0,
	@OnlyImmediateChildren bit = 0
)
RETURNS @tblSiteTreeList TABLE
(	
	[RowIndex] bigint IDENTITY(1,1)
	,[SiteGuid] uniqueidentifier
	,[SiteID] nvarchar(60)
	,[Level] int
	,[SiteGroupFlag] bit
	,[NodeType] nvarchar(25)
	,[EnablePeriodicSyncFlag] bit
	,[PeriodicSyncIntervalMinutes] int
)
AS
BEGIN
	DECLARE @SiteID nvarchar(30)
	DECLARE @IsSiteGroup bit
	DECLARE @EnablePeriodicSyncFlag bit
	DECLARE @PeriodicSyncIntervalMinutes int
	DECLARE @DebugTrace nvarchar(512)

	SET @IsSiteGroup = 0
	SET @EnablePeriodicSyncFlag = 0
	SET @PeriodicSyncIntervalMinutes = 0

	SELECT @SiteID = ID
			, @IsSiteGroup = SiteGroupFlag 
			, @EnablePeriodicSyncFlag = EnablePeriodicSyncFlag
			, @PeriodicSyncIntervalMinutes = PeriodicSyncIntervalMinutes FROM dbo.tblSites WHERE SiteGuid = @SiteGuid;

	-- Every Site/SiteGroup is mapped to itself and also to SiteAdmin.  
	-- During the recursive call that walks up the SiteToSite mapping, we don't want to include SiteAdmin unless we are directly below it.
	DECLARE @ImmediateAncestor TABLE 
	(
		SiteGuid uniqueidentifier
		,SiteID nvarchar(60)
		,ParentSiteGuid uniqueidentifier
		,ParentSiteID nvarchar(60)
		,LVL nvarchar(200)
		,Hierarchy nvarchar(2048)
		,RecursionLevel int
		,SiteGroupFlag bit
		,NodeType nvarchar(25)
		,EnablePeriodicSyncFlag bit
		,PeriodicSyncIntervalMinutes int
	);

	DECLARE @ImmediateDescendant TABLE 
	(
		SiteGuid uniqueidentifier
		,SiteID nvarchar(60)
		,ChildSiteGuid uniqueidentifier
		,ChildSiteID nvarchar(60)
		,LVL nvarchar(200)
		,Hierarchy nvarchar(2048)
		,RecursionLevel int
		,SiteGroupFlag bit
		,NodeType nvarchar(25)
		,EnablePeriodicSyncFlag bit
		,PeriodicSyncIntervalMinutes int
	);

	-- The SiteID and corresponding SiteGuid must exist in order to establish any SiteToSite relationship.
	IF (@SiteID IS NOT NULL AND @SiteGuid IS NOT NULL)
	BEGIN
		INSERT INTO @tblSiteTreeList (SiteGuid, SiteID, EnablePeriodicSyncFlag, PeriodicSyncIntervalMinutes, Level, NodeType, SiteGroupFlag) VALUES ('00000000-0000-0000-0000-000000000001', 'SiteAdmin', 0, 0, 0, 'Root', 1)

		DECLARE @SiteGroupCount int

		-- Identify how many SiteGroups the SiteGuid is a member of.
		SELECT @SiteGroupCount = count(*) 
			FROM map.tblSiteToSite 
			WHERE (ChildSiteGuid = @SiteGuid)
					AND (ChildSiteGuid <> ParentSiteGuid)

		IF (@SiteGroupCount = 0)
		BEGIN
			-- SiteID isn't a child of anyone so it must be a SiteGroup or SiteAdmin - Root
			INSERT INTO @ImmediateAncestor 
			SELECT  CAST(@SiteGuid AS uniqueidentifier) AS SiteGuid
					,CAST(@SiteID AS nvarchar(60)) AS SiteID
					,CAST(NULL AS uniqueidentifier) AS ParentSiteGuid
					,CAST(NULL AS nvarchar(60)) AS ParentSiteID
					,CAST('' AS nvarchar(200)) AS LVL
					,CAST((CAST(@SiteGuid AS nvarchar(60)) + ':0') AS nvarchar(2048)) AS Hierarchy
					,0 AS RecursionLevel
					,CAST(1 AS Bit) AS SiteGroupFlag
					,CAST('Root' AS nvarchar(25)) AS NodeType
					,CAST(@EnablePeriodicSyncFlag AS bit) AS EnablePeriodicSyncFlag
					,CAST(@PeriodicSyncIntervalMinutes AS int) AS PeriodicSyncIntervalMinutes
			END
		ELSE IF (@SiteGroupCount = 1)
		BEGIN
			-- SiteID only belongs to a single SiteGroup.  Return the SiteGroup. SiteAdmin could be our immediate parent if only one parent exists.
			INSERT INTO @ImmediateAncestor 
			SELECT  ChildSiteGuid AS SiteGuid
					,childSite.ID AS SiteID
					,ParentSiteGuid AS ParentSiteGuid
					,parentSite.ID AS ParentSiteID
					,CAST('' AS nvarchar(200)) AS LVL
					,CAST(CAST(@SiteGuid AS nvarchar(60)) + ':-1:' + CAST(ParentSiteGuid as nvarchar(60)) AS nvarchar(2048)) AS Hierarchy
					,CAST(-1 AS int) AS RecursionLevel
					,childSite.SiteGroupFlag AS SiteGroupFlag
					,CAST('SiteGroup' AS nvarchar(25)) AS NodeType
					,CAST(parentSite.EnablePeriodicSyncFlag AS bit) AS EnablePeriodicSyncFlag
					,CAST(parentSite.PeriodicSyncIntervalMinutes AS int) AS PeriodicSyncIntervalMinutes
				FROM map.tblSiteToSite map
						LEFT OUTER JOIN dbo.tblSites childSite
							ON map.ChildSiteGuid = childSite.SiteGuid
						LEFT OUTER JOIN dbo.tblSites parentSite
							ON map.ParentSiteGuid = parentSite.SiteGuid
				WHERE (ChildSiteGuid = @SiteGuid) AND (@SiteID = 'SiteAdmin' OR (ChildSiteGuid <> ParentSiteGuid))
		END
		ELSE
		BEGIN
			-- SiteID belongs to one or more SiteGroups which did not include SiteAdmin or self-relationships.
			INSERT INTO @ImmediateAncestor 
			SELECT  ChildSiteGuid AS SiteGuid
					,childSite.ID AS SiteID
					,ParentSiteGuid AS ParentSiteGuid
					,parentSite.ID AS ParentSiteID
					,CAST('' AS nvarchar(200)) AS LVL
					,CAST(CAST(@SiteGuid AS nvarchar(60)) + ':-1:' + CAST(ParentSiteGuid as nvarchar(60)) AS nvarchar(2048)) AS Hierarchy
					,CAST((CASE WHEN childSite.SiteGroupFlag = 0 OR childSite.SiteGroupFlag IS NULL THEN -1 ELSE -1 END ) AS int) AS RecursionLevel
					,childSite.SiteGroupFlag AS SiteGroupFlag
					,CAST((CASE WHEN childSite.SiteGroupFlag = 0 OR childSite.SiteGroupFlag IS NULL THEN 'Site' ELSE 'SiteGroup' END) AS nvarchar(25)) AS NodeType
					,CAST(parentSite.EnablePeriodicSyncFlag AS bit) AS EnablePeriodicSyncFlag
					,CAST(parentSite.PeriodicSyncIntervalMinutes AS int) AS PeriodicSyncIntervalMinutes
				FROM map.tblSiteToSite map
						LEFT OUTER JOIN dbo.tblSites childSite
							ON map.ChildSiteGuid = childSite.SiteGuid
						LEFT OUTER JOIN dbo.tblSites parentSite
							ON map.ParentSiteGuid = parentSite.SiteGuid
				WHERE (ChildSiteGuid = @SiteGuid)
						AND (ChildSiteGuid <> ParentSiteGuid)
						AND (ParentSiteGuid <> '00000000-0000-0000-0000-000000000001')
		END

		--Based on the tblSite Record for this SiteID, if we are a SiteGroup then we need to look for any immediate descendants.
		IF (@IsSiteGroup = 1)
		BEGIN
			DECLARE @SiteMemberCount int
			
			-- See if we are a parent of any Site/SiteGroup
			SELECT @SiteMemberCount = count(*) 
				FROM map.tblSiteToSite 
				WHERE (ParentSiteGuid = @SiteGuid)
						AND (ParentSiteGuid <> ChildSiteGuid)

			-- There is one Site Member.
			IF (@SiteMemberCount = 1)
			BEGIN
				INSERT INTO @ImmediateDescendant
				SELECT  ParentSiteGuid AS SiteGuid
						,parentSite.ID AS SiteID
						,ChildSiteGuid AS ChildSiteGuid
						,childSite.ID AS ChildSiteID
						,CAST('' as nvarchar(200)) AS LVL
						,CAST(CAST(@SiteGuid as nvarchar(60)) + ':1:' + CAST(ChildSiteGuid as nvarchar(60)) as nvarchar(2048)) AS Hierarchy
						,CAST(1 AS int) AS RecursionLevel
						,childSite.SiteGroupFlag AS SiteGroupFlag
						,CAST('SiteGroup' AS nvarchar(25)) AS NodeType
						,CAST(childSite.EnablePeriodicSyncFlag AS bit) AS EnablePeriodicSyncFlag
						,CAST(childSite.PeriodicSyncIntervalMinutes AS int) AS PeriodicSyncIntervalMinutes
					FROM map.tblSiteToSite map
							LEFT OUTER JOIN dbo.tblSites childSite
								ON map.ChildSiteGuid = childSite.SiteGuid
							LEFT OUTER JOIN dbo.tblSites parentSite
								ON map.ParentSiteGuid = parentSite.SiteGuid
					WHERE (ParentSiteGuid = @SiteGuid) AND (@SiteGuid = '00000000-0000-0000-0000-000000000001' OR ChildSiteGuid <> ParentSiteGuid)
			END
			ELSE
			BEGIN
				-- There are multiple sites that we are a parent of (and that are not self mapping)
				INSERT INTO @ImmediateDescendant
				SELECT  ParentSiteGuid AS SiteGuid
						,parentSite.ID AS SiteID
						,ChildSiteGuid AS ChildSiteGuid
						,childSite.ID AS ChildSiteID
						,CAST('' AS nvarchar(200)) AS LVL
						,CAST(CAST(@SiteGuid AS nvarchar(60)) + ':1:' + CAST(ChildSiteGuid as nvarchar(60)) as nvarchar(2048)) AS Hierarchy
						,CAST(1 AS int) AS RecursionLevel
						,childSite.SiteGroupFlag AS SiteGroupFlag
						,CAST('SiteGroup' AS nvarchar(25)) AS NodeType
						,CAST(childSite.EnablePeriodicSyncFlag AS bit) AS EnablePeriodicSyncFlag
						,CAST(childSite.PeriodicSyncIntervalMinutes AS int) AS PeriodicSyncIntervalMinutes
					FROM map.tblSiteToSite map
							LEFT OUTER JOIN dbo.tblSites childSite
								ON map.ChildSiteGuid = childSite.SiteGuid
							LEFT OUTER JOIN dbo.tblSites parentSite
								ON map.ParentSiteGuid = parentSite.SiteGuid
					WHERE (ParentSiteGuid = @SiteGuid)
							AND (ChildSiteGuid <> ParentSiteGuid)
			END
		END
	END
	ELSE
	BEGIN
		INSERT INTO @ImmediateAncestor 
		SELECT  CAST(NULL AS uniqueidentifier) AS SiteGuid
				,CAST(NULL AS nvarchar(60)) AS SiteID
				,CAST(NULL AS uniqueidentifier) AS ParentSiteGuid
				,CAST(NULL AS nvarchar(60)) AS ParentSiteID
				,CAST(NULL AS nvarchar(200)) AS LVL
				,CAST(NULL AS nvarchar(2048)) AS Hierarchy
				,CAST(NULL AS int) AS RecursionLevel
				,CAST(NULL AS bit) AS SiteGroupFlag
				,CAST(NULL AS nvarchar(25)) AS NodeType
				,CAST(NULL AS bit) AS EnablePeriodicSyncFlag
				,CAST(NULL AS int) AS PeriodicSyncIntervalMinutes

		INSERT INTO @ImmediateDescendant
		SELECT  CAST(NULL AS uniqueidentifier) AS SiteGuid
				,CAST(NULL AS nvarchar(60)) AS SiteID
				,CAST(NULL AS uniqueidentifier) AS ChildSiteGuid
				,CAST(NULL AS nvarchar(60)) AS ChildSiteID
				,CAST(NULL AS nvarchar(200)) AS LVL
				,CAST(NULL AS nvarchar(2048)) AS Hierarchy
				,CAST(NULL AS int) AS RecursionLevel
				,CAST(NULL AS bit) AS SiteGroupFlag
				,CAST(NULL AS nvarchar(25)) AS NodeType
				,CAST(NULL AS bit) AS EnablePeriodicSyncFlag
				,CAST(NULL AS int) AS PeriodicSyncIntervalMinutes
	END

	-- FlatSiteMap identifies all of the Sites and any relationship they have with another Site/SiteGroup.  
	-- We are creating a union of two queries, one where the Site acts as the Child and the other where the Site acts as the Parent.
	-- This must be done because a Site will never be a Parent so we wouldn't see any of these records.
	-- We could have attempted to inner join to both sides of the mapping table for each site record but then we would 
	-- have CASE conditions to determine which value to return in the columns.
	; WITH FlatSiteMap AS (
		SELECT ChildSiteGuid
				,ChildSiteID
				,ParentSiteGuid
				,ParentSiteID
				,SiteGuid
				,SiteID
				,SiteGroupFlag
				,CAST((CASE WHEN (ParentSiteGuid IS NULL AND SiteGroupFlag = 1) OR SiteGroupFlag IS NULL  THEN 'Root' 
						   WHEN ParentSiteGuid IS NOT NULL AND SiteGroupFlag = 1 THEN 'SiteGroup' 
						   ELSE 'Site' END) AS nvarchar(25)) AS NodeType
				,EnablePeriodicSyncFlag
				,PeriodicSyncIntervalMinutes
				FROM (SELECT s.SiteGuid AS SiteGuid
							,s.ID AS SiteID
							,s.SiteGuid AS ChildSiteGuid
							,s.ID AS ChildSiteID
							,map.ParentSiteGuid AS ParentSiteGuid
							,parentSite.ID AS ParentSiteID
							,s.SiteGroupFlag AS SiteGroupFlag
							,s.EnablePeriodicSyncFlag AS EnablePeriodicSyncFlag
							,s.PeriodicSyncIntervalMinutes AS PeriodicSyncIntervalMinutes
							FROM dbo.tblSites s
								LEFT OUTER JOIN map.tblSiteToSite map
									ON s.SiteGuid = map.ChildSiteGuid
								LEFT OUTER JOIN dbo.tblSites parentSite
									ON map.ParentSiteGuid = parentSite.SiteGuid
									) as FullMap
		WHERE ParentSiteGuid IS NULL OR SiteGuid <> ParentSiteGuid
		UNION
		SELECT  ChildSiteGuid
				,ChildSiteID
				,ParentSiteGuid
				,ParentSiteID
				,SiteGuid
				,SiteID
				,SiteGroupFlag
				,CAST((CASE WHEN (ChildSiteID IS NULL) OR ((ChildSiteGuid <> ParentSiteGuid) AND (SiteGroupFlag IS NULL OR SiteGroupFlag = 0)) THEN 'Site' 
							ELSE 'SiteGroup' END ) AS nvarchar(25)) AS NodeType
				,EnablePeriodicSyncFlag
				,PeriodicSyncIntervalMinutes
				FROM (SELECT s.SiteGuid AS SiteGuid
							,s.ID AS SiteID
							,map.ChildSiteGuid AS ChildSiteGuid
							,childSite.ID AS ChildSiteID
							,s.SiteGuid AS ParentSiteGuid
							,s.ID AS ParentSiteID
							,s.SiteGroupFlag AS SiteGroupFlag
							,s.EnablePeriodicSyncFlag AS EnablePeriodicSyncFlag
							,s.PeriodicSyncIntervalMinutes AS PeriodicSyncIntervalMinutes
							FROM dbo.tblSites s
								LEFT OUTER JOIN map.tblSiteToSite map
									ON s.SiteGuid = map.ParentSiteGuid AND s.SiteGuid <> map.ChildSiteGuid
								LEFT OUTER JOIN dbo.tblSites childSite
									ON map.ChildSiteGuid = childSite.SiteGuid
						WHERE (s.SiteGroupFlag = 1 AND childSite.SiteGroupFlag = 1) 
								OR (s.SiteGroupFlag = 0)) as FullMap
		WHERE (ChildSiteGuid IS NULL
			OR SiteGuid <> ChildSiteGuid)
	)
	,Ancestors AS (
		SELECT CAST(ParentSiteGuid AS uniqueidentifier) AS RelatedSiteGuid
				,CAST(ParentSiteID AS nvarchar(60)) AS RelatedSiteID
				,CAST(SiteGuid AS uniqueidentifier) AS SiteGuid
				,CAST(SiteID AS nvarchar(60)) AS SiteID
				,CAST(LVL AS nvarchar(200)) AS LVL
				,CAST(Hierarchy AS nvarchar(2048)) AS Hierarchy
				,RecursionLevel AS RecursionLevel
				,CAST(SiteGroupFlag AS bit) AS SiteGroupFlag
				,CAST((CASE WHEN (ParentSiteGuid IS NULL AND SiteGroupFlag = 1) OR SiteGroupFlag IS NULL  THEN 'Root' 
						   WHEN ParentSiteGuid IS NOT NULL THEN 'Parent' 
						   ELSE 'Child' END) AS nvarchar(25)) AS NodeType
				,CAST(EnablePeriodicSyncFlag AS bit) AS EnablePeriodicSyncFlag
				,CAST(PeriodicSyncIntervalMinutes AS int) AS PeriodicSyncIntervalMinutes
			FROM @ImmediateAncestor 
	)
	, Descendants AS (
		SELECT CAST(ChildSiteGuid AS uniqueidentifier) AS RelatedSiteGuid
				,CAST(ChildSiteID AS nvarchar(60)) AS RelatedSiteID
				,CAST(SiteGuid AS uniqueidentifier) AS SiteGuid
				,CAST(SiteID AS nvarchar(60)) AS SiteID
				,CAST(LVL AS nvarchar(200)) AS LVL
				,CAST(Hierarchy AS nvarchar(2048)) AS Hierarchy
				,RecursionLevel AS RecursionLevel
				,CAST(SiteGroupFlag AS bit) AS SiteGroupFlag
				,CAST('Child' AS nvarchar(25)) AS NodeType
				,CAST(EnablePeriodicSyncFlag AS bit) AS EnablePeriodicSyncFlag
				,CAST(PeriodicSyncIntervalMinutes AS int) AS PeriodicSyncIntervalMinutes
			FROM @ImmediateDescendant
	)
	,RecursiveAncestors_CTE AS (
		SELECT * FROM Ancestors
		UNION ALL
		SELECT
			CAST((CASE WHEN grandParent.ParentSiteGuid IS NULL THEN grandParent.SiteGuid ELSE grandParent.ParentSiteGuid END) AS uniqueidentifier) AS RelatedSiteGuid
			,CAST(LVL + (CASE WHEN grandParent.ParentSiteID IS NULL THEN grandParent.SiteID ELSE grandParent.ParentSiteID END ) AS nvarchar(60)) AS RelatedSiteID
			,CAST(parent.RelatedSiteGuid AS uniqueidentifier) AS SiteGuid
			,CAST(parent.RelatedSiteID AS nvarchar(60)) AS SiteID
			,CAST(LVL AS nvarchar(200)) AS LVL
			,CAST(Hierarchy + ':' + CAST(RecursionLevel - 1 as nvarchar(20)) + ':' + CAST(grandParent.ParentSiteGuid as nvarchar(60)) AS nvarchar(2048)) AS Hierarchy
			,RecursionLevel - 1 AS RecursionLevel
			,parent.SiteGroupFlag AS SiteGroupFlag
			,CAST((CASE WHEN (grandParent.ParentSiteGuid IS NULL AND grandParent.SiteGroupFlag = 1) OR grandParent.SiteGroupFlag IS NULL  THEN 'Root' 
						   WHEN grandParent.ParentSiteGuid IS NOT NULL AND grandParent.SiteGroupFlag = 1 THEN 'Parent' 
						   ELSE 'Child' END) AS nvarchar(25)) AS NodeType
			,CAST((CASE WHEN grandParent.ParentSiteGuid IS NULL THEN grandParent.EnablePeriodicSyncFlag ELSE grandParent.EnablePeriodicSyncFlag END) AS bit) AS EnablePeriodicSyncFlag
			,CAST((CASE WHEN grandParent.ParentSiteGuid IS NULL THEN grandParent.PeriodicSyncIntervalMinutes ELSE grandParent.PeriodicSyncIntervalMinutes END) AS int) AS PeriodicSyncIntervalMinutes
		FROM RecursiveAncestors_CTE parent
			INNER JOIN FlatSiteMap grandParent
				ON parent.RelatedSiteGuid = grandParent.SiteGuid
		WHERE (grandParent.SiteGuid <> grandParent.ParentSiteGuid)
	)
	,RecursiveDescendants_CTE AS (
		SELECT * FROM Descendants
		UNION ALL
		SELECT
			CAST((CASE WHEN grandChild.ChildSiteGuid IS NULL THEN grandChild.SiteGuid ELSE grandChild.ChildSiteGuid END) AS uniqueidentifier) AS RelatedSiteGuid
			,CAST(LVL + (CASE WHEN grandChild.ChildSiteID IS NULL THEN grandChild.SiteID ELSE grandChild.ChildSiteID END ) AS nvarchar(60)) AS RelatedSiteID
			,CAST(child.RelatedSiteGuid AS uniqueidentifier) AS SiteGuid
			,CAST(child.RelatedSiteID AS nvarchar(60)) AS SiteID
			,CAST(LVL as nvarchar(200)) AS LVL
			,CAST(Hierarchy + ':' + CAST(RecursionLevel + 1 as nvarchar(20)) + ':' + CAST(grandChild.ChildSiteGuid as nvarchar(60)) AS nvarchar(2048)) AS Hierarchy
			,RecursionLevel + 1 AS RecursionLevel
			,grandChild.SiteGroupFlag AS SiteGroupFlag
			,CAST((CASE WHEN (grandChild.ChildSiteGuid IS NULL AND grandChild.SiteGroupFlag = 1) OR grandChild.SiteGroupFlag IS NULL  THEN 'Root' 
						   WHEN grandChild.ChildSiteGuid IS NOT NULL AND grandChild.SiteGroupFlag = 1 THEN 'Parent' 
						   ELSE 'Child' END) AS nvarchar(25)) AS NodeType
			,CAST((CASE WHEN grandChild.ChildSiteGuid IS NULL THEN grandChild.EnablePeriodicSyncFlag ELSE grandChild.EnablePeriodicSyncFlag END) AS bit) AS EnablePeriodicSyncFlag
			,CAST((CASE WHEN grandChild.ChildSiteGuid IS NULL THEN grandChild.PeriodicSyncIntervalMinutes ELSE grandChild.PeriodicSyncIntervalMinutes END) AS int) AS PeriodicSyncIntervalMinutes
		FROM RecursiveDescendants_CTE child
			INNER JOIN FlatSiteMap grandChild
				ON child.RelatedSiteGuid = grandChild.ParentSiteGuid
		WHERE (grandChild.ChildSiteGuid IS NOT NULL)
	)
	,LastDescendant_CTE AS (
		-- We do this so that we can identify the LOWEST level in the SiteTree where a Site appears.  This ultimately determines
		-- when it is able to be synchronized
		SELECT RelatedSiteGuid
				, Max(RecursionLevel) RecursionLevel
		FROM RecursiveDescendants_CTE
		GROUP BY RelatedSiteGuid, RelatedSiteID
	)
	, SiteTree_CTE AS (
		-- This CTE produces the final SiteTree.  Ancestors, Self, Descendants
		-- It includes all Ancestors up to SiteAdmin (removes duplicate SiteAdmin branches and only returns the highest entry for SiteAdmin)
		-- It adds back the original Site/SiteGroup that was passed in
		-- It includes all descendants (removes duplicate branches for each Site/SiteGroup and only returns the lowest entry in the SiteTree where a descendant appears)
		--SELECT RelatedSiteGuid AS SiteGuid
		--		,RelatedSiteID AS SiteID
		--		,RecursionLevel AS Level
		--		,NodeType AS NodeType
		--		,EnablePeriodicSyncFlag AS EnablePeriodicSyncFlag
		--		,PeriodicSyncIntervalMinutes AS PeriodicSyncIntervalMinutes
		--		FROM RecursiveAncestors_CTE
		--		WHERE RelatedSiteGuid = '00000000-0000-0000-0000-000000000001' AND RecursionLevel = (SELECT MIN(RecursionLevel) FROM RecursiveAncestors_CTE)
		--UNION
		SELECT RelatedSiteGuid AS SiteGuid
				,RelatedSiteID AS SiteID
				,RecursionLevel AS Level
				,SiteGroupFlag AS SiteGroupFlag
				,NodeType AS NodeType
				,EnablePeriodicSyncFlag AS EnablePeriodicSyncFlag
				,PeriodicSyncIntervalMinutes AS PeriodicSyncIntervalMinutes
				FROM RecursiveAncestors_CTE
				WHERE RelatedSiteGuid <> '00000000-0000-0000-0000-000000000001'
		UNION
		SELECT CAST(SiteGuid AS uniqueidentifier) AS SiteGuid
				,CAST(ID AS nvarchar(60)) AS SiteID
				,0 AS Level
				,SiteGroupFlag AS SiteGroupFlag
				,CAST((CASE WHEN (SiteGroupFlag = 1 AND ID = 'SiteAdmin') THEN 'Root' 
						   ELSE 'Self' END) AS nvarchar(25)) AS NodeType
				,EnablePeriodicSyncFlag AS EnablePeriodicSyncFlag
				,PeriodicSyncIntervalMinutes AS PeriodicSyncIntervalMinutes
			FROM dbo.tblSites WHERE ID = @SiteID
		UNION
		SELECT cte.RelatedSiteGuid AS SiteGuid
				,cte.RelatedSiteID AS SiteID
				,cte.RecursionLevel AS Level
				,cte.SiteGroupFlag AS SiteGroupFlag
				,cte.NodeType AS NodeType
				,cte.EnablePeriodicSyncFlag AS EnablePeriodicSyncFlag
				,cte.PeriodicSyncIntervalMinutes AS PeriodicSyncIntervalMinutes
				FROM LastDescendant_CTE ld 
					INNER JOIN RecursiveDescendants_CTE cte
						ON ld.RelatedSiteGuid = cte.RelatedSiteGuid AND ld.RecursionLevel = cte.RecursionLevel

	)
	INSERT INTO @tblSiteTreeList ([SiteGuid]
									,[SiteID]
									,[Level]
									,[SiteGroupFlag]
									,[NodeType]
									,[EnablePeriodicSyncFlag]
									,[PeriodicSyncIntervalMinutes]) 
									SELECT SiteGuid
											, SiteID
											, Level
											, SiteGroupFlag
											, NodeType
											, EnablePeriodicSyncFlag
											, PeriodicSyncIntervalMinutes 
											FROM SiteTree_CTE ORDER BY Level;

	--IF EXISTS (SELECT 1 FROM @tblSiteSyncList WHERE SiteID = 'SiteAdmin')
	--BEGIN
	--	DELETE FROM @tblSiteSyncList WHERE SiteId = 'SiteAdmin'
	--END

	DECLARE @minLevel int
	SELECT @minLevel = MIN(Level) - 1 FROM @tblSiteTreeList;
	
	-- Update the SiteAdmin entry so that it has the correct level
	UPDATE @tblSiteTreeList SET Level = @minLevel WHERE SiteGuid = '00000000-0000-0000-0000-000000000001'

	-- We need to be careful with multiple parent trees that share the same SiteGroups further up the hiearchy.  If a SiteGroup appears more than once, we want to keep the entry for that SiteGroup
	-- that is closest to SiteAdmin.  This ensures that the SiteGroup will be synchronized early enough to support any SiteGroup that is a member of it.
	DELETE FROM @tblSiteTreeList WHERE RowIndex in (
	SELECT RowIndex FROM @tblSiteTreeList ml
		INNER JOIN (SELECT data1.SiteGuid, min(data1.Level) 'Level'
						FROM @tblSiteTreeList data1
							INNER JOIN (SELECT SiteGuid FROM @tblSiteTreeList GROUP BY SiteGuid HAVING COUNT(*) > 1) data2
								ON data1.SiteGuid = data2.SiteGuid 
						WHERE data1.Level <> 0
						GROUP BY data1.SiteGuid) keepList
			ON ml.SiteGuid = keepList.SiteGuid AND ml.Level <> keepList.Level);

	IF (@ExcludeSiteGroups IS NOT NULL AND @ExcludeSiteGroups = 1)
		DELETE FROM @tblSiteTreeList WHERE SiteGroupFlag = 1;

	IF (@ExcludeSites IS NOT NULL AND @ExcludeSites = 1)
		DELETE FROM @tblSiteTreeList WHERE SiteGroupFlag = 0;

	IF (@ExcludeParentTree IS NOT NULL AND @ExcludeParentTree = 1)
		DELETE FROM @tblSiteTreeList WHERE Level < 0;

	IF (@ExcludeChildTree IS NOT NULL AND @ExcludeChildTree = 1)
		DELETE FROM @tblSiteTreeList WHERE Level > 0;

	IF (@OnlyImmediateParents IS NOT NULL AND @OnlyImmediateParents = 1)
		DELETE FROM @tblSiteTreeList WHERE Level < -1;

	IF (@OnlyImmediateChildren IS NOT NULL AND @OnlyImmediateChildren = 1)
		DELETE FROM @tblSiteTreeList WHERE Level > 1;

	RETURN;
END

GO
