CREATE PROCEDURE [rpt].[Nspa_GetAdjustmentSubtotals] @SiteID VARCHAR(30)
	, @BeginDate DATETIME
	, @EndDate DATETIME
AS

BEGIN

DECLARE @VolumeUnitIndex INT
DECLARE @VolumeDecimalPlaces INT
DECLARE @SiteGuid UNIQUEIDENTIFIER

SELECT @SiteGuid = SiteGuid
FROM tblSites
WHERE ID = @SiteId

SELECT @VolumeUnitIndex = s.VolumeUnitIndex
	, @VolumeDecimalPlaces = s.VolumeDecimalPlaces
FROM tblSites s
WHERE s.SiteGuid = @SiteGuid

SET @EndDate = DATEADD(MILLISECOND, - 1, DATEADD(Day, 1, @EndDate))

SELECT @SiteId AS SiteId
	, ap.ProductId AS ProductId
	, Coalesce(mainTrx.NegativeNet, 0) AS NegativeNet
	, Coalesce(mainTrx.PositiveNet, 0) AS PositiveNet
	, Coalesce(mainTrx.NegativeGross, 0) AS NegativeGross
	, Coalesce(mainTrx.PositiveGross, 0) AS PositiveGross
	, ap.AliasName
	, mainTrx.ManagerID as ManagerId
FROM (
	SELECT li.Product
		, SUM(CASE 
				WHEN li.NetQuantity > 0
					THEN dbo.udf_ConvertFromSIUnits(li.NetQuantity, @VolumeUnitIndex, @VolumeDecimalPlaces)
				WHEN li.NetQuantity < 0
					THEN 0
				END) AS PositiveNet
		, SUM(CASE 
				WHEN li.NetQuantity < 0
					THEN dbo.udf_ConvertFromSIUnits(li.NetQuantity, @VolumeUnitIndex, @VolumeDecimalPlaces)
				WHEN li.NetQuantity > 0
					THEN 0
				END) AS NegativeNet
		, SUM(CASE 
				WHEN li.GrossQuantity > 0
					THEN dbo.udf_ConvertFromSIUnits(li.GrossQuantity, @VolumeUnitIndex, @VolumeDecimalPlaces)
				WHEN li.GrossQuantity < 0
					THEN 0
				END) AS PositiveGross
		, SUM(CASE 
				WHEN li.GrossQuantity < 0
					THEN dbo.udf_ConvertFromSIUnits(li.GrossQuantity, @VolumeUnitIndex, @VolumeDecimalPlaces)
				WHEN li.GrossQuantity > 0
					THEN 0
				END) AS NegativeGross
		, t.AliasName
		, t.ManagerID
	FROM tblTransactions t
	INNER JOIN tblTransactionLineItems li
		ON li.TransactionGuid = t.TransactionGuid
	WHERE t.LookupTransTypeIndex IN (
			SELECT TransactionTypesIndex
			FROM lookup.tblTransactionTypes
			WHERE TransactionTypesCode IN ('T1_PrimaryAdjustment', 'T15_PrimaryRegrade')
				AND t.InventoryDate BETWEEN @BeginDate AND @EndDate
				AND SiteGuid IN (
					SELECT s.SiteGuid
					FROM map.tblSiteToSite sts
					INNER JOIN tblSites s
						ON s.SiteGuid = sts.ChildSiteGuid
					WHERE sts.ParentSiteGuid = (
							SELECT SiteGuid
							FROM tblSites
							WHERE ID = @SiteID
							)
					)
				AND t.DeleteFlag = 0
				AND li.DeleteFlag = 0
				AND (
					t.ReversalType IS NULL
					OR t.ReversalType IN ('', 'U')
					)
			)
	GROUP BY li.Product
		, t.managerid
		, t.AliasName
	) AS mainTrx
RIGHT OUTER JOIN (
	SELECT *
	FROM (
		SELECT ta.AliasName
		FROM tblTransactionAliases ta
		WHERE ta.aliasName IN ('Adjustment', 'Regrade')
		) aliases
		, (
			SELECT ProductID
			FROM vw_ProductGroupProducts
			WHERE ProductGroupID = 'Fuel Products'
			) products
	) ap
	ON ap.ProductId = mainTrx.Product
		AND ap.AliasName = mainTrx.AliasName

END