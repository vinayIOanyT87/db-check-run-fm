CREATE FUNCTION [dbo].[udf_CheckTankFarmSiteToVendorSiteMap]
(
	@ChildSiteGuid UniqueIdentifier
	, @ParentSiteGuid UniqueIdentifier
)
RETURNS BIT
AS
BEGIN
	/*
	------------------------------------------------------------------------------------------------------
	-- Function: [dbo].[udf_CheckTankFarmSiteToVendorSiteMap] 
	-- Author: Richard R. Panachida
	-- Version/Date: 1.0.000 / 2023-02-15 14:21:10.4470770 -04:00
	-- Purpose: Function to return true if: Child = Parent or Child is a site group or Parent is a site group.
	------------------------------------------------------------------------------------------------------
	*/
	-- Do not allow site mapping to self
	IF @ParentSiteGuid = @ChildSiteGuid
		RETURN CAST(1 AS BIT)

	-- Only allow sites and not site groups
	IF ((SELECT COUNT(*) FROM dbo.tblSites S WHERE S.SiteGuid = @ParentSiteGuid AND S.SiteGroupFlag = CAST(1 AS BIT)) = 1)
		RETURN CAST(1 AS BIT)

	-- Only allow sites and not site groups
	IF ((SELECT COUNT(*) FROM dbo.tblSites S WHERE S.SiteGuid = @ChildSiteGuid AND S.SiteGroupFlag = CAST(1 AS BIT)) = 1)
		RETURN CAST(1 AS BIT)

	RETURN CAST(0 AS BIT)
END
