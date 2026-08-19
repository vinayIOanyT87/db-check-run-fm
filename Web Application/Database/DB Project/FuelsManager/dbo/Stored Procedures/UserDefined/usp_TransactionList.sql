

CREATE PROCEDURE [dbo].[usp_TransactionList]
	@AliasName NVARCHAR (200), 
	@TransTypeID NVARCHAR (2), 
	@BeginDate DATE, 
	@EndDate DATE, 
	@ManagerID NVARCHAR (100), 
	@OwnerID NVARCHAR (100), 
	@ShipperID NVARCHAR (100), 
	@BillToID NVARCHAR (100), 
	@ShipToID NVARCHAR (100), 
	@CarrierID NVARCHAR (100), 
	@DocumentNumber NVARCHAR (30), 
	@TransactionStatus NVARCHAR (2), 
	@DriverIdentificationNumber NVARCHAR (50), 
	@LoginSiteGuid UNIQUEIDENTIFIER, 
	@SiteGuid UNIQUEIDENTIFIER, 
	@UserGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @SiteList TABLE
	(
		Site NVARCHAR (50) NOT NULL
	)
	
	INSERT   INTO @SiteList
				SELECT   ID
				FROM     dbo.tblSites,
							map.tblSiteToSite
				WHERE    ParentSiteGuid = @SiteGuid
				AND      map.tblSiteToSite.ChildSiteGuid = dbo.tblSites.SiteGuid

	DECLARE @AuthorizedCompanies TABLE
	(
		[ID] [nvarchar](100) NOT NULL
	)
	
	INSERT   INTO @AuthorizedCompanies
				SELECT   *
				FROM     udf_AuthorizedCompanies(@LoginSiteGuid, @SiteGuid, @UserGuid)

	SELECT   T.TransID,
				T.AliasName,
				T.transactionAliasGuid,
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
				T.RequestedDeliveryDate,
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
				Carrier.State AS CarrierState
	FROM 
		dbo.tblTransactions T
		LEFT JOIN tblAutoDistributionReasonCodes RC WITH (NOLOCK) on T.ReasonCodeGuid = RC.AutoDistributionReasonCodeGuid
		LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Manager ON T.ManagerCompanyGuid = Manager.CompanyGuid OR T.ManagerCompanyGuid = Manager._MasterRecordGuid
		LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Owner ON T.OwnerCompanyGuid = Owner.CompanyGuid OR T.OwnerCompanyGuid = Owner._MasterRecordGuid
		LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Shipper ON T.ShipperCompanyGuid = Shipper.CompanyGuid OR T.ShipperCompanyGuid = Shipper._MasterRecordGuid
		LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) BillTo ON T.BillToCompanyGuid = BillTo.CompanyGuid OR T.BillToCompanyGuid = BillTo._MasterRecordGuid
		LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) ShipTo ON T.ShipToCompanyGuid = ShipTo.CompanyGuid OR T.ShipToCompanyGuid = ShipTo._MasterRecordGuid
		LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Supplier ON T.SupplierCompanyGuid = Supplier.CompanyGuid OR T.SupplierCompanyGuid = Supplier._MasterRecordGuid
		LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (@SiteGuid) LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Carrier ON T.CarrierCompanyGuid = Carrier.CompanyGuid OR T.CarrierCompanyGuid = Carrier._MasterRecordGuid
					
	WHERE ((T.DeleteFlag IS NULL) OR (T.DeleteFlag = 0))
	  AND (@AliasName = '' OR (T.AliasName = @AliasName))
	  AND (@TransTypeID = '' OR (T.LookupTransTypeIndex = CAST(@TransTypeID AS INT)))
	  AND (@ManagerID = '' OR (T.ManagerID = @ManagerID))
	  AND (@OwnerID = '' OR (T.OwnerID = @OwnerID))
	  AND (@ShipperID = '' OR (T.ShipperID = @ShipperID))
	  AND (@BillToID = '' OR (T.BillToID = @BillToID))
	  AND (@ShipToID = '' OR (T.ShipToID = @ShipToID))
	  AND (@CarrierID = '' OR (T.CarrierID = @CarrierID))
	  AND (@DocumentNumber = '' OR (T.DocumentNumber = @DocumentNumber))
	  AND (@TransactionStatus = '' OR (T.LookupTransactionStatusIndex = CAST(@TransactionStatus AS INT)))
	  AND (@DriverIdentificationNumber = '' OR (T.DriverIdentificationNumber = @DriverIdentificationNumber))
	  AND (T.TransDateTime >= @BeginDate)
	  AND (T.TransDateTime <= @EndDate)
	  AND (Site IN (SELECT  * FROM    @SiteList))
	  AND (@UserGuid IS NULL
		OR ((T.CarrierID IN (SELECT   * FROM     @AuthorizedCompanies))
		OR (T.ShipperID IN (SELECT   * FROM     @AuthorizedCompanies))
		OR (T.ShipToID IN (SELECT * FROM   @AuthorizedCompanies))
		OR (T.SupplierID IN (SELECT  * FROM    @AuthorizedCompanies))
		OR (T.ManagerID IN (SELECT   * FROM     @AuthorizedCompanies))
		OR (T.OwnerID IN (SELECT  * FROM    @AuthorizedCompanies))
		OR (T.BillToID IN (SELECT * FROM   @AuthorizedCompanies))
		)
	)

	ORDER BY TransDateTime
END
GO
