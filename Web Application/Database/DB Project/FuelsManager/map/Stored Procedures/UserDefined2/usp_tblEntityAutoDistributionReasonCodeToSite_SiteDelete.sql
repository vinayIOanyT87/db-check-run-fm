
/*-----------------------------  Stored Procedures for tblEntityAutoDistributionReasonCodeToSite -----------------------------*/
/****************************** usp_tblEntityAutoDistributionReasonCodeToSite_SiteDelete ******************************/
CREATE PROCEDURE map.usp_tblEntityAutoDistributionReasonCodeToSite_SiteDelete
	@SiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	DELETE FROM map.tblEntityAutoDistributionReasonCodeToSite
	WHERE SiteGuid = @SiteGuid
END