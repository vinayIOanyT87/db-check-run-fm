CREATE PROCEDURE [dbo].[gsp_TransactionLineItemsInsertByPK]
(
		@TransactionLineItemGuid uniqueidentifier=NULL OUTPUT
	,	@SequenceID smallint=NULL
	,	@MeterStart float=NULL
	,	@MeterStop float=NULL
	,	@GrossQuantity float=NULL
	,	@Temperature float=NULL
	,	@Vcf float=NULL
	,	@Density float=NULL
	,	@Product nvarchar(30)=NULL
	,	@ProductCode nvarchar(30)=NULL
	,	@ProductType nvarchar(20)=NULL
	,	@ProductPrice float=NULL
	,	@CLIN nvarchar(10)=NULL
	,	@NetQuantity float=NULL
	,	@ContractNumber nvarchar(30)=NULL
	,	@DestinationRegistrationID nvarchar(30)=NULL
	,	@DestinationSerialNumber nvarchar(10)=NULL
	,	@DestinationEquipmentType nvarchar(50)=NULL
	,	@DestinationEquipmentModel nvarchar(20)=NULL
	,	@DestinationCompanyEquipmentID nvarchar(30)=NULL
	,	@DestinationCompartmentID nvarchar(50)=NULL
	,	@SourceRegistrationID nvarchar(30)=NULL
	,	@SourceSerialNumber nvarchar(10)=NULL
	,	@SourceEquipmentType nvarchar(50)=NULL
	,	@SourceEquipmentModel nvarchar(20)=NULL
	,	@SourceCompanyEquipmentID nvarchar(30)=NULL
	,	@SourceCompartmentID nvarchar(50)=NULL
	,	@MeterFactor float=NULL
	,	@LineItemSequenceNumber nvarchar(5)=NULL
	,	@BatchNumber nvarchar(20)=NULL
	,	@DocumentNumber nvarchar(30)=NULL
	,	@LineFill float=NULL
	,	@BottomVolume float=NULL
	,	@NetCapacity float=NULL
	,	@Customs nvarchar(20)=NULL
	,	@ArmNumber int=NULL
	,	@LineNumber int=NULL
	,	@OperatorID nvarchar(50)=NULL
	,	@TankStatus nvarchar(30)=NULL
	,	@MeterStartDateTime datetimeoffset(7)=NULL
	,	@MeterStopDateTime datetimeoffset(7)=NULL
	,	@Pit nvarchar(10)=NULL
	,	@RequestedDateTime datetimeoffset(7)=NULL
	,	@DispatchedDateTime datetimeoffset(7)=NULL
	,	@AcknowledgedDateTime datetimeoffset(7)=NULL
	,	@OnLocationTime datetimeoffset(7)=NULL
	,	@ValidationDateTime datetimeoffset(7)=NULL
	,	@CompletionDateTime datetimeoffset(7)=NULL
	,	@ReceiptVariance float=NULL
	,	@DifferentialPressure float=NULL
	,	@LoadRackVariance float=NULL
	,	@RequestedBy nvarchar(50)=NULL
	,	@FreezePoint float=NULL
	,	@DeleteFlag bit=NULL
	,	@StorageLocationID nvarchar(50)=NULL
	,	@MeterID nvarchar(50)=NULL
	,	@AdditiveProfileID nvarchar(50)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@PresetAmount float=NULL
	,	@EngineeringUnitsIndex int=NULL
	,	@CustomerProductName nvarchar(50)=NULL
	,	@CustomerProductCode nvarchar(20)=NULL
	,	@TransactionInventoryDate date=NULL
	,	@COAWaiver bit=NULL
	,	@COANote nvarchar(50)=NULL
	,	@COAID nvarchar(40)=NULL
	,	@Tax1 float=NULL
	,	@Tax2 float=NULL
	,	@Tax3 float=NULL
	,	@Tax4 float=NULL
	,	@Tax5 float=NULL
	,	@TransVersion bigint=NULL
	,	@LoadingLocationID nvarchar(30)=NULL
	,	@ImproperAdditization bit=NULL
	,	@BrokenBlend bit=NULL
	,	@ContaminatePrompt bit=NULL
	,	@CompartmentsPreviouslyLoaded bit=NULL
	,	@CompartmentsEmpty bit=NULL
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
	,	@OdometerHours float=NULL
	,	@EndDeliveryDate datetimeoffset(7)=NULL
	,	@RequestedDeliveryDate datetimeoffset(7)=NULL
	,	@InvoiceNumber nvarchar(50)=NULL
	,	@InvoiceLineNumber nvarchar(50)=NULL
	,	@AlternativeGrossVolume float=NULL
	,	@AlternativeNetVolume float=NULL
	,	@AlternativeUnits int=NULL
	,	@TankLevel float=NULL
	,	@TankLevelUnits int=NULL
	,	@Date01 datetimeoffset(7)=NULL
	,	@Date02 datetimeoffset(7)=NULL
	,	@Date03 datetimeoffset(7)=NULL
	,	@Date04 datetimeoffset(7)=NULL
	,	@NonDomesticPrice float=NULL
	,	@CurrencyUnit int=NULL
	,	@ExchangeRate float=NULL
	,	@QualityTestNumber nvarchar(50)=NULL
	,	@Odometer float=NULL
	,	@DeliveryLocation nvarchar(50)=NULL
	,	@Variance float=NULL
	,	@PartialFill bit=NULL
	,	@MassQuantity float=NULL
	,	@NetManualValueFlag bit=NULL
	,	@MassManualValueFlag bit=NULL
	,	@GrossManualValueFlag bit=NULL
	,	@VcfManualValueFlag bit=NULL
	,	@LookupTransactionStatusIndex int=NULL
	,	@LookupQualityIndex int=NULL
	,	@StorageLocationTankGuid uniqueidentifier=NULL
	,	@AdditiveProfileGuid uniqueidentifier=NULL
	,	@DestinationCompartmentEquipmentGuid uniqueidentifier=NULL
	,	@DestinationEquipmentGuid uniqueidentifier=NULL
	,	@OperatorPersonnelGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@SourceCompartmentEquipmentGuid uniqueidentifier=NULL
	,	@SourceEquipmentGuid uniqueidentifier=NULL
	,	@TransactionGuid uniqueidentifier=NULL
	,	@CurrencyGuid uniqueidentifier=NULL
	,	@OrderReferenceTransactionLineItemGuid uniqueidentifier=NULL
	,	@LoadingLocationStationGuid uniqueidentifier=NULL
	,	@MeterGuid uniqueidentifier=NULL
	,	@PackageManualValueFlag bit=NULL
	,	@CleanLineItem bit=NULL
	,	@CleanLineDeductItem bit=NULL
	,	@CleanLineDeductQuantity float=NULL
	,	@CleanLinePackQuantity float=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionLineItemsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5262767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactionLineItems]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionLineItemGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactionLineItems] 
		(
			[TransactionLineItemGuid]
		,	[SequenceID]
		,	[MeterStart]
		,	[MeterStop]
		,	[GrossQuantity]
		,	[Temperature]
		,	[Vcf]
		,	[Density]
		,	[Product]
		,	[ProductCode]
		,	[ProductType]
		,	[ProductPrice]
		,	[CLIN]
		,	[NetQuantity]
		,	[ContractNumber]
		,	[DestinationRegistrationID]
		,	[DestinationSerialNumber]
		,	[DestinationEquipmentType]
		,	[DestinationEquipmentModel]
		,	[DestinationCompanyEquipmentID]
		,	[DestinationCompartmentID]
		,	[SourceRegistrationID]
		,	[SourceSerialNumber]
		,	[SourceEquipmentType]
		,	[SourceEquipmentModel]
		,	[SourceCompanyEquipmentID]
		,	[SourceCompartmentID]
		,	[MeterFactor]
		,	[LineItemSequenceNumber]
		,	[BatchNumber]
		,	[DocumentNumber]
		,	[LineFill]
		,	[BottomVolume]
		,	[NetCapacity]
		,	[Customs]
		,	[ArmNumber]
		,	[LineNumber]
		,	[OperatorID]
		,	[TankStatus]
		,	[MeterStartDateTime]
		,	[MeterStopDateTime]
		,	[Pit]
		,	[RequestedDateTime]
		,	[DispatchedDateTime]
		,	[AcknowledgedDateTime]
		,	[OnLocationTime]
		,	[ValidationDateTime]
		,	[CompletionDateTime]
		,	[ReceiptVariance]
		,	[DifferentialPressure]
		,	[LoadRackVariance]
		,	[RequestedBy]
		,	[FreezePoint]
		,	[DeleteFlag]
		,	[StorageLocationID]
		,	[MeterID]
		,	[AdditiveProfileID]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[PresetAmount]
		,	[EngineeringUnitsIndex]
		,	[CustomerProductName]
		,	[CustomerProductCode]
		,	[TransactionInventoryDate]
		,	[COAWaiver]
		,	[COANote]
		,	[COAID]
		,	[Tax1]
		,	[Tax2]
		,	[Tax3]
		,	[Tax4]
		,	[Tax5]
		,	[TransVersion]
		,	[LoadingLocationID]
		,	[ImproperAdditization]
		,	[BrokenBlend]
		,	[ContaminatePrompt]
		,	[CompartmentsPreviouslyLoaded]
		,	[CompartmentsEmpty]
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
		,	[OdometerHours]
		,	[EndDeliveryDate]
		,	[RequestedDeliveryDate]
		,	[InvoiceNumber]
		,	[InvoiceLineNumber]
		,	[AlternativeGrossVolume]
		,	[AlternativeNetVolume]
		,	[AlternativeUnits]
		,	[TankLevel]
		,	[TankLevelUnits]
		,	[Date01]
		,	[Date02]
		,	[Date03]
		,	[Date04]
		,	[NonDomesticPrice]
		,	[CurrencyUnit]
		,	[ExchangeRate]
		,	[QualityTestNumber]
		,	[Odometer]
		,	[DeliveryLocation]
		,	[Variance]
		,	[PartialFill]
		,	[MassQuantity]
		,	[NetManualValueFlag]
		,	[MassManualValueFlag]
		,	[GrossManualValueFlag]
		,	[VcfManualValueFlag]
		,	[LookupTransactionStatusIndex]
		,	[LookupQualityIndex]
		,	[StorageLocationTankGuid]
		,	[AdditiveProfileGuid]
		,	[DestinationCompartmentEquipmentGuid]
		,	[DestinationEquipmentGuid]
		,	[OperatorPersonnelGuid]
		,	[ProductGuid]
		,	[SourceCompartmentEquipmentGuid]
		,	[SourceEquipmentGuid]
		,	[TransactionGuid]
		,	[CurrencyGuid]
		,	[OrderReferenceTransactionLineItemGuid]
		,	[LoadingLocationStationGuid]
		,	[MeterGuid]
		,	[PackageManualValueFlag]
		,	[CleanLineItem]
		,	[CleanLineDeductItem]
		,	[CleanLineDeductQuantity]
		,	[CleanLinePackQuantity]
		)
		VALUES
		(
			@TransactionLineItemGuid
		,	@SequenceID
		,	@MeterStart
		,	@MeterStop
		,	@GrossQuantity
		,	@Temperature
		,	@Vcf
		,	@Density
		,	@Product
		,	@ProductCode
		,	@ProductType
		,	@ProductPrice
		,	@CLIN
		,	@NetQuantity
		,	@ContractNumber
		,	@DestinationRegistrationID
		,	@DestinationSerialNumber
		,	@DestinationEquipmentType
		,	@DestinationEquipmentModel
		,	@DestinationCompanyEquipmentID
		,	@DestinationCompartmentID
		,	@SourceRegistrationID
		,	@SourceSerialNumber
		,	@SourceEquipmentType
		,	@SourceEquipmentModel
		,	@SourceCompanyEquipmentID
		,	@SourceCompartmentID
		,	@MeterFactor
		,	@LineItemSequenceNumber
		,	@BatchNumber
		,	@DocumentNumber
		,	@LineFill
		,	@BottomVolume
		,	@NetCapacity
		,	@Customs
		,	@ArmNumber
		,	@LineNumber
		,	@OperatorID
		,	@TankStatus
		,	@MeterStartDateTime
		,	@MeterStopDateTime
		,	@Pit
		,	@RequestedDateTime
		,	@DispatchedDateTime
		,	@AcknowledgedDateTime
		,	@OnLocationTime
		,	@ValidationDateTime
		,	@CompletionDateTime
		,	@ReceiptVariance
		,	@DifferentialPressure
		,	@LoadRackVariance
		,	@RequestedBy
		,	@FreezePoint
		,	@DeleteFlag
		,	@StorageLocationID
		,	@MeterID
		,	@AdditiveProfileID
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@PresetAmount
		,	@EngineeringUnitsIndex
		,	@CustomerProductName
		,	@CustomerProductCode
		,	@TransactionInventoryDate
		,	@COAWaiver
		,	@COANote
		,	@COAID
		,	@Tax1
		,	@Tax2
		,	@Tax3
		,	@Tax4
		,	@Tax5
		,	@TransVersion
		,	@LoadingLocationID
		,	@ImproperAdditization
		,	@BrokenBlend
		,	@ContaminatePrompt
		,	@CompartmentsPreviouslyLoaded
		,	@CompartmentsEmpty
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
		,	@OdometerHours
		,	@EndDeliveryDate
		,	@RequestedDeliveryDate
		,	@InvoiceNumber
		,	@InvoiceLineNumber
		,	@AlternativeGrossVolume
		,	@AlternativeNetVolume
		,	@AlternativeUnits
		,	@TankLevel
		,	@TankLevelUnits
		,	@Date01
		,	@Date02
		,	@Date03
		,	@Date04
		,	@NonDomesticPrice
		,	@CurrencyUnit
		,	@ExchangeRate
		,	@QualityTestNumber
		,	@Odometer
		,	@DeliveryLocation
		,	@Variance
		,	@PartialFill
		,	@MassQuantity
		,	@NetManualValueFlag
		,	@MassManualValueFlag
		,	@GrossManualValueFlag
		,	@VcfManualValueFlag
		,	@LookupTransactionStatusIndex
		,	@LookupQualityIndex
		,	@StorageLocationTankGuid
		,	@AdditiveProfileGuid
		,	@DestinationCompartmentEquipmentGuid
		,	@DestinationEquipmentGuid
		,	@OperatorPersonnelGuid
		,	@ProductGuid
		,	@SourceCompartmentEquipmentGuid
		,	@SourceEquipmentGuid
		,	@TransactionGuid
		,	@CurrencyGuid
		,	@OrderReferenceTransactionLineItemGuid
		,	@LoadingLocationStationGuid
		,	@MeterGuid
		,	@PackageManualValueFlag
		,	@CleanLineItem
		,	@CleanLineDeductItem
		,	@CleanLineDeductQuantity
		,	@CleanLinePackQuantity
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionLineItems]           
		WHERE TransactionLineItemGuid=@TransactionLineItemGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionLineItemsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
