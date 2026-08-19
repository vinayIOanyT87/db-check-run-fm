
CREATE FUNCTION [dbo].[udf_CheckSiteToSiteMap]
(@ChildSiteGuid UniqueIdentifier, @ParentSiteGuid UniqueIdentifier)
RETURNS BIT
AS
BEGIN
	/*
	------------------------------------------------------------------------------------------------------
	-- Function: [dbo].[udf_CheckSiteToSiteMap] 
	-- Author: Warren Gray
	-- Version/Date: 1.0.001 / 2013-07-03 14:21:10.4470770 -04:00
	-- Purpose: Function to return true if a SiteToSiteMap will create a circular assignment between site groups.
	------------------------------------------------------------------------------------------------------
	*/
	-- ignore site mapping to self
	IF @ParentSiteGuid = @ChildSiteGuid
		RETURN CAST(0 AS BIT)

	-- check only site group assignments
	IF NOT EXISTS(SELECT S.SiteGuid FROM dbo.tblSites S WHERE S.SiteGuid = @ChildSiteGuid AND S.SiteGroupFlag = CAST(1 AS BIT))
		RETURN CAST(0 AS BIT) 

	-- the following will throw if @ChildSiteGuid results in a circular site hierarchy
	DECLARE @ChildSites TABLE ([SiteGuid]  UNIQUEIDENTIFIER NOT NULL)
	INSERT INTO @ChildSites SELECT SiteGuid FROM erv.udf_GetSiteHierarchy(@ChildSiteGuid,1) 

	RETURN CAST(0 AS BIT)
END

