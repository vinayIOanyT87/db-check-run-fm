CREATE PROCEDURE [dbo].[gsp_TransactionSubLineItemsUpdateByPK]
(
		@TransactionSubLineItemGuid uniqueidentifier
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
	,	@NullOverrideSequenceID BIT=0 
	,	@NullOverrideProduct BIT=0 
	,	@NullOverrideProductCode BIT=0 
	,	@NullOverrideProductType BIT=0 
	,	@NullOverrideGrossQuantity BIT=0 
	,	@NullOverrideNetQuantity BIT=0 
	,	@NullOverrideVcf BIT=0 
	,	@NullOverrideDensity BIT=0 
	,	@NullOverrideTemperature BIT=0 
	,	@NullOverrideCustoms BIT=0 
	,	@NullOverrideArmNumber BIT=0 
	,	@NullOverrideLineNumber BIT=0 
	,	@NullOverrideBatchNumber BIT=0 
	,	@NullOverrideLineFill BIT=0 
	,	@NullOverrideBottomVolume BIT=0 
	,	@NullOverrideNetCapacity BIT=0 
	,	@NullOverrideTankStatus BIT=0 
	,	@NullOverrideMeterFactor BIT=0 
	,	@NullOverrideMeterStart BIT=0 
	,	@NullOverrideMeterStop BIT=0 
	,	@NullOverrideMeterStopDateTime BIT=0 
	,	@NullOverrideMeterStartDateTime BIT=0 
	,	@NullOverrideFreezePoint BIT=0 
	,	@NullOverrideDifferentialPressure BIT=0 
	,	@NullOverrideDosageRate BIT=0 
	,	@NullOverrideDeleteFlag BIT=0 
	,	@NullOverridePresetAmount BIT=0 
	,	@NullOverrideStorageLocationID BIT=0 
	,	@NullOverrideMeterID BIT=0 
	,	@NullOverrideCOAID BIT=0 
	,	@NullOverrideUpdatedDate BIT=0 
	,	@NullOverrideTransactionInventoryDate BIT=0 
	,	@NullOverrideTax1 BIT=0 
	,	@NullOverrideTax2 BIT=0 
	,	@NullOverrideTax3 BIT=0 
	,	@NullOverrideTax4 BIT=0 
	,	@NullOverrideTax5 BIT=0 
	,	@NullOverrideTransVersion BIT=0 
	,	@NullOverrideImproperAdditization BIT=0 
	,	@NullOverrideBrokenBlend BIT=0 
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
	,	@NullOverrideDate01 BIT=0 
	,	@NullOverrideDate02 BIT=0 
	,	@NullOverrideDate03 BIT=0 
	,	@NullOverrideDate04 BIT=0 
	,	@NullOverrideMassQuantity BIT=0 
	,	@NullOverrideNetManualValueFlag BIT=0 
	,	@NullOverrideMassManualValueFlag BIT=0 
	,	@NullOverrideGrossManualValueFlag BIT=0 
	,	@NullOverrideVcfManualValueFlag BIT=0 
	,	@NullOverrideLookupTransactionStatusIndex BIT=0 
	,	@NullOverrideLookupQualityIndex BIT=0 
	,	@NullOverrideTransactionLineItemGuid BIT=0 
	,	@NullOverrideProductGuid BIT=0 
	,	@NullOverrideTransactionGuid BIT=0 
	,	@NullOverrideStorageLocationTankGuid BIT=0 
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
	-- Stored procedure: [dbo].[gsp_TransactionSubLineItemsUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.003 / 2014-02-05 16:52:29.9064378 -05:00
	-- Purpose: Update table [dbo].[tblTransactionSubLineItems]
	-- Notes:
	-- 1. @TransactionSubLineItemGuid and @UpdatedBy are required parameter.
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
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblTransactionSubLineItems] WHERE TransactionSubLineItemGuid=@TransactionSubLineItemGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END
 
		UPDATE [dbo].[tblTransactionSubLineItems] SET
			[SequenceID]=(CASE ISNULL(@NullOverrideSequenceID,0) WHEN 1 THEN @SequenceID ELSE ISNULL(@SequenceID,[SequenceID]) END)
		,	[Product]=(CASE ISNULL(@NullOverrideProduct,0) WHEN 1 THEN @Product ELSE ISNULL(@Product,[Product]) END)
		,	[ProductCode]=(CASE ISNULL(@NullOverrideProductCode,0) WHEN 1 THEN @ProductCode ELSE ISNULL(@ProductCode,[ProductCode]) END)
		,	[ProductType]=(CASE ISNULL(@NullOverrideProductType,0) WHEN 1 THEN @ProductType ELSE ISNULL(@ProductType,[ProductType]) END)
		,	[GrossQuantity]=(CASE ISNULL(@NullOverrideGrossQuantity,0) WHEN 1 THEN @GrossQuantity ELSE ISNULL(@GrossQuantity,[GrossQuantity]) END)
		,	[NetQuantity]=(CASE ISNULL(@NullOverrideNetQuantity,0) WHEN 1 THEN @NetQuantity ELSE ISNULL(@NetQuantity,[NetQuantity]) END)
		,	[Vcf]=(CASE ISNULL(@NullOverrideVcf,0) WHEN 1 THEN @Vcf ELSE ISNULL(@Vcf,[Vcf]) END)
		,	[Density]=(CASE ISNULL(@NullOverrideDensity,0) WHEN 1 THEN @Density ELSE ISNULL(@Density,[Density]) END)
		,	[Temperature]=(CASE ISNULL(@NullOverrideTemperature,0) WHEN 1 THEN @Temperature ELSE ISNULL(@Temperature,[Temperature]) END)
		,	[Customs]=(CASE ISNULL(@NullOverrideCustoms,0) WHEN 1 THEN @Customs ELSE ISNULL(@Customs,[Customs]) END)
		,	[ArmNumber]=(CASE ISNULL(@NullOverrideArmNumber,0) WHEN 1 THEN @ArmNumber ELSE ISNULL(@ArmNumber,[ArmNumber]) END)
		,	[LineNumber]=(CASE ISNULL(@NullOverrideLineNumber,0) WHEN 1 THEN @LineNumber ELSE ISNULL(@LineNumber,[LineNumber]) END)
		,	[BatchNumber]=(CASE ISNULL(@NullOverrideBatchNumber,0) WHEN 1 THEN @BatchNumber ELSE ISNULL(@BatchNumber,[BatchNumber]) END)
		,	[LineFill]=(CASE ISNULL(@NullOverrideLineFill,0) WHEN 1 THEN @LineFill ELSE ISNULL(@LineFill,[LineFill]) END)
		,	[BottomVolume]=(CASE ISNULL(@NullOverrideBottomVolume,0) WHEN 1 THEN @BottomVolume ELSE ISNULL(@BottomVolume,[BottomVolume]) END)
		,	[NetCapacity]=(CASE ISNULL(@NullOverrideNetCapacity,0) WHEN 1 THEN @NetCapacity ELSE ISNULL(@NetCapacity,[NetCapacity]) END)
		,	[TankStatus]=(CASE ISNULL(@NullOverrideTankStatus,0) WHEN 1 THEN @TankStatus ELSE ISNULL(@TankStatus,[TankStatus]) END)
		,	[MeterFactor]=(CASE ISNULL(@NullOverrideMeterFactor,0) WHEN 1 THEN @MeterFactor ELSE ISNULL(@MeterFactor,[MeterFactor]) END)
		,	[MeterStart]=(CASE ISNULL(@NullOverrideMeterStart,0) WHEN 1 THEN @MeterStart ELSE ISNULL(@MeterStart,[MeterStart]) END)
		,	[MeterStop]=(CASE ISNULL(@NullOverrideMeterStop,0) WHEN 1 THEN @MeterStop ELSE ISNULL(@MeterStop,[MeterStop]) END)
		,	[MeterStopDateTime]=(CASE ISNULL(@NullOverrideMeterStopDateTime,0) WHEN 1 THEN @MeterStopDateTime ELSE ISNULL(@MeterStopDateTime,[MeterStopDateTime]) END)
		,	[MeterStartDateTime]=(CASE ISNULL(@NullOverrideMeterStartDateTime,0) WHEN 1 THEN @MeterStartDateTime ELSE ISNULL(@MeterStartDateTime,[MeterStartDateTime]) END)
		,	[FreezePoint]=(CASE ISNULL(@NullOverrideFreezePoint,0) WHEN 1 THEN @FreezePoint ELSE ISNULL(@FreezePoint,[FreezePoint]) END)
		,	[DifferentialPressure]=(CASE ISNULL(@NullOverrideDifferentialPressure,0) WHEN 1 THEN @DifferentialPressure ELSE ISNULL(@DifferentialPressure,[DifferentialPressure]) END)
		,	[DosageRate]=(CASE ISNULL(@NullOverrideDosageRate,0) WHEN 1 THEN @DosageRate ELSE ISNULL(@DosageRate,[DosageRate]) END)
		,	[DeleteFlag]=(CASE ISNULL(@NullOverrideDeleteFlag,0) WHEN 1 THEN @DeleteFlag ELSE ISNULL(@DeleteFlag,[DeleteFlag]) END)
		,	[PresetAmount]=(CASE ISNULL(@NullOverridePresetAmount,0) WHEN 1 THEN @PresetAmount ELSE ISNULL(@PresetAmount,[PresetAmount]) END)
		,	[StorageLocationID]=(CASE ISNULL(@NullOverrideStorageLocationID,0) WHEN 1 THEN @StorageLocationID ELSE ISNULL(@StorageLocationID,[StorageLocationID]) END)
		,	[MeterID]=(CASE ISNULL(@NullOverrideMeterID,0) WHEN 1 THEN @MeterID ELSE ISNULL(@MeterID,[MeterID]) END)
		,	[COAID]=(CASE ISNULL(@NullOverrideCOAID,0) WHEN 1 THEN @COAID ELSE ISNULL(@COAID,[COAID]) END)
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		,	[UpdatedDate]=ISNULL(@UpdatedDate,SYSDATETIMEOFFSET())
		,	[TransactionInventoryDate]=(CASE ISNULL(@NullOverrideTransactionInventoryDate,0) WHEN 1 THEN @TransactionInventoryDate ELSE ISNULL(@TransactionInventoryDate,[TransactionInventoryDate]) END)
		,	[Tax1]=(CASE ISNULL(@NullOverrideTax1,0) WHEN 1 THEN @Tax1 ELSE ISNULL(@Tax1,[Tax1]) END)
		,	[Tax2]=(CASE ISNULL(@NullOverrideTax2,0) WHEN 1 THEN @Tax2 ELSE ISNULL(@Tax2,[Tax2]) END)
		,	[Tax3]=(CASE ISNULL(@NullOverrideTax3,0) WHEN 1 THEN @Tax3 ELSE ISNULL(@Tax3,[Tax3]) END)
		,	[Tax4]=(CASE ISNULL(@NullOverrideTax4,0) WHEN 1 THEN @Tax4 ELSE ISNULL(@Tax4,[Tax4]) END)
		,	[Tax5]=(CASE ISNULL(@NullOverrideTax5,0) WHEN 1 THEN @Tax5 ELSE ISNULL(@Tax5,[Tax5]) END)
		,	[TransVersion]=(CASE ISNULL(@NullOverrideTransVersion,0) WHEN 1 THEN @TransVersion ELSE ISNULL(@TransVersion,[TransVersion]) END)
		,	[ImproperAdditization]=(CASE ISNULL(@NullOverrideImproperAdditization,0) WHEN 1 THEN @ImproperAdditization ELSE ISNULL(@ImproperAdditization,[ImproperAdditization]) END)
		,	[BrokenBlend]=(CASE ISNULL(@NullOverrideBrokenBlend,0) WHEN 1 THEN @BrokenBlend ELSE ISNULL(@BrokenBlend,[BrokenBlend]) END)
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
		,	[Date01]=(CASE ISNULL(@NullOverrideDate01,0) WHEN 1 THEN @Date01 ELSE ISNULL(@Date01,[Date01]) END)
		,	[Date02]=(CASE ISNULL(@NullOverrideDate02,0) WHEN 1 THEN @Date02 ELSE ISNULL(@Date02,[Date02]) END)
		,	[Date03]=(CASE ISNULL(@NullOverrideDate03,0) WHEN 1 THEN @Date03 ELSE ISNULL(@Date03,[Date03]) END)
		,	[Date04]=(CASE ISNULL(@NullOverrideDate04,0) WHEN 1 THEN @Date04 ELSE ISNULL(@Date04,[Date04]) END)
		,	[MassQuantity]=(CASE ISNULL(@NullOverrideMassQuantity,0) WHEN 1 THEN @MassQuantity ELSE ISNULL(@MassQuantity,[MassQuantity]) END)
		,	[NetManualValueFlag]=(CASE ISNULL(@NullOverrideNetManualValueFlag,0) WHEN 1 THEN @NetManualValueFlag ELSE ISNULL(@NetManualValueFlag,[NetManualValueFlag]) END)
		,	[MassManualValueFlag]=(CASE ISNULL(@NullOverrideMassManualValueFlag,0) WHEN 1 THEN @MassManualValueFlag ELSE ISNULL(@MassManualValueFlag,[MassManualValueFlag]) END)
		,	[GrossManualValueFlag]=(CASE ISNULL(@NullOverrideGrossManualValueFlag,0) WHEN 1 THEN @GrossManualValueFlag ELSE ISNULL(@GrossManualValueFlag,[GrossManualValueFlag]) END)
		,	[VcfManualValueFlag]=(CASE ISNULL(@NullOverrideVcfManualValueFlag,0) WHEN 1 THEN @VcfManualValueFlag ELSE ISNULL(@VcfManualValueFlag,[VcfManualValueFlag]) END)
		,	[LookupTransactionStatusIndex]=(CASE ISNULL(@NullOverrideLookupTransactionStatusIndex,0) WHEN 1 THEN @LookupTransactionStatusIndex ELSE ISNULL(@LookupTransactionStatusIndex,[LookupTransactionStatusIndex]) END)
		,	[LookupQualityIndex]=(CASE ISNULL(@NullOverrideLookupQualityIndex,0) WHEN 1 THEN @LookupQualityIndex ELSE ISNULL(@LookupQualityIndex,[LookupQualityIndex]) END)
		,	[TransactionLineItemGuid]=(CASE ISNULL(@NullOverrideTransactionLineItemGuid,0) WHEN 1 THEN @TransactionLineItemGuid ELSE ISNULL(@TransactionLineItemGuid,[TransactionLineItemGuid]) END)
		,	[ProductGuid]=(CASE ISNULL(@NullOverrideProductGuid,0) WHEN 1 THEN @ProductGuid ELSE ISNULL(@ProductGuid,[ProductGuid]) END)
		,	[TransactionGuid]=(CASE ISNULL(@NullOverrideTransactionGuid,0) WHEN 1 THEN @TransactionGuid ELSE ISNULL(@TransactionGuid,[TransactionGuid]) END)
		,	[StorageLocationTankGuid]=(CASE ISNULL(@NullOverrideStorageLocationTankGuid,0) WHEN 1 THEN @StorageLocationTankGuid ELSE ISNULL(@StorageLocationTankGuid,[StorageLocationTankGuid]) END)
		,	[MeterGuid]=(CASE ISNULL(@NullOverrideMeterGuid,0) WHEN 1 THEN @MeterGuid ELSE ISNULL(@MeterGuid,[MeterGuid]) END)
		,	[PackageManualValueFlag]=(CASE ISNULL(@NullOverridePackageManualValueFlag,0) WHEN 1 THEN @PackageManualValueFlag ELSE ISNULL(@PackageManualValueFlag,[PackageManualValueFlag]) END)
		,	[CleanLineItem]=(CASE ISNULL(@NullOverrideCleanLineItem,0) WHEN 1 THEN @CleanLineItem ELSE ISNULL(@CleanLineItem,[CleanLineItem]) END)
		,	[CleanLineDeductItem]=(CASE ISNULL(@NullOverrideCleanLineDeductItem,0) WHEN 1 THEN @CleanLineDeductItem ELSE ISNULL(@CleanLineDeductItem,[CleanLineDeductItem]) END)
		,	[CleanLineDeductQuantity]=(CASE ISNULL(@NullOverrideCleanLineDeductQuantity,0) WHEN 1 THEN @CleanLineDeductQuantity ELSE ISNULL(@CleanLineDeductQuantity,[CleanLineDeductQuantity]) END)
		,	[CleanLinePackQuantity]=(CASE ISNULL(@NullOverrideCleanLinePackQuantity,0) WHEN 1 THEN @CleanLinePackQuantity ELSE ISNULL(@CleanLinePackQuantity,[CleanLinePackQuantity]) END)
		WHERE	TransactionSubLineItemGuid=@TransactionSubLineItemGuid;
 
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
						+ 'Procedure Name: gsp_TransactionSubLineItemsUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
