CREATE PROCEDURE [rpt].[usp_Nspa_PhysicalInventoryReport]
(
	@ViewingSiteID	NVARCHAR(60),
	@InventoryDate	DATETIME
)
AS
BEGIN
SET NOCOUNT ON

DECLARE @VolumeUnits int
SET		@VolumeUnits = (SELECT tblSites.VolumeUnitIndex FROM tblSites WHERE tblSites.ID = @ViewingSiteID)

DECLARE @VolumeDecimalPlaces int
SET		@VolumeDecimalPlaces = (SELECT tblSites.VolumeDecimalPlaces FROM tblSites WHERE tblSites.ID = @ViewingSiteID)

DECLARE @ResultTable TABLE
(
	SiteID			NVARCHAR(60),
	ManagerID		NVARCHAR(60),
	TankID			NVARCHAR(60),
	ProductID		NVARCHAR(60),
	GrossQuantity	FLOAT,
	NetQuantity		FLOAT,
	Density			FLOAT,
	Temperature		FLOAT,
	VCF				FLOAT
)

DECLARE @AggregateTable TABLE
(
	PartitionNumber	INT,
	SiteID			NVARCHAR(60),
	ManagerID		NVARCHAR(60),
	TankID			NVARCHAR(60),
	ProductID		NVARCHAR(60),
	GrossQuantity	FLOAT,
	NetQuantity		FLOAT,
	Density			FLOAT,
	Temperature		FLOAT,
	VCF				FLOAT,
	TankGuid		NVARCHAR(64)
)

DECLARE @SiteList TABLE (SiteID NVARCHAR(60))
INSERT INTO @SiteList
SELECT	*
FROM	[rpt].[udf_Nspa_SitesFromSiteGroup](@ViewingSiteID)

INSERT INTO @AggregateTable
SELECT	ROW_NUMBER() OVER 
		(
			PARTITION BY t.[Site], t.ManagerID, l.StorageLocationID, l.Product
			ORDER BY TransDateTime DESC
		), -- use partition to find the latest dip on the day (i.e. tank was dipped twice)
		--t.InventoryDate,
		t.[Site],
		t.ManagerID,
		l.StorageLocationID,
		l.Product,
		l.GrossQuantity,
		l.NetQuantity,
		l.Density,
		l.Temperature,
		l.Vcf,
		l.StorageLocationTankGuid
FROM	tblTransactions t INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
WHERE	t.DeleteFlag = 0
AND		t.AliasName = 'Physical Inventory'
AND		t.InventoryDate = @InventoryDate
AND		t.[Site] IN (SELECT SiteID FROM @SiteList)
--order by [site], [managerid], [StorageLocationID], InventoryDate

--select * from @AggregateTable

-- get the latest dips
INSERT INTO @ResultTable
SELECT	SiteID,
		ManagerID,
		TankID,
		ProductID,
		dbo.udf_ConvertFromSIUnits(ISNULL(GrossQuantity, 0), @VolumeUnits, @VolumeDecimalPlaces),
		dbo.udf_ConvertFromSIUnits(ISNULL(NetQuantity, 0), @VolumeUnits, @VolumeDecimalPlaces),
		Density,
		Temperature,
		Vcf
FROM	@AggregateTable
WHERE	PartitionNumber = 1

-- get tanks that weren't dipped
INSERT INTO @ResultTable
SELECT	s.ID,
		c.ID,
		t.TankID,
		p.ProductID,
		NULL,
		NULL,
		NULL,
		NULL,
		NULL
FROM	tblTanks t	INNER JOIN tblCompanies c ON t.ManagerCompanyGuid = c.CompanyGuid
					INNER JOIN tblSites s ON t.SiteGuid = s.SiteGuid
					INNER JOIN tblProducts p ON t.ProductGuid = p.ProductGuid
WHERE	t.TankGuid NOT IN (SELECT TankGuid FROM @AggregateTable WHERE PartitionNumber = 1 AND TankGuid IS NOT NULL)
AND		s.ID IN (SELECT SiteID FROM @SiteList)

SELECT * FROM @ResultTable
ORDER BY SiteID, ManagerID, ProductID, TankID, NetQuantity

END