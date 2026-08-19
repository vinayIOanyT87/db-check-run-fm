CREATE PROCEDURE [rpt].[usp_NspaReceiptReport_Section2] @BeginDate DATETIME
	,@EndDate DATETIME
	,@SiteID VARCHAR(30)
	,@SecurityToken UNIQUEIDENTIFIER
AS
BEGIN

DECLARE @SiteGuid UNIQUEIDENTIFIER

SELECT @SiteGuid = SiteGuid
FROM tblSites
WHERE ID = @SiteID

DECLARE @VolumeUnitIndex INT
DECLARE @VolumeDecimalPlaces INT

SELECT @VolumeUnitIndex = s.VolumeUnitIndex
	,@VolumeDecimalPlaces = s.VolumeDecimalPlaces
FROM tblSites s
WHERE s.SiteGuid = @SiteGuid

DECLARE @EngineeringUnitName VARCHAR(100)

SELECT @EngineeringUnitName = eu.EngineeringUnitName
FROM lookup.tblEngineeringUnit eu
WHERE EngineeringUnitIndex = @VolumeUnitIndex

SET @EndDate = DATEADD(MILLISECOND, - 1, DATEADD(Day, 1, @EndDate))

SELECT t.Site AS SiteID
	,t.ManagerID
	,li.Product
	,SUM(COALESCE(t.Number02, 0)) AS [Shipping Quantity]
	,dbo.udf_ConvertFromSIUnits(sum(COALESCE(li.NetQuantity, 0)), @VolumeUnitIndex, @VolumeDecimalPlaces) AS [Quantity @ 15C]
	,dbo.udf_ConvertFromSIUnits(sum(COALESCE(li.NetQuantity, 0)), @VolumeUnitIndex, @VolumeDecimalPlaces) - SUM(COALESCE(t.Number02, 0)) AS Variance
FROM tblTransactions t
INNER JOIN tblTransactionLineItems li ON li.TransactionGuid = t.TransactionGuid
INNER JOIN tblSites s ON s.SiteGuid = t.SiteGuid
WHERE t.LookupTransTypeIndex = (
		SELECT TransactionTypesIndex
		FROM lookup.tblTransactionTypes
		WHERE TransactionTypesCode = 'T8_Receipt'
		)
	AND t.InventoryDate BETWEEN @BeginDate
		AND @EndDate
	AND t.DeleteFlag = 0
	AND li.DeleteFlag = 0
	AND (
		t.ReversalType IS NULL
		OR t.ReversalType IN (
			''
			,'U'
			)
		)
GROUP BY t.Site
	,t.ManagerID
	,li.Product
ORDER BY t.Site
	,li.Product

END