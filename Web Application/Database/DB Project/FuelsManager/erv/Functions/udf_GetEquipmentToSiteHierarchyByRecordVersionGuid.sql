

/*
	SELECT * FROM [erv].[udf_GetEquipmentToSiteHierarchyByRecordVersionGuid] ('b44649ad-877a-4a41-93b1-9b0e048be377') ORDER BY HierarchyLevel, SiteId
	SELECT * FROM [erv].[udf_GetEquipmentToSiteHierarchyByRecordVersionGuid] ('117FF81D-8FCF-4456-80FA-53AC8C63A1FB') ORDER BY HierarchyLevel, SiteId	
*/



	CREATE FUNCTION [erv].[udf_GetEquipmentToSiteHierarchyByRecordVersionGuid]
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
	/*
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetEquipmentToSiteHierarchyByRecordVersionGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to return the site hierarchy below a given sitegroup based on the assignments of a given entity record version from its owner sitegroup. The owner sitegroup is included in the result set and is set with a hierarchy level of 0.
	-- Notes:
	-- 1. @EntityGuid: Exact Guid of the entity record version for which the assignment tree is to be queried.  This is NOT the MasterRecordVersion Guid.
	-- 2. 
	------------------------------------------------------------------------------------------------------
	*/

		DECLARE @startSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		SELECT @startSiteGuid = SiteGuid, @masterRecordGuid = _MasterRecordGuid FROM tblEquipment WHERE EquipmentGuid = @EntityGuid;


		WITH SiteHierarchy (SiteGuid, ParentSiteGuid, lvl)
		AS 
		(
			--Anchor
			SELECT TOP(1)SiteGuid, AssignedFromSiteGuid, 0
			FROM map.tblEntityEquipmentToSite
			WHERE EquipmentGuid = @masterRecordGuid
			AND SiteGuid = @StartSiteGuid -- node for which the tree structure underneath it needs to be retrieved.
			-- Recursive Call
			UNION ALL
			SELECT a.SiteGuid, a.AssignedFromSiteGuid, lvl + 1
			FROM map.tblEntityEquipmentToSite a 
			INNER JOIN SiteHierarchy b
			ON a.AssignedFromSiteGuid = b.SiteGuid
			WHERE EquipmentGuid = @masterRecordGuid
			AND a.SiteGuid <> a.AssignedFromSiteGuid
		)
		
		INSERT INTO @tblSiteGroupTree
		(SiteGuid, SiteId, SiteGroupFlag, AssignedFromSiteGuid, AssignedFromSiteId, HierarchyLevel)
		SELECT a.SiteGuid, b.id SiteId, b.SiteGroupFlag, a.ParentSiteGuid, c.ID, a.lvl
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