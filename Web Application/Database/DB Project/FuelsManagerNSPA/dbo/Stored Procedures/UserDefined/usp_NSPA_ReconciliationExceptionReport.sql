CREATE PROCEDURE dbo.usp_NSPA_ReconciliationExceptionReport
(
	@ViewingSiteID	NVARCHAR(60),
	@InventoryDate	DATETIME
)
AS
BEGIN
SET NOCOUNT ON

IF @ViewingSiteID <> 'NSPA'
	RAISERROR ('Report can only be run from NSPA', 16, 1)

DECLARE @VolumeUnits int
SET		@VolumeUnits = (SELECT tblSites.VolumeUnitIndex FROM tblSites WHERE tblSites.ID = @ViewingSiteID)

DECLARE @VolumeDecimalPlaces int
SET		@VolumeDecimalPlaces = (SELECT tblSites.VolumeDecimalPlaces FROM tblSites WHERE tblSites.ID = @ViewingSiteID)

DECLARE @ResultTable TABLE
(
	Installation		NVARCHAR(60),
	AllowableVariance	FLOAT,
	InventoryVariance	FLOAT,
	MeterVariance		FLOAT,
	CloseoutDate		DATETIME,
	ClosedBy			NVARCHAR(60),
	EOMApprovalDate		DATETIME,
	EOMApprovedBy		NVARCHAR(60),
	NoDipFlag			BIT,
	NoCloseoutFlag		BIT,
	ToKeep				BIT
)

DECLARE @WorkingDate DATETIME
SET @WorkingDate = DATEADD(DAY, -1, @InventoryDate)

-- calculate total allowable variances by site
--DECLARE @AllowableVarianceTable TABLE ( Installation NVARCHAR(60), AllowableVariance FLOAT )
INSERT INTO @ResultTable
SELECT	DISTINCT
		t.[Site],
		SUM(p.VarianceTolerance * ISNULL(l.GrossQuantity, 0)),
		0,
		0,
		NULL,
		NULL,
		NULL,
		NULL,
		0,
		0,
		0
FROM	tblTransactions t	INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
							INNER JOIN tblProducts p ON l.ProductGuid = p.ProductGuid
WHERE	t.DeleteFlag = 0
AND		t.InventoryDate = @WorkingDate
GROUP BY t.[Site], l.Product

-- easiest to do this with a cursor, then we can make reuse of the meter recon functions
DECLARE @SiteID	NVARCHAR(60)
DECLARE @AllowableVariance FLOAT
DECLARE @InventoryVariance FLOAT
DECLARE @MeterVariance FLOAT
DECLARE @CloseoutTable TABLE ( CloseoutDate DATETIME, ClosedBy NVARCHAR(60) )
DECLARE @EOMApprovalTable TABLE ( EOMApprovalDate DATETIME, EOMApprovedBy NVARCHAR(60) )

DECLARE SiteCursor CURSOR FOR SELECT ID FROM tblSites WHERE [Enabled] = 1
OPEN SiteCursor
FETCH NEXT FROM SiteCursor INTO @SiteID
WHILE @@FETCH_STATUS = 0
BEGIN

	SELECT	@AllowableVariance = AllowableVariance
	FROM	@ResultTable
	WHERE	Installation = @SiteID

	-- get OOT meter variance (from previous date)
	SET @MeterVariance = dbo.udp_NSPA_ReconciliationException_MeterVariance(@SiteID, @WorkingDate)
	IF (@MeterVariance > @AllowableVariance)
		UPDATE @ResultTable SET ToKeep = 1, MeterVariance = @MeterVariance WHERE Installation = @SiteID

	-- get OOT inventory variance (to current date)
	SET @InventoryVariance = dbo.udp_NSPA_ReconciliationException_InventoryVariance(@SiteID, @InventoryDate)
	IF (@InventoryVariance IS NULL)
		UPDATE @ResultTable SET ToKeep = 1, NoDipFlag = 1 WHERE Installation = @SiteID
	ELSE IF (ABS(@InventoryVariance) > @AllowableVariance)
		UPDATE @ResultTable SET ToKeep = 1, InventoryVariance = @InventoryVariance WHERE Installation = @SiteID

	-- check for all active products that have not been closed out in the last three days, otherwise just report the most recent closeout
	INSERT INTO @CloseoutTable SELECT * FROM dbo.udp_NSPA_ReconciliationException_Closeouts(@SiteID, @InventoryDate)
	IF (SELECT COUNT(*) FROM @CloseoutTable) = 0
		UPDATE @ResultTable SET ToKeep = 1, NoCloseoutFlag = 1 WHERE Installation = @SiteID
	ELSE
		UPDATE @ResultTable SET ToKeep = 1, 
							CloseoutDate = (SELECT CloseoutDate FROM @CloseoutTable), 
							ClosedBy = (SELECT ClosedBy FROM @CloseoutTable)
		WHERE Installation = @SiteID

	-- check for EOM (TBC Someone else, I don't have specs to EOM functionality)
	-- see @EOMApprovalTable for schema example
	INSERT INTO @EOMApprovalTable SELECT * FROM dbo.udp_NSPA_ReconciliationException_Closeouts(@SiteID, @InventoryDate)
	IF (SELECT COUNT(*) FROM @EOMApprovalTable) > 0
		UPDATE @ResultTable SET ToKeep = 1,
			EOMApprovalDate = (SELECT EOMApprovalDate FROM @EOMApprovalTable),
			EOMApprovedBy = (SELECT EOMApprovedBy FROM @EOMApprovalTable)
		WHERE Installation = @SiteID

FETCH NEXT FROM SiteCursor INTO @SiteID
END
CLOSE SiteCursor
DEALLOCATE SiteCursor

-- delete rows that weren't marked for keeping
DELETE FROM @ResultTable WHERE ToKeep = 0

-- adjust for site precision
UPDATE	@ResultTable SET
	InventoryVariance = dbo.udf_ConvertFromSIUnits(InventoryVariance, @VolumeUnits, @VolumeDecimalPlaces),
	MeterVariance = dbo.udf_ConvertFromSIUnits(MeterVariance, @VolumeUnits, @VolumeDecimalPlaces)

-- return results to report
SELECT * FROM @ResultTable

END -- usp_NSPA_ReconciliationExceptionReport