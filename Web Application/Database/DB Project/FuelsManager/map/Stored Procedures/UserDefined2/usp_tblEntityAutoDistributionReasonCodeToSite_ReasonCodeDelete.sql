
/****************************** usp_tblEntityAutoDistributionReasonCodeToSite_ReasonCodeDelete ******************************/
CREATE PROCEDURE map.usp_tblEntityAutoDistributionReasonCodeToSite_ReasonCodeDelete
	@ReasonCodeGuid UNIQUEIDENTIFIER
AS
BEGIN
	DELETE FROM map.tblEntityAutoDistributionReasonCodeToSite
	WHERE AutoDistributionReasonCodeGuid = @ReasonCodeGuid
END