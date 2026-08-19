CREATE FUNCTION dbo.udp_NSPA_MeterReconciliation_TransThroughput (
	@SiteID NVARCHAR(60)
	, @InventoryDate DATETIME
	)
RETURNS @ResultTable TABLE (
	SiteID NVARCHAR(60)
	, ManagerID NVARCHAR(60)
	, Product NVARCHAR(60)
	, Throughput FLOAT
	)
AS
BEGIN
	DECLARE @SiteTable TABLE (SiteID NVARCHAR(60))

	INSERT INTO @SiteTable
	SELECT *
	FROM dbo.udp_SitesFromSiteGroup(@SiteID)

	DECLARE @ReceiptData TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, GrossQuantity FLOAT
		)
	DECLARE @IssueData TABLE (
		SiteID NVARCHAR(60)
		, ManagerID NVARCHAR(60)
		, Product NVARCHAR(60)
		, GrossQuantity FLOAT
		)

	-- receipt defuel
	INSERT INTO @ReceiptData
	SELECT t.[Site]
		, t.ManagerID
		, l.Product
		, SUM(ISNULL(l.GrossQuantity, 0))
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems l
		ON t.TransactionGuid = l.TransactionGuid
	WHERE t.DeleteFlag = 0
		AND t.AliasName IN ('Receipt', 'Defuel')
		AND t.[Site] IN (
			SELECT *
			FROM @SiteTable
			)
		AND t.InventoryDate = @InventoryDate
	GROUP BY t.[Site]
		, t.ManagerID
		, l.Product

	-- sales issues
	INSERT INTO @IssueData
	SELECT t.[Site]
		, t.ManagerID
		, l.Product
		, SUM(ISNULL(l.GrossQuantity, 0))
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems l
		ON t.TransactionGuid = l.TransactionGuid
	WHERE t.DeleteFlag = 0
		AND t.AliasName IN ('Shipment', 'Retail Sale', 'Delivery Sale', 'Third Party Sale')
		AND t.[Site] IN (
			SELECT *
			FROM @SiteTable
			)
		AND t.InventoryDate = @InventoryDate
	GROUP BY t.[Site]
		, t.ManagerID
		, l.Product

	-- prepare results
	INSERT INTO @ResultTable
	SELECT ISNULL(receiptData.SiteID, issueData.SiteID)
		, ISNULL(receiptData.ManagerID, issueData.ManagerID)
		, ISNULL(receiptData.Product, issueData.Product)
		, ABS(ISNULL(receiptData.GrossQuantity, 0)) - ABS(ISNULL(issueData.GrossQuantity, 0)) -- abs as per specs
	FROM @ReceiptData receiptData
	FULL OUTER JOIN @IssueData issueData
		ON receiptData.SiteID = issueData.SiteID
			AND receiptData.ManagerID = issueData.ManagerID
			AND receiptData.Product = issueData.Product

	RETURN
END
