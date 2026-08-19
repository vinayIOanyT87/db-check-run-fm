CREATE FUNCTION [erv].[udf_GetPersonnelRecordVersionsById]
(
	@TargetSiteGuid uniqueidentifier,
	@Id nvarchar(100) 
)
RETURNS @tblRecordVersionSet TABLE
(
	PersonnelGuid uniqueidentifier,
	MasterRecordGuid uniqueidentifier,
	AssignedFromSiteGuid uniqueidentifier,
	AssignedToSiteGuid uniqueidentifier,
	MasterSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetPersonnelRecordVersions] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:59:10.4470770 -10:00
	-- Purpose: Function to return the Personnel record versions for a given target site/sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Guid of the site/sitegroup for which to limit the search.
	-- 2. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	-- 3. The SiteGuid of the master record is returned to support the decryption of the Personnel.PINNumber
	------------------------------------------------------------------------------------------------------
	DECLARE @emptyGuid uniqueidentifier
	SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

	DECLARE @tblChildRecordVersion TABLE
	(
		PersonnelGuid uniqueidentifier,
		MasterRecordGuid uniqueidentifier,
		AssignedFromSiteGuid uniqueidentifier,
		AssignedToSiteGuid uniqueidentifier
	)
	DECLARE @tblParentRecordVersion TABLE
	(
		PersonnelGuid uniqueidentifier,
		MasterRecordGuid uniqueidentifier,
		AssignedFromSiteGuid uniqueidentifier,
		AssignedToSiteGuid uniqueidentifier
	)

	INSERT INTO @tblChildRecordVersion
	(PersonnelGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.PersonnelGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.SiteGuid FROM tblPersonnel a
	INNER JOIN map.tblEntityPersonnelToSite b 
	ON b.PersonnelGuid = a._MasterRecordGuid 
	AND b.SiteGuid = a.SiteGuid
	WHERE ((a.PersonID = @Id) OR (@Id IS NULL))
	AND ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND a.PersonnelGuid <> a._MasterRecordGuid

	INSERT INTO @tblParentRecordVersion
	(PersonnelGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.PersonnelGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.SiteGuid FROM tblPersonnel a
	INNER JOIN map.tblEntityPersonnelToSite b 
	ON b.PersonnelGuid = a._MasterRecordGuid 
	AND a.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', b.PersonnelGuid, b.AssignedFromSiteGuid) 
	WHERE ((a.PersonID = @Id) OR (@Id IS NULL))
	AND ((b.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND NOT EXISTS
	(	
		--Exclude those parent records for which a child record version exists for the AssignedTo site (irrespective of whether the child record version meet the query business filtering criteria or not)	
		SELECT * FROM @tblChildRecordVersion c
		WHERE c.MasterRecordGuid = a._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		/*
		--The exclusion of parent record versions that have a child record version for the AssignedTo site/sitegroup was simply done by excluding those records from @tblChildRecordVersion, because the query that populated @tblChildRecordVersion does not include any filtering on FLC configured fields (e.g. Description, TaxCode, etc.)
		--If the query used to populate @tblChildRecordVersion contains filtering using FLC configured fields, then need to use the query below to filter out the parent record versions that have a child record version for the AssignedTo site/sitegroup :
		SELECT * FROM tblPersonnel c
		WHERE c._MasterRecordGuid = b.PersonnelGuid
		AND c.SiteGuid =  b.SiteGuid
		AND c.PersonnelGuid <> c._MasterRecordGuid			
		*/
	)

	INSERT INTO @tblRecordVersionSet
	(PersonnelGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT PersonnelGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblChildRecordVersion
	UNION
	SELECT PersonnelGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblParentRecordVersion

	UPDATE a 
	SET a.MasterSiteGuid = b.AssignedFromSiteGuid
	FROM @tblRecordVersionSet a
	INNER JOIN map.tblEntityPersonnelToSite b 
	ON b.PersonnelGuid = a.MasterRecordGuid 
	WHERE b.AssignedFromSiteGuid = b.SiteGuid 

	RETURN;
	
END