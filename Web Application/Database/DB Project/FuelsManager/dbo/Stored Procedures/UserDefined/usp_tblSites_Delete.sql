
/****************************** usp_tblSites_Delete ******************************/
CREATE PROCEDURE dbo.usp_tblSites_Delete
	@SiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	DELETE FROM dbo.tblSites
	WHERE SiteGuid = @SiteGuid
END