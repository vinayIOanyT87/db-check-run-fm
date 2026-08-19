CREATE PROCEDURE [dbo].[gsp_TransactionLineItemsUpdateByPK]
(
		@TransactionLineItemGuid uniqueidentifier
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
	,	@NullOverrideSequenceID BIT=0 
	,	@NullOverrideMeterStart BIT=0 
	,	@NullOverrideMeterStop BIT=0 
	,	@NullOverrideGrossQuantity BIT=0 
	,	@NullOverrideTemperature BIT=0 
	,	@NullOverrideVcf BIT=0 
	,	@NullOverrideDensity BIT=0 
	,	@NullOverrideProduct BIT=0 
	,	@NullOverrideProductCode BIT=0 
	,	@NullOverrideProductType BIT=0 
	,	@NullOverrideProductPrice BIT=0 
	,	@NullOverrideCLIN BIT=0 
	,	@NullOverrideNetQuantity BIT=0 
	,	@NullOverrideContractNumber BIT=0 
	,	@NullOverrideDestinationRegistrationID BIT=0 
	,	@NullOverrideDestinationSerialNumber BIT=0 
	,	@NullOverrideDestinationEquipmentType BIT=0 
	,	@NullOverrideDestinationEquipmentModel BIT=0 
	,	@NullOverrideDestinationCompanyEquipmentID BIT=0 
	,	@NullOverrideDestinationCompartmentID BIT=0 
	,	@NullOverrideSourceRegistrationID BIT=0 
	,	@NullOverrideSourceSerialNumber BIT=0 
	,	@NullOverrideSourceEquipmentType BIT=0 
	,	@NullOverrideSourceEquipmentModel BIT=0 
	,	@NullOverrideSourceCompanyEquipmentID BIT=0 
	,	@NullOverrideSourceCompartmentID BIT=0 
	,	@NullOverrideMeterFactor BIT=0 
	,	@NullOverrideLineItemSequenceNumber BIT=0 
	,	@NullOverrideBatchNumber BIT=0 
	,	@NullOverrideDocumentNumber BIT=0 
	,	@NullOverrideLineFill BIT=0 
	,	@NullOverrideBottomVolume BIT=0 
	,	@NullOverrideNetCapacity BIT=0 
	,	@NullOverrideCustoms BIT=0 
	,	@NullOverrideArmNumber BIT=0 
	,	@NullOverrideLineNumber BIT=0 
	,	@NullOverrideOperatorID BIT=0 
	,	@NullOverrideTankStatus BIT=0 
	,	@NullOverrideMeterStartDateTime BIT=0 
	,	@NullOverrideMeterStopDateTime BIT=0 
	,	@NullOverridePit BIT=0 
	,	@NullOverrideRequestedDateTime BIT=0 
	,	@NullOverrideDispatchedDateTime BIT=0 
	,	@NullOverrideAcknowledgedDateTime BIT=0 
	,	@NullOverrideOnLocationTime BIT=0 
	,	@NullOverrideValidationDateTime BIT=0 
	,	@NullOverrideCompletionDateTime BIT=0 
	,	@NullOverrideReceiptVariance BIT=0 
	,	@NullOverrideDifferentialPressure BIT=0 
	,	@NullOverrideLoadRackVariance BIT=0 
	,	@NullOverrideRequestedBy BIT=0 
	,	@NullOverrideFreezePoint BIT=0 
	,	@NullOverrideDeleteFlag BIT=0 
	,	@NullOverrideStorageLocationID BIT=0 
	,	@NullOverrideMeterID BIT=0 
	,	@NullOverrideAdditiveProfileID BIT=0 
	,	@NullOverrideUpdatedDate BIT=0 
	,	@NullOverridePresetAmount BIT=0 
	,	@NullOverrideEngineeringUnitsIndex BIT=0 
	,	@NullOverrideCustomerProductName BIT=0 
	,	@NullOverrideCustomerProductCode BIT=0 
	,	@NullOverrideTransactionInventoryDate BIT=0 
	,	@NullOverrideCOAWaiver BIT=0 
	,	@NullOverrideCOANote BIT=0 
	,	@NullOverrideCOAID BIT=0 
	,	@NullOverrideTax1 BIT=0 
	,	@NullOverrideTax2 BIT=0 
	,	@NullOverrideTax3 BIT=0 
	,	@NullOverrideTax4 BIT=0 
	,	@NullOverrideTax5 BIT=0 
	,	@NullOverrideTransVersion BIT=0 
	,	@NullOverrideLoadingLocationID BIT=0 
	,	@NullOverrideImproperAdditization BIT=0 
	,	@NullOverrideBrokenBlend BIT=0 
	,	@NullOverrideContaminatePrompt BIT=0 
	,	@NullOverrideCompartmentsPreviouslyLoaded BIT=0 
	,	@NullOverrideCompartmentsEmpty BIT=0 
	,	@NullOverrideFlag01 BIT=0 
	,	@NullOverrideFlag02 BIT=0 
	,	@NullOverrideFlag03 BIT=0 
	,	@NullOverrideFlag04 BIT=0 
	,	@NullOverrideFlag05 BIT=0 
	,	@NullOverrideFlag06 BIT=0 
	,	@NullOverrideNumber01 BIT=0 
	,	@NullOverrideNumber02 BIT=0 
	,	@NullOverrideNumber03 BIT=0 
	,	@NullOverrideNumber04 BIT=0 
	,	@NullOverrideNumber05 BIT=0 
	,	@NullOverrideNumber06 BIT=0 
	,	@NullOverrideOdometerHours BIT=0 
	,	@NullOverrideEndDeliveryDate BIT=0 
	,	@NullOverrideRequestedDeliveryDate BIT=0 
	,	@NullOverrideInvoiceNumber BIT=0 
	,	@NullOverrideInvoiceLineNumber BIT=0 
	,	@NullOverrideAlternativeGrossVolume BIT=0 
	,	@NullOverrideAlternativeNetVolume BIT=0 
	,	@NullOverrideAlternativeUnits BIT=0 
	,	@NullOverrideTankLevel BIT=0 
	,	@NullOverrideTankLevelUnits BIT=0 
	,	@NullOverrideDate01 BIT=0 
	,	@NullOverrideDate02 BIT=0 
	,	@NullOverrideDate03 BIT=0 
	,	@NullOverrideDate04 BIT=0 
	,	@NullOverrideNonDomesticPrice BIT=0 
	,	@NullOverrideCurrencyUnit BIT=0 
	,	@NullOverrideExchangeRate BIT=0 
	,	@NullOverrideQualityTestNumber BIT=0 
	,	@NullOverrideOdometer BIT=0 
	,	@NullOverrideDeliveryLocation BIT=0 
	,	@NullOverrideVariance BIT=0 
	,	@NullOverridePartialFill BIT=0 
	,	@NullOverrideMassQuantity BIT=0 
	,	@NullOverrideNetManualValueFlag BIT=0 
	,	@NullOverrideMassManualValueFlag BIT=0 
	,	@NullOverrideGrossManualValueFlag BIT=0 
	,	@NullOverrideVcfManualValueFlag BIT=0 
	,	@NullOverrideLookupTransactionStatusIndex BIT=0 
	,	@NullOverrideLookupQualityIndex BIT=0 
	,	@NullOverrideStorageLocationTankGuid BIT=0 
	,	@NullOverrideAdditiveProfileGuid BIT=0 
	,	@NullOverrideDestinationCompartmentEquipmentGuid BIT=0 
	,	@NullOverrideDestinationEquipmentGuid BIT=0 
	,	@NullOverrideOperatorPersonnelGuid BIT=0 
	,	@NullOverrideProductGuid BIT=0 
	,	@NullOverrideSourceCompartmentEquipmentGuid BIT=0 
	,	@NullOverrideSourceEquipmentGuid BIT=0 
	,	@NullOverrideTransactionGuid BIT=0 
	,	@NullOverrideCurrencyGuid BIT=0 
	,	@NullOverrideOrderReferenceTransactionLineItemGuid BIT=0 
	,	@NullOverrideLoadingLocationStationGuid BIT=0 
	,	@NullOverrideMeterGuid BIT=0 
	,	@NullOverridePackageManualValueFlag BIT=0 
	,	@NullOverrideCleanLineItem BIT=0 
	,	@NullOverrideCleanLineDeductItem BIT=0 
	,	@NullOverrideCleanLineDeductQuantity BIT=0 
	,	@NullOverrideCleanLinePackQuantity BIT=0 
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_TransactionLineItemsUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.003 / 2014-02-05 16:52:29.8554327 -05:00
	-- Purpose: Update table [dbo].[tblTransactionLineItems]
	-- Notes:
	-- 1. @TransactionLineItemGuid and @UpdatedBy are required parameter.
	-- 2. If a value other than NULL is passed on @_RowVersion parameter then the stored procedure verifies whether _RowVersion of the record matches with the  
	--    @_RowVersion parameter and it will throw an exception if they don't match, otherwise it saves the parameters regardless.
	-- 3. The @_RowVersion output parameter will always be updated with new timestamp generated by the updating of the record.
	-- 4. To update a column with NULL then set the corresponding "@NullOverride..." parameter to 1 and either pass NULL through the correlated parameter 
	--    or do not include the parameter at all. 
	--    Example - Saving NULL to SiteGuid on tblEquipment:
	--            EXEC gsp_EquipmentUpdateByPK @EquipmentGuid='0000-...',@SiteGuid=NULL, @NullOverrideSiteGuid=1 
	--       or   EXEC gsp_EquipmentUpdateByPK @EquipmentGuid='0000-...', @NullOverrideSiteGuid=1 
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblTransactionLineItems] WHERE TransactionLineItemGuid=@TransactionLineItemGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END
 
		UPDATE [dbo].[tblTransactionLineItems] SET
			[SequenceID]=(CASE ISNULL(@NullOverrideSequenceID,0) WHEN 1 THEN @SequenceID ELSE ISNULL(@SequenceID,[SequenceID]) END)
		,	[MeterStart]=(CASE ISNULL(@NullOverrideMeterStart,0) WHEN 1 THEN @MeterStart ELSE ISNULL(@MeterStart,[MeterStart]) END)
		,	[MeterStop]=(CASE ISNULL(@NullOverrideMeterStop,0) WHEN 1 THEN @MeterStop ELSE ISNULL(@MeterStop,[MeterStop]) END)
		,	[GrossQuantity]=(CASE ISNULL(@NullOverrideGrossQuantity,0) WHEN 1 THEN @GrossQuantity ELSE ISNULL(@GrossQuantity,[GrossQuantity]) END)
		,	[Temperature]=(CASE ISNULL(@NullOverrideTemperature,0) WHEN 1 THEN @Temperature ELSE ISNULL(@Temperature,[Temperature]) END)
		,	[Vcf]=(CASE ISNULL(@NullOverrideVcf,0) WHEN 1 THEN @Vcf ELSE ISNULL(@Vcf,[Vcf]) END)
		,	[Density]=(CASE ISNULL(@NullOverrideDensity,0) WHEN 1 THEN @Density ELSE ISNULL(@Density,[Density]) END)
		,	[Product]=(CASE ISNULL(@NullOverrideProduct,0) WHEN 1 THEN @Product ELSE ISNULL(@Product,[Product]) END)
		,	[ProductCode]=(CASE ISNULL(@NullOverrideProductCode,0) WHEN 1 THEN @ProductCode ELSE ISNULL(@ProductCode,[ProductCode]) END)
		,	[ProductType]=(CASE ISNULL(@NullOverrideProductType,0) WHEN 1 THEN @ProductType ELSE ISNULL(@ProductType,[ProductType]) END)
		,	[ProductPrice]=(CASE ISNULL(@NullOverrideProductPrice,0) WHEN 1 THEN @ProductPrice ELSE ISNULL(@ProductPrice,[ProductPrice]) END)
		,	[CLIN]=(CASE ISNULL(@NullOverrideCLIN,0) WHEN 1 THEN @CLIN ELSE ISNULL(@CLIN,[CLIN]) END)
		,	[NetQuantity]=(CASE ISNULL(@NullOverrideNetQuantity,0) WHEN 1 THEN @NetQuantity ELSE ISNULL(@NetQuantity,[NetQuantity]) END)
		,	[ContractNumber]=(CASE ISNULL(@NullOverrideContractNumber,0) WHEN 1 THEN @ContractNumber ELSE ISNULL(@ContractNumber,[ContractNumber]) END)
		,	[DestinationRegistrationID]=(CASE ISNULL(@NullOverrideDestinationRegistrationID,0) WHEN 1 THEN @DestinationRegistrationID ELSE ISNULL(@DestinationRegistrationID,[DestinationRegistrationID]) END)
		,	[DestinationSerialNumber]=(CASE ISNULL(@NullOverrideDestinationSerialNumber,0) WHEN 1 THEN @DestinationSerialNumber ELSE ISNULL(@DestinationSerialNumber,[DestinationSerialNumber]) END)
		,	[DestinationEquipmentType]=(CASE ISNULL(@NullOverrideDestinationEquipmentType,0) WHEN 1 THEN @DestinationEquipmentType ELSE ISNULL(@DestinationEquipmentType,[DestinationEquipmentType]) END)
		,	[DestinationEquipmentModel]=(CASE ISNULL(@NullOverrideDestinationEquipmentModel,0) WHEN 1 THEN @DestinationEquipmentModel ELSE ISNULL(@DestinationEquipmentModel,[DestinationEquipmentModel]) END)
		,	[DestinationCompanyEquipmentID]=(CASE ISNULL(@NullOverrideDestinationCompanyEquipmentID,0) WHEN 1 THEN @DestinationCompanyEquipmentID ELSE ISNULL(@DestinationCompanyEquipmentID,[DestinationCompanyEquipmentID]) END)
		,	[DestinationCompartmentID]=(CASE ISNULL(@NullOverrideDestinationCompartmentID,0) WHEN 1 THEN @DestinationCompartmentID ELSE ISNULL(@DestinationCompartmentID,[DestinationCompartmentID]) END)
		,	[SourceRegistrationID]=(CASE ISNULL(@NullOverrideSourceRegistrationID,0) WHEN 1 THEN @SourceRegistrationID ELSE ISNULL(@SourceRegistrationID,[SourceRegistrationID]) END)
		,	[SourceSerialNumber]=(CASE ISNULL(@NullOverrideSourceSerialNumber,0) WHEN 1 THEN @SourceSerialNumber ELSE ISNULL(@SourceSerialNumber,[SourceSerialNumber]) END)
		,	[SourceEquipmentType]=(CASE ISNULL(@NullOverrideSourceEquipmentType,0) WHEN 1 THEN @SourceEquipmentType ELSE ISNULL(@SourceEquipmentType,[SourceEquipmentType]) END)
		,	[SourceEquipmentModel]=(CASE ISNULL(@NullOverrideSourceEquipmentModel,0) WHEN 1 THEN @SourceEquipmentModel ELSE ISNULL(@SourceEquipmentModel,[SourceEquipmentModel]) END)
		,	[SourceCompanyEquipmentID]=(CASE ISNULL(@NullOverrideSourceCompanyEquipmentID,0) WHEN 1 THEN @SourceCompanyEquipmentID ELSE ISNULL(@SourceCompanyEquipmentID,[SourceCompanyEquipmentID]) END)
		,	[SourceCompartmentID]=(CASE ISNULL(@NullOverrideSourceCompartmentID,0) WHEN 1 THEN @SourceCompartmentID ELSE ISNULL(@SourceCompartmentID,[SourceCompartmentID]) END)
		,	[MeterFactor]=(CASE ISNULL(@NullOverrideMeterFactor,0) WHEN 1 THEN @MeterFactor ELSE ISNULL(@MeterFactor,[MeterFactor]) END)
		,	[LineItemSequenceNumber]=(CASE ISNULL(@NullOverrideLineItemSequenceNumber,0) WHEN 1 THEN @LineItemSequenceNumber ELSE ISNULL(@LineItemSequenceNumber,[LineItemSequenceNumber]) END)
		,	[BatchNumber]=(CASE ISNULL(@NullOverrideBatchNumber,0) WHEN 1 THEN @BatchNumber ELSE ISNULL(@BatchNumber,[BatchNumber]) END)
		,	[DocumentNumber]=(CASE ISNULL(@NullOverrideDocumentNumber,0) WHEN 1 THEN @DocumentNumber ELSE ISNULL(@DocumentNumber,[DocumentNumber]) END)
		,	[LineFill]=(CASE ISNULL(@NullOverrideLineFill,0) WHEN 1 THEN @LineFill ELSE ISNULL(@LineFill,[LineFill]) END)
		,	[BottomVolume]=(CASE ISNULL(@NullOverrideBottomVolume,0) WHEN 1 THEN @BottomVolume ELSE ISNULL(@BottomVolume,[BottomVolume]) END)
		,	[NetCapacity]=(CASE ISNULL(@NullOverrideNetCapacity,0) WHEN 1 THEN @NetCapacity ELSE ISNULL(@NetCapacity,[NetCapacity]) END)
		,	[Customs]=(CASE ISNULL(@NullOverrideCustoms,0) WHEN 1 THEN @Customs ELSE ISNULL(@Customs,[Customs]) END)
		,	[ArmNumber]=(CASE ISNULL(@NullOverrideArmNumber,0) WHEN 1 THEN @ArmNumber ELSE ISNULL(@ArmNumber,[ArmNumber]) END)
		,	[LineNumber]=(CASE ISNULL(@NullOverrideLineNumber,0) WHEN 1 THEN @LineNumber ELSE ISNULL(@LineNumber,[LineNumber]) END)
		,	[OperatorID]=(CASE ISNULL(@NullOverrideOperatorID,0) WHEN 1 THEN @OperatorID ELSE ISNULL(@OperatorID,[OperatorID]) END)
		,	[TankStatus]=(CASE ISNULL(@NullOverrideTankStatus,0) WHEN 1 THEN @TankStatus ELSE ISNULL(@TankStatus,[TankStatus]) END)
		,	[MeterStartDateTime]=(CASE ISNULL(@NullOverrideMeterStartDateTime,0) WHEN 1 THEN @MeterStartDateTime ELSE ISNULL(@MeterStartDateTime,[MeterStartDateTime]) END)
		,	[MeterStopDateTime]=(CASE ISNULL(@NullOverrideMeterStopDateTime,0) WHEN 1 THEN @MeterStopDateTime ELSE ISNULL(@MeterStopDateTime,[MeterStopDateTime]) END)
		,	[Pit]=(CASE ISNULL(@NullOverridePit,0) WHEN 1 THEN @Pit ELSE ISNULL(@Pit,[Pit]) END)
		,	[RequestedDateTime]=(CASE ISNULL(@NullOverrideRequestedDateTime,0) WHEN 1 THEN @RequestedDateTime ELSE ISNULL(@RequestedDateTime,[RequestedDateTime]) END)
		,	[DispatchedDateTime]=(CASE ISNULL(@NullOverrideDispatchedDateTime,0) WHEN 1 THEN @DispatchedDateTime ELSE ISNULL(@DispatchedDateTime,[DispatchedDateTime]) END)
		,	[AcknowledgedDateTime]=(CASE ISNULL(@NullOverrideAcknowledgedDateTime,0) WHEN 1 THEN @AcknowledgedDateTime ELSE ISNULL(@AcknowledgedDateTime,[AcknowledgedDateTime]) END)
		,	[OnLocationTime]=(CASE ISNULL(@NullOverrideOnLocationTime,0) WHEN 1 THEN @OnLocationTime ELSE ISNULL(@OnLocationTime,[OnLocationTime]) END)
		,	[ValidationDateTime]=(CASE ISNULL(@NullOverrideValidationDateTime,0) WHEN 1 THEN @ValidationDateTime ELSE ISNULL(@ValidationDateTime,[ValidationDateTime]) END)
		,	[CompletionDateTime]=(CASE ISNULL(@NullOverrideCompletionDateTime,0) WHEN 1 THEN @CompletionDateTime ELSE ISNULL(@CompletionDateTime,[CompletionDateTime]) END)
		,	[ReceiptVariance]=(CASE ISNULL(@NullOverrideReceiptVariance,0) WHEN 1 THEN @ReceiptVariance ELSE ISNULL(@ReceiptVariance,[ReceiptVariance]) END)
		,	[DifferentialPressure]=(CASE ISNULL(@NullOverrideDifferentialPressure,0) WHEN 1 THEN @DifferentialPressure ELSE ISNULL(@DifferentialPressure,[DifferentialPressure]) END)
		,	[LoadRackVariance]=(CASE ISNULL(@NullOverrideLoadRackVariance,0) WHEN 1 THEN @LoadRackVariance ELSE ISNULL(@LoadRackVariance,[LoadRackVariance]) END)
		,	[RequestedBy]=(CASE ISNULL(@NullOverrideRequestedBy,0) WHEN 1 THEN @RequestedBy ELSE ISNULL(@RequestedBy,[RequestedBy]) END)
		,	[FreezePoint]=(CASE ISNULL(@NullOverrideFreezePoint,0) WHEN 1 THEN @FreezePoint ELSE ISNULL(@FreezePoint,[FreezePoint]) END)
		,	[DeleteFlag]=(CASE ISNULL(@NullOverrideDeleteFlag,0) WHEN 1 THEN @DeleteFlag ELSE ISNULL(@DeleteFlag,[DeleteFlag]) END)
		,	[StorageLocationID]=(CASE ISNULL(@NullOverrideStorageLocationID,0) WHEN 1 THEN @StorageLocationID ELSE ISNULL(@StorageLocationID,[StorageLocationID]) END)
		,	[MeterID]=(CASE ISNULL(@NullOverrideMeterID,0) WHEN 1 THEN @MeterID ELSE ISNULL(@MeterID,[MeterID]) END)
		,	[AdditiveProfileID]=(CASE ISNULL(@NullOverrideAdditiveProfileID,0) WHEN 1 THEN @AdditiveProfileID ELSE ISNULL(@AdditiveProfileID,[AdditiveProfileID]) END)
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		,	[UpdatedDate]=ISNULL(@UpdatedDate,SYSDATETIMEOFFSET())
		,	[PresetAmount]=(CASE ISNULL(@NullOverridePresetAmount,0) WHEN 1 THEN @PresetAmount ELSE ISNULL(@PresetAmount,[PresetAmount]) END)
		,	[EngineeringUnitsIndex]=(CASE ISNULL(@NullOverrideEngineeringUnitsIndex,0) WHEN 1 THEN @EngineeringUnitsIndex ELSE ISNULL(@EngineeringUnitsIndex,[EngineeringUnitsIndex]) END)
		,	[CustomerProductName]=(CASE ISNULL(@NullOverrideCustomerProductName,0) WHEN 1 THEN @CustomerProductName ELSE ISNULL(@CustomerProductName,[CustomerProductName]) END)
		,	[CustomerProductCode]=(CASE ISNULL(@NullOverrideCustomerProductCode,0) WHEN 1 THEN @CustomerProductCode ELSE ISNULL(@CustomerProductCode,[CustomerProductCode]) END)
		,	[TransactionInventoryDate]=(CASE ISNULL(@NullOverrideTransactionInventoryDate,0) WHEN 1 THEN @TransactionInventoryDate ELSE ISNULL(@TransactionInventoryDate,[TransactionInventoryDate]) END)
		,	[COAWaiver]=(CASE ISNULL(@NullOverrideCOAWaiver,0) WHEN 1 THEN @COAWaiver ELSE ISNULL(@COAWaiver,[COAWaiver]) END)
		,	[COANote]=(CASE ISNULL(@NullOverrideCOANote,0) WHEN 1 THEN @COANote ELSE ISNULL(@COANote,[COANote]) END)
		,	[COAID]=(CASE ISNULL(@NullOverrideCOAID,0) WHEN 1 THEN @COAID ELSE ISNULL(@COAID,[COAID]) END)
		,	[Tax1]=(CASE ISNULL(@NullOverrideTax1,0) WHEN 1 THEN @Tax1 ELSE ISNULL(@Tax1,[Tax1]) END)
		,	[Tax2]=(CASE ISNULL(@NullOverrideTax2,0) WHEN 1 THEN @Tax2 ELSE ISNULL(@Tax2,[Tax2]) END)
		,	[Tax3]=(CASE ISNULL(@NullOverrideTax3,0) WHEN 1 THEN @Tax3 ELSE ISNULL(@Tax3,[Tax3]) END)
		,	[Tax4]=(CASE ISNULL(@NullOverrideTax4,0) WHEN 1 THEN @Tax4 ELSE ISNULL(@Tax4,[Tax4]) END)
		,	[Tax5]=(CASE ISNULL(@NullOverrideTax5,0) WHEN 1 THEN @Tax5 ELSE ISNULL(@Tax5,[Tax5]) END)
		,	[TransVersion]=(CASE ISNULL(@NullOverrideTransVersion,0) WHEN 1 THEN @TransVersion ELSE ISNULL(@TransVersion,[TransVersion]) END)
		,	[LoadingLocationID]=(CASE ISNULL(@NullOverrideLoadingLocationID,0) WHEN 1 THEN @LoadingLocationID ELSE ISNULL(@LoadingLocationID,[LoadingLocationID]) END)
		,	[ImproperAdditization]=(CASE ISNULL(@NullOverrideImproperAdditization,0) WHEN 1 THEN @ImproperAdditization ELSE ISNULL(@ImproperAdditization,[ImproperAdditization]) END)
		,	[BrokenBlend]=(CASE ISNULL(@NullOverrideBrokenBlend,0) WHEN 1 THEN @BrokenBlend ELSE ISNULL(@BrokenBlend,[BrokenBlend]) END)
		,	[ContaminatePrompt]=(CASE ISNULL(@NullOverrideContaminatePrompt,0) WHEN 1 THEN @ContaminatePrompt ELSE ISNULL(@ContaminatePrompt,[ContaminatePrompt]) END)
		,	[CompartmentsPreviouslyLoaded]=(CASE ISNULL(@NullOverrideCompartmentsPreviouslyLoaded,0) WHEN 1 THEN @CompartmentsPreviouslyLoaded ELSE ISNULL(@CompartmentsPreviouslyLoaded,[CompartmentsPreviouslyLoaded]) END)
		,	[CompartmentsEmpty]=(CASE ISNULL(@NullOverrideCompartmentsEmpty,0) WHEN 1 THEN @CompartmentsEmpty ELSE ISNULL(@CompartmentsEmpty,[CompartmentsEmpty]) END)
		,	[Flag01]=(CASE ISNULL(@NullOverrideFlag01,0) WHEN 1 THEN @Flag01 ELSE ISNULL(@Flag01,[Flag01]) END)
		,	[Flag02]=(CASE ISNULL(@NullOverrideFlag02,0) WHEN 1 THEN @Flag02 ELSE ISNULL(@Flag02,[Flag02]) END)
		,	[Flag03]=(CASE ISNULL(@NullOverrideFlag03,0) WHEN 1 THEN @Flag03 ELSE ISNULL(@Flag03,[Flag03]) END)
		,	[Flag04]=(CASE ISNULL(@NullOverrideFlag04,0) WHEN 1 THEN @Flag04 ELSE ISNULL(@Flag04,[Flag04]) END)
		,	[Flag05]=(CASE ISNULL(@NullOverrideFlag05,0) WHEN 1 THEN @Flag05 ELSE ISNULL(@Flag05,[Flag05]) END)
		,	[Flag06]=(CASE ISNULL(@NullOverrideFlag06,0) WHEN 1 THEN @Flag06 ELSE ISNULL(@Flag06,[Flag06]) END)
		,	[Number01]=(CASE ISNULL(@NullOverrideNumber01,0) WHEN 1 THEN @Number01 ELSE ISNULL(@Number01,[Number01]) END)
		,	[Number02]=(CASE ISNULL(@NullOverrideNumber02,0) WHEN 1 THEN @Number02 ELSE ISNULL(@Number02,[Number02]) END)
		,	[Number03]=(CASE ISNULL(@NullOverrideNumber03,0) WHEN 1 THEN @Number03 ELSE ISNULL(@Number03,[Number03]) END)
		,	[Number04]=(CASE ISNULL(@NullOverrideNumber04,0) WHEN 1 THEN @Number04 ELSE ISNULL(@Number04,[Number04]) END)
		,	[Number05]=(CASE ISNULL(@NullOverrideNumber05,0) WHEN 1 THEN @Number05 ELSE ISNULL(@Number05,[Number05]) END)
		,	[Number06]=(CASE ISNULL(@NullOverrideNumber06,0) WHEN 1 THEN @Number06 ELSE ISNULL(@Number06,[Number06]) END)
		,	[OdometerHours]=(CASE ISNULL(@NullOverrideOdometerHours,0) WHEN 1 THEN @OdometerHours ELSE ISNULL(@OdometerHours,[OdometerHours]) END)
		,	[EndDeliveryDate]=(CASE ISNULL(@NullOverrideEndDeliveryDate,0) WHEN 1 THEN @EndDeliveryDate ELSE ISNULL(@EndDeliveryDate,[EndDeliveryDate]) END)
		,	[RequestedDeliveryDate]=(CASE ISNULL(@NullOverrideRequestedDeliveryDate,0) WHEN 1 THEN @RequestedDeliveryDate ELSE ISNULL(@RequestedDeliveryDate,[RequestedDeliveryDate]) END)
		,	[InvoiceNumber]=(CASE ISNULL(@NullOverrideInvoiceNumber,0) WHEN 1 THEN @InvoiceNumber ELSE ISNULL(@InvoiceNumber,[InvoiceNumber]) END)
		,	[InvoiceLineNumber]=(CASE ISNULL(@NullOverrideInvoiceLineNumber,0) WHEN 1 THEN @InvoiceLineNumber ELSE ISNULL(@InvoiceLineNumber,[InvoiceLineNumber]) END)
		,	[AlternativeGrossVolume]=(CASE ISNULL(@NullOverrideAlternativeGrossVolume,0) WHEN 1 THEN @AlternativeGrossVolume ELSE ISNULL(@AlternativeGrossVolume,[AlternativeGrossVolume]) END)
		,	[AlternativeNetVolume]=(CASE ISNULL(@NullOverrideAlternativeNetVolume,0) WHEN 1 THEN @AlternativeNetVolume ELSE ISNULL(@AlternativeNetVolume,[AlternativeNetVolume]) END)
		,	[AlternativeUnits]=(CASE ISNULL(@NullOverrideAlternativeUnits,0) WHEN 1 THEN @AlternativeUnits ELSE ISNULL(@AlternativeUnits,[AlternativeUnits]) END)
		,	[TankLevel]=(CASE ISNULL(@NullOverrideTankLevel,0) WHEN 1 THEN @TankLevel ELSE ISNULL(@TankLevel,[TankLevel]) END)
		,	[TankLevelUnits]=(CASE ISNULL(@NullOverrideTankLevelUnits,0) WHEN 1 THEN @TankLevelUnits ELSE ISNULL(@TankLevelUnits,[TankLevelUnits]) END)
		,	[Date01]=(CASE ISNULL(@NullOverrideDate01,0) WHEN 1 THEN @Date01 ELSE ISNULL(@Date01,[Date01]) END)
		,	[Date02]=(CASE ISNULL(@NullOverrideDate02,0) WHEN 1 THEN @Date02 ELSE ISNULL(@Date02,[Date02]) END)
		,	[Date03]=(CASE ISNULL(@NullOverrideDate03,0) WHEN 1 THEN @Date03 ELSE ISNULL(@Date03,[Date03]) END)
		,	[Date04]=(CASE ISNULL(@NullOverrideDate04,0) WHEN 1 THEN @Date04 ELSE ISNULL(@Date04,[Date04]) END)
		,	[NonDomesticPrice]=(CASE ISNULL(@NullOverrideNonDomesticPrice,0) WHEN 1 THEN @NonDomesticPrice ELSE ISNULL(@NonDomesticPrice,[NonDomesticPrice]) END)
		,	[CurrencyUnit]=(CASE ISNULL(@NullOverrideCurrencyUnit,0) WHEN 1 THEN @CurrencyUnit ELSE ISNULL(@CurrencyUnit,[CurrencyUnit]) END)
		,	[ExchangeRate]=(CASE ISNULL(@NullOverrideExchangeRate,0) WHEN 1 THEN @ExchangeRate ELSE ISNULL(@ExchangeRate,[ExchangeRate]) END)
		,	[QualityTestNumber]=(CASE ISNULL(@NullOverrideQualityTestNumber,0) WHEN 1 THEN @QualityTestNumber ELSE ISNULL(@QualityTestNumber,[QualityTestNumber]) END)
		,	[Odometer]=(CASE ISNULL(@NullOverrideOdometer,0) WHEN 1 THEN @Odometer ELSE ISNULL(@Odometer,[Odometer]) END)
		,	[DeliveryLocation]=(CASE ISNULL(@NullOverrideDeliveryLocation,0) WHEN 1 THEN @DeliveryLocation ELSE ISNULL(@DeliveryLocation,[DeliveryLocation]) END)
		,	[Variance]=(CASE ISNULL(@NullOverrideVariance,0) WHEN 1 THEN @Variance ELSE ISNULL(@Variance,[Variance]) END)
		,	[PartialFill]=(CASE ISNULL(@NullOverridePartialFill,0) WHEN 1 THEN @PartialFill ELSE ISNULL(@PartialFill,[PartialFill]) END)
		,	[MassQuantity]=(CASE ISNULL(@NullOverrideMassQuantity,0) WHEN 1 THEN @MassQuantity ELSE ISNULL(@MassQuantity,[MassQuantity]) END)
		,	[NetManualValueFlag]=(CASE ISNULL(@NullOverrideNetManualValueFlag,0) WHEN 1 THEN @NetManualValueFlag ELSE ISNULL(@NetManualValueFlag,[NetManualValueFlag]) END)
		,	[MassManualValueFlag]=(CASE ISNULL(@NullOverrideMassManualValueFlag,0) WHEN 1 THEN @MassManualValueFlag ELSE ISNULL(@MassManualValueFlag,[MassManualValueFlag]) END)
		,	[GrossManualValueFlag]=(CASE ISNULL(@NullOverrideGrossManualValueFlag,0) WHEN 1 THEN @GrossManualValueFlag ELSE ISNULL(@GrossManualValueFlag,[GrossManualValueFlag]) END)
		,	[VcfManualValueFlag]=(CASE ISNULL(@NullOverrideVcfManualValueFlag,0) WHEN 1 THEN @VcfManualValueFlag ELSE ISNULL(@VcfManualValueFlag,[VcfManualValueFlag]) END)
		,	[LookupTransactionStatusIndex]=(CASE ISNULL(@NullOverrideLookupTransactionStatusIndex,0) WHEN 1 THEN @LookupTransactionStatusIndex ELSE ISNULL(@LookupTransactionStatusIndex,[LookupTransactionStatusIndex]) END)
		,	[LookupQualityIndex]=(CASE ISNULL(@NullOverrideLookupQualityIndex,0) WHEN 1 THEN @LookupQualityIndex ELSE ISNULL(@LookupQualityIndex,[LookupQualityIndex]) END)
		,	[StorageLocationTankGuid]=(CASE ISNULL(@NullOverrideStorageLocationTankGuid,0) WHEN 1 THEN @StorageLocationTankGuid ELSE ISNULL(@StorageLocationTankGuid,[StorageLocationTankGuid]) END)
		,	[AdditiveProfileGuid]=(CASE ISNULL(@NullOverrideAdditiveProfileGuid,0) WHEN 1 THEN @AdditiveProfileGuid ELSE ISNULL(@AdditiveProfileGuid,[AdditiveProfileGuid]) END)
		,	[DestinationCompartmentEquipmentGuid]=(CASE ISNULL(@NullOverrideDestinationCompartmentEquipmentGuid,0) WHEN 1 THEN @DestinationCompartmentEquipmentGuid ELSE ISNULL(@DestinationCompartmentEquipmentGuid,[DestinationCompartmentEquipmentGuid]) END)
		,	[DestinationEquipmentGuid]=(CASE ISNULL(@NullOverrideDestinationEquipmentGuid,0) WHEN 1 THEN @DestinationEquipmentGuid ELSE ISNULL(@DestinationEquipmentGuid,[DestinationEquipmentGuid]) END)
		,	[OperatorPersonnelGuid]=(CASE ISNULL(@NullOverrideOperatorPersonnelGuid,0) WHEN 1 THEN @OperatorPersonnelGuid ELSE ISNULL(@OperatorPersonnelGuid,[OperatorPersonnelGuid]) END)
		,	[ProductGuid]=(CASE ISNULL(@NullOverrideProductGuid,0) WHEN 1 THEN @ProductGuid ELSE ISNULL(@ProductGuid,[ProductGuid]) END)
		,	[SourceCompartmentEquipmentGuid]=(CASE ISNULL(@NullOverrideSourceCompartmentEquipmentGuid,0) WHEN 1 THEN @SourceCompartmentEquipmentGuid ELSE ISNULL(@SourceCompartmentEquipmentGuid,[SourceCompartmentEquipmentGuid]) END)
		,	[SourceEquipmentGuid]=(CASE ISNULL(@NullOverrideSourceEquipmentGuid,0) WHEN 1 THEN @SourceEquipmentGuid ELSE ISNULL(@SourceEquipmentGuid,[SourceEquipmentGuid]) END)
		,	[TransactionGuid]=(CASE ISNULL(@NullOverrideTransactionGuid,0) WHEN 1 THEN @TransactionGuid ELSE ISNULL(@TransactionGuid,[TransactionGuid]) END)
		,	[CurrencyGuid]=(CASE ISNULL(@NullOverrideCurrencyGuid,0) WHEN 1 THEN @CurrencyGuid ELSE ISNULL(@CurrencyGuid,[CurrencyGuid]) END)
		,	[OrderReferenceTransactionLineItemGuid]=(CASE ISNULL(@NullOverrideOrderReferenceTransactionLineItemGuid,0) WHEN 1 THEN @OrderReferenceTransactionLineItemGuid ELSE ISNULL(@OrderReferenceTransactionLineItemGuid,[OrderReferenceTransactionLineItemGuid]) END)
		,	[LoadingLocationStationGuid]=(CASE ISNULL(@NullOverrideLoadingLocationStationGuid,0) WHEN 1 THEN @LoadingLocationStationGuid ELSE ISNULL(@LoadingLocationStationGuid,[LoadingLocationStationGuid]) END)
		,	[MeterGuid]=(CASE ISNULL(@NullOverrideMeterGuid,0) WHEN 1 THEN @MeterGuid ELSE ISNULL(@MeterGuid,[MeterGuid]) END)
		,	[PackageManualValueFlag]=(CASE ISNULL(@NullOverridePackageManualValueFlag,0) WHEN 1 THEN @PackageManualValueFlag ELSE ISNULL(@PackageManualValueFlag,[PackageManualValueFlag]) END)
		,	[CleanLineItem]=(CASE ISNULL(@NullOverrideCleanLineItem,0) WHEN 1 THEN @CleanLineItem ELSE ISNULL(@CleanLineItem,[CleanLineItem]) END)
		,	[CleanLineDeductItem]=(CASE ISNULL(@NullOverrideCleanLineDeductItem,0) WHEN 1 THEN @CleanLineDeductItem ELSE ISNULL(@CleanLineDeductItem,[CleanLineDeductItem]) END)
		,	[CleanLineDeductQuantity]=(CASE ISNULL(@NullOverrideCleanLineDeductQuantity,0) WHEN 1 THEN @CleanLineDeductQuantity ELSE ISNULL(@CleanLineDeductQuantity,[CleanLineDeductQuantity]) END)
		,	[CleanLinePackQuantity]=(CASE ISNULL(@NullOverrideCleanLinePackQuantity,0) WHEN 1 THEN @CleanLinePackQuantity ELSE ISNULL(@CleanLinePackQuantity,[CleanLinePackQuantity]) END)
		WHERE	TransactionLineItemGuid=@TransactionLineItemGuid;
 
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
						+ 'Procedure Name: gsp_TransactionLineItemsUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
