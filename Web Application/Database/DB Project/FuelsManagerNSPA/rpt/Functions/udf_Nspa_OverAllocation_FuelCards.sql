CREATE FUNCTION [rpt].[udf_Nspa_OverAllocation_FuelCards]
(
	@Period	NVARCHAR(16)
)
RETURNS @ResultTable TABLE 
(
	FuelCardGuid	UNIQUEIDENTIFIER,
	LimitGuid		UNIQUEIDENTIFIER,
	AllocationType	NVARCHAR(60),		-- used for report display
	ProductGuid		UNIQUEIDENTIFIER,	-- used for joining to transactions
	Limit			FLOAT
)
AS
BEGIN

-- get all the cards with a product as type
INSERT INTO @ResultTable
SELECT	c.FuelCardGuid,
		m.FuelCardLimitGuid,
		'Product',
		l.ProductGuid,
		l.Limit
FROM	tblFuelCards c	INNER JOIN map.tblFuelCardLimitToFuelCard m ON c.FuelCardGuid = m.FuelCardGuid
						-- for allocation type
						INNER JOIN tblFuelCardLimitLineItem l ON l.FuelCardLimitGuid = m.FuelCardLimitGuid
						-- for period and period limits
						INNER JOIN lookup.tblFuelCardLimitPeriod p ON p.FuelCardLimitPeriodIndex = l.Period
WHERE	l.ProductGuid IS NOT NULL
AND		p.FuelCardLimitPeriodName = @Period

-- get all the groups
DECLARE @CardProductGroups TABLE
(
	FuelCardGuid	UNIQUEIDENTIFIER,
	LimitGuid		UNIQUEIDENTIFIER,
	GroupGuid		UNIQUEIDENTIFIER,
	Limit			FLOAT
)

-- get all the groups
INSERT INTO @CardProductGroups
SELECT	c.FuelCardGuid,
		m.FuelCardLimitGuid,
		l.ProductGroupApplicationStringGuid,
		l.Limit
FROM	tblFuelCards c	INNER JOIN map.tblFuelCardLimitToFuelCard m ON c.FuelCardGuid = m.FuelCardGuid
						-- for allocation type
						INNER JOIN tblFuelCardLimitLineItem l ON l.FuelCardLimitGuid = m.FuelCardLimitGuid
						-- for period and period limits
						INNER JOIN lookup.tblFuelCardLimitPeriod p ON p.FuelCardLimitPeriodIndex = l.Period
WHERE	l.ProductGuid IS NULL
AND		p.FuelCardLimitPeriodName = @Period

-- now split the null ones into result table
INSERT INTO @ResultTable
SELECT	DISTINCT
		g.FuelCardGuid,
		g.LimitGuid,
		'All Products',	
		p.ProductGuid,
		g.Limit
FROM	@CardProductGroups g, tblProducts p
WHERE	g.GroupGuid IS NULL

-- now split the groups into result table
INSERT INTO @ResultTable
SELECT	DISTINCT
		g.FuelCardGuid,
		g.LimitGuid,
		(SELECT ID FROM tblApplicationString WHERE ApplicationStringGuid = g.GroupGuid),
		p.ProductGuid,
		g.Limit
FROM	tblProducts p	INNER JOIN map.tblProductToProductGroup m ON m.ProductGuid = p.ProductGuid
						INNER JOIN @CardProductGroups g ON g.GroupGuid = m.AssignedToApplicationStringGuid
WHERE	g.GroupGuid IS NOT NULL

RETURN

END