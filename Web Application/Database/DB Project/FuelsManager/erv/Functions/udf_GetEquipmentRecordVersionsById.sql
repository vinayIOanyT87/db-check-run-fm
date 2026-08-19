CREATE FUNCTION [erv].[udf_GetEquipmentRecordVersionsById]
(
	@TargetSiteGuid uniqueidentifier,
	@Id nvarchar(100) 
)
RETURNS @tblRecordVersionSet TABLE
(
	EquipmentGuid uniqueidentifier,
	MasterRecordGuid uniqueidentifier,
	AssignedFromSiteGuid uniqueidentifier,
	AssignedToSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetEquipmentRecordVersions] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to return the Equipment record versions for a given target site/sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Guid of the site/sitegroup for which to limit the search.
	-- 2. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	DECLARE @emptyGuid uniqueidentifier
	SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

	DECLARE @tblChildRecordVersion TABLE
	(
		EquipmentGuid uniqueidentifier,
		MasterRecordGuid uniqueidentifier,
		AssignedFromSiteGuid uniqueidentifier,
		AssignedToSiteGuid uniqueidentifier
	)
	DECLARE @tblParentRecordVersion TABLE
	(
		EquipmentGuid uniqueidentifier,
		MasterRecordGuid uniqueidentifier,
		AssignedFromSiteGuid uniqueidentifier,
		AssignedToSiteGuid uniqueidentifier
	)

	INSERT INTO @tblChildRecordVersion
	(EquipmentGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.EquipmentGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.SiteGuid FROM tblEquipment a
	INNER JOIN map.tblEntityEquipmentToSite b 
	ON b.EquipmentGuid = a._MasterRecordGuid 
	AND b.SiteGuid = a.SiteGuid
	WHERE ((a.ID = @Id) OR (@Id IS NULL))
	AND ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND a.EquipmentGuid <> a._MasterRecordGuid

	INSERT INTO @tblParentRecordVersion
	(EquipmentGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.EquipmentGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.SiteGuid FROM tblEquipment a
	INNER JOIN map.tblEntityEquipmentToSite b 
	ON b.EquipmentGuid = a._MasterRecordGuid 
	AND a.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', b.EquipmentGuid, b.AssignedFromSiteGuid) 
	WHERE ((a.ID = @Id) OR (@Id IS NULL))
	AND ((b.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND NOT EXISTS
	(	
		--Exclude those parent records for which a child record version exists for the AssignedTo site (irrespective of whether the child record version meet the query business filtering criteria or not)	
		SELECT * FROM @tblChildRecordVersion c
		WHERE c.MasterRecordGuid = a._MasterRecordGuid
		AND c.AssignedToSiteGuid = b.SiteGuid
		/*
		--The exclusion of parent record versions that have a child record version for the AssignedTo site/sitegroup was simply done by excluding those records from @tblChildRecordVersion, because the query that populated @tblChildRecordVersion does not include any filtering on FLC configured fields (e.g. Make, Model, etc.)
		--If the query used to populate @tblChildRecordVersion contains filtering using FLC configured fields, then need to use the query below to filter out the parent record versions that have a child record version for the AssignedTo site/sitegroup :
		SELECT * FROM tblEquipment c
		WHERE c._MasterRecordGuid = b.EquipmentGuid
		AND c.SiteGuid =  b.SiteGuid
		AND c.EquipmentGuid <> c._MasterRecordGuid			
		*/
	)

	INSERT INTO @tblRecordVersionSet
	(EquipmentGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT EquipmentGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblChildRecordVersion
	UNION
	SELECT EquipmentGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblParentRecordVersion

	RETURN;
	
END