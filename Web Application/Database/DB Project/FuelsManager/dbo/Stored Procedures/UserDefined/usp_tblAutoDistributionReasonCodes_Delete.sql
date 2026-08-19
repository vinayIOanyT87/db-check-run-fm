
/****************************** usp_tblAutoDistributionReasonCodes_Delete ******************************/
CREATE PROCEDURE dbo.usp_tblAutoDistributionReasonCodes_Delete
	@ReasonCodeGuid UNIQUEIDENTIFIER
AS
BEGIN
	DELETE FROM dbo.tblAutoDistributionReasonCodes
	WHERE AutoDistributionReasonCodeGuid=@ReasonCodeGuid
END