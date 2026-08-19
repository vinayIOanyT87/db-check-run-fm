CREATE FUNCTION [map].[udf_GetModuleToSiteHierarchyByAssignment]
(
	@EntityRecordGuid uniqueidentifier,
	@AssignedFromSiteGuid uniqueidentifier,
	@AssignedToSiteGuid uniqueidentifier
)
RETURNS @tblAssignmentHierarchy TABLE
(
	MappingGuid uniqueidentifier
	, SiteGuid uniqueidentifier
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
	-- Function: [map].[udf_GetModuleToSiteHierarchyByAssignment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves the ModuleToSite assignment hierarchy below a given Module assignment. 	
	-- Notes:
	-- 1. @EntityRecordGuid: Guid of the Module record for which the mapping hierarchy is to be retrieved.
	-- 2. @AssignedFromSiteGuid: Guid of the AssignedFrom site/sitegroup for which the mapping is to be retrieved
	-- 3. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup for which the mapping is to be retrieved
	-- 4. The hierarchy includes every subsequent assignment that has been made possible by the given assigment. The hierarchy also includes the given assigment record itself.
	-- 5. The given/starting assignment record can be specified either by a combination of EntityRecordGuid and AssignedToSiteGuid or by a combination of EntityRecordGuid and AssignedFromSiteGuid.
	-- 6. The Stored Procedure supports a combination of EntityRecordGuid and AssignedFromSiteGuid, with a null AssignedToSiteGuid, i.e. the case where there are multiple starting assignment records.
	-- 7. If both the @AssignedToSiteGuid and the @AssignedFromSiteGuid are null, then the owner siteguid of the master record is used as the @AssignedToSiteGuid and the @AssignedFromSiteGuid, i.e. the base mapping is used as the starting point.
	-- 8. If the @AssignedFromSiteGuid is null but the @AssignedToSiteGuid is provided, then it is simply derived from the AssignedFromSiteGuid value of the corresponding mapping entry for which the AssignedTositeGuid is equal to the @AssignedToSiteGuid.
	-- 9. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	-- 10. The base mapping of the entity record is excluded from the result set, i.e. the assignment mapping that maps the entity record from the owner site/sitegroup to the owner site/sitegroup itself.
	------------------------------------------------------------------------------------------------------
	*/

	IF ((@AssignedFromSiteGuid IS NULL) AND (@AssignedToSiteGuid IS NULL))
	BEGIN
		SELECT @AssignedFromSiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = @EntityRecordGuid
		SET @AssignedToSiteGuid = @AssignedFromSiteGuid
	END;

	IF (@AssignedFromSiteGuid IS NULL)
	BEGIN
		SELECT @AssignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityModuleToSite WHERE ModuleGuid = @EntityRecordGuid AND SiteGuid = @AssignedToSiteGuid
	END;

	WITH AssignmentHierarchy (MappingGuid, AssignedFromSiteGuid, AssignedToSiteGuid, lvl)
	AS 
	(
		--Anchor (node/s for which the underneath tree structure needs to be retrieved.)
		SELECT ModuleToSiteGuid, AssignedFromSiteGuid, SiteGuid, 0
		FROM map.tblEntityModuleToSite
		WHERE ModuleGuid = @EntityRecordGuid
		AND ((AssignedFromSiteGuid = @AssignedFromSiteGuid) OR (@AssignedFromSiteGuid IS NULL))
		AND ((SiteGuid = @AssignedToSiteGuid) OR (@AssignedToSiteGuid IS NULL)) 
		-- Recursive Call
		UNION ALL
		SELECT a.ModuleToSiteGuid, a.AssignedFromSiteGuid, a.SiteGuid, lvl + 1
		FROM map.tblEntityModuleToSite a 
		INNER JOIN AssignmentHierarchy b
		ON a.ModuleGuid = @EntityRecordGuid
		AND a.AssignedFromSiteGuid = b.AssignedToSiteGuid
		AND a.SiteGuid <> a.AssignedFromSiteGuid
	)	
		
	INSERT INTO @tblAssignmentHierarchy
	(MappingGuid, SiteGuid, SiteId, SiteGroupFlag, AssignedFromSiteGuid, AssignedFromSiteId, HierarchyLevel)
	SELECT a.MappingGuid, a.AssignedToSiteGuid, b.id SiteId, b.SiteGroupFlag, a.AssignedFromSiteGuid, c.Id, a.lvl
	FROM AssignmentHierarchy a
	INNER JOIN tblSites b
	ON b.SiteGuid = a.AssignedToSiteGuid
	LEFT OUTER JOIN tblSites c
	ON c.SiteGuid = a.AssignedFromSiteGuid
		
	--Note: No need to group on SiteGuid because, unlike the site hierarchy where a site/sitegroup can be the child to more than one parent sitegroup, 
	--in the case of entity-to-site hierarchy, an entity can only be mapped to a site/sitegroup from one parent sitegroup.

	OPTION (MAXRECURSION 10000);

	RETURN;
END 

