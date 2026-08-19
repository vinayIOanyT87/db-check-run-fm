

CREATE PROCEDURE [dbo].[usp_CompanyListForShipperAndShipTo]
@LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @HasAllItem BIT
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @CombinedCompanyTable TABLE (CompanyName nvarchar(100), CompanyCode nvarchar(10))

	INSERT INTO @CombinedCompanyTable
		SELECT * FROM udf_CompanyList(@LoginSiteGuid, @SiteGuid, 2, @HasAllItem)

	INSERT INTO @CombinedCompanyTable
		SELECT * FROM udf_CompanyList(@LoginSiteGuid, @SiteGuid, 4, @HasAllItem) cl
		WHERE cl.CompanyName NOT IN (SELECT CompanyName FROM @CombinedCompanyTable)

	SELECT * FROM @CombinedCompanyTable ORDER BY CompanyName
END

