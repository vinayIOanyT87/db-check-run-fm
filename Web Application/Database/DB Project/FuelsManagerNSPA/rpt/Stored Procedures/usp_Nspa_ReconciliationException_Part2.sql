CREATE PROCEDURE [rpt].usp_Nspa_ReconciliationException_Part2 (
	@SiteID VARCHAR(60)
	, @InventoryDate DATETIME
	)
AS
BEGIN

DECLARE @BeginDate DATETIME

SET @BeginDate = DATEADD(MONTH, DATEDIFF(MONTH, 0, @InventoryDate), 0)

DECLARE @PreviousDate DATETIME

SET @PreviousDate = DateAdd(Day, - 1, @InventoryDate)

DECLARE @siteguidstr VARCHAR(100)

SELECT @siteguidstr = s.SiteGuid
FROM tblSites s
WHERE s.ID = @SiteID

DECLARE @DecimalPlaces INT
DECLARE @VolumeUnits INT

SELECT @VolumeUnits = s.VolumeUnitIndex
	, @DecimalPlaces = s.VolumeDecimalPlaces
FROM tblsites s
WHERE s.SiteGuid = @SiteGuidStr

DECLARE @Products TABLE (ProductId VARCHAR(60))

INSERT INTO @Products
SELECT ProductId
FROM vw_ProductGroupProducts pgp
WHERE pgp.ProductGroupID = 'Fuel Products'

DECLARE @ChildSites TABLE (
	SiteID VARCHAR(60)
	, SiteGuid UNIQUEIDENTIFIER
	, SiteGroupFlag BIT
	)

INSERT INTO @ChildSites
SELECT s.ID
	, s.SiteGuid
	, s.SiteGroupFlag
FROM map.tblSiteToSite sts
INNER JOIN tblSites s
	ON s.SiteGuid = sts.ChildSiteGuid
WHERE sts.ParentSiteGuid = CASE ISNULL(@SiteGuidStr, '')
		WHEN ''
			THEN (
					SELECT TOP 1 SiteGuid
					FROM tblsites
					WHERE ID = @SiteID
					)
		ELSE @SiteGuidStr
		END

--SELECT * FROM @ChildSites
DECLARE @ResultTable TABLE (
	SiteID VARCHAR(60)
	, ManagerId VARCHAR(60)
	, AliasName VARCHAR(60)
	, ProductID VARCHAR(60)
	, ReceiptDelta FLOAT
	, IssueDelta FLOAT
	, MeterRotationDelta FLOAT
	, PhysicalInventoryPrevious FLOAT
	, PhysicalInventoryCurrent FLOAT
	)

INSERT INTO @ResultTable
SELECT s.SiteID SiteID
	, ManagersPerSite.ManagerId
	, a.AliasName
	, p.ProductID
	, 0 ReceiptDelta
	, 0 IssueDelta
	, 0 MeterRotationDelta
	, 0 PhysicalInventoryPrevious
	, 0 PhysicalInventoryCurrent
FROM tblTransactionAliases a
	, @Products p
	, @ChildSites s
	, (
		SELECT c.ID ManagerId
		FROM map.tblCompanyToRole ctr
		INNER JOIN tblsites s
			ON s.SiteGuid = ctr.SiteGuid
		INNER JOIN tblCompanies c
			ON c.CompanyGuid = ctr.CompanyGuid
		WHERE s.SiteGuid = @SiteGuidStr
			AND ctr.LookupCompanyRoleIndex = 0
		) ManagersPerSite
WHERE AliasName IN ('Receipt', 'Issue', 'Meter Rotation', 'Physical Inventory')
	AND p.ProductID IN (
		SELECT *
		FROM @Products
		)
	AND (s.SiteGroupFlag = 0)
ORDER BY p.ProductID

--SELECT 'Step 0 - Setup'
--	, *
--FROM @ResultTable
UPDATE @ResultTable
SET ReceiptDelta = DeltaTemp.Delta
FROM @ResultTable
INNER JOIN (
	SELECT t.Site AS SiteID
		, t.ManagerID
		, 'Receipt' AS AliasName
		, li.Product
		, sum(li.MeterStop) - sum(li.MeterStart) AS Delta
	FROM tbltransactions t
	INNER JOIN tblTransactionLineItems li
		ON li.TransactionGuid = t.TransactionGuid
	WHERE t.AliasName IN ('Receipt', 'Defuel')
		AND t.TransID IS NOT NULL
		AND t.InventoryDate BETWEEN @BeginDate AND @InventoryDate
	GROUP BY t.Site
		, t.ManagerID
		, li.Product
	) AS DeltaTemp
	ON DeltaTemp.AliasName = [@ResultTable].AliasName
		AND DeltaTemp.Product = [@ResultTable].ProductID
		AND DeltaTemp.SiteID = [@ResultTable].SiteID
		AND DeltaTemp.ManagerID = [@ResultTable].ManagerId

--and li.Product='f-54-sum'
--select 'Step 1 - Receipt Meter Deltas', * from @ResultTable
UPDATE @ResultTable
SET IssueDelta = DeltaTemp.Delta
FROM @ResultTable
INNER JOIN (
	SELECT t.Site AS SiteID
		, t.ManagerID
		, 'Issue' AS AliasName
		, li.Product
		, sum(li.MeterStop) - sum(li.MeterStart) AS Delta
	--, li.MeterStop, li.MeterStart
	FROM tbltransactions t
	INNER JOIN tblTransactionLineItems li
		ON li.TransactionGuid = t.TransactionGuid
	WHERE t.AliasName IN ('Retail Sale', 'Delivery Sale', 'Third Party Sale', 'Shipment')
		AND t.TransID IS NOT NULL
		AND t.InventoryDate BETWEEN @BeginDate AND @InventoryDate
	GROUP BY t.Site
		, t.ManagerID
		, li.Product
	) AS DeltaTemp
	ON DeltaTemp.AliasName = [@ResultTable].AliasName
		AND DeltaTemp.Product = [@ResultTable].ProductID
		AND DeltaTemp.SiteID = [@ResultTable].SiteID
		AND DeltaTemp.ManagerID = [@ResultTable].ManagerId

--select 'Step 2 - Issue Meter Deltas', * from @ResultTable
--where aliasname='issue'
UPDATE @ResultTable
SET MeterRotationDelta = DeltaTemp.Delta
FROM @ResultTable
INNER JOIN (
	SELECT t.Site AS SiteID
		, t.ManagerID
		, 'Meter Rotation' AS AliasName
		, li.Product
		, sum(li.MeterStop) - sum(li.MeterStart) AS Delta
	FROM tbltransactions t
	INNER JOIN tblTransactionLineItems li
		ON li.TransactionGuid = t.TransactionGuid
	WHERE t.AliasName IN ('Meter Rotation')
		AND t.TransID IS NOT NULL
		AND t.InventoryDate BETWEEN @BeginDate AND @InventoryDate
	GROUP BY t.Site
		, t.ManagerID
		, li.Product
	) AS DeltaTemp
	ON DeltaTemp.AliasName = [@ResultTable].AliasName
		AND DeltaTemp.Product = [@ResultTable].ProductID
		AND DeltaTemp.SiteID = [@ResultTable].SiteID
		AND DeltaTemp.ManagerID = [@ResultTable].ManagerId

--select 'Step 3 - Meter Rotation Deltas', * from @ResultTable
--where aliasname='Meter Rotation'
UPDATE @ResultTable
SET PhysicalInventoryPrevious = grossTemp.Quantity
FROM @ResultTable
INNER JOIN (
	SELECT t.Site AS SiteID
		, t.ManagerID
		, 'Physical Inventory' AS aliasname
		, li.Product
		, sum(dbo.udf_ConvertFromSIUnits(li.GrossQuantity, @VolumeUnits, @DecimalPlaces)) AS Quantity
	FROM tbltransactions t
	INNER JOIN tblTransactionLineItems li
		ON li.TransactionGuid = t.TransactionGuid
	WHERE t.AliasName IN ('Physical Inventory')
		AND t.TransID IS NOT NULL
		AND t.InventoryDate = @PreviousDate
	GROUP BY t.Site
		, t.ManagerID
		, li.Product
	) AS grossTemp
	ON grossTemp.AliasName = [@ResultTable].AliasName
		AND grossTemp.Product = [@ResultTable].ProductID
		AND grossTemp.SiteID = [@ResultTable].SiteID
		AND grossTemp.ManagerID = [@ResultTable].ManagerId

--select 'Step 4 - Previous Physical Inventory', * from @ResultTable
--where aliasname='Physical Inventory'
UPDATE @ResultTable
SET PhysicalInventoryCurrent = grossTemp.Quantity
FROM @ResultTable
INNER JOIN (
	SELECT t.Site AS SiteID
		, t.ManagerID AS ManagerId
		, 'Physical Inventory' AS aliasname
		, li.Product
		, sum(dbo.udf_ConvertFromSIUnits(li.GrossQuantity, @VolumeUnits, @DecimalPlaces)) AS Quantity
	FROM tbltransactions t
	INNER JOIN tblTransactionLineItems li
		ON li.TransactionGuid = t.TransactionGuid
	WHERE t.AliasName IN ('Physical Inventory')
		AND t.TransID IS NOT NULL
		AND t.InventoryDate = @InventoryDate
	GROUP BY t.Site
		, t.ManagerID
		, li.Product
	) AS grossTemp
	ON grossTemp.AliasName = [@ResultTable].AliasName
		AND grossTemp.Product = [@ResultTable].ProductID
		AND grossTemp.SiteID = [@ResultTable].SiteID
		AND grossTemp.ManagerID = [@ResultTable].ManagerId

--SELECT *
--FROM @ResultTable
SELECT SiteID
	, ManagerID
	--, sum(PhysicalInventoryCurrent) - sum(PhysicalInventoryPrevious) AS PhysicalInventoryVariance
	, sum(PhysicalInventoryCurrent) - sum(PhysicalInventoryPrevious) - (sum(rt.ReceiptDelta) - sum(rt.IssueDelta)) AS MeterVariance
FROM @ResultTable rt
GROUP BY SiteID
	, ManagerID
ORDER BY SiteID
	, ManagerID

END
GO


