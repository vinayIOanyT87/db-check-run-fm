
CREATE PROCEDURE [dbo].[usp_CompanyList]
@LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @Role INT, @HasAllItem BIT
AS
BEGIN
	SET NOCOUNT ON
	SELECT* FROM udf_CompanyList(@LoginSiteGuid,@SiteGuid,@Role,@HasAllItem) ORDER BY CompanyName
END