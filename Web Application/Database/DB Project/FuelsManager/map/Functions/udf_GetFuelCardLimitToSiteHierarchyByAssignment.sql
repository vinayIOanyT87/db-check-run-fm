CREATE FUNCTION [map].[udf_GetFuelCardLimitToSiteHierarchyByAssignment]
(
	@EntityRecordGuid UNIQUEIDENTIFIER,
	@AssignedFromSiteGuid UNIQUEIDENTIFIER,
	@AssignedToSiteGuid UNIQUEIDENTIFIER
)
RETURNS @tblAssignmentHierarchy TABLE
(
	MappingGuid UNIQUEIDENTIFIER
	, SiteGuid UNIQUEIDENTIFIER
	, SiteId NVARCHAR(30)
	, SiteGroupFlag BIT
	, AssignedFromSiteGuid UNIQUEIDENTIFIER
	, AssignedFromSiteId NVARCHAR(30)
	, HierarchyLevel INT
)
AS
BEGIN
	/*
	------------------------------------------------------------------------------------------------------
	-- Function: [map].[udf_GetFuelCardLimitToSiteHierarchyByAssignment] 
	-- Author: Ryan Hill
	-- Purpose: Retrieves the FuelCardLimitToSite assignment hierarchy below a given FuelCardLimit assignment. 	
	-- Notes:
	-- 1. @EntityRecordGuid: Guid of the FuelCardLimit record for which the mapping hierarchy is to be retrieved.
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

	IF (@AssignedFromSiteGuid IS NULL AND @AssignedToSiteGuid IS NULL)
	BEGIN
		SELECT @AssignedFromSiteGuid = SiteGuid 
		FROM tblFuelCardLimit
		WHERE FuelCardLimitGuid = @EntityRecordGuid

		SET @AssignedToSiteGuid = @AssignedFromSiteGuid
	END

	IF (@AssignedFromSiteGuid IS NULL)
	BEGIN
		SELECT @AssignedFromSiteGuid = AssignedFromSiteGuid 
		FROM map.tblEntityFuelCardLimitToSite 
		WHERE FuelCardLimitGuid = @EntityRecordGuid AND SiteGuid = @AssignedToSiteGuid
	END;

	WITH AssignmentHierarchy (MappingGuid, AssignedFromSiteGuid, AssignedToSiteGuid, lvl)
	AS 
	(
		--Anchor (node/s for which the underneath tree structure needs to be retrieved.)
		SELECT FuelCardLimitToSiteGuid, 
			AssignedFromSiteGuid, 
			SiteGuid, 
			0
		FROM map.tblEntityFuelCardLimitToSite
		WHERE FuelCardLimitGuid = @EntityRecordGuid
		AND (AssignedFromSiteGuid = @AssignedFromSiteGuid OR @AssignedFromSiteGuid IS NULL)
		AND (SiteGuid = @AssignedToSiteGuid OR @AssignedToSiteGuid IS NULL) 
		-- Recursive Call
		UNION ALL
		SELECT a.FuelCardLimitToSiteGuid, 
			a.AssignedFromSiteGuid, 
			a.SiteGuid, 
			lvl + 1
		FROM map.tblEntityFuelCardLimitToSite a 
		INNER JOIN AssignmentHierarchy b ON a.FuelCardLimitGuid = @EntityRecordGuid
		AND a.AssignedFromSiteGuid = b.AssignedToSiteGuid
		AND a.SiteGuid <> a.AssignedFromSiteGuid
	)	
		
	INSERT INTO @tblAssignmentHierarchy
	(
		MappingGuid, 
		SiteGuid, 
		SiteId, 
		SiteGroupFlag, 
		AssignedFromSiteGuid, 
		AssignedFromSiteId, 
		HierarchyLevel
	)
	SELECT a.MappingGuid, 
		a.AssignedToSiteGuid, 
		b.ID SiteId, 
		b.SiteGroupFlag, 
		a.AssignedFromSiteGuid, 
		c.ID, 
		a.lvl
	FROM AssignmentHierarchy a
	INNER JOIN tblSites b ON b.SiteGuid = a.AssignedToSiteGuid
	LEFT OUTER JOIN tblSites c ON c.SiteGuid = a.AssignedFromSiteGuid
		
	--Note: No need to group on SiteGuid because, unlike the site hierarchy where a site/sitegroup can be the child to more than one parent sitegroup, 
	--in the case of entity-to-site hierarchy, an entity can only be mapped to a site/sitegroup from one parent sitegroup.

	OPTION (MAXRECURSION 10000);

	RETURN;
END 

