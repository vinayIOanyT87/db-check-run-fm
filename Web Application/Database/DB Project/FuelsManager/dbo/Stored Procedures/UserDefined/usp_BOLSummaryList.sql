
CREATE PROCEDURE [dbo].[usp_BOLSummaryList]
@AliasName NVARCHAR (200), @LookupTransTypeIndex NVARCHAR (2), @BeginDate DATETIMEOFFSET, @EndDate DATETIMEOFFSET, @ManagerID NVARCHAR (100), @OwnerID NVARCHAR (100), @ShipperID NVARCHAR (100), @BillToID NVARCHAR (100), @ShipToID NVARCHAR (100), @CarrierID NVARCHAR (100), @DocumentNumber NVARCHAR (30), @LookupTransactionStatusIndex NVARCHAR (2), @DriverIdentificationNumber NVARCHAR (50), @LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @Location NVARCHAR (30), @Product NVARCHAR (30), @DestinationSerialNumber1 NVARCHAR (10), @DestinationSerialNumber2 NVARCHAR (10), @DestinationSerialNumber3 NVARCHAR (10)
AS
BEGIN 
SET NOCOUNT ON
	DECLARE @UtcBeginDate DATETIMEOFFSET
	SET @UtcBeginDate=@BeginDate

	DECLARE @UtcEndDate DATETIMEOFFSET
	SET @UtcEndDate=@EndDate
	
	CREATE TABLE #SiteList(ID NVARCHAR(30),SiteGuid UNIQUEIDENTIFIER, ShowDeletedTrxFlag bit)
	
	INSERT INTO #SiteList(ID,SiteGuid,ShowDeletedTrxFlag)
	Select ID,
			dbo.tblSites.SiteGuid,
			ShowDeletedTrxFlag
	FROM dbo.tblSites 
	INNER JOIN [map].[tblSiteToSite] on dbo.tblSites.SiteGuid = [map].[tblSiteToSite].ChildSiteGuid
	INNER JOIN dbo.tblGeneralConfiguration on dbo.tblSites.SiteGuid = dbo.tblGeneralConfiguration.SiteGuid
	where ParentSiteGuid = @SiteGuid


	DECLARE @AuthorizedCompanies TABLE 
	(
		[ID] [nvarchar] (100) NOT NULL
	);
	INSERT INTO @AuthorizedCompanies 
	SELECT * FROM udf_AuthorizedCompanies(@LoginSiteGuid,@SiteGuid,@UserGuid)

	CREATE TABLE #TransID(
			SiteGuid UNIQUEIDENTIFIER,
			TransID NVARCHAR(64),
			AliasName NVARCHAR(32),
			LookupTransTypeIndex INT,
			CarrierID NVARCHAR(100),
			ShipperID NVARCHAR(100),
			ShipToID NVARCHAR(100),
			SupplierID NVARCHAR(100),
			ManagerID NVARCHAR(100),
			OwnerID NVARCHAR(100),
			BillToID NVARCHAR(100),
			RequestedDeliveryDate DATETIMEOFFSET(7),
			TransDateTime DATETIMEOFFSET(7),
			TimeIn DATETIMEOFFSET(7),
			[TimeOut] DATETIMEOFFSET(7),
			TimeEnd DATETIMEOFFSET(7),
			RouteOriginationDate DATETIMEOFFSET(7),
			STD DATETIMEOFFSET(7),
			ETD DATETIMEOFFSET(7),
			STA DATETIMEOFFSET(7),
			ETA DATETIMEOFFSET(7),
			SFT DATETIMEOFFSET(7),
			FST DATETIMEOFFSET(7),
			LookupTransactionStatusIndex INT,
			TransactionGuid UNIQUEIDENTIFIER,
			DocumentNumber NVARCHAR(100),
			DriverIdentificationNumber NVARCHAR(100),
			DestinationSerialNumber1 NVARCHAR(10),
			DestinationSerialNumber2 NVARCHAR(10),
			DestinationSerialNumber3 NVARCHAR(10),
		)

	INSERT INTO #TransID
	SELECT dbo.tblTransactions.SiteGuid,
			TransID,
			AliasName,
			LookupTransTypeIndex,
			CarrierID,
			ShipperID,
			ShipToID,
			SupplierID,
			ManagerID,
			OwnerID,
			BillToID,
			RequestedDeliveryDate,
			TransDateTime,
			TimeIn,
			TimeOut,
			TimeEnd,
			RouteOriginationDate,
			STD,
			ETD,
			STA,
			ETA,
			SFT,
			FST,
			LookupTransactionStatusIndex,
			TransactionGuid,
			DocumentNumber,
			DriverIdentificationNumber,
			DestinationSerialNumber1,
			DestinationSerialNumber2,
			DestinationSerialNumber3
	FROM dbo.tblTransactions
		INNER JOIN #SiteList on dbo.tblTransactions.SiteGuid = #SiteList.SiteGuid
	WHERE TransDateTime Between @UtcBeginDate AND @UtcEndDate
		AND (DeleteFlag = cast(0 as bit) OR #SiteList.ShowDeletedTrxFlag = 1)
		AND (@UserGuid IS NULL
		OR ((CarrierID IN (Select * FROM @AuthorizedCompanies))
		OR (ShipperID IN (Select * FROM @AuthorizedCompanies))
		OR (ShipToID IN (Select * FROM @AuthorizedCompanies))
		OR (SupplierID IN (Select * FROM @AuthorizedCompanies))
		OR (ManagerID IN (Select * FROM @AuthorizedCompanies))
		OR (OwnerID IN (Select * FROM @AuthorizedCompanies))
		OR (BillToID IN (Select * FROM @AuthorizedCompanies))))

	IF @AliasName <>''
	BEGIN
		DELETE FROM #TransID
		WHERE AliasName <>@AliasName
	END 

	IF @LookupTransTypeIndex<>''
	BEGIN
		DELETE FROM #TransID
		WHERE LookupTransTypeIndex <> CAST(@LookupTransTypeIndex AS int)
	END

	IF @ManagerID <>''
	BEGIN
		DELETE FROM #TransID
		WHERE ManagerID <>@ManagerID
	END

	IF @OwnerID <>''
	BEGIN
		DELETE FROM #TransID
		WHERE OwnerID <>@OwnerID
	END

	IF @ShipperID <>''
	BEGIN
		DELETE FROM #TransID
		WHERE ShipperID<>@ShipperID
	END

	IF @BillToID <>''
	BEGIN
		DELETE FROM #TransID
		WHERE BillToID<>@BillToID
	END

	IF @ShipToID <>''
	BEGIN
		DELETE FROM #TransID
		WHERE ShipToID<>@ShipToID
	END

	IF @CarrierID <>''
	BEGIN
		DELETE FROM #TransID
		WHERE CarrierID<>@CarrierID
	END

	IF @DocumentNumber <>''
	BEGIN
		DELETE FROM #TransID
		WHERE DocumentNumber <> @DocumentNumber
	END

	IF @LookupTransactionStatusIndex <>''
	BEGIN
		DELETE FROM #TransID
		WHERE LookupTransactionStatusIndex <>CAST(@LookupTransactionStatusIndex AS int)
	END

	IF @DriverIdentificationNumber <>''
	BEGIN
		DELETE FROM #TransID
		WHERE DriverIdentificationNumber <> @DriverIdentificationNumber
	END

	IF @Product <>''
	BEGIN
		DELETE FROM #TransID
		WHERE TransactionGuid NOT IN (SELECT T.TransactionGuid FROM dbo.tblTransactionLineItems L
												RIGHT JOIN #TransID T ON L.TransactionGuid = T.TransactionGuid
										WHERE Product =@Product )
	END

	IF @Location <>''
	BEGIN
		DELETE FROM #TransID
		WHERE TransactionGuid NOT IN (SELECT T.TransactionGuid FROM dbo.tblTransactionLineItems L
												RIGHT JOIN #TransID T ON L.TransactionGuid = T.TransactionGuid
										WHERE LoadingLocationID = @Location )
	END

		IF @DestinationSerialNumber1 <>''
	BEGIN
		DELETE FROM #TransID
		WHERE DestinationSerialNumber1 IS NULL OR  DestinationSerialNumber1 <> @DestinationSerialNumber1
	END

		IF @DestinationSerialNumber2 <>''
	BEGIN
		DELETE FROM #TransID
		WHERE DestinationSerialNumber2 IS NULL OR DestinationSerialNumber2 <> @DestinationSerialNumber2
	END

		IF @DestinationSerialNumber3 <>''
	BEGIN
		DELETE FROM #TransID
		WHERE DestinationSerialNumber3 IS NULL OR DestinationSerialNumber3 <> @DestinationSerialNumber3
	END

	--Convert Time
	UPDATE #TransID
	SET RequestedDeliveryDate=RequestedDeliveryDate ,
		TransDateTime=TransDateTime,
		TimeIn=TimeIN,
		TimeOut=TimeOut,
		TimeEnd=TimeEnd,
		RouteOriginationDate=RouteOriginationDate,
		STD=STD,
		ETD=ETD,
		STA=STA,
		ETA=ETA,
		SFT=SFT,
		FST=FST


		SELECT
		A.TransID,
		A.AliasName,
		A.TransactionAliasGuid,
		A.LookupTransTypeIndex,
		A.SubType,
		A.Site,
		A.SiteGuid,
		A.TransReferenceID,
		A.InventoryDate,
		A.ShipToID,
		A.ShipToCode,
		A.ShipToCompanyGuid,
		A.SupplierID,
		A.SupplierCode,
		A.SupplierCompanyGuid,
		A.CreatedDate,
		A.CreatedBy,
		B.RequestedDeliveryDate,
		A.UpdatedDate,
		A.UpdatedBy,
		B.TransDateTime,
		A.TransVersion,
		A.SCACCode,
		A.CardNumber,
		A.ShipmentNumber,
		A.ShipperID,
		A.ShipperCode,
		A.ShipperCompanyGuid,
		A.OwnerID,
		A.OwnerCode,
		A.OwnerCompanyGuid,
		A.ManagerID,
		A.ManagerCode,
		A.ManagerCompanyGuid,
		A.CarrierID,
		A.CarrierCode,
		A.CarrierCompanyGuid,
		A.ConjoinTransID,
		A.ReversedTransID,
		A.LinkedDocumentNumber,
		A.ReversalType,
		A.PONumber,
		B.TimeIn,
		B.TimeOut,
		B.TimeEnd,
		A.RoutingID,
		A.TicketSource,
		A.LoadID,
		A.LookupTransactionStatusIndex,
		A.BillToID,
		A.BillToCode,
		A.BillToCompanyGuid,
		A.DriverIdentificationNumber,
		A.CreditAmount,
		A.CardExpiration,
		A.CardName,
		A.CardType,
		A.CashAmount,
		B.RouteOriginationDate,
		A.InternationalRouteIndicator,
		A.PreviousRoutingID,
		A.FinalStationIATAGuid,
		A.FinalStationIATAID,
		A.PreviousStationIATAGuid,
		A.PreviousStationIATAID,
		A.NextStationIATAGuid,
		A.NextStationIATAID,
		A.OriginStationIATAGuid,
		A.OriginStationIATAID,
		A.ShippingDocumentNumber,
		A.DocumentNumber,
		B.STD,
		B.ETD,
		B.STA,
		B.ETA,
		B.SFT,
		B.FST,
		A.EstimatedFuelingDuration,
		A.DeleteFlag,
		A.TicketMode,
		A.DestinationRegistrationID1,
		A.DestinationSerialNumber1,
		A.DestinationEquipmentType1,
		A.DestinationEquipmentModel1,
		A.DestinationCompanyEquipmentID1,
		A.Destination1EquipmentGuid,
		A.DestinationRegistrationID2,
		A.DestinationSerialNumber2,
		A.DestinationEquipmentType2,
		A.DestinationEquipmentModel2,
		A.DestinationCompanyEquipmentID2,
		A.Destination2EquipmentGuid,
		A.DestinationRegistrationID3,
		A.DestinationSerialNumber3,
		A.DestinationEquipmentType3,
		A.DestinationEquipmentModel3,
		A.DestinationCompanyEquipmentID3,
		A.Destination3EquipmentGuid,
		A.SourceRegistrationID1,
		A.SourceSerialNumber1,
		A.SourceEquipmentType1,
		A.SourceEquipmentModel1,
		A.SourceCompanyEquipmentID1,
		A.Source1EquipmentGuid,
		A.SourceRegistrationID2,
		A.SourceSerialNumber2,
		A.SourceEquipmentType2,
		A.SourceEquipmentModel2,
		A.SourceCompanyEquipmentID2,
		A.Source2EquipmentGuid,
		A.SourceRegistrationID3,
		A.SourceSerialNumber3,
		A.SourceEquipmentType3,
		A.SourceEquipmentModel3,
		A.SourceCompanyEquipmentID3,
		A.Source3EquipmentGuid,
		A.OperatorID,
		A.OperatorPersonnelGuid,
		A.EffectiveDate,
		A.ExpirationDate,
		A.ScheduledDate,
		A.AutoComplete,
		Manager.Name AS ManagerName,Manager.Address1 AS ManagerAddress,Manager.City AS ManagerCity,Manager.State AS ManagerState,
		Owner.Name AS OwnerName,Owner.Address1 AS OwnerAddress,Owner.City AS OwnerCity,Owner.State AS OwnerState,
		Shipper.Name AS ShipperName,Shipper.Address1 AS ShipperAddress,Shipper.City AS ShipperCity,Shipper.State AS ShipperState,
		BillTo.Name AS BillToName,BillTo.Address1 AS BillToAddress,BillTo.City AS BillToCity,BillTo.State AS BillToState,
		ShipTo.Name AS ShipToName,ShipTo.Address1 AS ShipToAddress,ShipTo.City AS ShipToCity,ShipTo.State AS ShipToState,
		Supplier.Name AS SupplierName,Supplier.Address1 AS SupplierAddress,Supplier.City AS SupplierCity,Supplier.State AS SupplierState,
		Carrier.Name AS CarrierName,Carrier.Address1 AS CarrierAddress,Carrier.City AS CarrierCity,Carrier.State AS CarrierState
	FROM ((((((((dbo.tblTransactions A RIGHT JOIN #TransID B ON B.TransactionGuid = A.TransactionGuid)
		LEFT JOIN dbo.tblCompanies Manager ON A.ManagerCompanyGuid = Manager.CompanyGuid)
		LEFT JOIN dbo.tblCompanies Owner ON A.OwnerCompanyGuid = Owner.CompanyGuid)
		LEFT JOIN dbo.tblCompanies Shipper ON A.ShipperCompanyGuid = Shipper.CompanyGuid)
		LEFT JOIN dbo.tblCompanies BillTo ON A.BillToCompanyGuid = BillTo.CompanyGuid)
		LEFT JOIN dbo.tblCompanies ShipTo ON A.ShipToCompanyGuid = ShipTo.CompanyGuid)
		LEFT JOIN dbo.tblCompanies Supplier ON A.SupplierCompanyGuid = Supplier.CompanyGuid)
		LEFT JOIN dbo.tblCompanies Carrier ON A.CarrierCompanyGuid = Carrier.CompanyGuid)
	ORDER BY B.TransDateTime

	DROP TABLE #SiteList
	DROP TABLE #TransID
END

SET ANSI_NULLS ON
GO