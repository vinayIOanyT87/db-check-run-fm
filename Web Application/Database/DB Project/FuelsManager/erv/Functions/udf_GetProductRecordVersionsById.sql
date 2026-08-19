CREATE FUNCTION [erv].[udf_GetProductRecordVersionsById]
(
	@TargetSiteGuid uniqueidentifier,
	@Id nvarchar(100) 
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
	-- Purpose: Function to return the Product record versions for a given target site/sitegroup.
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
	WHERE ((a.ProductID = @Id) OR (@Id IS NULL))
	AND ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND a.ProductGuid <> a._MasterRecordGuid

	INSERT INTO @tblParentRecordVersion
	(ProductGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.ProductGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.SiteGuid FROM tblProducts a
	INNER JOIN map.tblEntityProductToSite b 
	ON b.ProductGuid = a._MasterRecordGuid 
	AND a.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', b.ProductGuid, b.AssignedFromSiteGuid) 
	WHERE ((a.ProductID = @Id) OR (@Id IS NULL))
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
		SELECT * FROM tblProducts c
		WHERE c._MasterRecordGuid = b.ProductGuid
		AND c.SiteGuid =  b.SiteGuid
		AND c.ProductGuid <> c._MasterRecordGuid			
		*/
	)

	INSERT INTO @tblRecordVersionSet
	(ProductGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT ProductGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblChildRecordVersion
	UNION
	SELECT ProductGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblParentRecordVersion

	RETURN;
	
END