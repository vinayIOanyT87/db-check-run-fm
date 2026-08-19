CREATE FUNCTION dbo.udp_NSPA_ReconciliationException_Closeouts
(
	@SiteID			NVARCHAR(60),
	@InventoryDate	DATETIME
)
RETURNS @ResultTable TABLE ( CloseoutDate DATETIME, ClosedBy NVARCHAR(60) )
AS
BEGIN

INSERT INTO @ResultTable
SELECT	TOP 1 CloseoutDate, CreatedBy
FROM	tblCloseoutInventory
WHERE	[Site] = @SiteID
AND		CloseoutDate <= @InventoryDate

RETURN

END