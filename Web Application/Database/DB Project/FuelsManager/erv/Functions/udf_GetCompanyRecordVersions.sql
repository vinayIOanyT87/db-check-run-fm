
/*
	DROP FUNCTION [erv].[udf_GetCompanyRecordVersions]

	SELECT * FROM [erv].[udf_GetCompanyRecordVersions] (NULL) ORDER BY CompanyGuid
	SELECT * FROM [erv].[udf_GetCompanyRecordVersions] ('F4761A16-AB2F-41EE-B6FA-D17658DF2602')
	
*/

CREATE FUNCTION [erv].[udf_GetCompanyRecordVersions]
(
	@TargetSiteGuid uniqueidentifier
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
	-- Purpose: Function to return the applicable Company record versions for a given target site/sitegroup.
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
	DECLARE @tblParentMasterRecords TABLE
	(
		MasterRecordGuid uniqueidentifier,
		AssignedFromSiteGuid uniqueidentifier,
		AssignedToSiteGuid uniqueidentifier
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
	WHERE ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND a.CompanyGuid <> a._MasterRecordGuid

	INSERT INTO @tblParentMasterRecords
	(MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.CompanyGuid, a.AssignedFromSiteGuid, a.SiteGuid 
	FROM map.tblEntityCompanyToSite a 
	WHERE ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND NOT EXISTS
	(
		--Exclude those parent records for which a child record version exists for the AssignedTo site
		SELECT * FROM @tblChildRecordVersion c
		WHERE c.MasterRecordGuid = a.CompanyGuid
		AND c.AssignedToSiteGuid = a.SiteGuid
	)

	INSERT INTO @tblParentRecordVersion
	(CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.CompanyGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.AssignedToSiteGuid FROM tblCompanies a
	INNER JOIN @tblParentMasterRecords b 
	ON b.MasterRecordGuid = a._MasterRecordGuid 
	AND a.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', b.MasterRecordGuid, b.AssignedFromSiteGuid) 
	WHERE ((b.AssignedToSiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))


	INSERT INTO @tblRecordVersionSet
	(CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblChildRecordVersion
	UNION
	SELECT CompanyGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblParentRecordVersion

	RETURN;
	
END
