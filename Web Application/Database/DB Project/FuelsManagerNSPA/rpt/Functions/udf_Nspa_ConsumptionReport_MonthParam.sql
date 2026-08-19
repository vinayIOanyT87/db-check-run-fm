CREATE FUNCTION [rpt].[udf_Nspa_ConsumptionReport_MonthParam]
(
	@SiteID NVARCHAR(60)
)
RETURNS @ResultTable TABLE (SiteID NVARCHAR(60), Label NVARCHAR(60), Period DATETIME)
AS
BEGIN

DECLARE @SiteTable TABLE (ID NVARCHAR(60))

INSERT INTO @SiteTable SELECT * FROM [rpt].[udf_Nspa_SitesFromSiteGroup] ( @SiteID )

INSERT INTO @ResultTable
SELECT	DISTINCT 
		s.ID,
		DATENAME(MM, InventoryDate) + ' ' + CAST(YEAR(InventoryDate) AS VARCHAR(4)),
		NULL
FROM	tblTransactions t INNER JOIN @SiteTable s ON t.[Site] = s.ID
WHERE	DeleteFlag = 0
AND		(AliasName IN ('Retail Sales', 'Delivery Sales', 'Third-Party Sales') OR AliasName LIKE '%sale%')

UPDATE @ResultTable
SET	Period = CONVERT(DATETIME, Label)

RETURN

END