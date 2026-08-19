

/*

	SELECT [erv].[udf_IsASiteParent] ('F4761A16-AB2F-41EE-B6FA-D17658DF2602', '00000000-0000-0000-0000-000000000001')
	SELECT [erv].[udf_IsASiteParent] ('46426312-E408-4AF8-85FD-338B622B32BF', '00000000-0000-0000-0000-000000000001')
	SELECT [erv].[udf_IsASiteParent] ('00000000-0000-0000-0000-000000000001', '46426312-E408-4AF8-85FD-338B622B32BF')

	
*/

	CREATE FUNCTION [erv].[udf_IsASiteParent]
	(
		@SourceSiteGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier
	)
	RETURNS bit
	AS
	BEGIN
	/*
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_IsASiteParent] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Verifies if a site (target) is a parent (direct or indirect) of a given site (source).
	-- Notes:
	-- 1. @SourceSiteGuid: Child Site or sitegroup guid for which the parent sitegroup relationship is to be verified.
	   2. @TargetSiteGuid: Parent sitegroup for which the child site relationship is to be verified.
	------------------------------------------------------------------------------------------------------
	*/
		DECLARE @result bit
		SET @result = 0;
		
		/* Retrieve the Site hierarchy for the target sitegroup */
		DECLARE @tblSiteHierarchy TABLE
		(
			SiteGuid uniqueidentifier
			, SiteId nvarchar(30)
			, HierarchyLevel int
		);

		INSERT INTO @tblSiteHierarchy
		(SiteGuid, SiteId, HierarchyLevel)
		SELECT SiteGuid, SiteId, HierarchyLevel 
		FROM [erv].[udf_GetSiteHierarchy](@TargetSiteGuid, 1)
		ORDER BY HierarchyLevel, SiteId

		IF ((SELECT COUNT(*) FROM @tblSiteHierarchy WHERE SiteGuid = @SourceSiteGuid) > 0)
			SET @result = 1
		
		RETURN @result;
	END 

