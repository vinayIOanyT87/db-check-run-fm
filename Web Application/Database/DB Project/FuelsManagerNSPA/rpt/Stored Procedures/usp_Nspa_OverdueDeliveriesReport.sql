CREATE PROCEDURE [rpt].[usp_Nspa_OverdueDeliveriesReport] (
	@ViewingSiteID NVARCHAR(60)
	, @Aging INT
	)
AS
BEGIN
SET NOCOUNT ON

DECLARE @VolumeUnits INT

SET @VolumeUnits = (
		SELECT tblSites.VolumeUnitIndex
		FROM tblSites
		WHERE tblSites.ID = @ViewingSiteID
		)

DECLARE @VolumeDecimalPlaces INT

SET @VolumeDecimalPlaces = (
		SELECT tblSites.VolumeDecimalPlaces
		FROM tblSites
		WHERE tblSites.ID = @ViewingSiteID
		)

DECLARE @ReceiptData TABLE (
	FormNumber NVARCHAR(60)
	--, AliasName NVARCHAR(30)
	--, TransID NVARCHAR(64)
	--, ReversalType NVARCHAR(10)
	--, ReversedTransID NVARCHAR(64)
	--, BXP NVARCHAR(60)
	--, CreatedDate DATETIME
	)

-- Save the receipt data so we don't have to do query the database
-- each time. Querying the in-memory table is much faster.
INSERT INTO @ReceiptData
SELECT ud.UserData7
	--, t.AliasName
	--, t.TransID
	--, t.ReversalType
	--, t.ReversedTransID
	--, t.Site
	--, t.CreatedDate
FROM tblTransactions t
INNER JOIN tblTransactionUserData ud
	ON ud.TransactionGuid = t.TransactionGuid
WHERE AliasName = 'Receipt'
	AND ud.UserData7 IS NOT NULL
	AND (ReversalType IS NULL
		OR ReversalType IN ('', 'U'))

--select * from @ReceiptTable
DECLARE @ResultTable TABLE (
	DocumentNumber NVARCHAR(60)
	, VehicleNumber NVARCHAR(60)
	, BXP NVARCHAR(60)
	, EntryDate DATETIME
	, Destination NVARCHAR(120)
	, Operator NVARCHAR(120)
	, Quantity FLOAT
	, Aging INT
	)
DECLARE @CurrentDate DATETIME = GETDATE()
DECLARE @SiteList TABLE (SiteID NVARCHAR(60))

INSERT INTO @SiteList
SELECT *
FROM [rpt].[udf_Nspa_SitesFromSiteGroup](@ViewingSiteID)

INSERT INTO @ResultTable
SELECT t.DocumentNumber -- DocumentNumber
	, ud.UserData4 -- VehicleNumber
	, t.[Site] -- BXP
	, t.Date01 -- EntryDate
	, t.FinalStationIATAID -- Destination
	, t.OperatorID -- Operator
	, dbo.udf_ConvertFromSIUnits(ISNULL(li.NetQuantity, 0), @VolumeUnits, @VolumeDecimalPlaces) -- Quantity
	, DATEDIFF(DAY, t.Date01, @CurrentDate) -- aging
FROM tblTransactions t
INNER JOIN tblTransactionLineItems li
	ON t.TransactionGuid = li.TransactionGuid
INNER JOIN tblTransactionUserData ud
	ON ud.TransactionGuid = t.TransactionGuid
WHERE t.AliasName = 'Customs Transfer'
	AND t.DocumentNumber NOT IN (
		SELECT FormNumber
		FROM @ReceiptData
		)
	AND t.[Site] IN (
		SELECT SiteID
		FROM @SiteList
		)
	AND t.DeleteFlag = 0
	AND li.DeleteFlag = 0
	AND (
		t.ReversalType IS NULL
		OR t.ReversalType IN ('', 'U')
	)

-- filter against aging, use <= because spec is "more than" not "equal or more".
SELECT *
FROM @ResultTable
WHERE Aging > @Aging
ORDER BY EntryDate
	, Quantity DESC

END