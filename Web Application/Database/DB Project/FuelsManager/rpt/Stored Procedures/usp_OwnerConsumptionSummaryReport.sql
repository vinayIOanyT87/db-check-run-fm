
CREATE PROCEDURE [rpt].[usp_OwnerConsumptionSummaryReport]
@SiteGuid UNIQUEIDENTIFIER, @LoginSiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @BeginDate DATE, @EndDate DATE, @Product NVARCHAR (50), @Owner NVARCHAR (4000), @Manager NVARCHAR (4000), @Consumer NVARCHAR (4000)
AS
SET NOCOUNT ON
	DECLARE @VolumeUnits int
	SET @VolumeUnits = (SELECT dbo.tblSites.VolumeUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @VolumeDecimalPlaces int
	SET @VolumeDecimalPlaces = (SELECT dbo.tblSites.VolumeDecimalPlaces FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @AuthorizedCompanies TABLE (
		[ID] [nvarchar] (100) NOT NULL
	);
	INSERT INTO @AuthorizedCompanies SELECT * FROM udf_AuthorizedCompanies(@LoginSiteGuid,@SiteGuid,@UserGuid)

	SELECT InventoryDate, 
		 t.Site,
		 t.AliasName,
		 t.ManagerID,
		 t.OwnerID, 
		 t.CarrierID, 
		 t.ShipToID, 
		 l.Product,  
		 l.DocumentNumber AS ShipmentNumber, 
		 t.DestinationRegistrationID1, 
		 t.RoutingID, 
		 t.SourceRegistrationID1, 
		 t.DestinationEquipmentModel1,
		 t.SourceEquipmentModel1,
		 l.MeterStart, 
		 l.MeterStop, 
		 CASE t.AliasName
			WHEN 'Defuel' then (-1 * (dbo.udf_ConvertFromSIUnits(abs(l.GrossQuantity), @VolumeUnits, @VolumeDecimalPlaces)))
			ELSE [dbo].[udf_ConvertFromSIUnits](abs(l.GrossQuantity), @VolumeUnits, @VolumeDecimalPlaces) 
			END AS GrossQuantity,
		 CASE t.AliasName
			WHEN 'Defuel' then (-1 * (dbo.udf_ConvertFromSIUnits(abs(l.NetQuantity), @VolumeUnits, @VolumeDecimalPlaces)))
			ELSE [dbo].[udf_ConvertFromSIUnits](abs(l.NetQuantity), @VolumeUnits, @VolumeDecimalPlaces) 
			END AS NetQuantity,
		 1 AS FlightCnt,
		 n.Notes
	FROM   ((dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionNotes n ON t.TransactionGuid = n.TransactionGuid) 
				LEFT OUTER JOIN dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid)
	WHERE  (((t.DeleteFlag IS NULL) OR (t.DeleteFlag = 0)) AND
			 ((l.DeleteFlag IS NULL) OR (l.DeleteFlag = 0)) AND
				t.InventoryDate BETWEEN @BeginDate AND @EndDate ) AND
			 t.AliasName IN ('Bulk Issue', 'Defuel', 'Issue') AND
		  (@Product = '<All>' OR (l.Product = @Product)) AND
		  t.ManagerID = @Manager AND
			EXISTS (SELECT ID
						FROM @AuthorizedCompanies 
						WHERE ID IN(t.CarrierID, 
								t.ShipperID, 
								t.ShipToID, 
								t.SupplierID, 
								t.ManagerID, 
								t.OwnerID, 
								t.BillToID)) AND 
		  (@Consumer  = '<All>' OR (ShipToID = @Consumer)) AND 
		  (@Owner = '<All>' OR (OwnerID = @Owner)) AND
		  Site in (Select ID from dbo.tblSites, [map].[tblSiteToSite] where ParentSiteGuid = @SiteGuid and [map].[tblSiteToSite].ChildSiteGuid = dbo.tblSites.SiteGuid) 
	ORDER BY OwnerID, ShipToID, Product, InventoryDate, AliasName, RoutingID
