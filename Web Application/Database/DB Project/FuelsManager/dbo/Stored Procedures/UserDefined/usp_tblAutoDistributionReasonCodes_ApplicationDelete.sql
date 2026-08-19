
/****************************** usp_tblAutoDistributionReasonCodes_ApplicationDelete ******************************/
CREATE PROCEDURE dbo.usp_tblAutoDistributionReasonCodes_ApplicationDelete
	@ReasonCodeGuid UNIQUEIDENTIFIER
AS
BEGIN
	-- Cascade Delete
	EXEC map.usp_tblEntityAutoDistributionReasonCodeToSite_ReasonCodeDelete @ReasonCodeGuid
	
	EXEC dbo.usp_tblAutoDistributionReasonCodes_Delete @ReasonCodeGuid	
END