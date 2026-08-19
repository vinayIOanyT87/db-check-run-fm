CREATE PROCEDURE [dbo].[gsp_TransactionsInsertByPK]
(
		@TransactionGuid uniqueidentifier=NULL OUTPUT
	,	@TransID nvarchar(64)=NULL
	,	@AliasName nvarchar(32)=NULL
	,	@SubType nvarchar(20)=NULL
	,	@Site nvarchar(30)=NULL
	,	@TransReferenceID nvarchar(64)=NULL
	,	@InventoryDate date=NULL
	,	@ShipToID nvarchar(100)=NULL
	,	@ShipToCode nvarchar(10)=NULL
	,	@SupplierID nvarchar(100)=NULL
	,	@SupplierCode nvarchar(10)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@RequestedDeliveryDate datetimeoffset(7)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@TransDateTime datetimeoffset(7)=NULL
	,	@TransVersion bigint=NULL
	,	@SCACCode nvarchar(4)=NULL
	,	@CardNumber nvarchar(30)=NULL
	,	@ShipmentNumber nvarchar(30)=NULL
	,	@ShipperID nvarchar(100)=NULL
	,	@ShipperCode nvarchar(10)=NULL
	,	@OwnerID nvarchar(100)=NULL
	,	@OwnerCode nvarchar(10)=NULL
	,	@ManagerID nvarchar(100)=NULL
	,	@ManagerCode nvarchar(10)=NULL
	,	@CarrierID nvarchar(100)=NULL
	,	@CarrierCode nvarchar(10)=NULL
	,	@ConjoinTransID nvarchar(64)=NULL
	,	@ReversedTransID nvarchar(64)=NULL
	,	@LinkedDocumentNumber nvarchar(64)=NULL
	,	@ReversalType nvarchar(2)=NULL
	,	@PONumber nvarchar(14)=NULL
	,	@TimeIn datetimeoffset(7)=NULL
	,	@TimeOut datetimeoffset(7)=NULL
	,	@TimeEnd datetimeoffset(7)=NULL
	,	@RoutingID nvarchar(30)=NULL
	,	@TicketSource nvarchar(20)=NULL
	,	@LoadID nvarchar(50)=NULL
	,	@BillToID nvarchar(100)=NULL
	,	@BillToCode nvarchar(10)=NULL
	,	@DriverIdentificationNumber nvarchar(50)=NULL
	,	@CreditAmount float=NULL
	,	@CardExpiration datetimeoffset(7)=NULL
	,	@CardName nvarchar(30)=NULL
	,	@CardType nvarchar(30)=NULL
	,	@CashAmount float=NULL
	,	@RouteOriginationDate datetimeoffset(7)=NULL
	,	@InternationalRouteIndicator bit=NULL
	,	@PreviousRoutingID nvarchar(30)=NULL
	,	@ShippingDocumentNumber nvarchar(30)=NULL
	,	@DocumentNumber nvarchar(30)=NULL
	,	@STD datetimeoffset(7)=NULL
	,	@ETD datetimeoffset(7)=NULL
	,	@STA datetimeoffset(7)=NULL
	,	@ETA datetimeoffset(7)=NULL
	,	@SFT datetimeoffset(7)=NULL
	,	@FST datetimeoffset(7)=NULL
	,	@EstimatedFuelingDuration int=NULL
	,	@DeleteFlag bit=NULL
	,	@TicketMode nvarchar(15)=NULL
	,	@DestinationRegistrationID1 nvarchar(30)=NULL
	,	@DestinationSerialNumber1 nvarchar(10)=NULL
	,	@DestinationEquipmentType1 nvarchar(50)=NULL
	,	@DestinationEquipmentModel1 nvarchar(20)=NULL
	,	@DestinationCompanyEquipmentID1 nvarchar(30)=NULL
	,	@DestinationRegistrationID2 nvarchar(30)=NULL
	,	@DestinationSerialNumber2 nvarchar(10)=NULL
	,	@DestinationEquipmentType2 nvarchar(50)=NULL
	,	@DestinationEquipmentModel2 nvarchar(20)=NULL
	,	@DestinationCompanyEquipmentID2 nvarchar(30)=NULL
	,	@DestinationRegistrationID3 nvarchar(30)=NULL
	,	@DestinationSerialNumber3 nvarchar(10)=NULL
	,	@DestinationEquipmentType3 nvarchar(50)=NULL
	,	@DestinationEquipmentModel3 nvarchar(20)=NULL
	,	@DestinationCompanyEquipmentID3 nvarchar(30)=NULL
	,	@SourceRegistrationID1 nvarchar(30)=NULL
	,	@SourceSerialNumber1 nvarchar(10)=NULL
	,	@SourceEquipmentType1 nvarchar(50)=NULL
	,	@SourceEquipmentModel1 nvarchar(20)=NULL
	,	@SourceCompanyEquipmentID1 nvarchar(30)=NULL
	,	@SourceRegistrationID2 nvarchar(30)=NULL
	,	@SourceSerialNumber2 nvarchar(10)=NULL
	,	@SourceEquipmentType2 nvarchar(50)=NULL
	,	@SourceEquipmentModel2 nvarchar(20)=NULL
	,	@SourceCompanyEquipmentID2 nvarchar(30)=NULL
	,	@SourceRegistrationID3 nvarchar(30)=NULL
	,	@SourceSerialNumber3 nvarchar(10)=NULL
	,	@SourceEquipmentType3 nvarchar(50)=NULL
	,	@SourceEquipmentModel3 nvarchar(20)=NULL
	,	@SourceCompanyEquipmentID3 nvarchar(30)=NULL
	,	@OperatorID nvarchar(50)=NULL
	,	@EffectiveDate datetimeoffset(7)=NULL
	,	@ExpirationDate datetimeoffset(7)=NULL
	,	@ScheduledDate datetimeoffset(7)=NULL
	,	@AutoComplete bit=NULL
	,	@Flag01 bit=NULL
	,	@Flag02 bit=NULL
	,	@Flag03 bit=NULL
	,	@Flag04 bit=NULL
	,	@Flag05 bit=NULL
	,	@Flag06 bit=NULL
	,	@Number01 float=NULL
	,	@Number02 float=NULL
	,	@Number03 float=NULL
	,	@Number04 float=NULL
	,	@Number05 float=NULL
	,	@Number06 float=NULL
	,	@ContactFirstName nvarchar(50)=NULL
	,	@ContactSurname nvarchar(50)=NULL
	,	@Date01 datetimeoffset(7)=NULL
	,	@Date02 datetimeoffset(7)=NULL
	,	@Date03 datetimeoffset(7)=NULL
	,	@Date04 datetimeoffset(7)=NULL
	,	@LegacyNumber nvarchar(50)=NULL
	,	@Country nvarchar(50)=NULL
	,	@ContactInfo nvarchar(50)=NULL
	,	@AssociatedDocNumber nvarchar(30)=NULL
	,	@AssociatedCLIN nvarchar(10)=NULL
	,	@SubmittedToAccounting bit=NULL
	,	@FuelCardID nvarchar(50)=NULL
	,	@AssociatedTransportOrderNumber nvarchar(30)=NULL
	,	@RequestedDateTime datetimeoffset(7)=NULL
	,	@DispatchedDateTime datetimeoffset(7)=NULL
	,	@ErrorFlag bit=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupTransTypeIndex smallint=NULL
	,	@LookupTransactionStatusIndex int=NULL
	,	@LookupOriginApplicationIndex int=NULL
	,	@TransactionAliasGuid uniqueidentifier=NULL
	,	@BillToCompanyGuid uniqueidentifier=NULL
	,	@Destination1EquipmentGuid uniqueidentifier=NULL
	,	@Destination2EquipmentGuid uniqueidentifier=NULL
	,	@Destination3EquipmentGuid uniqueidentifier=NULL
	,	@FinalStationIATAGuid uniqueidentifier=NULL
	,	@FuelCardGuid uniqueidentifier=NULL
	,	@ManagerCompanyGuid uniqueidentifier=NULL
	,	@NextStationIATAGuid uniqueidentifier=NULL
	,	@OperatorPersonnelGuid uniqueidentifier=NULL
	,	@OriginStationIATAGuid uniqueidentifier=NULL
	,	@OwnerCompanyGuid uniqueidentifier=NULL
	,	@PreviousStationIATAGuid uniqueidentifier=NULL
	,	@ShipperCompanyGuid uniqueidentifier=NULL
	,	@ShipToCompanyGuid uniqueidentifier=NULL
	,	@Source1EquipmentGuid uniqueidentifier=NULL
	,	@Source2EquipmentGuid uniqueidentifier=NULL
	,	@Source3EquipmentGuid uniqueidentifier=NULL
	,	@SupplierCompanyGuid uniqueidentifier=NULL
	,	@CarrierCompanyGuid uniqueidentifier=NULL
	,	@ReasonCodeGuid uniqueidentifier=NULL
	,	@OriginStationIATAID nvarchar(50)=NULL
	,	@PreviousStationIATAID nvarchar(50)=NULL
	,	@NextStationIATAID nvarchar(50)=NULL
	,	@FinalStationIATAID nvarchar(50)=NULL
	,	@OperatorName nvarchar(150)=NULL
	,	@FuelAdditiveFlag bit=NULL
	,	@IssuePoint nvarchar(max)=NULL
	,	@IssuePointNumber nvarchar(max)=NULL
	,	@RadioNumber nvarchar(max)=NULL
	,	@GateID nvarchar(10)=NULL
	,	@GateGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5522767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactions]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactions] 
		(
			[TransactionGuid]
		,	[TransID]
		,	[AliasName]
		,	[SubType]
		,	[Site]
		,	[TransReferenceID]
		,	[InventoryDate]
		,	[ShipToID]
		,	[ShipToCode]
		,	[SupplierID]
		,	[SupplierCode]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[RequestedDeliveryDate]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[TransDateTime]
		,	[TransVersion]
		,	[SCACCode]
		,	[CardNumber]
		,	[ShipmentNumber]
		,	[ShipperID]
		,	[ShipperCode]
		,	[OwnerID]
		,	[OwnerCode]
		,	[ManagerID]
		,	[ManagerCode]
		,	[CarrierID]
		,	[CarrierCode]
		,	[ConjoinTransID]
		,	[ReversedTransID]
		,	[LinkedDocumentNumber]
		,	[ReversalType]
		,	[PONumber]
		,	[TimeIn]
		,	[TimeOut]
		,	[TimeEnd]
		,	[RoutingID]
		,	[TicketSource]
		,	[LoadID]
		,	[BillToID]
		,	[BillToCode]
		,	[DriverIdentificationNumber]
		,	[CreditAmount]
		,	[CardExpiration]
		,	[CardName]
		,	[CardType]
		,	[CashAmount]
		,	[RouteOriginationDate]
		,	[InternationalRouteIndicator]
		,	[PreviousRoutingID]
		,	[ShippingDocumentNumber]
		,	[DocumentNumber]
		,	[STD]
		,	[ETD]
		,	[STA]
		,	[ETA]
		,	[SFT]
		,	[FST]
		,	[EstimatedFuelingDuration]
		,	[DeleteFlag]
		,	[TicketMode]
		,	[DestinationRegistrationID1]
		,	[DestinationSerialNumber1]
		,	[DestinationEquipmentType1]
		,	[DestinationEquipmentModel1]
		,	[DestinationCompanyEquipmentID1]
		,	[DestinationRegistrationID2]
		,	[DestinationSerialNumber2]
		,	[DestinationEquipmentType2]
		,	[DestinationEquipmentModel2]
		,	[DestinationCompanyEquipmentID2]
		,	[DestinationRegistrationID3]
		,	[DestinationSerialNumber3]
		,	[DestinationEquipmentType3]
		,	[DestinationEquipmentModel3]
		,	[DestinationCompanyEquipmentID3]
		,	[SourceRegistrationID1]
		,	[SourceSerialNumber1]
		,	[SourceEquipmentType1]
		,	[SourceEquipmentModel1]
		,	[SourceCompanyEquipmentID1]
		,	[SourceRegistrationID2]
		,	[SourceSerialNumber2]
		,	[SourceEquipmentType2]
		,	[SourceEquipmentModel2]
		,	[SourceCompanyEquipmentID2]
		,	[SourceRegistrationID3]
		,	[SourceSerialNumber3]
		,	[SourceEquipmentType3]
		,	[SourceEquipmentModel3]
		,	[SourceCompanyEquipmentID3]
		,	[OperatorID]
		,	[EffectiveDate]
		,	[ExpirationDate]
		,	[ScheduledDate]
		,	[AutoComplete]
		,	[Flag01]
		,	[Flag02]
		,	[Flag03]
		,	[Flag04]
		,	[Flag05]
		,	[Flag06]
		,	[Number01]
		,	[Number02]
		,	[Number03]
		,	[Number04]
		,	[Number05]
		,	[Number06]
		,	[ContactFirstName]
		,	[ContactSurname]
		,	[Date01]
		,	[Date02]
		,	[Date03]
		,	[Date04]
		,	[LegacyNumber]
		,	[Country]
		,	[ContactInfo]
		,	[AssociatedDocNumber]
		,	[AssociatedCLIN]
		,	[SubmittedToAccounting]
		,	[FuelCardID]
		,	[AssociatedTransportOrderNumber]
		,	[RequestedDateTime]
		,	[DispatchedDateTime]
		,	[ErrorFlag]
		,	[SiteGuid]
		,	[LookupTransTypeIndex]
		,	[LookupTransactionStatusIndex]
		,	[LookupOriginApplicationIndex]
		,	[TransactionAliasGuid]
		,	[BillToCompanyGuid]
		,	[Destination1EquipmentGuid]
		,	[Destination2EquipmentGuid]
		,	[Destination3EquipmentGuid]
		,	[FinalStationIATAGuid]
		,	[FuelCardGuid]
		,	[ManagerCompanyGuid]
		,	[NextStationIATAGuid]
		,	[OperatorPersonnelGuid]
		,	[OriginStationIATAGuid]
		,	[OwnerCompanyGuid]
		,	[PreviousStationIATAGuid]
		,	[ShipperCompanyGuid]
		,	[ShipToCompanyGuid]
		,	[Source1EquipmentGuid]
		,	[Source2EquipmentGuid]
		,	[Source3EquipmentGuid]
		,	[SupplierCompanyGuid]
		,	[CarrierCompanyGuid]
		,	[ReasonCodeGuid]
		,	[OriginStationIATAID]
		,	[PreviousStationIATAID]
		,	[NextStationIATAID]
		,	[FinalStationIATAID]
		,	[OperatorName]
		,	[FuelAdditiveFlag]
		,	[IssuePoint]
		,	[IssuePointNumber]
		,	[RadioNumber]
		,	[GateID]
		,	[GateGuid]
		)
		VALUES
		(
			@TransactionGuid
		,	@TransID
		,	@AliasName
		,	@SubType
		,	@Site
		,	@TransReferenceID
		,	@InventoryDate
		,	@ShipToID
		,	@ShipToCode
		,	@SupplierID
		,	@SupplierCode
		,	@CreatedDate
		,	@CreatedBy
		,	@RequestedDeliveryDate
		,	@UpdatedDate
		,	@UpdatedBy
		,	@TransDateTime
		,	@TransVersion
		,	@SCACCode
		,	@CardNumber
		,	@ShipmentNumber
		,	@ShipperID
		,	@ShipperCode
		,	@OwnerID
		,	@OwnerCode
		,	@ManagerID
		,	@ManagerCode
		,	@CarrierID
		,	@CarrierCode
		,	@ConjoinTransID
		,	@ReversedTransID
		,	@LinkedDocumentNumber
		,	@ReversalType
		,	@PONumber
		,	@TimeIn
		,	@TimeOut
		,	@TimeEnd
		,	@RoutingID
		,	@TicketSource
		,	@LoadID
		,	@BillToID
		,	@BillToCode
		,	@DriverIdentificationNumber
		,	@CreditAmount
		,	@CardExpiration
		,	@CardName
		,	@CardType
		,	@CashAmount
		,	@RouteOriginationDate
		,	@InternationalRouteIndicator
		,	@PreviousRoutingID
		,	@ShippingDocumentNumber
		,	@DocumentNumber
		,	@STD
		,	@ETD
		,	@STA
		,	@ETA
		,	@SFT
		,	@FST
		,	@EstimatedFuelingDuration
		,	@DeleteFlag
		,	@TicketMode
		,	@DestinationRegistrationID1
		,	@DestinationSerialNumber1
		,	@DestinationEquipmentType1
		,	@DestinationEquipmentModel1
		,	@DestinationCompanyEquipmentID1
		,	@DestinationRegistrationID2
		,	@DestinationSerialNumber2
		,	@DestinationEquipmentType2
		,	@DestinationEquipmentModel2
		,	@DestinationCompanyEquipmentID2
		,	@DestinationRegistrationID3
		,	@DestinationSerialNumber3
		,	@DestinationEquipmentType3
		,	@DestinationEquipmentModel3
		,	@DestinationCompanyEquipmentID3
		,	@SourceRegistrationID1
		,	@SourceSerialNumber1
		,	@SourceEquipmentType1
		,	@SourceEquipmentModel1
		,	@SourceCompanyEquipmentID1
		,	@SourceRegistrationID2
		,	@SourceSerialNumber2
		,	@SourceEquipmentType2
		,	@SourceEquipmentModel2
		,	@SourceCompanyEquipmentID2
		,	@SourceRegistrationID3
		,	@SourceSerialNumber3
		,	@SourceEquipmentType3
		,	@SourceEquipmentModel3
		,	@SourceCompanyEquipmentID3
		,	@OperatorID
		,	@EffectiveDate
		,	@ExpirationDate
		,	@ScheduledDate
		,	@AutoComplete
		,	@Flag01
		,	@Flag02
		,	@Flag03
		,	@Flag04
		,	@Flag05
		,	@Flag06
		,	@Number01
		,	@Number02
		,	@Number03
		,	@Number04
		,	@Number05
		,	@Number06
		,	@ContactFirstName
		,	@ContactSurname
		,	@Date01
		,	@Date02
		,	@Date03
		,	@Date04
		,	@LegacyNumber
		,	@Country
		,	@ContactInfo
		,	@AssociatedDocNumber
		,	@AssociatedCLIN
		,	@SubmittedToAccounting
		,	@FuelCardID
		,	@AssociatedTransportOrderNumber
		,	@RequestedDateTime
		,	@DispatchedDateTime
		,	@ErrorFlag
		,	@SiteGuid
		,	@LookupTransTypeIndex
		,	@LookupTransactionStatusIndex
		,	@LookupOriginApplicationIndex
		,	@TransactionAliasGuid
		,	@BillToCompanyGuid
		,	@Destination1EquipmentGuid
		,	@Destination2EquipmentGuid
		,	@Destination3EquipmentGuid
		,	@FinalStationIATAGuid
		,	@FuelCardGuid
		,	@ManagerCompanyGuid
		,	@NextStationIATAGuid
		,	@OperatorPersonnelGuid
		,	@OriginStationIATAGuid
		,	@OwnerCompanyGuid
		,	@PreviousStationIATAGuid
		,	@ShipperCompanyGuid
		,	@ShipToCompanyGuid
		,	@Source1EquipmentGuid
		,	@Source2EquipmentGuid
		,	@Source3EquipmentGuid
		,	@SupplierCompanyGuid
		,	@CarrierCompanyGuid
		,	@ReasonCodeGuid
		,	@OriginStationIATAID
		,	@PreviousStationIATAID
		,	@NextStationIATAID
		,	@FinalStationIATAID
		,	@OperatorName
		,	@FuelAdditiveFlag
		,	@IssuePoint
		,	@IssuePointNumber
		,	@RadioNumber
		,	@GateID
		,	@GateGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactions]           
		WHERE TransactionGuid=@TransactionGuid;
	
 
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: gsp_TransactionsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
