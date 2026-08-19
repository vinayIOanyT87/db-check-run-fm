CREATE FUNCTION dbo.udp_NSPA_ReconciliationException_InventoryVariance
(
	@SiteID			NVARCHAR(60),
	@InventoryDate	DATETIME
)
RETURNS FLOAT
AS
BEGIN

-- ideally should use a CLR to invoke ledger WCF, but due to time pressures everything will be SQL based
-- basically the total variance to current date is the today's physical inventory - total monthly movement + begin inventory

DECLARE @BeginInventoryTable TABLE (Product NVARCHAR(60), GrossQuantity FLOAT)
DECLARE @MovementTable TABLE (Product NVARCHAR(60), Movement FLOAT )
DECLARE @PhysicalInventoryTable TABLE (Product NVARCHAR(60), GrossQuantity FLOAT)

-- get begin inventory...

-- first get the latest physical inventory values
DECLARE @LatestPhysicalInventoryTable TABLE ( Product NVARCHAR(60), InventoryDate DATETIME, GrossQuantity FLOAT )
INSERT INTO @LatestPhysicalInventoryTable
SELECT	l.Product, MAX(t.InventoryDate), 0
FROM	tblTransactions t INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
WHERE	t.DeleteFlag = 0
AND		t.AliasName = 'Physical Inventory'
AND		t.[Site] = @SiteID
AND		t.InventoryDate < (DATEADD(MONTH, DATEDIFF(MONTH, 0, @InventoryDate), 0)) -- before month start
GROUP BY l.Product

UPDATE	p
SET		p.GrossQuantity = (
			SELECT	SUM(ISNULL(l.GrossQuantity, 0))
			FROM	tblTransactions t INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
			WHERE	t.DeleteFlag = 0
			AND		t.AliasName = 'Physical Inventory'
			AND		t.[Site] = @SiteID
			AND		t.InventoryDate = p.InventoryDate
			AND		l.Product = p.Product
			GROUP BY t.InventoryDate, l.Product
		)
FROM	@LatestPhysicalInventoryTable p

-- second add physical inventory to movement for begin inventory
INSERT	@BeginInventoryTable
SELECT	p.Product,
		p.GrossQuantity + ISNULL((
			SELECT	SUM(ISNULL(l.GrossQuantity, 0))
			FROM	tblTransactions t	INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
										INNER JOIN tblTransactionAliases a ON t.TransactionAliasGuid = a.TransactionAliasGuid
			WHERE	t.DeleteFlag = 0
			AND		a.LookupTransTypeIndex IN ( 1,2,3,4,5,6,8,13,15,16,25 ) -- someone to double check this
			AND		t.[Site] = @SiteID
			AND		t.InventoryDate > p.InventoryDate -- after latest dip
			AND		t.InventoryDate < (DATEADD(MONTH, DATEDIFF(MONTH, 0, @InventoryDate), 0)) -- before month start
		),0)
FROM	@LatestPhysicalInventoryTable p

-- get today (base) movements
INSERT INTO @MovementTable
SELECT	l.Product,
		ISNULL(SUM(ISNULL(l.GrossQuantity, 0)), 0)
FROM	tblTransactions t	INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
							INNER JOIN tblTransactionAliases a ON t.TransactionAliasGuid = a.TransactionAliasGuid
WHERE	t.DeleteFlag = 0
AND		a.LookupTransTypeIndex IN ( 1,2,3,4,5,6,8,13,15,16,25 ) -- someone to double check this
AND		t.[Site] = @SiteID
AND		MONTH(t.InventoryDate) = MONTH(@InventoryDate)
AND		DAY(t.InventoryDate) <= DAY(@InventoryDate)
GROUP BY l.Product

-- add begin book
UPDATE	m
SET		Movement = ISNULL(m.Movement, 0) + ISNULL(i.GrossQuantity, 0)
FROM	@MovementTable m INNER JOIN @BeginInventoryTable i ON m.Product = i.Product


-- get today's physical inventory
INSERT INTO @PhysicalInventoryTable
SELECT	l.Product,
		SUM(ISNULL(l.GrossQuantity, 0))
FROM	tblTransactions t	INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
WHERE	t.DeleteFlag = 0
AND		t.AliasName = 'Physical Inventory'
AND		t.[Site] = @SiteID
AND		t.InventoryDate = @InventoryDate
GROUP BY l.Product

IF (SELECT COUNT(*) FROM @PhysicalInventoryTable) = 0 RETURN NULL

-- calculate variance
RETURN	(
	SELECT	SUM(p.GrossQuantity - ISNULL(m.Movement, 0))
	FROM	@PhysicalInventoryTable p FULL OUTER JOIN @MovementTable m ON p.Product = m.Product
	WHERE	p.GrossQuantity IS NOT NULL -- no dip, no variance
)

END