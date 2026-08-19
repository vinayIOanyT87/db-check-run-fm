CREATE PROCEDURE [dbo].[usp_TransactionAndLineItemList]
@AliasName NVARCHAR (200), 
@NominationKey nvarchar (64),
@BeginDate DATE, 
@EndDate DATE, 
@ManagerID NVARCHAR (100), 
@OwnerID NVARCHAR (100), 
@Product NVARCHAR (30), 
@LoginSiteGuid UNIQUEIDENTIFIER, 
@SiteGuid UNIQUEIDENTIFIER, 
@UserGuid UNIQUEIDENTIFIER, 
@ShowDeletedTrx BIT


--WITH EXECUTE AS CALLER
/** Modified to recompile... this procedure runs faster with recompile ***/
WITH RECOMPILE, EXECUTE AS CALLER 
AS
BEGIN

	--------------------------------------------------------------------------- 
	-- Get the ProductGuid 
	--------------------------------------------------------------------------- 
	DECLARE @ProductGuid UNIQUEIDENTIFIER
	SET @ProductGuid = (SELECT b.ProductGuid 
						FROM erv.udf_GetProductRecordVersions (@SiteGuid) a INNER JOIN tblProducts b on a.ProductGuid = b.ProductGuid 
						WHERE b.ProductID = @Product)

	DECLARE @ErvCompanies TABLE
	(
		CompanyGuid uniqueidentifier,
		MasterRecordGuid uniqueidentifier
	)

	INSERT INTO @ErvCompanies (CompanyGuid, MasterRecordGuid)
	SELECT CompanyGuid, MasterRecordGuid
	FROM erv.udf_GetCompanyRecordVersions (@SiteGuid)

	DECLARE @VolumePackageSize float
	DECLARE @MassPackageSize float

	DECLARE @TemperatureUnits int
	DECLARE @TemperatureDecimalPlaces int
	DECLARE @TemperatureUnitsProducts int
	DECLARE @TemperatureDecimalPlacesProducts int
	DECLARE @TemperatureUnitsSites int
	DECLARE @TemperatureDecimalPlacesSites int

	DECLARE @AdditiveVolumeUnits int
	DECLARE @AdditiveVolumeDecimalPlaces int
	DECLARE @AdditiveVolumeUnitsProducts int
	DECLARE @AdditiveVolumeDecimalPlacesProducts int
	DECLARE @AdditiveVolumeUnitsSites int
	DECLARE @AdditiveVolumeDecimalPlacesSites int

	DECLARE @VolumeUnits int
	DECLARE @VolumeDecimalPlaces int
	DECLARE @VolumeUnitsProducts int
	DECLARE @VolumeDecimalPlacesProducts int
	DECLARE @VolumeUnitsSites int
	DECLARE @VolumeDecimalPlacesSites int

	DECLARE @PressureUnits int
	DECLARE @PressureDecimalPlaces int
	DECLARE @PressureUnitsProducts int
	DECLARE @PressureDecimalPlacesProducts int
	DECLARE @PressureUnitsSites int
	DECLARE @PressureDecimalPlaceSites int

	DECLARE @DensityUnits int
	DECLARE @DensityDecimalPlaces int
	DECLARE @DensityUnitsProducts int
	DECLARE @DensityDecimalPlacesProducts int
	DECLARE @DensityUnitsSites int
	DECLARE @DensityDecimalPlacesSites int

	DECLARE @LevelUnits int
	DECLARE @LevelDecimalPlaces int
	DECLARE @LevelUnitsProducts int
	DECLARE @LevelDecimalPlacesProducts int
	DECLARE @LevelUnitsSites int
	DECLARE @LevelDecimalPlacesSites int

	DECLARE @MassUnits int
	DECLARE @MassDecimalPlaces int
	DECLARE @MassUnitsProducts int
	DECLARE @MassDecimalPlacesProducts int
	DECLARE @MassUnitsSites int
	DECLARE @MassDecimalPlacesSites int

	DECLARE @FlowUnits int
	DECLARE @FlowDecimalPlaces int
	DECLARE @FlowUnitsProducts int
	DECLARE @FlowDecimalPlacesProducts int
	DECLARE @FlowUnitsSites int
	DECLARE @FlowDecimalPlacesSites int


	SELECT @VolumePackageSize					= ISNULL(tblProducts.VolumePackageSize, 0),
		   @MassPackageSize						= ISNULL(tblProducts.MassPackageSize, 0),
		   @TemperatureUnitsProducts			= ISNULL(tblProducts.TemperatureUnitIndex, 0),
		   @TemperatureDecimalPlacesProducts	= ISNULL(tblProducts.TemperatureDecimalPlaces, 2),
		   @AdditiveVolumeUnitsProducts			= ISNULL(tblProducts.VolumeUnitIndex, 0),
		   @AdditiveVolumeDecimalPlacesProducts = ISNULL(tblProducts.VolumeDecimalPlaces, 2),
		   @VolumeUnitsProducts					= ISNULL(tblProducts.VolumeUnitIndex, 0),
		   @VolumeDecimalPlacesProducts			= ISNULL(tblProducts.VolumeDecimalPlaces, 2),
		   @PressureUnitsProducts				= ISNULL(tblProducts.PressureUnitIndex, 0),
		   @PressureDecimalPlacesProducts		= ISNULL(tblProducts.PressureDecimalPlaces, 2),
		   @DensityUnitsProducts				= ISNULL(tblProducts.DensityUnitIndex, 0),
		   @DensityDecimalPlacesProducts		= ISNULL(tblProducts.DensityDecimalPlaces, 2),
		   @LevelUnitsProducts					= ISNULL(tblProducts.LevelUnitIndex, 0),
		   @LevelDecimalPlacesProducts			= ISNULL(tblProducts.LevelDecimalPlaces, 2),
		   @MassUnitsProducts					= ISNULL(tblProducts.MassUnitIndex, 0),
		   @MassDecimalPlacesProducts			= ISNULL(tblProducts.MassDecimalPlaces, 2),
		   @FlowUnitsProducts					= ISNULL(tblProducts.FlowUnitIndex, 0),
		   @FlowDecimalPlacesProducts			= ISNULL(tblProducts.FlowDecimalPlaces, 2)
	FROM tblProducts WHERE ProductGuid = @ProductGuid

	SELECT @TemperatureUnitsSites				= tblSites.TemperatureUnitIndex,
		   @TemperatureDecimalPlacesSites		= tblSites.TemperatureDecimalPlaces,
		   @AdditiveVolumeUnitsSites			= tblSites.AdditiveVolumeUnitIndex,
		   @AdditiveVolumeDecimalPlacesSites	= tblSites.AdditiveVolumeDecimalPlaces,
		   @VolumeUnitsSites					= tblSites.VolumeUnitIndex,
		   @VolumeDecimalPlacesSites			= tblSites.VolumeDecimalPlaces,
		   @PressureUnitsSites					= tblSites.PressureUnitIndex,
		   @PressureDecimalPlaceSites			= tblSites.PressureDecimalPlaces,
		   @DensityUnitsSites					= tblSites.DensityUnitIndex,
		   @DensityDecimalPlacesSites			= tblSites.DensityDecimalPlaces,
		   @LevelUnitsSites						= tblSites.LevelUnitIndex,
		   @LevelDecimalPlacesSites				= tblSites.LevelDecimalPlaces,
		   @MassUnitsSites						= tblSites.MassUnitIndex,
		   @MassDecimalPlacesSites				= tblSites.MassDecimalPlaces,
		   @FlowUnitsSites						= tblSites.FlowUnitIndex,
		   @FlowDecimalPlacesSites				= tblSites.FlowDecimalPlaces
	FROM tblSites WHERE SiteGuid = @SiteGuid


	IF (@TemperatureUnitsProducts = 0 OR @TemperatureUnitsProducts IS NULL)
	BEGIN
		SET @TemperatureUnits = @TemperatureUnitsSites
		SET @TemperatureDecimalPlaces = @TemperatureDecimalPlacesSites
	END
	ELSE
	BEGIN
		SET @TemperatureUnits = @TemperatureUnitsProducts
		SET @TemperatureDecimalPlaces = @TemperatureDecimalPlacesProducts
	END

	IF (@AdditiveVolumeUnitsProducts = 0 OR @AdditiveVolumeUnitsProducts IS NULL)
	BEGIN
		SET @AdditiveVolumeUnits = @AdditiveVolumeUnitsSites
		SET @AdditiveVolumeDecimalPlaces = @AdditiveVolumeDecimalPlacesSites
	END
	ELSE
	BEGIN
		SET @AdditiveVolumeDecimalPlaces = @AdditiveVolumeDecimalPlacesProducts
		SET @AdditiveVolumeUnits = @AdditiveVolumeUnitsProducts
	END

	IF (@VolumeUnitsProducts = 0 OR @VolumeUnitsProducts IS NULL)
	BEGIN
		SET @VolumeUnits = @VolumeUnitsSites
		SET @VolumeDecimalPlaces = @VolumeDecimalPlacesSites
	END
	ELSE
	BEGIN
		SET @VolumeUnits = @VolumeUnitsProducts
		SET @VolumeDecimalPlaces = @VolumeDecimalPlacesProducts	
	END
	
	IF (@PressureUnitsProducts = 0 OR @PressureUnitsProducts IS NULL)
	BEGIN
		SET @PressureUnits = @PressureUnitsSites
		SET @PressureDecimalPlaces = @PressureDecimalPlaceSites
	END
	ELSE
	BEGIN
		SET @PressureUnits = @PressureUnitsProducts
		SET @PressureDecimalPlaces = @PressureDecimalPlacesProducts
	END


	IF (@DensityUnitsProducts = 0 OR @DensityUnitsProducts IS NULL)
	BEGIN
		SET @DensityUnits = @DensityUnitsSites
		SET @DensityDecimalPlaces = @DensityDecimalPlacesSites
	END
	ELSE
	BEGIN
		SET @DensityUnits = @DensityUnitsProducts
		SET @DensityDecimalPlaces = @DensityDecimalPlacesProducts
	END


	IF (@LevelUnitsProducts = 0 OR @LevelUnitsProducts IS NULL)
	BEGIN
		SET @LevelUnits = @LevelUnitsSites
		SET @LevelDecimalPlaces = @LevelDecimalPlacesSites
	END
	ELSE
	BEGIN
		SET @LevelUnits = @LevelUnitsProducts
		SET @LevelDecimalPlaces = @LevelDecimalPlacesProducts
	END


	IF (@MassUnitsProducts = 0 OR @MassUnitsProducts IS NULL)
	BEGIN
		SET @MassUnits = @MassUnitsSites
		SET @MassDecimalPlaces = @MassDecimalPlacesSites
	END
	ELSE
	BEGIN
		SET @MassUnits = @MassUnitsProducts
		SET @MassDecimalPlaces = @MassDecimalPlacesProducts
	END

	IF (@FlowUnitsProducts = 0 OR @FlowUnitsProducts IS NULL)
	BEGIN
		SET @FlowUnits = @FlowUnitsSites
		SET @FlowDecimalPlaces = @FlowDecimalPlacesSites
	END
	ELSE
	BEGIN
		SET @FlowUnits = @FlowUnitsProducts
		SET @FlowDecimalPlaces = @FlowDecimalPlacesProducts
	END

	DECLARE @TransactionType smallint
	SET @TransactionType = (SELECT LookupTransTypeIndex FROM tblTransactionAliases
							WHERE AliasName = @AliasName
								  AND TransactionAliasGuid IN (SELECT TransactionAliasGuid FROM map.tblEntityTransactionAliasToSite
																WHERE SiteGuid = @SiteGuid))

	DECLARE @AuthorizedCompanies TABLE (
		[ID] [nvarchar] (100) NOT NULL
	)

	INSERT INTO @AuthorizedCompanies SELECT * FROM dbo.udf_AuthorizedCompanies(@LoginSiteGuid,@SiteGuid,@UserGuid)

	DECLARE @Transactions TABLE (
		[TransactionGuid]  UNIQUEIDENTIFIER NOT NULL
	)

	--------------------------------------------------------------------------- 
	-- Get the products assigned to Tracking Product 
	--------------------------------------------------------------------------- 
	DECLARE @ProductAliasTable TABLE (AliasName nvarchar (30))
	INSERT INTO @ProductAliasTable
		SELECT b.ProductID FROM erv.udf_GetProductRecordVersions (@SiteGuid) a inner join tblProducts b on a.ProductGuid = b.ProductGuid
		WHERE b.TrackingProductGuid = @ProductGuid


	-- For a transaction type 14 (physical inventory) the owner should be empty or null. 
	IF (@TransactionType = 14)
		BEGIN
			SET @OwnerID = ''
		END

	IF (@NominationKey <> '')
	BEGIN
		-----------------------------------------------------------------------------
		-- Get associated Movement transactions when the Nomination Key is specified
		-----------------------------------------------------------------------------
		DECLARE @MovementTransId NVARCHAR(64)
		DECLARE @ManagerGuid UNIQUEIDENTIFIER
		DECLARE @OwnerGuid UNIQUEIDENTIFIER
	
		SELECT @MovementTransId = TransID, @SiteGuid = SiteGuid, @ProductGuid = ProductGuid, @Product = Product, @ManagerGuid = t.ManagerCompanyGuid, @OwnerGuid = t.OwnerCompanyGuid 
		FROM tblTransactions t JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
		WHERE AliasName='Movement'  AND ISNULL(ReversalType,'U') = 'U' AND TransReferenceID = @NominationKey
	
		INSERT INTO @Transactions SELECT TransactionGuid
			FROM tblTransactions T
			WHERE (@ShowDeletedTrx = 1 OR (T.DeleteFlag IS NULL OR T.DeleteFlag = 0))
			AND (T.LinkedDocumentNumber = @MovementTransID)
			AND (T.ManagerCompanyGuid = @ManagerGuid)
			AND (T.OwnerCompanyGuid = @OwnerGuid)
			AND ((T.SiteGuid IN (Select SiteGuid from tblSites INNER JOIN map.tblSiteToSite on map.tblSiteToSite.ChildSiteGuid=tblSites.SiteGuid where ParentSiteGuid = @SiteGuid) ) 
									OR (Site IN (Select SiteGuid from tblSites where SiteGuid = @SiteGuid)))
			AND (@UserGuid IS NULL
			OR ((T.CarrierID IN (Select * FROM @AuthorizedCompanies))
			OR (T.ShipperID  IN (Select * FROM @AuthorizedCompanies))
			OR (T.ShipToID   IN (Select * FROM @AuthorizedCompanies))
			OR (T.SupplierID IN (Select * FROM @AuthorizedCompanies))
			OR (T.ManagerID  IN (Select * FROM @AuthorizedCompanies))
			OR (T.OwnerID    IN (Select * FROM @AuthorizedCompanies))
			OR (T.BillToID   IN (Select * FROM @AuthorizedCompanies))))
	END
	ELSE
	BEGIN
		Select SiteGuid 
		INTO #sitelist
		from tblSites 
		INNER JOIN map.tblSiteToSite 
		on map.tblSiteToSite.ChildSiteGuid=tblSites.SiteGuid 
		where ParentSiteGuid = @SiteGuid
		UNION 
		Select SiteGuid 
		from tblSites 
		where SiteGuid = @SiteGuid

		INSERT INTO @Transactions SELECT TransactionGuid
						FROM tblTransactions
						JOIN #sitelist s
						ON tblTransactions.siteguid = s.siteguid
						WHERE (@ShowDeletedTrx = 1 OR (tblTransactions.DeleteFlag IS NULL OR tblTransactions.DeleteFlag = 0))
						AND (tblTransactions.SubmittedToAccounting = 1)
						AND (tblTransactions.AliasName = @AliasName)
						AND (@ManagerID = '' OR (tblTransactions.ManagerID = @ManagerID))
						AND (@OwnerID = '' OR (tblTransactions.OwnerID = @OwnerID))
						AND (tblTransactions.InventoryDate >= @BeginDate)
						AND (tblTransactions.InventoryDate < @EndDate)
						AND (@UserGuid IS NULL
						OR ((tblTransactions.CarrierID IN (Select * FROM @AuthorizedCompanies))
						OR (tblTransactions.ShipperID  IN (Select * FROM @AuthorizedCompanies))
						OR (tblTransactions.ShipToID   IN (Select * FROM @AuthorizedCompanies))
						OR (tblTransactions.SupplierID IN (Select * FROM @AuthorizedCompanies))
						OR (tblTransactions.ManagerID  IN (Select * FROM @AuthorizedCompanies))
						OR (tblTransactions.OwnerID    IN (Select * FROM @AuthorizedCompanies))
						OR (tblTransactions.BillToID   IN (Select * FROM @AuthorizedCompanies)))) 

		DROP TABLE #sitelist
	END	

	SELECT 
		T.TransID,
		T.AliasName,
		T.TransactionAliasGuid,
		T.LookupTransTypeIndex,
		T.SubType,
		T.Site,
		T.SiteGuid,
		T.TransReferenceID,
		T.InventoryDate,
		T.ShipToID,
		T.ShipToCode,
		T.ShipToCompanyGuid,
		T.SupplierID,
		T.SupplierCode,
		T.SupplierCompanyGuid,
		T.CreatedDate,
		T.CreatedBy,
		T.UpdatedDate,
		T.UpdatedBy,
		T.TransDateTime,
		T.TransVersion,
		T.SCACCode,
		T.CardNumber,
		T.ShipmentNumber,
		T.ShipperID,
		T.ShipperCode,
		T.ShipperCompanyGuid,
		T.OwnerID,
		T.OwnerCode,
		T.OwnerCompanyGuid,
		T.ManagerID,
		T.ManagerCode,
		T.ManagerCompanyGuid,
		T.CarrierID,
		T.CarrierCode,
		T.CarrierCompanyGuid,
		T.ConjoinTransID,
		T.ReversedTransID,
		T.LinkedDocumentNumber,
		T.ReversalType,
		T.PONumber,
		T.TimeIn,
		T.TimeOut,
		T.TimeEnd,
		T.RoutingID,
		T.TicketSource,
		T.LoadID,
		T.LookupTransactionStatusIndex,
		T.BillToID,
		T.BillToCode,
		T.BillToCompanyGuid,
		T.DriverIdentificationNumber,
		T.CreditAmount,
		T.CardExpiration,
		T.CardName,
		T.CardType,
		T.CashAmount,
		T.RouteOriginationDate,
		T.InternationalRouteIndicator,
		T.PreviousRoutingID,
		T.FinalStationIATAGuid,
		T.FinalStationIATAID,
		T.PreviousStationIATAGuid,
		T.PreviousStationIATAID,
		T.NextStationIATAGuid,
		T.NextStationIATAID,
		T.OriginStationIATAGuid,
		T.OriginStationIATAID,
		T.ShippingDocumentNumber,
		T.DocumentNumber,
		T.STD,
		T.ETD,
		T.STA,
		T.ETA,
		T.SFT,
		T.FST,
		T.EstimatedFuelingDuration,
		T.DeleteFlag,
		T.TicketMode,
		T.DestinationRegistrationID1,
		T.DestinationSerialNumber1,
		T.DestinationEquipmentType1,
		T.DestinationEquipmentModel1,
		T.DestinationCompanyEquipmentID1,
		T.Destination1EquipmentGuid,
		T.DestinationRegistrationID2,
		T.DestinationSerialNumber2,
		T.DestinationEquipmentType2,
		T.DestinationEquipmentModel2,
		T.DestinationCompanyEquipmentID2,
		T.Destination2EquipmentGuid,
		T.DestinationRegistrationID3,
		T.DestinationSerialNumber3,
		T.DestinationEquipmentType3,
		T.DestinationEquipmentModel3,
		T.DestinationCompanyEquipmentID3,
		T.Destination3EquipmentGuid,
		T.SourceRegistrationID1,
		T.SourceSerialNumber1,
		T.SourceEquipmentType1,
		T.SourceEquipmentModel1,
		T.SourceCompanyEquipmentID1,
		T.Source1EquipmentGuid,
		T.SourceRegistrationID2,
		T.SourceSerialNumber2,
		T.SourceEquipmentType2,
		T.SourceEquipmentModel2,
		T.SourceCompanyEquipmentID2,
		T.Source2EquipmentGuid,
		T.SourceRegistrationID3,
		T.SourceSerialNumber3,
		T.SourceEquipmentType3,
		T.SourceEquipmentModel3,
		T.SourceCompanyEquipmentID3,
		T.Source3EquipmentGuid,
		T.OperatorID,
		T.OperatorPersonnelGuid,
		T.EffectiveDate,
		T.ExpirationDate,
		T.ScheduledDate,
		T.AutoComplete,
		T.Flag01,
		T.Flag02,
		T.Flag03,
		T.Flag04,
		T.Flag05,
		T.Flag06,
		T.Number01,
		T.Number02,
		T.Number03,
		T.Number04,
		T.Number05,
		T.Number06,
		T.ContactFirstName,
		T.ContactSurname,
		T.Date01,
		T.Date02,
		T.Date03,
		T.Date04,
		T.LegacyNumber,
		T.Country,
		T.ContactInfo,
		T.AssociatedDocNumber,
		T.AssociatedCLIN,
		T.SubmittedToAccounting,
		T.LookupOriginApplicationIndex,
		T.FuelCardGuid,
		T.FuelCardID,
		T.AssociatedTransportOrderNumber,
		T.RequestedDateTime,
		T.DispatchedDateTime,
		T.OperatorName,
		T.FuelAdditiveFlag,
		T.IssuePoint,
		T.IssuePointNumber,
		T.RadioNumber,
		T.GateID,
		T.GateGuid,
		T.ShippingMethod,
		RC.ReasonCode + ' - ' + RC.Description AS ReasonCode,
		'N/A' AS TotalExcise,
		'N/A' AS TotalGST,
		'N/A' AS TotalMarkup,
		I.SequenceID,
		I.MeterStart,
		I.MeterStop,
		dbo.udf_ConvertFromSIUnits(I.GrossQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS GrossQuantity,
		dbo.udf_ConvertFromSIUnits(I.DeliveredGrossQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS DeliveredGrossQuantity,
		dbo.udf_ConvertFromSIUnits(I.Temperature,@TemperatureUnits,@TemperatureDecimalPlaces) AS Temperature,
		I.Vcf,
		dbo.udf_ConvertFromSIUnits(I.Density,@DensityUnits,@DensityDecimalPlaces) AS Density,
		I.Product,
		I.ProductCode,
		I.ProductType,
		I.ProductPrice,
		I.CLIN,
		dbo.udf_ConvertFromSIUnits(I.NetQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS NetQuantity,
		dbo.udf_ConvertFromSIUnits(I.DeliveredNetQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS DeliveredNetQuantity,
		dbo.udf_ConvertFromSIUnits(I.Pressure,dbo.udf_ProductTypeFactor(I.ProductType,@PressureUnits,@PressureUnits),dbo.udf_ProductTypeFactor(I.ProductType,@PressureDecimalPlaces,@PressureDecimalPlaces)) AS Pressure,
		I.ContractNumber,
		I.DestinationRegistrationID,
		I.DestinationSerialNumber,
		I.DestinationEquipmentType,
		I.DestinationEquipmentModel,
		I.DestinationCompartmentID,
		I.SourceRegistrationID,
		I.SourceSerialNumber,
		I.SourceEquipmentType,
		I.SourceEquipmentModel,
		I.SourceCompartmentID,
		I.MeterFactor,
		I.LineItemSequenceNumber,
		I.BatchNumber,
		I.DocumentNumber,
		dbo.udf_ConvertFromSIUnits(I.LineFill,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS LineFill,
		dbo.udf_ConvertFromSIUnits(I.BottomVolume,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS BottomVolume,
		dbo.udf_ConvertFromSIUnits(I.NetCapacity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS NetCapacity,
		I.Customs,
		I.LookupTransactionStatusIndex AS ItemLookupTransactionStatusIndex,
		I.ArmNumber,
		I.LineNumber,
		I.OperatorID,
		I.TankStatus,
		I.MeterStartDateTime,
		I.MeterStopDateTime,
		I.Pit,
		I.RequestedDateTime AS ItemRequestedDateTime,
		I.DispatchedDateTime AS ItemDispatchedDateTime,
		I.AcknowledgedDateTime,
		I.OnLocationTime,
		I.ValidationDateTime,
		I.CompletionDateTime,
		dbo.udf_ConvertFromSIUnits(I.ReceiptVariance,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS ReceiptVariance,
		dbo.udf_ConvertFromSIUnits(I.DifferentialPressure,@PressureUnits,@PressureDecimalPlaces) AS DifferentialPressure,
		dbo.udf_ConvertFromSIUnits(I.LoadRackVariance,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS LoadRackVariance,
		I.RequestedBy,
		dbo.udf_ConvertFromSIUnits(I.FreezePoint,@TemperatureUnits,@TemperatureDecimalPlaces) AS FreezePoint,
		I.DeleteFlag AS ItemDeleteFlag,
		I.StorageLocationID,
		I.MeterID,
		I.AdditiveProfileID,
		dbo.udf_ConvertFromSIUnits(I.PresetAmount,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS PresetAmount,
		I.CustomerProductName,
		I.CustomerProductCode,
		I.COAWaiver,
		I.COANote,
		I.COAID,
		I.InvoiceNumber,
		I.LookupQualityIndex,
		I.Tax1,
		I.Tax2,
		I.Tax3,
		I.Tax4,
		I.Tax5,
		I.Flag01 AS ItemFlag01,
		I.Flag02 AS ItemFlag02,
		I.Flag03 AS ItemFlag03,
		I.Flag04 AS ItemFlag04,
		I.Flag05 AS ItemFlag05,
		I.Flag06 AS ItemFlag06,
		I.Date01 AS ItemDate01,
		I.Date02 AS ItemDate02,
		I.Date03 AS ItemDate03,
		I.Date04 AS ItemDate04,
		I.Number01 AS ItemNumber01,
		I.Number02 AS ItemNumber02,
		I.Number03 AS ItemNumber03,
		I.Number04 AS ItemNumber04,
		I.Number05 AS ItemNumber05,
		I.Number06 AS ItemNumber06,
		I.AlternativeGrossVolume,
		I.AlternativeUnits,
		I.AlternativeNetVolume,
		I.LoadingLocationID,
		I.LoadingLocationStationGuid,
		I.DeliveryLocation,
		
		dbo.udf_ConvertFromSIUnits
		(
			I.Variance,
			dbo.udf_ProductTypeFactor(I.ProductType, @AdditiveVolumeUnits, @VolumeUnits),
			dbo.udf_ProductTypeFactor(I.ProductType, @AdditiveVolumeDecimalPlaces, @VolumeDecimalPlaces)
		) AS Variance,

		I.PartialFill,
		I.CompartmentsEmpty,
		I.CompartmentsPreviouslyLoaded,
		I.ContaminatePrompt,
		I.CurrencyUnit,
		I.EndDeliveryDate,
		I.ExchangeRate,
		I.InvoiceLineNumber,
		I.NonDomesticPrice,
		I.Odometer,
		I.OdometerHours,
		I.QualityTestNumber,
		I.TankLevel,
		I.TankLevelUnits,
		dbo.udf_ConvertFromSIUnits(I.MassQuantity,@MassUnits,@MassDecimalPlaces) AS MassQuantity,
		I.NetManualValueFlag,
		I.MassManualValueFlag,
		I.GrossManualValueFlag,
		I.VcfManualValueFlag,
		I.DeliveredGrossManualValueFlag,
		I.DeliveredNetManualValueFlag,
		@VolumePackageSize AS VolumePackageSize,
		@MassPackageSize AS MassPackageSize,
		CASE
			WHEN dbo.udf_ConvertFromSIUnits(@VolumePackageSize, @VolumeUnits, @VolumeDecimalPlaces) <> 0 
			THEN dbo.udf_ConvertFromSIUnits(I.NetQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) /  dbo.udf_ConvertFromSIUnits(@VolumePackageSize, @VolumeUnits, @VolumeDecimalPlaces)
			WHEN dbo.udf_ConvertFromSIUnits(@MassPackageSize,@MassUnits,@MassDecimalPlaces) <> 0 
			THEN dbo.udf_ConvertFromSIUnits(I.MassQuantity,@MassUnits,@MassDecimalPlaces) / dbo.udf_ConvertFromSIUnits(@MassPackageSize,@MassUnits,@MassDecimalPlaces)
			ELSE 0
		END AS PackageQuantity,
		N.Notes,
		N.AdditionalInformation,
		U.UserData1,
		U.UserData2,
		U.UserData3,
		U.UserData4,
		U.UserData5,
		U.UserData6,
		U.UserData7,
		U.UserData8,
		U.UserData9,
		U.UserData10,
		U.UserData11,
		U.UserData12,
		U.UserData13,
		U.UserData14,
		U.UserData15,
		U.UserData16,
		U.UserData17,
		U.UserData18,
		U.UserData19,
		U.UserData20,
		U.UserData21,
		U.UserData22,
		U.UserData23,
		U.UserData24,
		Manager.Name AS ManagerName,
		Manager.Address1 AS ManagerAddress,
		Manager.City AS ManagerCity,
		Manager.State AS ManagerState,
		Owner.Name AS OwnerName,
		Owner.Address1 AS OwnerAddress,
		Owner.City AS OwnerCity,
		Owner.State AS OwnerState,
		Shipper.Name AS ShipperName,
		Shipper.Address1 AS ShipperAddress,
		Shipper.City AS ShipperCity,
		Shipper.State AS ShipperState,
		BillTo.Name AS BillToName,
		BillTo.Address1 AS BillToAddress,
		BillTo.City AS BillToCity,
		BillTo.State AS BillToState,
		ShipTo.Name AS ShipToName,
		ShipTo.Address1 AS ShipToAddress,
		ShipTo.City AS ShipToCity,
		ShipTo.State AS ShipToState,
		Supplier.Name AS SupplierName,
		Supplier.Address1 AS SupplierAddress,
		Supplier.City AS SupplierCity,
		Supplier.State AS SupplierState,
		Carrier.Name AS CarrierName,
		Carrier.Address1 AS CarrierAddress,
		Carrier.City AS CarrierCity,
		Carrier.State AS CarrierState,
		I.ImproperAdditization,
		I.BrokenBlend,
		I.RequestedDeliveryDate,
		LU.UserData1 AS LineItemUserData1,
		LU.UserData2 AS LineItemUserData2,
		LU.UserData3 AS LineItemUserData3,
		LU.UserData4 AS LineItemUserData4,
		LU.UserData5 AS LineItemUserData5,
		LU.UserData6 AS LineItemUserData6,
		LU.UserData7 AS LineItemUserData7,
		LU.UserData8 AS LineItemUserData8,
		LU.UserData9 AS LineItemUserData9,
		LU.UserData10 AS LineItemUserData10,
		LU.UserData11 AS LineItemUserData11,
		LU.UserData12 AS LineItemUserData12,
		LU.UserData13 AS LineItemUserData13,
		LU.UserData14 AS LineItemUserData14,
		LU.UserData15 AS LineItemUserData15,
		LU.UserData16 AS LineItemUserData16,
		LU.UserData17 AS LineItemUserData17,
		LU.UserData18 AS LineItemUserData18,
		LU.UserData19 AS LineItemUserData19,
		LU.UserData20 AS LineItemUserData20,
		LU.UserData21 AS LineItemUserData21,
		LU.UserData22 AS LineItemUserData22,
		LU.UserData23 AS LineItemUserData23,
		LU.UserData24 AS LineItemUserData24,
		T.ErrorFlag,
		dbo.udf_GetUnitAbbrev(@VolumeUnits, 0) AS VolumeUnit,
		dbo.udf_GetUnitAbbrev(@TemperatureUnits, 0) AS TemperatureUnit,
		dbo.udf_GetUnitAbbrev(@DensityUnits, 0) AS DensityUnit,
		dbo.udf_GetUnitAbbrev(@MassUnits, 0) AS MassUnit,
		dbo.udf_GetUnitAbbrev(@LevelUnits, 0) AS LevelUnit,
		dbo.udf_GetUnitAbbrev(@FlowUnits, 0) AS FlowUnit,
		dbo.udf_GetUnitAbbrev(@PressureUnits, 0) AS PressureUnit,
		I.CleanLineItem,
		I.CleanLineDeductItem,
		I.CleanLineDeductQuantity,
		I.CleanLinePackQuantity,
		I.DualFuelingModeFlag,
		I.DualFuelingPrimaryFlag,
		I.EngineRunTime,
		I.FlowRate,
		I.FuelCompressionFactor,
		I.HydrantPressure,
		I.MobileDeviceID,
		I.TemperatureQualityStatus,
		I.MeterStartObtainedAutomaticallyFlag,
		I.MeterStopObtainedAutomaticallyFlag,
		I.NetVolumeIndicator,
		ERD.Error,
		ERD.InterfaceData01,
		ERD.InterfaceData02,
		ERD.InterfaceData03,
		ERD.InterfaceData04,
		ERD.InterfaceData05,
		ERD.InterfaceData06,
		ERD.InterfaceData07,
		ERD.InterfaceData08,
		T.TransactionGuid
	FROM (((((((((((((@Transactions J JOIN tblTransactionLineItems I WITH (NOLOCK) on J.TransactionGuid = I.TransactionGuid
		AND (@Product = '' OR @Product = I.Product OR I.Product IN (SELECT AliasName FROM @ProductAliasTable))
		AND (@ShowDeletedTrx = 1 OR (I.DeleteFlag IS NULL OR I.DeleteFlag = 0)))
		left join tblTransactionUserData U WITH (NOLOCK) on J.TransactionGuid = U.TransactionGuid) 
		left join tblTransactionNotes N WITH (NOLOCK) on J.TransactionGuid = N.TransactionGuid) 
		left join 	tblTransactions T WITH (NOLOCK) on J.TransactionGuid = T.TransactionGuid)
		LEFT JOIN tblAutoDistributionReasonCodes RC WITH (NOLOCK) on T.ReasonCodeGuid = RC.AutoDistributionReasonCodeGuid
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Manager ON T.ManagerCompanyGuid = Manager.CompanyGuid OR T.ManagerCompanyGuid = Manager._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Owner ON T.OwnerCompanyGuid = Owner.CompanyGuid OR T.OwnerCompanyGuid = Owner._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Shipper ON T.ShipperCompanyGuid = Shipper.CompanyGuid OR T.ShipperCompanyGuid = Shipper._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) BillTo ON T.BillToCompanyGuid = BillTo.CompanyGuid OR T.BillToCompanyGuid = BillTo._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) ShipTo ON T.ShipToCompanyGuid = ShipTo.CompanyGuid OR T.ShipToCompanyGuid = ShipTo._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Supplier ON T.SupplierCompanyGuid = Supplier.CompanyGuid OR T.SupplierCompanyGuid = Supplier._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Carrier ON T.CarrierCompanyGuid = Carrier.CompanyGuid OR T.CarrierCompanyGuid = Carrier._MasterRecordGuid)
		LEFT JOIN tblTransactionLineItemUserData LU WITH (NOLOCK) on I.TransactionLineItemGuid = LU.TransactionLineItemGuid)
		OUTER APPLY (SELECT TOP 1 ex.* FROM tblExportResultDetails ex WHERE ex.RecordID = t.TransID AND ex.TransVersion = t.TransVersion ORDER BY ex.UpdatedDate DESC) AS ERD
	) 

	UNION ALL 
	
	
	SELECT
		T.TransID,
		T.AliasName,
		T.TransactionAliasGuid,
		T.LookupTransTypeIndex,
		T.SubType,
		T.Site,
		T.SiteGuid,
		T.TransReferenceID,
		T.InventoryDate,
		T.ShipToID,
		T.ShipToCode,
		T.ShipToCompanyGuid,
		T.SupplierID,
		T.SupplierCode,
		T.SupplierCompanyGuid,
		T.CreatedDate,
		T.CreatedBy,
		T.UpdatedDate,
		T.UpdatedBy,
		T.TransDateTime,
		T.TransVersion,
		T.SCACCode,
		T.CardNumber,
		T.ShipmentNumber,
		T.ShipperID,
		T.ShipperCode,
		T.ShipperCompanyGuid,
		T.OwnerID,
		T.OwnerCode,
		T.OwnerCompanyGuid,
		T.ManagerID,
		T.ManagerCode,
		T.ManagerCompanyGuid,
		T.CarrierID,
		T.CarrierCode,
		T.CarrierCompanyGuid,
		T.ConjoinTransID,
		T.ReversedTransID,
		T.LinkedDocumentNumber,
		T.ReversalType,
		T.PONumber,
		T.TimeIn,
		T.TimeOut,
		T.TimeEnd,
		T.RoutingID,
		T.TicketSource,
		T.LoadID,
		T.LookupTransactionStatusIndex,
		T.BillToID,
		T.BillToCode,
		T.BillToCompanyGuid,
		T.DriverIdentificationNumber,
		T.CreditAmount,
		T.CardExpiration,
		T.CardName,
		T.CardType,
		T.CashAmount,
		T.RouteOriginationDate,
		T.InternationalRouteIndicator,
		T.PreviousRoutingID,
		T.FinalStationIATAGuid,
		T.FinalStationIATAID,
		T.PreviousStationIATAGuid,
		T.PreviousStationIATAID,
		T.NextStationIATAGuid,
		T.NextStationIATAID,
		T.OriginStationIATAGuid,
		T.OriginStationIATAID,
		T.ShippingDocumentNumber,
		T.DocumentNumber,
		T.STD,
		T.ETD,
		T.STA,
		T.ETA,
		T.SFT,
		T.FST,
		T.EstimatedFuelingDuration,
		T.DeleteFlag,
		T.TicketMode,
		T.DestinationRegistrationID1,
		T.DestinationSerialNumber1,
		T.DestinationEquipmentType1,
		T.DestinationEquipmentModel1,
		T.DestinationCompanyEquipmentID1,
		T.Destination1EquipmentGuid,
		T.DestinationRegistrationID2,
		T.DestinationSerialNumber2,
		T.DestinationEquipmentType2,
		T.DestinationEquipmentModel2,
		T.DestinationCompanyEquipmentID2,
		T.Destination2EquipmentGuid,
		T.DestinationRegistrationID3,
		T.DestinationSerialNumber3,
		T.DestinationEquipmentType3,
		T.DestinationEquipmentModel3,
		T.DestinationCompanyEquipmentID3,
		T.Destination3EquipmentGuid,
		T.SourceRegistrationID1,
		T.SourceSerialNumber1,
		T.SourceEquipmentType1,
		T.SourceEquipmentModel1,
		T.SourceCompanyEquipmentID1,
		T.Source1EquipmentGuid,
		T.SourceRegistrationID2,
		T.SourceSerialNumber2,
		T.SourceEquipmentType2,
		T.SourceEquipmentModel2,
		T.SourceCompanyEquipmentID2,
		T.Source2EquipmentGuid,
		T.SourceRegistrationID3,
		T.SourceSerialNumber3,
		T.SourceEquipmentType3,
		T.SourceEquipmentModel3,
		T.SourceCompanyEquipmentID3,
		T.Source3EquipmentGuid,
		T.OperatorID,
		T.OperatorPersonnelGuid,
		T.EffectiveDate,
		T.ExpirationDate,
		T.ScheduledDate,
		T.AutoComplete,
		T.Flag01,
		T.Flag02,
		T.Flag03,
		T.Flag04,
		T.Flag05,
		T.Flag06,
		T.Number01,
		T.Number02,
		T.Number03,
		T.Number04,
		T.Number05,
		T.Number06,
		T.ContactFirstName,
		T.ContactSurname,
		T.Date01,
		T.Date02,
		T.Date03,
		T.Date04,
		T.LegacyNumber,
		T.Country,
		T.ContactInfo,
		T.AssociatedDocNumber,
		T.AssociatedCLIN,
		T.SubmittedToAccounting,
		T.LookupOriginApplicationIndex,
		T.FuelCardGuid,
		T.FuelCardID,
		T.AssociatedTransportOrderNumber,
		T.RequestedDateTime,
		T.DispatchedDateTime,
		T.OperatorName,
		T.FuelAdditiveFlag,
		T.IssuePoint,
		T.IssuePointNumber,
		T.RadioNumber,
		T.GateID,
		T.GateGuid,
		T.ShippingMethod,
		RC.ReasonCode + ' - ' + RC.Description AS ReasonCode,
		'N/A' AS TotalExcise,
		'N/A' AS TotalGST,
		'N/A' AS TotalMarkup,
		I.SequenceID,
		I.MeterStart,
		I.MeterStop,
		dbo.udf_ConvertFromSIUnits(I.GrossQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS GrossQuantity,
		dbo.udf_ConvertFromSIUnits(I.DeliveredGrossQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS DeliveredGrossQuantity,
		dbo.udf_ConvertFromSIUnits(I.Temperature,@TemperatureUnits,@TemperatureDecimalPlaces) AS Temperature,
		I.Vcf,
		dbo.udf_ConvertFromSIUnits(I.Density,@DensityUnits,@DensityDecimalPlaces) AS Density,
		I.Product,
		I.ProductCode,
		I.ProductType,
		S.ProductPrice,
		S.CLIN,
		dbo.udf_ConvertFromSIUnits(I.NetQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS NetQuantity,
		dbo.udf_ConvertFromSIUnits(I.DeliveredNetQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS DeliveredNetQuantity,
		dbo.udf_ConvertFromSIUnits(I.Pressure,dbo.udf_ProductTypeFactor(I.ProductType,@PressureUnits,@PressureUnits),dbo.udf_ProductTypeFactor(I.ProductType,@PressureDecimalPlaces,@PressureDecimalPlaces)) AS Pressure,
		S.ContractNumber,
		S.DestinationRegistrationID,
		S.DestinationSerialNumber,
		S.DestinationEquipmentType,
		S.DestinationEquipmentModel,
		S.DestinationCompartmentID,
		S.SourceRegistrationID,
		S.SourceSerialNumber,
		S.SourceEquipmentType,
		S.SourceEquipmentModel,
		S.SourceCompartmentID,
		I.MeterFactor,
		S.LineItemSequenceNumber,
		I.BatchNumber,
		S.DocumentNumber,
		dbo.udf_ConvertFromSIUnits(I.LineFill,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS LineFill,
		dbo.udf_ConvertFromSIUnits(I.BottomVolume,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS BottomVolume,
		dbo.udf_ConvertFromSIUnits(I.NetCapacity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS NetCapacity,
		I.Customs,
		I.LookupTransactionStatusIndex,
		I.ArmNumber,
		I.LineNumber,
		S.OperatorID,
		I.TankStatus,
		I.MeterStartDateTime,
		I.MeterStopDateTime,
		S.Pit,
		S.RequestedDateTime AS ItemRequestedDateTime,
		S.DispatchedDateTime AS ItemDispatchedDateTime,
		S.AcknowledgedDateTime,
		S.OnLocationTime,
		S.ValidationDateTime,
		S.CompletionDateTime,
		dbo.udf_ConvertFromSIUnits(S.ReceiptVariance ,@VolumeUnits,@VolumeDecimalPlaces) AS ReceiptVariance,
		dbo.udf_ConvertFromSIUnits(I.DifferentialPressure,@PressureUnits,@PressureDecimalPlaces) AS DifferentialPressure,
		dbo.udf_ConvertFromSIUnits(S.LoadRackVariance,@VolumeUnits,@VolumeDecimalPlaces) AS LoadRackVariance,
		S.RequestedBy,
		dbo.udf_ConvertFromSIUnits(I.FreezePoint,@TemperatureUnits,@TemperatureDecimalPlaces) AS FreezePoint,
		S.DeleteFlag AS ItemDeleteFlag,
		S.StorageLocationID,
		S.MeterID,
		S.AdditiveProfileID,
		dbo.udf_ConvertFromSIUnits(I.PresetAmount,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) AS PresetAmount,
		S.CustomerProductName,
		S.CustomerProductCode,
		S.COAWaiver,
		S.COANote,
		S.COAID,
		S.InvoiceNumber,
		I.LookupQualityIndex,
		I.Tax1,
		I.Tax2,
		I.Tax3,
		I.Tax4,
		I.Tax5,
		I.Flag01 AS ItemFlag01,
		I.Flag02 AS ItemFlag02,
		I.Flag03 AS ItemFlag03,
		I.Flag04 AS ItemFlag04,
		I.Flag05 AS ItemFlag05,
		I.Flag06 AS ItemFlag06,
		I.Date01 AS ItemDate01,
		I.Date02 AS ItemDate02,
		I.Date03 AS ItemDate03,
		I.Date04 AS ItemDate04,
		I.Number01 AS ItemNumber01,
		I.Number02 AS ItemNumber02,
		I.Number03 AS ItemNumber03,
		I.Number04 AS ItemNumber04,
		I.Number05 AS ItemNumber05,
		I.Number06 AS ItemNumber06,
		S.AlternativeGrossVolume,
		S.AlternativeUnits,
		S.AlternativeNetVolume,
		S.LoadingLocationID,
		S.LoadingLocationStationGuid,
		S.DeliveryLocation,
		NULL, -- Variance
		NULL, -- PartialFill
		NULL, -- CompartmentsEmpty
		NULL, -- CompartmentsPreviouslyLoaded
		NULL, -- ContaminatePrompt
		NULL, -- CurrencyUnit
		NULL, -- EndDeliveryDate
		NULL, -- ExchangeRate
		NULL, -- InvoiceLineNumber
		NULL, -- NonDomesticPrice
		NULL, -- Odometer
		NULL, -- OdometerHours
		NULL, -- LookupQualityIndexTestNumber
		NULL, -- TankLevel
		NULL, -- TankLevelUnits
		dbo.udf_ConvertFromSIUnits(I.MassQuantity,@MassUnits,@MassDecimalPlaces) AS MassQuantity,
		I.NetManualValueFlag,
		I.MassManualValueFlag,
		I.GrossManualValueFlag,
		I.VcfManualValueFlag,
		I.DeliveredGrossManualValueFlag,
		I.DeliveredNetManualValueFlag,
		@VolumePackageSize AS VolumePackageSize,
		@MassPackageSize AS MassPackageSize,
		CASE
			WHEN @VolumePackageSize <> 0 
			THEN dbo.udf_ConvertFromSIUnits(I.NetQuantity,dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeUnits,@VolumeUnits),dbo.udf_ProductTypeFactor(I.ProductType,@AdditiveVolumeDecimalPlaces,@VolumeDecimalPlaces)) /  dbo.udf_ConvertFromSIUnits(@VolumePackageSize, @VolumeUnits, @VolumeDecimalPlaces)
			WHEN @MassPackageSize <> 0 
			THEN dbo.udf_ConvertFromSIUnits(I.MassQuantity,@MassUnits,@MassDecimalPlaces) / dbo.udf_ConvertFromSIUnits(@MassPackageSize,@MassUnits,@MassDecimalPlaces)
			ELSE 0
		END AS PackageQuantity,
		N.Notes,
		N.AdditionalInformation,
		U.UserData1,
		U.UserData2,
		U.UserData3,
		U.UserData4,
		U.UserData5,
		U.UserData6,
		U.UserData7,
		U.UserData8,
		U.UserData9,
		U.UserData10,
		U.UserData11,
		U.UserData12,
		U.UserData13,
		U.UserData14,
		U.UserData15,
		U.UserData16,
		U.UserData17,
		U.UserData18,
		U.UserData19,
		U.UserData20,
		U.UserData21,
		U.UserData22,
		U.UserData23,
		U.UserData24,
		Manager.Name AS ManagerName,Manager.Address1 AS ManagerAddress,Manager.City AS ManagerCity,Manager.State AS ManagerState,
		Owner.Name AS OwnerName,Owner.Address1 AS OwnerAddress,Owner.City AS OwnerCity,Owner.State AS OwnerState,
		Shipper.Name AS ShipperName,Shipper.Address1 AS ShipperAddress,Shipper.City AS ShipperCity,Shipper.State AS ShipperState,
		BillTo.Name AS BillToName,BillTo.Address1 AS BillToAddress,BillTo.City AS BillToCity,BillTo.State AS BillToState,
		ShipTo.Name AS ShipToName,ShipTo.Address1 AS ShipToAddress,ShipTo.City AS ShipToCity,ShipTo.State AS ShipToState,
		Supplier.Name AS SupplierName,Supplier.Address1 AS SupplierAddress,Supplier.City AS SupplierCity,Supplier.State AS SupplierState,
		Carrier.Name AS CarrierName,Carrier.Address1 AS CarrierAddress,Carrier.City AS CarrierCity,Carrier.State AS CarrierState,
		I.ImproperAdditization,
		I.BrokenBlend,
		SYSDATETIMEOFFSET() AS RequestedDeliveryDate,
		LU.UserData1 AS LineItemUserData1,
		LU.UserData2 AS LineItemUserData2,
		LU.UserData3 AS LineItemUserData3,
		LU.UserData4 AS LineItemUserData4,
		LU.UserData5 AS LineItemUserData5,
		LU.UserData6 AS LineItemUserData6,
		LU.UserData7 AS LineItemUserData7,
		LU.UserData8 AS LineItemUserData8,
		LU.UserData9 AS LineItemUserData9,
		LU.UserData10 AS LineItemUserData10,
		LU.UserData11 AS LineItemUserData11,
		LU.UserData12 AS LineItemUserData12,
		LU.UserData13 AS LineItemUserData13,
		LU.UserData14 AS LineItemUserData14,
		LU.UserData15 AS LineItemUserData15,
		LU.UserData16 AS LineItemUserData16,
		LU.UserData17 AS LineItemUserData17,
		LU.UserData18 AS LineItemUserData18,
		LU.UserData19 AS LineItemUserData19,
		LU.UserData20 AS LineItemUserData20,
		LU.UserData21 AS LineItemUserData21,
		LU.UserData22 AS LineItemUserData22,
		LU.UserData23 AS LineItemUserData23,
		LU.UserData24 AS LineItemUserData24,
		T.ErrorFlag,
		dbo.udf_GetUnitAbbrev(@VolumeUnits, 0) AS VolumeUnit,
		dbo.udf_GetUnitAbbrev(@TemperatureUnits, 0) AS TemperatureUnit,
		dbo.udf_GetUnitAbbrev(@DensityUnits, 0) AS DensityUnit,
		dbo.udf_GetUnitAbbrev(@MassUnits, 0) AS MassUnit,
		dbo.udf_GetUnitAbbrev(@LevelUnits, 0) AS LevelUnit,
		dbo.udf_GetUnitAbbrev(@FlowUnits, 0) AS FlowUnit,
		dbo.udf_GetUnitAbbrev(@PressureUnits, 0) AS PressureUnit,
		I.CleanLineItem,
		I.CleanLineDeductItem,
		I.CleanLineDeductQuantity,
		I.CleanLinePackQuantity,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		null,
		ERD.Error,
		ERD.InterfaceData01,
		ERD.InterfaceData02,
		ERD.InterfaceData03,
		ERD.InterfaceData04,
		ERD.InterfaceData05,
		ERD.InterfaceData06,
		ERD.InterfaceData07,
		ERD.InterfaceData08,
		T.TransactionGuid
	FROM ((((((((((((((@Transactions J inner join tblTransactionSubLineItems I WITH (NOLOCK) on J.TransactionGuid = I.TransactionGuid
		AND (@Product = '' OR @Product = I.Product OR I.Product IN (SELECT AliasName FROM @ProductAliasTable))
		AND (@ShowDeletedTrx = 1 OR (I.DeleteFlag IS NULL OR I.DeleteFlag = 0)))
		left join tblTransactionUserData U WITH (NOLOCK) on J.TransactionGuid = U.TransactionGuid) 
		left join tblTransactionNotes N WITH (NOLOCK) on J.TransactionGuid = N.TransactionGuid) 
		left join tblTransactions T WITH (NOLOCK) on J.TransactionGuid = T.TransactionGuid)
		LEFT JOIN tblAutoDistributionReasonCodes RC WITH (NOLOCK) on T.ReasonCodeGuid = RC.AutoDistributionReasonCodeGuid
		left join tblTransactionLineItems S WITH (NOLOCK) on S.TransactionLineItemGuid = I.TransactionLineItemGuid AND (@Product = '' OR @Product = S.Product) AND (@ShowDeletedTrx = 1 OR (S.DeleteFlag IS NULL OR S.DeleteFlag = 0)))
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Manager ON T.ManagerCompanyGuid = Manager.CompanyGuid OR T.ManagerCompanyGuid = Manager._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Owner ON T.OwnerCompanyGuid = Owner.CompanyGuid OR T.OwnerCompanyGuid = Owner._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Shipper ON T.ShipperCompanyGuid = Shipper.CompanyGuid OR T.ShipperCompanyGuid = Shipper._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) BillTo ON T.BillToCompanyGuid = BillTo.CompanyGuid OR T.BillToCompanyGuid = BillTo._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) ShipTo ON T.ShipToCompanyGuid = ShipTo.CompanyGuid OR T.ShipToCompanyGuid = ShipTo._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Supplier ON T.SupplierCompanyGuid = Supplier.CompanyGuid OR T.SupplierCompanyGuid = Supplier._MasterRecordGuid)
		LEFT JOIN (select SRM.* from @ErvCompanies LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Carrier ON T.CarrierCompanyGuid = Carrier.CompanyGuid OR T.CarrierCompanyGuid = Carrier._MasterRecordGuid)
		LEFT JOIN tblTransactionLineItemUserData LU WITH (NOLOCK) on I.TransactionLineItemGuid = LU.TransactionLineItemGuid) 
		OUTER APPLY (SELECT TOP 1 ex.* FROM tblExportResultDetails ex WHERE ex.RecordID = t.TransID AND ex.TransVersion = t.TransVersion ORDER BY ex.UpdatedDate DESC) AS ERD
	) 

	ORDER BY T.CreatedDate,I.LineNumber

END
