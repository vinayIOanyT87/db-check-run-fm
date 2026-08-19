
CREATE PROCEDURE [rpt].[usp_ManagedConsumptionDailyReport]
@SiteGuid UNIQUEIDENTIFIER, @LoginSiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @PriorStartDate DATETIMEOFFSET(7), @Product NVARCHAR (50)
AS
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
SET NOCOUNT ON
	DECLARE @VolumeUnits int
	SET @VolumeUnits = (SELECT dbo.tblSites.VolumeUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @LoginSiteGuid)

	DECLARE @VolumeDecimalPlaces int
	SET @VolumeDecimalPlaces = (SELECT dbo.tblSites.VolumeDecimalPlaces FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @LoginSiteGuid)

	DECLARE @AuthorizedCompanies TABLE (
		[ID] [nvarchar] (100) NOT NULL
	);
	INSERT INTO @AuthorizedCompanies SELECT * FROM udf_AuthorizedCompanies(@LoginSiteGuid,@SiteGuid,@UserGuid)

	DECLARE @InventoryDates TABLE( 
		[InventoryDate][DATE] NOT NULL );
	INSERT INTO @InventoryDates SELECT DISTINCT InventoryDate FROM dbo.tblTransactions 
	WHERE dbo.tblTransactions.UpdatedDate >= @PriorStartDate AND 
		 dbo.tblTransactions.UpdatedDate <= GetDate() AND
		 AliasName IN ('Bulk Issue','Defuel','Issue') AND 
		 ((CarrierID IN (SELECT * FROM  @AuthorizedCompanies))  OR 
		 (ShipperID IN (SELECT * FROM @AuthorizedCompanies)) OR 
		 (ShipToID IN (SELECT * FROM @AuthorizedCompanies)) OR 
		 (SupplierID IN (SELECT * FROM @AuthorizedCompanies)) OR 
		 (ManagerID IN (SELECT * FROM @AuthorizedCompanies)) OR 
		 (OwnerID IN (SELECT * FROM @AuthorizedCompanies))  OR 
		 (BillToID IN (SELECT * FROM @AuthorizedCompanies))) AND 
		 Site in (Select ID from dbo.tblSites, [map].[tblSiteToSite] where ParentSiteGuid = @SiteGuid and [map].[tblSiteToSite].ChildSiteGuid = dbo.tblSites.SiteGuid) 

	SELECT InventoryDate, 
		 Site,
		 AliasName,
		 ManagerID,
		 OwnerID, 
		 CarrierID, 
		 ShipToID, 
		 Product,  
		 dbo.tblTransactionLineItems.DocumentNumber AS ShipmentNumber, 
		 DestinationRegistrationID1, 
		 RoutingID, 
		 SourceRegistrationID1, 
		 DestinationEquipmentModel1,
		 SourceEquipmentModel1,
		 MeterStart, 
		 MeterStop, 
		 dbo.udf_ConvertFromSIUnits(abs(GrossQuantity),@VolumeUnits,@VolumeDecimalPlaces) AS GrossQuantity,
		 dbo.udf_ConvertFromSIUnits(abs(NetQuantity),@VolumeUnits,@VolumeDecimalPlaces) AS NetQuantity,
		 1 AS FlightCnt,
		 CASE SIGN(DATEDIFF(second, dbo.tblTransactions.UpdatedDate, @PriorStartDate)) 
			WHEN -1 then dbo.tblTransactions.UpdatedDate
			ELSE NULL 
			END AS IsModified,
		dbo.tblTransactions.DeleteFlag AS IsDeleted,
		Notes
	FROM   dbo.tblTransactions, dbo.tblTransactionLineItems, dbo.tblTransactionNotes
	WHERE  (dbo.tblTransactions.TransactionGuid = dbo.tblTransactionLineItems.TransactionGuid AND 
		dbo.tblTransactions.TransactionGuid = dbo.tblTransactionNotes.TransactionGuid AND
		InventoryDate IN (SELECT * FROM  @InventoryDates)) AND
		AliasName IN ('Bulk Issue','Defuel','Issue') AND 
		(@Product = '<All>' OR (Product = @Product)) AND
		((CarrierID IN (SELECT * FROM  @AuthorizedCompanies))  OR 
		(ShipperID IN (SELECT * FROM @AuthorizedCompanies)) OR 
		(ShipToID IN (SELECT * FROM @AuthorizedCompanies)) OR 
		(SupplierID IN (SELECT * FROM @AuthorizedCompanies)) OR 
		(ManagerID IN (SELECT * FROM @AuthorizedCompanies)) OR 
		(OwnerID IN (SELECT * FROM @AuthorizedCompanies))  OR 
		(BillToID IN (SELECT * FROM @AuthorizedCompanies))) AND 
		Site in (Select ID from dbo.tblSites, [map].[tblSiteToSite] where ParentSiteGuid = @SiteGuid and [map].[tblSiteToSite].ChildSiteGuid = dbo.tblSites.SiteGuid) 
	ORDER BY ShipToID, Product, InventoryDate DESC, AliasName, RoutingID