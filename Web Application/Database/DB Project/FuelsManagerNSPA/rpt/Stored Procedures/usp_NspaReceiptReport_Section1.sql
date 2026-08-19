CREATE PROCEDURE [rpt].[usp_NspaReceiptReport_Section1] @BeginDate DATETIME
	,@EndDate DATETIME
	,@SiteID VARCHAR(30)
	,@SecurityToken UNIQUEIDENTIFIER
AS
BEGIN

DECLARE @VolumeUnitIndex INT
DECLARE @VolumeDecimalPlaces INT
DECLARE @SiteGuid UNIQUEIDENTIFIER

SELECT @SiteGuid = SiteGuid
FROM tblSites
WHERE ID = @SiteID

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
	,t.InventoryDate
	,t.GateID AS BFI
	,t.ShippingDocumentNumber AS [GBL Number]
	,ud.UserData7 AS [Customs Document Number]
	,li.StorageLocationID AS TankId
	,SUM(COALESCE(t.Number02, 0)) AS [Shipping Quantity]
	,dbo.udf_ConvertFromSIUnits(sum(COALESCE(li.NetQuantity, 0)), @VolumeUnitIndex, @VolumeDecimalPlaces) AS [Quantity @ 15C]
	,dbo.udf_ConvertFromSIUnits(sum(COALESCE(li.NetQuantity, 0)), @VolumeUnitIndex, @VolumeDecimalPlaces) - SUM(COALESCE(t.Number02, 0)) AS Variance
	,@EngineeringUnitName EngineeringUnitName
FROM tblTransactions t
INNER JOIN tblTransactionLineItems li ON li.TransactionGuid = t.TransactionGuid
INNER JOIN tblTransactionUserData ud ON ud.TransactionGuid = t.TransactionGuid
WHERE t.LookupTransTypeIndex = (
		SELECT TransactionTypesIndex
		FROM lookup.tblTransactionTypes
		WHERE TransactionTypesCode = 'T8_Receipt'
		)
	AND t.InventoryDate BETWEEN @BeginDate
		AND @EndDate
	AND SiteGuid IN (
		SELECT s.SiteGuid
		FROM map.tblSiteToSite sts
		INNER JOIN tblSites s ON s.SiteGuid = sts.ChildSiteGuid
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
		OR t.ReversalType IN (
			''
			,'U'
			)
		) -- verify that these are correct
GROUP BY t.Site
	,t.managerid
	,t.TransactionGuid
	,li.Product
	,t.InventoryDate
	,t.GateID
	,t.ShippingDocumentNumber
	,t.AssociatedDocNumber
	,li.StorageLocationID
	,ud.UserData7
ORDER BY t.InventoryDate ASC

END