CREATE FUNCTION dbo.NSPA_ConsumptionReport_MonthParam
(
	@SiteID NVARCHAR(60)
)
RETURNS @ResultTable TABLE (SiteID NVARCHAR(60), Label NVARCHAR(60), Period DATETIME)
AS
BEGIN

DECLARE @SiteTable TABLE (ID NVARCHAR(60))

INSERT INTO @SiteTable SELECT * FROM dbo.NSPA_SitesFromSiteGroup ( @SiteID )

INSERT INTO @ResultTable
SELECT	DISTINCT 
		s.ID,
		DATENAME(MM, InventoryDate) + ' ' + CAST(YEAR(InventoryDate) AS VARCHAR(4)),
		NULL
FROM	tblTransactions t INNER JOIN @SiteTable s ON t.[Site] = s.ID
WHERE	DeleteFlag = 0
AND		(AliasName IN ('Retail Sale', 'Delivery Sale', 'Third-Party Sale'))

UPDATE @ResultTable
SET	Period = CONVERT(DATETIME, Label)

RETURN

END