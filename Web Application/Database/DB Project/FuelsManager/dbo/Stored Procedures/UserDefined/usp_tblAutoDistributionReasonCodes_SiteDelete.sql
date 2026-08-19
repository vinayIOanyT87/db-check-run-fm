
/****************************** usp_tblAutoDistributionReasonCodes_SiteDelete ******************************/
CREATE PROCEDURE dbo.usp_tblAutoDistributionReasonCodes_SiteDelete
	@SiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	DELETE FROM dbo.tblAutoDistributionReasonCodes
	WHERE SiteGuid = @SiteGuid
END