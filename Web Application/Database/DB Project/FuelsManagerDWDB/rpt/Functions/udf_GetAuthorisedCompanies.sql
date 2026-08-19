CREATE FUNCTION [rpt].[udf_GetAuthorisedCompanies]
(@UserKey [nvarchar](50) = NULL)
RETURNS @tblId TABLE(Id nvarchar(100))
AS
BEGIN
	-- If @UserKey = NULL then return all companies
	IF @UserKey IS NULL
	BEGIN
		INSERT INTO @tblId(Id)
		SELECT DISTINCT CompanyId FROM dbo.DimCompany
	END
	ELSE
	BEGIN
		INSERT INTO @tblId(Id)
		SELECT DISTINCT a.CompanyId FROM dbo.DimCompany a
		INNER JOIN dbo.FactFMUserToCompany b
		ON b.CompanySKey = a.SKey
		INNER JOIN dbo.DimFMUser d
		ON d.SKey = b.FMUserSKey
		WHERE d.AKey = @UserKey
	END
	RETURN
END