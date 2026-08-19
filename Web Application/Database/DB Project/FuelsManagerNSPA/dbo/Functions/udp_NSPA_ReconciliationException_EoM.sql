CREATE FUNCTION dbo.udp_NSPA_ReconciliationException_EoM
(
	@SiteID			NVARCHAR(60),
	@InventoryDate	DATETIME
)
RETURNS @ResultTable TABLE ( EOMApprovalDate DATETIME, EOMApprovedBy NVARCHAR(60) )
AS
BEGIN

INSERT INTO @ResultTable VALUES ( GETDATE(), 'Jack.Shen' )

RETURN

END