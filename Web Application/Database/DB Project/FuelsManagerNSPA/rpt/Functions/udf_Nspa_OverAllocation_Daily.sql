CREATE FUNCTION [rpt].[udf_Nspa_OverAllocation_Daily]
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
	InventoryDate		DATETIME
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

INSERT INTO @CardTable SELECT	* FROM	[rpt].[udf_Nspa_OverAllocation_FuelCards]('Day')

INSERT INTO @ResultTable
SELECT	c.FuelCardGuid,
		c.AllocationType,
		c.Limit,
		SUM(ISNULL(l.NetQuantity,0)),
		InventoryDate
FROM	tblTransactions t	INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
							INNER JOIN @CardTable c ON t.FuelCardGuid = c.FuelCardGuid AND l.ProductGuid = c.ProductGuid
							-- restrict to cards assigned to transaction site
							INNER JOIN map.tblEntityFuelCardLimitToSite e ON e.FuelCardLimitGuid = c.LimitGuid AND e.SiteGuid = t.SiteGuid
WHERE	t.AliasName IN ('Retail Sale', 'Delivery Sale', 'Third-Party Sale', 'Oil & Lube Sale')
AND		t.DeleteFlag = 0
AND		t.InventoryDate BETWEEN @StartDate AND @EndDate
GROUP BY 
		c.FuelCardGuid, 
		c.AllocationType, 
		c.Limit, 
		t.InventoryDate
HAVING	-SUM(ISNULL(l.NetQuantity,0)) > c.Limit -- limit is in SI (as per Ryan's response)

RETURN

END