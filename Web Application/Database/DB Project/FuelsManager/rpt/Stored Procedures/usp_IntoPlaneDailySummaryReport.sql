
CREATE PROCEDURE [rpt].[usp_IntoPlaneDailySummaryReport]
@BeginDate DATE, @Carrier NVARCHAR (200), @ShipTo NVARCHAR (200), @Owner NVARCHAR (200), @Product NVARCHAR (200), @SiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @LoginSiteGuid UNIQUEIDENTIFIER
AS
SET NOCOUNT ON
	DECLARE @SDate DATETIMEOFFSET(7);
	DECLARE @Year nvarchar(5);
	DECLARE @Month nvarchar(3);
	DECLARE @Day nvarchar(3)
	DECLARE @Hour nvarchar(3);
	DECLARE @Minute nvarchar(3);
	DECLARE @Second nvarchar(3);
	DECLARE @MSecond nvarchar(4);
	DECLARE @Ckey nvarchar(50);
	DECLARE @volumefactor float;
	SELECT @volumefactor = 0.003785412;
	SELECT @SDate = DATEADD(day, 0, SYSDATETIMEOFFSET());
	SELECT @Year = DATEPART(Year, @SDate);
	SELECT @Month = DATEPART(Month, @SDate);
	SELECT @Day = DATEPART(Day, @SDate);
	SELECT @Hour = DATEPART(Hour, @SDate);
	SELECT @Minute = DATEPART(Minute, @SDate);
	SELECT @Second = DATEPART(Second, @SDate);
	SELECT @MSecond = DATEPART(Millisecond, @SDate);
	SELECT @CKey = @Year + @Month + @Day + @Hour + @Minute + @Second + @MSecond;

	DECLARE @VolumeUnits int
	SET @VolumeUnits = (SELECT dbo.tblSites.VolumeUnitIndex FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @VolumeDecimalPlaces int
	SET @VolumeDecimalPlaces = (SELECT dbo.tblSites.VolumeDecimalPlaces FROM dbo.tblSites WHERE dbo.tblSites.SiteGuid = @SiteGuid)

	DECLARE @AuthorizedCompanies TABLE ([ID] [nvarchar] (100) NOT NULL);
	INSERT INTO @AuthorizedCompanies SELECT * FROM udf_AuthorizedCompanies(@LoginSiteGuid, @SiteGuid, @UserGuid)

	IF( @ShipTo = '<All>')
		BEGIN
			SELECT t.InventoryDate, 
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
						l.MeterStart,
						l.MeterStop, 
						t.DestinationEquipmentModel1,
						t.SourceEquipmentModel1,
					CASE t.AliasName
						WHEN 'Defuel' THEN (-1 * ([dbo].[udf_ConvertFromSIUnits](ABS(l.GrossQuantity), @VolumeUnits, @VolumeDecimalPlaces))) 
						ELSE [dbo].[udf_ConvertFromSIUnits](ABS(l.GrossQuantity), @VolumeUnits, @VolumeDecimalPlaces) 
						END AS GrossQuantity,
					CASE t.AliasName
						WHEN 'Defuel' then (-1 * ([dbo].[udf_ConvertFromSIUnits](ABS(l.NetQuantity), @VolumeUnits, @VolumeDecimalPlaces))) 
						ELSE [dbo].[udf_ConvertFromSIUnits](ABS(l.NetQuantity), @VolumeUnits, @VolumeDecimalPlaces) 
						END AS NetQuantity,
					CASE IsNull(t.InternationalRouteIndicator, 0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END AS InternationalRouteIndicator, 
					1 AS FlightCnt 
			FROM   dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
			WHERE  t.InventoryDate = @BeginDate 
						AND ((t.DeleteFlag IS NULL) OR (t.DeleteFlag = 0))
						AND ((l.DeleteFlag IS NULL) OR (l.DeleteFlag = 0))
						AND t.AliasName IN ('Bulk Issue', 'Defuel', 'Issue')
						AND EXISTS (SELECT ID
										FROM @AuthorizedCompanies 
										WHERE ID IN(t.CarrierID, 
												t.ShipperID, 
												t.ShipToID, 
												t.SupplierID, 
												t.ManagerID, 
												t.OwnerID, 
												t.BillToID))
					AND t.OwnerID = @Owner
						AND l.Product = @Product
						AND (t.Site in (SELECT ID FROM [dbo].[tblSites], [map].[tblSiteToSite]
											 WHERE [map].[tblSiteToSite].[ParentSiteGuid] = @SiteGuid and [map].[tblSiteToSite].[ChildSiteGuid] = [dbo].[tblSites].[SiteGuid]
											)
							  OR t.Site = (SELECT ID FROM [dbo].[tblSites] WHERE SiteGuid = @SiteGuid)
							 )
						AND t.CarrierID = @Carrier
			ORDER BY CASE IsNull(t.InternationalRouteIndicator,0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END, t.ShipToID, t.InventoryDate, t.AliasName
		END
	ELSE
		BEGIN
			SELECT t.InventoryDate,
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
						l.MeterStart,
						l.MeterStop,
						t.DestinationEquipmentModel1,
						t.SourceEquipmentModel1,
					CASE t.AliasName
						WHEN 'Defuel' THEN (-1 * ([dbo].[udf_ConvertFromSIUnits](ABS(l.GrossQuantity), @VolumeUnits, @VolumeDecimalPlaces))) 
						ELSE [dbo].[udf_ConvertFromSIUnits](ABS(l.GrossQuantity), @VolumeUnits, @VolumeDecimalPlaces) 
						END AS GrossQuantity,
					CASE t.AliasName
						WHEN 'Defuel' then (-1 * ([dbo].[udf_ConvertFromSIUnits](ABS(l.NetQuantity), @VolumeUnits, @VolumeDecimalPlaces))) 
						ELSE [dbo].[udf_ConvertFromSIUnits](ABS(l.NetQuantity), @VolumeUnits, @VolumeDecimalPlaces) 
						END AS NetQuantity,
					CASE IsNull(t.InternationalRouteIndicator, 0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END AS InternationalRouteIndicator, 
					1 AS FlightCnt 
			FROM   dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
			WHERE  t.InventoryDate = @BeginDate
						AND ((t.DeleteFlag IS NULL) OR (t.DeleteFlag = 0))
						AND ((l.DeleteFlag IS NULL) OR (l.DeleteFlag = 0))
						AND t.AliasName IN ('Bulk Issue', 'Defuel', 'Issue')
						AND EXISTS (SELECT ID
										FROM @AuthorizedCompanies 
										WHERE ID IN(t.CarrierID, 
												t.ShipperID, 
												t.ShipToID, 
												t.SupplierID, 
												t.ManagerID, 
												t.OwnerID, 
												t.BillToID))
					AND t.ShipToID = @ShipTo
						AND t.OwnerID = @Owner
						AND l.Product = @Product
						AND (t.Site IN (SELECT ID FROM [dbo].[tblSites], [map].[tblSiteToSite]
											 WHERE ParentSiteGuid = @SiteGuid and [map].[tblSiteToSite].[ChildSiteGuid] = [dbo].[tblSites].[SiteGuid]
											)
							  OR t.Site = (SELECT ID FROM dbo.tblSites WHERE SiteGuid = @SiteGuid)
							 )
						AND t.CarrierID = @Carrier
			ORDER BY CASE IsNull(t.InternationalRouteIndicator,0) WHEN 0 THEN 'DOM' ELSE 'FTZ' END, t.ShipToID, t.InventoryDate, t.AliasName
		END
