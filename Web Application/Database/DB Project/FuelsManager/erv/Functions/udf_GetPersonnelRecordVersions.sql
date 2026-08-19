
/*
	DROP FUNCTION [erv].[udf_GetPersonnelRecordVersions]

	SELECT * FROM [erv].[udf_GetPersonnelRecordVersions] (NULL) ORDER BY PersonnelGuid
	SELECT * FROM [erv].[udf_GetPersonnelRecordVersions] ('F4761A16-AB2F-41EE-B6FA-D17658DF2602') ORDER BY PersonnelGuid
	
*/

CREATE FUNCTION [erv].[udf_GetPersonnelRecordVersions]
(
	@TargetSiteGuid uniqueidentifier
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
	-- Purpose: Function to return the applicable Personnel record versions for a given target site/sitegroup.
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
	DECLARE @tblParentMasterRecords TABLE
	(
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
	WHERE ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND a.PersonnelGuid <> a._MasterRecordGuid

	INSERT INTO @tblParentMasterRecords
	(MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.PersonnelGuid, a.AssignedFromSiteGuid, a.SiteGuid 
	FROM map.tblEntityPersonnelToSite a 
	WHERE ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND NOT EXISTS
	(
		--Exclude those parent records for which a child record version exists for the AssignedTo site
		SELECT * FROM @tblChildRecordVersion c
		WHERE c.MasterRecordGuid = a.PersonnelGuid
		AND c.AssignedToSiteGuid = a.SiteGuid
	)

	INSERT INTO @tblParentRecordVersion
	(PersonnelGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.PersonnelGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.AssignedToSiteGuid FROM tblPersonnel a
	INNER JOIN @tblParentMasterRecords b 
	ON b.MasterRecordGuid = a._MasterRecordGuid 
	AND a.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', b.MasterRecordGuid, b.AssignedFromSiteGuid) 
	WHERE ((b.AssignedToSiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))

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