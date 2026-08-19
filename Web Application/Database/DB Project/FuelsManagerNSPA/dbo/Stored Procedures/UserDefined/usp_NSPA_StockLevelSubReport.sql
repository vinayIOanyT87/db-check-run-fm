CREATE PROCEDURE dbo.usp_NSPA_StockLevelSubReport
(
	@ViewingSiteID	NVARCHAR(60),
	@SiteID			NVARCHAR(60),
	@ManagerID		NVARCHAR(60),
	@ProductID		NVARCHAR(60),
	@Month			DATETIME
)
AS
BEGIN
SET NOCOUNT ON

DECLARE @VolumeUnits int
SET		@VolumeUnits = (SELECT VolumeUnitIndex FROM tblSites WHERE ID = @ViewingSiteID)

DECLARE @VolumeDecimalPlaces int
SET		@VolumeDecimalPlaces = (SELECT VolumeDecimalPlaces FROM tblSites WHERE ID = @ViewingSiteID)

DECLARE @StartDate DATETIME
DECLARE @EndDate DATETIME

SET @StartDate = DATEADD(MONTH, DATEDIFF(MONTH, 0, @Month), 0)
SET @EndDate = DATEADD(DAY, -1, DATEADD(MONTH, DATEDIFF(MONTH, 0, @Month) + 1, 0))

DECLARE @ResultTable TABLE
(
	SiteID			NVARCHAR(60),
	ManagerID		NVARCHAR(60),
	ProductID		NVARCHAR(60),
	InventoryDate	DATETIME,
	GrossQuantity	FLOAT,
	MinimumTarget	FLOAT,
	MaximumTarget	FLOAT
)

INSERT INTO @ResultTable
SELECT	@SiteID,
		@ManagerID,
		@ProductID,
		t.InventoryDate,
		dbo.udf_ConvertFromSIUnits(SUM(ISNULL(l.GrossQuantity, 0)), @VolumeUnits, @VolumeDecimalPlaces),
		-- don't join here, performance wise too slow
		NULL,
		NULL
FROM	tblTransactions t INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
WHERE	t.DeleteFlag = 0
AND		l.DeleteFlag = 0
AND		t.AliasName = 'Physical Inventory'
AND		t.[Site] = @SiteID
AND		t.ManagerID = @ManagerID
AND		l.Product = @ProductID
AND		t.InventoryDate BETWEEN @StartDate AND @EndDate
GROUP BY t.InventoryDate

-- get reserve levels
UPDATE	r
SET		r.MinimumTarget = dbo.udf_ConvertFromSIUnits(l.MinimumLevel, @VolumeUnits, @VolumeDecimalPlaces),
		r.MaximumTarget = dbo.udf_ConvertFromSIUnits(l.WarningLevel, @VolumeUnits, @VolumeDecimalPlaces)
FROM	@ResultTable r	INNER JOIN tblProducts p ON r.ProductID = p.ProductID
						INNER JOIN tblSites s ON r.SiteID = s.ID
						INNER JOIN tblReserveLevels l ON p.ProductGuid = l.ProductGuid AND s.SiteGuid = l.SiteGuid

-- return results
SELECT	* 
FROM	@ResultTable 
ORDER BY InventoryDate

END -- usp_NSPA_StockLevelSubReport