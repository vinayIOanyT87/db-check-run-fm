


CREATE PROCEDURE [rpt].[usp_ConsumptionDailyReport]
@SiteGuid UNIQUEIDENTIFIER, @LoginSiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @BeginDate DATE, @EndDate DATE, @Product NVARCHAR (50), @Owner NVARCHAR (4000), @Manager NVARCHAR (4000), @Consumer NVARCHAR (4000), @AliasName NVARCHAR (200)
AS
SET NOCOUNT ON
	DECLARE @VolumeUnits int
	SET @VolumeUnits = (SELECT [dbo].[tblSites].VolumeUnitIndex FROM [dbo].[tblSites] WHERE [dbo].[tblSites].SiteGuid = @SiteGuid)

	DECLARE @VolumeDecimalPlaces int
	SET @VolumeDecimalPlaces = (SELECT [dbo].[tblSites].VolumeDecimalPlaces FROM [dbo].[tblSites] WHERE [dbo].[tblSites].SiteGuid = @SiteGuid)

	DECLARE @AuthorizedCompanies TABLE ([ID] [nvarchar] (30) NOT NULL);
	INSERT INTO @AuthorizedCompanies SELECT * FROM [dbo].[udf_AuthorizedCompanies](@LoginSiteGuid, @SiteGuid, @UserGuid)

	IF (@AliasName = '<All>')
	  BEGIN
		SELECT t.InventoryDate, 
				t.Site,
				'All' AS AliasName,
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
				dbo.udf_ConvertFromSIUnits(ABS(l.GrossQuantity), @VolumeUnits, @VolumeDecimalPlaces) AS GrossQuantity,
				dbo.udf_ConvertFromSIUnits(ABS(l.NetQuantity), @VolumeUnits, @VolumeDecimalPlaces) AS NetQuantity,
				1 AS FlightCnt,
				t.UpdatedDate AS ModifiedDate,
				t.DeleteFlag AS IsDeleted,
				n.Notes
		FROM   ((dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionNotes n ON t.TransactionGuid = n.TransactionGuid)
				 LEFT OUTER JOIN dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid)
		WHERE  t.InventoryDate BETWEEN @BeginDate AND @EndDate
				AND t.AliasName IN ('Bulk Issue', 'Defuel', 'Issue')
				AND (@Product = '<All>' OR (l.Product = @Product))
				AND t.ManagerID = @Manager
				AND EXISTS (SELECT ID 
							FROM @AuthorizedCompanies 
							WHERE ID IN(t.CarrierID, 
										t.ShipperID, 
										t.ShipToID, 
										t.SupplierID, 
										t.ManagerID, 
										t.OwnerID, 
										t.BillToID))
				AND (@Consumer  = '<All>' OR (t.ShipToID = @Consumer))
				AND (@Owner = '<All>' OR (t.OwnerID = @Owner))
				AND t.Site IN (SELECT ID FROM [dbo].[tblSites], [map].[tblSiteToSite]
							  WHERE ParentSiteGuid = @SiteGuid and [map].[tblSiteToSite].ChildSiteGuid = [dbo].[tblSites].SiteGuid
							 ) 
		ORDER BY t.ShipToID, l.Product, t.InventoryDate, t.AliasName, t.RoutingID
	  END
	ELSE
	  BEGIN
		SELECT t.InventoryDate, 
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
				dbo.udf_ConvertFromSIUnits(ABS(l.GrossQuantity), @VolumeUnits, @VolumeDecimalPlaces) AS GrossQuantity,
				dbo.udf_ConvertFromSIUnits(ABS(l.NetQuantity), @VolumeUnits, @VolumeDecimalPlaces) AS NetQuantity,
				1 AS FlightCnt,
				t.UpdatedDate AS ModifiedDate,
				t.DeleteFlag AS IsDeleted,
				n.Notes
		FROM   ((dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionNotes n ON t.TransactionGuid = n.TransactionGuid)
				 LEFT OUTER JOIN dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid)
		WHERE  t.InventoryDate BETWEEN @BeginDate AND @EndDate
				AND t.AliasName = @AliasName
				AND (@Product = '<All>' OR (l.Product = @Product))
				AND t.ManagerID = @Manager
				AND EXISTS (SELECT ID 
							FROM @AuthorizedCompanies 
							WHERE ID IN(t.CarrierID, 
										t.ShipperID, 
										t.ShipToID, 
										t.SupplierID, 
										t.ManagerID, 
										t.OwnerID, 
										t.BillToID))
				AND (@Consumer  = '<All>' OR (t.ShipToID = @Consumer))
				AND (@Owner = '<All>' OR (t.OwnerID = @Owner))
				AND t.Site IN (SELECT ID FROM [dbo].[tblSites], [map].[tblSiteToSite]
							  WHERE [map].[tblSiteToSite].[ParentSiteGuid] = @SiteGuid and [map].[tblSiteToSite].[ChildSiteGuid] = [dbo].[tblSites].[SiteGuid]
							 ) 
		ORDER BY t.ShipToID, l.Product, t.InventoryDate, t.AliasName, t.RoutingID
	  END