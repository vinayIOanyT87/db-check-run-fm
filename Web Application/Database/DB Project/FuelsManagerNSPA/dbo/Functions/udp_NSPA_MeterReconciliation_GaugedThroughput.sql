CREATE FUNCTION dbo.udp_NSPA_MeterReconciliation_GaugedThroughput (
	@SiteID NVARCHAR(60)
	, @InventoryDate DATETIME
	)
RETURNS @ResultTable TABLE (
	SiteID NVARCHAR(60)
	, ManagerID NVARCHAR(60)
	, Product NVARCHAR(60)
	, Throughput FLOAT
	, Error BIT -- when there is a missing physical inventory in the previous day
	)
AS
BEGIN
	DECLARE @SiteTable TABLE (SiteID NVARCHAR(60))

	INSERT INTO @SiteTable
	SELECT *
	FROM dbo.udp_SitesFromSiteGroup(@SiteID)

	DECLARE @PhysicalInventoryTable TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, GrossQuantity FLOAT
		, Today BIT
		)

	-- get all the relevant dips
	INSERT INTO @PhysicalInventoryTable
	SELECT t.[Site]
		, t.ManagerID
		, l.Product
		, SUM(ISNULL(l.GrossQuantity, 0))
		, (
			CASE t.InventoryDate
				WHEN @InventoryDate
					THEN 1
				ELSE 0
				END
			)
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems l
		ON t.TransactionGuid = l.TransactionGuid
	WHERE t.DeleteFlag = 0
		AND t.AliasName = 'Physical Inventory'
		AND t.[Site] IN (
			SELECT *
			FROM @SiteTable
			)
		AND t.InventoryDate IN (@InventoryDate, DATEADD(DAY, - 1, @InventoryDate))
	GROUP BY t.[Site]
		, t.ManagerID
		, l.Product
		, t.InventoryDate

	DECLARE @ErrorTable TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		)

	-- find missing previous day dips
	INSERT INTO @ErrorTable
	SELECT pt.SiteID
		, pt.ManagerID
		, pt.Product
	FROM @PhysicalInventoryTable pt
	WHERE Today = 1
		AND Product NOT IN (
			SELECT Product
			FROM @PhysicalInventoryTable py
			WHERE Today = 0
				AND py.SiteID = pt.SiteID
				AND py.ManagerID = py.ManagerID
			)

	-- calculate the final gauged throughput
	INSERT INTO @ResultTable
	SELECT pt.SiteID
		, pt.ManagerID
		, pt.Product
		, ABS(pt.GrossQuantity - py.GrossQuantity)
		, -- abs as per spec
		0
	FROM @PhysicalInventoryTable pt
	INNER JOIN @PhysicalInventoryTable py
		ON pt.SiteID = py.SiteID
			AND pt.ManagerID = py.ManagerID
			AND pt.Product = py.Product
	WHERE pt.Today = 1
		AND py.Today = 0

	-- insert the errors
	INSERT INTO @ResultTable
	SELECT SiteID
		, ManagerID
		, Product
		, NULL
		, 1
	FROM @ErrorTable

	RETURN
END
