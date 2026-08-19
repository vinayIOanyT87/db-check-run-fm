

/*
	DROP FUNCTION [erv].[udf_GetTransactionAliasRecordVersions]

	SELECT * FROM [erv].[udf_GetTransactionAliasRecordVersions] (NULL) ORDER BY TransactionAliasGuid
	SELECT * FROM [erv].[udf_GetTransactionAliasRecordVersions] ('F4761A16-AB2F-41EE-B6FA-D17658DF2602')
	
*/

CREATE FUNCTION [erv].[udf_GetTransactionAliasRecordVersions]
(
	@TargetSiteGuid uniqueidentifier
)
RETURNS @tblRecordVersionSet TABLE
(
	TransactionAliasGuid uniqueidentifier,
	MasterRecordGuid uniqueidentifier,
	AssignedFromSiteGuid uniqueidentifier,
	AssignedToSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetTransactionAliasRecordVersions] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to return the applicable TransactionAlias record versions for a given target site/sitegroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Guid of the site/sitegroup for which to limit the search.
	-- 2. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	DECLARE @emptyGuid uniqueidentifier
	SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

	DECLARE @tblChildRecordVersion TABLE
	(
		TransactionAliasGuid uniqueidentifier,
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
		TransactionAliasGuid uniqueidentifier,
		MasterRecordGuid uniqueidentifier,
		AssignedFromSiteGuid uniqueidentifier,
		AssignedToSiteGuid uniqueidentifier
	)

	INSERT INTO @tblChildRecordVersion
	(TransactionAliasGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.TransactionAliasGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.SiteGuid FROM tblTransactionAliases a
	INNER JOIN map.tblEntityTransactionAliasToSite b 
	ON b.TransactionAliasGuid = a._MasterRecordGuid 
	AND b.SiteGuid = a.SiteGuid
	WHERE ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND a.TransactionAliasGuid <> a._MasterRecordGuid

	INSERT INTO @tblParentMasterRecords
	(MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.TransactionAliasGuid, a.AssignedFromSiteGuid, a.SiteGuid 
	FROM map.tblEntityTransactionAliasToSite a 
	WHERE ((a.SiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))
	AND NOT EXISTS
	(
		--Exclude those parent records for which a child record version exists for the AssignedTo site
		SELECT * FROM @tblChildRecordVersion c
		WHERE c.MasterRecordGuid = a.TransactionAliasGuid
		AND c.AssignedToSiteGuid = a.SiteGuid
	)

	INSERT INTO @tblParentRecordVersion
	(TransactionAliasGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT a.TransactionAliasGuid, a._MasterRecordGuid, b.AssignedFromSiteGuid, b.AssignedToSiteGuid FROM tblTransactionAliases a
	INNER JOIN @tblParentMasterRecords b 
	ON b.MasterRecordGuid = a._MasterRecordGuid 
	AND a.TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', b.MasterRecordGuid, b.AssignedFromSiteGuid) 
	WHERE ((b.AssignedToSiteGuid = @TargetSiteGuid) OR (@TargetSiteGuid IS NULL))


	INSERT INTO @tblRecordVersionSet
	(TransactionAliasGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
	SELECT TransactionAliasGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblChildRecordVersion
	UNION
	SELECT TransactionAliasGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM @tblParentRecordVersion

	RETURN;
	
END