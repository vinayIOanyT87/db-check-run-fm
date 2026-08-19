CREATE FUNCTION [rpt].[udf_GetAuthorisedSites]
(@UserKey [nvarchar](50) = NULL)
RETURNS @tblId TABLE(Id nvarchar(100))
AS
BEGIN
	-- If @UserKey = NULL then return all sites
	IF @UserKey IS NULL
	BEGIN
		INSERT INTO @tblId(Id)
		SELECT DISTINCT SiteId FROM dbo.DimSite
	END
	ELSE
	BEGIN
		INSERT INTO @tblId(Id)
		SELECT DISTINCT a.SiteId FROM dbo.DimSite a
		INNER JOIN dbo.FactFMUserToSite b
		ON b.SiteSKey = a.SKey
		INNER JOIN dbo.DimFMUser d
		ON d.SKey = b.FMUserSKey
		WHERE d.AKey = @UserKey
	END
	RETURN
END