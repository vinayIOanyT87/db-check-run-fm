CREATE FUNCTION dbo.NSPA_OverAllocation_Monthly
(
	@StartDate		DATETIME,
	@EndDate		DATETIME
)
RETURNS @ResultTable TABLE
(
	FuelCardGuid		NVARCHAR(60),
	AllocationType		NVARCHAR(60),
	AllocationLimit		FLOAT,
	QuantityDelivered	FLOAT,
	Period				DATETIME
)
AS
BEGIN

-- get a list of fuel cards that's transactional
DECLARE @CardTable TABLE 
(
	FuelCardGuid	UNIQUEIDENTIFIER,
	LimitGuid		UNIQUEIDENTIFIER,
	AllocationType	NVARCHAR(60),		-- used for report display
	ProductGuid		UNIQUEIDENTIFIER,	-- used for joining to transactions
	Limit			FLOAT
)

INSERT INTO @CardTable SELECT	* FROM	NSPA_OverAllocation_FuelCards('Month')

INSERT INTO @ResultTable
SELECT	c.FuelCardGuid,
		c.AllocationType,
		c.Limit,
		SUM(ISNULL(l.NetQuantity,0)),
		(SELECT CONVERT(DATETIME, CONVERT(NVARCHAR, YEAR(t.InventoryDate)) + '-' + CONVERT(NVARCHAR, MONTH(t.InventoryDate)) + '-01'))
FROM	tblTransactions t	INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
							INNER JOIN @CardTable c ON t.FuelCardGuid = c.FuelCardGuid AND l.ProductGuid = c.ProductGuid
							-- ensure cards and limits are assigned to transaction site
							INNER JOIN map.tblEntityFuelCardLimitToSite e ON e.FuelCardLimitGuid = c.LimitGuid AND e.SiteGuid = t.SiteGuid
							INNER JOIN map.tblEntityFuelCardToSite f ON f.FuelCardGuid = c.FuelCardGuid AND f.SiteGuid = t.SiteGuid
WHERE	t.AliasName IN ('Retail Sale', 'Delivery Sale', 'Third-Party Sale', 'Oil & Lube Sale')
AND		t.DeleteFlag = 0
AND		(t.ReversalType IS NULL OR t.ReversalType IN ('', 'U'))
-- ensure monthly allocations only take transactions between specified dates rather than whole month
AND		t.InventoryDate BETWEEN @StartDate AND @EndDate
GROUP BY 
		c.FuelCardGuid, 
		c.AllocationType, 
		c.Limit, 
		MONTH(t.InventoryDate),
		YEAR(t.InventoryDate)
HAVING	-SUM(ISNULL(l.NetQuantity,0)) > c.Limit -- limit is in SI (as per Ryan's response)

RETURN

END