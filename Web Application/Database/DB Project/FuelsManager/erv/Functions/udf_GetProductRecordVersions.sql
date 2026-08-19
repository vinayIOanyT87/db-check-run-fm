

/*
	DROP FUNCTION [erv].[udf_GetProductRecordVersions]

	SELECT * FROM [erv].[udf_GetProductRecordVersions] (NULL) ORDER BY ProductGuid
	SELECT * FROM [erv].[udf_GetProductRecordVersions] ('F4761A16-AB2F-41EE-B6FA-D17658DF2602') ORDER BY ProductGuid
	
*/

CREATE FUNCTION [erv].[udf_GetProductRecordVersions]
(
	@TargetSiteGuid uniqueidentifier
)
RETURNS @tblRecordVersionSet TABLE
(
	ProductGuid uniqueidentifier,
	MasterRecordGuid uniqueidentifier,
	AssignedFromSiteGuid uniqueidentifier,
	AssignedToSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetProductRecordVersions] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to return the applicable Product record versions for a given target site/sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Guid of the site/sitegroup for which to limit the search.
	-- 2. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	DECLARE @emptyGuid uniqueidentifier
	SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

	DECLARE @tblChildRecordVersion TABLE
	(
		ProductGuid uniqueidentifier,
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
		ProductGuid uniqueidentifier,
		MasterRecordGuid uniqueidentifier,
		AssignedFromSiteGuid uniqueidentifier,
		AssignedToSiteGuid uniqueidentifier
	)

	INSERT INTO @tblChildRecordVersion
	(ProductGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.ProductGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.SiteGuid FROM tblProducts a
	INNER JOIN map.tblEntityProductToSite b 
	ON b.ProductGuid = a._MasterRecordGuid 
	AND b.SiteGuid = a.SiteGuid
	WHERE ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND a.ProductGuid <> a._MasterRecordGuid

	INSERT INTO @tblParentMasterRecords
	(MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.ProductGuid, a.AssignedFromSiteGuid, a.SiteGuid 
	FROM map.tblEntityProductToSite a 
	WHERE ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND NOT EXISTS
	(
		--Exclude those parent records for which a child record version exists for the AssignedTo site
		SELECT * FROM @tblChildRecordVersion c
		WHERE c.MasterRecordGuid = a.ProductGuid
		AND c.AssignedToSiteGuid = a.SiteGuid
	)

	INSERT INTO @tblParentRecordVersion
	(ProductGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.ProductGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.AssignedToSiteGuid FROM tblProducts a
	INNER JOIN @tblParentMasterRecords b 
	ON b.MasterRecordGuid = a._MasterRecordGuid 
	AND a.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', b.MasterRecordGuid, b.AssignedFromSiteGuid) 
	WHERE ((b.AssignedToSiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))


	INSERT INTO @tblRecordVersionSet
	(ProductGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT ProductGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblChildRecordVersion
	UNION
	SELECT ProductGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblParentRecordVersion

	RETURN;
	
END