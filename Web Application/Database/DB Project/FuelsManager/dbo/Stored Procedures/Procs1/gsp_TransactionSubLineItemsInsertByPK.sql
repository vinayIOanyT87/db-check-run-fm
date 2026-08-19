CREATE PROCEDURE [dbo].[gsp_TransactionSubLineItemsInsertByPK]
(
		@TransactionSubLineItemGuid uniqueidentifier=NULL OUTPUT
	,	@SequenceID int=NULL
	,	@Product nvarchar(30)=NULL
	,	@ProductCode nvarchar(50)=NULL
	,	@ProductType nvarchar(20)=NULL
	,	@GrossQuantity float=NULL
	,	@NetQuantity float=NULL
	,	@Vcf float=NULL
	,	@Density float=NULL
	,	@Temperature float=NULL
	,	@Customs nvarchar(20)=NULL
	,	@ArmNumber int=NULL
	,	@LineNumber int=NULL
	,	@BatchNumber nvarchar(20)=NULL
	,	@LineFill float=NULL
	,	@BottomVolume float=NULL
	,	@NetCapacity float=NULL
	,	@TankStatus nvarchar(30)=NULL
	,	@MeterFactor float=NULL
	,	@MeterStart float=NULL
	,	@MeterStop float=NULL
	,	@MeterStopDateTime datetimeoffset(7)=NULL
	,	@MeterStartDateTime datetimeoffset(7)=NULL
	,	@FreezePoint float=NULL
	,	@DifferentialPressure float=NULL
	,	@DosageRate float=NULL
	,	@DeleteFlag bit=NULL
	,	@PresetAmount float=NULL
	,	@StorageLocationID nvarchar(50)=NULL
	,	@MeterID nvarchar(50)=NULL
	,	@COAID nvarchar(40)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@TransactionInventoryDate date=NULL
	,	@Tax1 float=NULL
	,	@Tax2 float=NULL
	,	@Tax3 float=NULL
	,	@Tax4 float=NULL
	,	@Tax5 float=NULL
	,	@TransVersion bigint=NULL
	,	@ImproperAdditization bit=NULL
	,	@BrokenBlend bit=NULL
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
	,	@Date01 datetimeoffset(7)=NULL
	,	@Date02 datetimeoffset(7)=NULL
	,	@Date03 datetimeoffset(7)=NULL
	,	@Date04 datetimeoffset(7)=NULL
	,	@MassQuantity float=NULL
	,	@NetManualValueFlag bit=NULL
	,	@MassManualValueFlag bit=NULL
	,	@GrossManualValueFlag bit=NULL
	,	@VcfManualValueFlag bit=NULL
	,	@LookupTransactionStatusIndex int=NULL
	,	@LookupQualityIndex int=NULL
	,	@TransactionLineItemGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@TransactionGuid uniqueidentifier=NULL
	,	@StorageLocationTankGuid uniqueidentifier=NULL
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
	-- Stored procedure: [dbo].[gsp_TransactionSubLineItemsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.5712767 -05:00
	-- Purpose: Insert into table [dbo].[tblTransactionSubLineItems]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TransactionSubLineItemGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblTransactionSubLineItems] 
		(
			[TransactionSubLineItemGuid]
		,	[SequenceID]
		,	[Product]
		,	[ProductCode]
		,	[ProductType]
		,	[GrossQuantity]
		,	[NetQuantity]
		,	[Vcf]
		,	[Density]
		,	[Temperature]
		,	[Customs]
		,	[ArmNumber]
		,	[LineNumber]
		,	[BatchNumber]
		,	[LineFill]
		,	[BottomVolume]
		,	[NetCapacity]
		,	[TankStatus]
		,	[MeterFactor]
		,	[MeterStart]
		,	[MeterStop]
		,	[MeterStopDateTime]
		,	[MeterStartDateTime]
		,	[FreezePoint]
		,	[DifferentialPressure]
		,	[DosageRate]
		,	[DeleteFlag]
		,	[PresetAmount]
		,	[StorageLocationID]
		,	[MeterID]
		,	[COAID]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[TransactionInventoryDate]
		,	[Tax1]
		,	[Tax2]
		,	[Tax3]
		,	[Tax4]
		,	[Tax5]
		,	[TransVersion]
		,	[ImproperAdditization]
		,	[BrokenBlend]
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
		,	[Date01]
		,	[Date02]
		,	[Date03]
		,	[Date04]
		,	[MassQuantity]
		,	[NetManualValueFlag]
		,	[MassManualValueFlag]
		,	[GrossManualValueFlag]
		,	[VcfManualValueFlag]
		,	[LookupTransactionStatusIndex]
		,	[LookupQualityIndex]
		,	[TransactionLineItemGuid]
		,	[ProductGuid]
		,	[TransactionGuid]
		,	[StorageLocationTankGuid]
		,	[MeterGuid]
		,	[PackageManualValueFlag]
		,	[CleanLineItem]
		,	[CleanLineDeductItem]
		,	[CleanLineDeductQuantity]
		,	[CleanLinePackQuantity]
		)
		VALUES
		(
			@TransactionSubLineItemGuid
		,	@SequenceID
		,	@Product
		,	@ProductCode
		,	@ProductType
		,	@GrossQuantity
		,	@NetQuantity
		,	@Vcf
		,	@Density
		,	@Temperature
		,	@Customs
		,	@ArmNumber
		,	@LineNumber
		,	@BatchNumber
		,	@LineFill
		,	@BottomVolume
		,	@NetCapacity
		,	@TankStatus
		,	@MeterFactor
		,	@MeterStart
		,	@MeterStop
		,	@MeterStopDateTime
		,	@MeterStartDateTime
		,	@FreezePoint
		,	@DifferentialPressure
		,	@DosageRate
		,	@DeleteFlag
		,	@PresetAmount
		,	@StorageLocationID
		,	@MeterID
		,	@COAID
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@TransactionInventoryDate
		,	@Tax1
		,	@Tax2
		,	@Tax3
		,	@Tax4
		,	@Tax5
		,	@TransVersion
		,	@ImproperAdditization
		,	@BrokenBlend
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
		,	@Date01
		,	@Date02
		,	@Date03
		,	@Date04
		,	@MassQuantity
		,	@NetManualValueFlag
		,	@MassManualValueFlag
		,	@GrossManualValueFlag
		,	@VcfManualValueFlag
		,	@LookupTransactionStatusIndex
		,	@LookupQualityIndex
		,	@TransactionLineItemGuid
		,	@ProductGuid
		,	@TransactionGuid
		,	@StorageLocationTankGuid
		,	@MeterGuid
		,	@PackageManualValueFlag
		,	@CleanLineItem
		,	@CleanLineDeductItem
		,	@CleanLineDeductQuantity
		,	@CleanLinePackQuantity
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblTransactionSubLineItems]           
		WHERE TransactionSubLineItemGuid=@TransactionSubLineItemGuid;
	
 
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
						+ 'Procedure Name: gsp_TransactionSubLineItemsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
