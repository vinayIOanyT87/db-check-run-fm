
CREATE PROCEDURE [rpt].[usp_ProductList]
@LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @ShowAll INT
AS
BEGIN
	SET NOCOUNT ON

	SELECT NULL    AS ProductGuid,
			 '<All>' AS ProductID,
			 '<All>' AS Description
	 WHERE @ShowAll = 1

	UNION

	SELECT ProductGuid,
			 ProductID,
			 Description
	  FROM dbo.tblProducts
	 WHERE ProductGuid IN
				(SELECT ProductGuid
					FROM map.tblEntityProductToSite
				  WHERE SiteGuid = @SiteGuid)
		AND (
				SiteGuid = @SiteGuid
				 OR
				ProductGuid IN
					(SELECT ProductGuid
						FROM map.tblEntityProductToSite
					  WHERE SiteGuid = @LoginSiteGuid)
			)

	 ORDER BY ProductID
END