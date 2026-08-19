CREATE FUNCTION [erv].[udf_GetCompanyRecordVersionsById]
(
	@TargetSiteGuid uniqueidentifier,
	@Id nvarchar(100) 
)
RETURNS @tblRecordVersionSet TABLE
(
	CompanyGuid uniqueidentifier,
	MasterRecordGuid uniqueidentifier,
	AssignedFromSiteGuid uniqueidentifier,
	AssignedToSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetCompanyRecordVersions] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to return the Company record versions for a given target site/sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Guid of the site/sitegroup for which to limit the search.
	-- 2. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	DECLARE @emptyGuid uniqueidentifier
	SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

	DECLARE @tblChildRecordVersion TABLE
	(
		CompanyGuid uniqueidentifier,
		MasterRecordGuid uniqueidentifier,
		AssignedFromSiteGuid uniqueidentifier,
		AssignedToSiteGuid uniqueidentifier,
		primary key NONCLUSTERED ( MasterRecordGuid, AssignedToSiteGuid)
	)
	DECLARE @tblParentRecordVersion TABLE
	(
		CompanyGuid uniqueidentifier,
		MasterRecordGuid uniqueidentifier,
		AssignedFromSiteGuid uniqueidentifier,
		AssignedToSiteGuid uniqueidentifier
	)

	INSERT INTO @tblChildRecordVersion
	(CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.CompanyGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.SiteGuid FROM tblCompanies a
	INNER JOIN map.tblEntityCompanyToSite b 
	ON b.CompanyGuid = a._MasterRecordGuid 
	AND b.SiteGuid = a.SiteGuid
	WHERE ((a.ID = @Id) OR (@Id IS NULL))
	AND ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND a.CompanyGuid <> a._MasterRecordGuid

	INSERT INTO @tblParentRecordVersion
	(CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.CompanyGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.SiteGuid FROM tblCompanies a
	INNER JOIN map.tblEntityCompanyToSite b 
	ON b.CompanyGuid = a._MasterRecordGuid 
	AND a.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', b.CompanyGuid, b.AssignedFromSiteGuid) 
	WHERE ((a.ID = @Id) OR (@Id IS NULL))
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
		SELECT * FROM tblCompanies c
		WHERE c._MasterRecordGuid = b.CompanyGuid
		AND c.SiteGuid =  b.SiteGuid
		AND c.CompanyGuid <> c._MasterRecordGuid			
		*/
	)

	INSERT INTO @tblRecordVersionSet
	(CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblChildRecordVersion
	UNION
	SELECT CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblParentRecordVersion

	RETURN;
END