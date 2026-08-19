CREATE PROCEDURE [dbo].[gsp_ProductsUpdateByPK]
(
		@ProductGuid uniqueidentifier
	,	@ProductID nvarchar(30)=NULL
	,	@Description nvarchar(50)=NULL
	,	@GenericType nvarchar(10)=NULL
	,	@StockResetDate datetimeoffset(7)=NULL
	,	@StockTrack bit=NULL
	,	@DensityHighLimit float=NULL
	,	@DensityLowLimit float=NULL
	,	@DensityDeadband float=NULL
	,	@ApplyDensityLimits bit=NULL
	,	@TemperatureHiHiLimit float=NULL
	,	@TemperatureHighLimit float=NULL
	,	@TemperatureLowLimit float=NULL
	,	@TemperatureLoLoLimit float=NULL
	,	@TemperatureDeadband float=NULL
	,	@ApplyTemperatureLimits bit=NULL
	,	@Bonded bit=NULL
	,	@LowStockWarning float=NULL
	,	@GroundFuel bit=NULL
	,	@ProductCode nvarchar(15)=NULL
	,	@Price money=NULL
	,	@AviationFuelFlag bit=NULL
	,	@LookupMinorCorrectionMethodIndex int=NULL
	,	@CorrectionFactor0 float=NULL
	,	@CorrectionFactor1 float=NULL
	,	@CorrectionFactor2 float=NULL
	,	@CorrectionFactor3 float=NULL
	,	@CorrectionFactor4 float=NULL
	,	@StandardDensity float=NULL
	,	@StandardTemperature float=NULL
	,	@AlternateTemperature float=NULL
	,	@AlternatePressure float=NULL
	,	@ApplyVolumeCorrection bit=NULL
	,	@VolumeUnitIndex int=NULL
	,	@TemperatureUnitIndex int=NULL
	,	@DensityUnitIndex int=NULL
	,	@VolumeDecimalPlaces tinyint=NULL
	,	@TemperatureDecimalPlaces tinyint=NULL
	,	@DensityDecimalPlaces tinyint=NULL
	,	@Capitalize bit=NULL
	,	@OctaneNumber float=NULL
	,	@ReidVaporPressure float=NULL
	,	@HazardousMaterial bit=NULL
	,	@RegulatoryClass int=NULL
	,	@LoadRackDisplayText nvarchar(10)=NULL
	,	@ComponentTolerance float=NULL
	,	@VaporRecovery bit=NULL
	,	@LockedOut bit=NULL
	,	@LockedOutReason nvarchar(80)=NULL
	,	@LockedOutDate datetimeoffset(7)=NULL
	,	@VarianceTolerance float=NULL
	,	@LoadByWeight bit=NULL
	,	@PIDXCode nvarchar(3)=NULL
	,	@ContaminationPromptLoadRackText nvarchar(10)=NULL
	,	@InhibitAccounting bit=NULL
	,	@UserData1 nvarchar(60)=NULL
	,	@UserData2 nvarchar(60)=NULL
	,	@UserData3 nvarchar(60)=NULL
	,	@UserData4 nvarchar(60)=NULL
	,	@UserData5 nvarchar(60)=NULL
	,	@UserData6 nvarchar(60)=NULL
	,	@UserData7 nvarchar(60)=NULL
	,	@UserData8 nvarchar(60)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@MassUnitIndex int=NULL
	,	@LevelUnitIndex int=NULL
	,	@FlowUnitIndex int=NULL
	,	@PressureUnitIndex int=NULL
	,	@MassDecimalPlaces tinyint=NULL
	,	@LevelDecimalPlaces tinyint=NULL
	,	@FlowDecimalPlaces tinyint=NULL
	,	@PressureDecimalPlaces tinyint=NULL
	,	@VolumePackageSize float=NULL
	,	@MassPackageSize float=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LookupProductTypeIndex int=NULL
	,	@LookupMajorCorrectionMethodIndex int=NULL
	,	@TrackingProductGuid uniqueidentifier=NULL
	,	@TaxCode nvarchar(10)=NULL
	,	@_MasterRecordGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
	,	@NullOverrideProductID BIT=0 
	,	@NullOverrideDescription BIT=0 
	,	@NullOverrideGenericType BIT=0 
	,	@NullOverrideStockResetDate BIT=0 
	,	@NullOverrideStockTrack BIT=0 
	,	@NullOverrideDensityHighLimit BIT=0 
	,	@NullOverrideDensityLowLimit BIT=0 
	,	@NullOverrideDensityDeadband BIT=0 
	,	@NullOverrideApplyDensityLimits BIT=0 
	,	@NullOverrideTemperatureHiHiLimit BIT=0 
	,	@NullOverrideTemperatureHighLimit BIT=0 
	,	@NullOverrideTemperatureLowLimit BIT=0 
	,	@NullOverrideTemperatureLoLoLimit BIT=0 
	,	@NullOverrideTemperatureDeadband BIT=0 
	,	@NullOverrideApplyTemperatureLimits BIT=0 
	,	@NullOverrideBonded BIT=0 
	,	@NullOverrideLowStockWarning BIT=0 
	,	@NullOverrideGroundFuel BIT=0 
	,	@NullOverrideProductCode BIT=0 
	,	@NullOverridePrice BIT=0 
	,	@NullOverrideAviationFuelFlag BIT=0 
	,	@NullOverrideMinorCorrectionMethod BIT=0 
	,	@NullOverrideCorrectionFactor0 BIT=0 
	,	@NullOverrideCorrectionFactor1 BIT=0 
	,	@NullOverrideCorrectionFactor2 BIT=0 
	,	@NullOverrideCorrectionFactor3 BIT=0 
	,	@NullOverrideCorrectionFactor4 BIT=0 
	,	@NullOverrideStandardDensity BIT=0 
	,	@NullOverrideStandardTemperature BIT=0 
	,	@NullOverrideAlternateTemperature BIT=0 
	,	@NullOverrideAlternatePressure BIT=0 
	,	@NullOverrideApplyVolumeCorrection BIT=0 
	,	@NullOverrideVolumeUnitIndex BIT=0 
	,	@NullOverrideTemperatureUnitIndex BIT=0 
	,	@NullOverrideDensityUnitIndex BIT=0 
	,	@NullOverrideVolumeDecimalPlaces BIT=0 
	,	@NullOverrideTemperatureDecimalPlaces BIT=0 
	,	@NullOverrideDensityDecimalPlaces BIT=0 
	,	@NullOverrideCapitalize BIT=0 
	,	@NullOverrideOctaneNumber BIT=0 
	,	@NullOverrideReidVaporPressure BIT=0 
	,	@NullOverrideHazardousMaterial BIT=0 
	,	@NullOverrideRegulatoryClass BIT=0 
	,	@NullOverrideLoadRackDisplayText BIT=0 
	,	@NullOverrideComponentTolerance BIT=0 
	,	@NullOverrideVaporRecovery BIT=0 
	,	@NullOverrideLockedOut BIT=0 
	,	@NullOverrideLockedOutReason BIT=0 
	,	@NullOverrideLockedOutDate BIT=0 
	,	@NullOverrideVarianceTolerance BIT=0 
	,	@NullOverrideLoadByWeight BIT=0 
	,	@NullOverridePIDXCode BIT=0 
	,	@NullOverrideContaminationPromptLoadRackText BIT=0 
	,	@NullOverrideInhibitAccounting BIT=0 
	,	@NullOverrideUserData1 BIT=0 
	,	@NullOverrideUserData2 BIT=0 
	,	@NullOverrideUserData3 BIT=0 
	,	@NullOverrideUserData4 BIT=0 
	,	@NullOverrideUserData5 BIT=0 
	,	@NullOverrideUserData6 BIT=0 
	,	@NullOverrideUserData7 BIT=0 
	,	@NullOverrideUserData8 BIT=0 
	,	@NullOverrideUpdatedDate BIT=0 
	,	@NullOverrideMassUnitIndex BIT=0 
	,	@NullOverrideLevelUnitIndex BIT=0 
	,	@NullOverrideFlowUnitIndex BIT=0 
	,	@NullOverridePressureUnitIndex BIT=0 
	,	@NullOverrideMassDecimalPlaces BIT=0 
	,	@NullOverrideLevelDecimalPlaces BIT=0 
	,	@NullOverrideFlowDecimalPlaces BIT=0 
	,	@NullOverridePressureDecimalPlaces BIT=0 
	,	@NullOverrideVolumePackageSize BIT=0 
	,	@NullOverrideMassPackageSize BIT=0 
	,	@NullOverrideSiteGuid BIT=0 
	,	@NullOverrideLookupProductTypeIndex BIT=0 
	,	@NullOverrideLookupMajorCorrectionMethodIndex BIT=0 
	,	@NullOverrideTrackingProductGuid BIT=0 
	,	@NullOverrideTaxCode BIT=0 
	,	@NullOverride_MasterRecordGuid BIT=0 
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ProductsUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.003 / 2014-02-05 16:52:29.6994171 -05:00
	-- Purpose: Update table [dbo].[tblProducts]
	-- Notes:
	-- 1. @ProductGuid and @UpdatedBy are required parameter.
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
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblProducts] WHERE ProductGuid=@ProductGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END
 
		UPDATE [dbo].[tblProducts] SET
			[ProductID]=(CASE ISNULL(@NullOverrideProductID,0) WHEN 1 THEN @ProductID ELSE ISNULL(@ProductID,[ProductID]) END)
		,	[Description]=(CASE ISNULL(@NullOverrideDescription,0) WHEN 1 THEN @Description ELSE ISNULL(@Description,[Description]) END)
		,	[GenericType]=(CASE ISNULL(@NullOverrideGenericType,0) WHEN 1 THEN @GenericType ELSE ISNULL(@GenericType,[GenericType]) END)
		,	[StockResetDate]=(CASE ISNULL(@NullOverrideStockResetDate,0) WHEN 1 THEN @StockResetDate ELSE ISNULL(@StockResetDate,[StockResetDate]) END)
		,	[StockTrack]=(CASE ISNULL(@NullOverrideStockTrack,0) WHEN 1 THEN @StockTrack ELSE ISNULL(@StockTrack,[StockTrack]) END)
		,	[DensityHighLimit]=(CASE ISNULL(@NullOverrideDensityHighLimit,0) WHEN 1 THEN @DensityHighLimit ELSE ISNULL(@DensityHighLimit,[DensityHighLimit]) END)
		,	[DensityLowLimit]=(CASE ISNULL(@NullOverrideDensityLowLimit,0) WHEN 1 THEN @DensityLowLimit ELSE ISNULL(@DensityLowLimit,[DensityLowLimit]) END)
		,	[DensityDeadband]=(CASE ISNULL(@NullOverrideDensityDeadband,0) WHEN 1 THEN @DensityDeadband ELSE ISNULL(@DensityDeadband,[DensityDeadband]) END)
		,	[ApplyDensityLimits]=(CASE ISNULL(@NullOverrideApplyDensityLimits,0) WHEN 1 THEN @ApplyDensityLimits ELSE ISNULL(@ApplyDensityLimits,[ApplyDensityLimits]) END)
		,	[TemperatureHiHiLimit]=(CASE ISNULL(@NullOverrideTemperatureHiHiLimit,0) WHEN 1 THEN @TemperatureHiHiLimit ELSE ISNULL(@TemperatureHiHiLimit,[TemperatureHiHiLimit]) END)
		,	[TemperatureHighLimit]=(CASE ISNULL(@NullOverrideTemperatureHighLimit,0) WHEN 1 THEN @TemperatureHighLimit ELSE ISNULL(@TemperatureHighLimit,[TemperatureHighLimit]) END)
		,	[TemperatureLowLimit]=(CASE ISNULL(@NullOverrideTemperatureLowLimit,0) WHEN 1 THEN @TemperatureLowLimit ELSE ISNULL(@TemperatureLowLimit,[TemperatureLowLimit]) END)
		,	[TemperatureLoLoLimit]=(CASE ISNULL(@NullOverrideTemperatureLoLoLimit,0) WHEN 1 THEN @TemperatureLoLoLimit ELSE ISNULL(@TemperatureLoLoLimit,[TemperatureLoLoLimit]) END)
		,	[TemperatureDeadband]=(CASE ISNULL(@NullOverrideTemperatureDeadband,0) WHEN 1 THEN @TemperatureDeadband ELSE ISNULL(@TemperatureDeadband,[TemperatureDeadband]) END)
		,	[ApplyTemperatureLimits]=(CASE ISNULL(@NullOverrideApplyTemperatureLimits,0) WHEN 1 THEN @ApplyTemperatureLimits ELSE ISNULL(@ApplyTemperatureLimits,[ApplyTemperatureLimits]) END)
		,	[Bonded]=(CASE ISNULL(@NullOverrideBonded,0) WHEN 1 THEN @Bonded ELSE ISNULL(@Bonded,[Bonded]) END)
		,	[LowStockWarning]=(CASE ISNULL(@NullOverrideLowStockWarning,0) WHEN 1 THEN @LowStockWarning ELSE ISNULL(@LowStockWarning,[LowStockWarning]) END)
		,	[GroundFuel]=(CASE ISNULL(@NullOverrideGroundFuel,0) WHEN 1 THEN @GroundFuel ELSE ISNULL(@GroundFuel,[GroundFuel]) END)
		,	[ProductCode]=(CASE ISNULL(@NullOverrideProductCode,0) WHEN 1 THEN @ProductCode ELSE ISNULL(@ProductCode,[ProductCode]) END)
		,	[Price]=(CASE ISNULL(@NullOverridePrice,0) WHEN 1 THEN @Price ELSE ISNULL(@Price,[Price]) END)
		,	[AviationFuelFlag]=(CASE ISNULL(@NullOverrideAviationFuelFlag,0) WHEN 1 THEN @AviationFuelFlag ELSE ISNULL(@AviationFuelFlag,[AviationFuelFlag]) END)
		,	[LookupMinorCorrectionMethodIndex]=(CASE ISNULL(@NullOverrideMinorCorrectionMethod,0) WHEN 1 THEN @LookupMinorCorrectionMethodIndex ELSE ISNULL(@LookupMinorCorrectionMethodIndex,[LookupMinorCorrectionMethodIndex]) END)
		,	[CorrectionFactor0]=(CASE ISNULL(@NullOverrideCorrectionFactor0,0) WHEN 1 THEN @CorrectionFactor0 ELSE ISNULL(@CorrectionFactor0,[CorrectionFactor0]) END)
		,	[CorrectionFactor1]=(CASE ISNULL(@NullOverrideCorrectionFactor1,0) WHEN 1 THEN @CorrectionFactor1 ELSE ISNULL(@CorrectionFactor1,[CorrectionFactor1]) END)
		,	[CorrectionFactor2]=(CASE ISNULL(@NullOverrideCorrectionFactor2,0) WHEN 1 THEN @CorrectionFactor2 ELSE ISNULL(@CorrectionFactor2,[CorrectionFactor2]) END)
		,	[CorrectionFactor3]=(CASE ISNULL(@NullOverrideCorrectionFactor3,0) WHEN 1 THEN @CorrectionFactor3 ELSE ISNULL(@CorrectionFactor3,[CorrectionFactor3]) END)
		,	[CorrectionFactor4]=(CASE ISNULL(@NullOverrideCorrectionFactor4,0) WHEN 1 THEN @CorrectionFactor4 ELSE ISNULL(@CorrectionFactor4,[CorrectionFactor4]) END)
		,	[StandardDensity]=(CASE ISNULL(@NullOverrideStandardDensity,0) WHEN 1 THEN @StandardDensity ELSE ISNULL(@StandardDensity,[StandardDensity]) END)
		,	[StandardTemperature]=(CASE ISNULL(@NullOverrideStandardTemperature,0) WHEN 1 THEN @StandardTemperature ELSE ISNULL(@StandardTemperature,[StandardTemperature]) END)
		,	[AlternateTemperature]=(CASE ISNULL(@NullOverrideAlternateTemperature,0) WHEN 1 THEN @AlternateTemperature ELSE ISNULL(@AlternateTemperature,[AlternateTemperature]) END)
		,	[AlternatePressure]=(CASE ISNULL(@NullOverrideAlternatePressure,0) WHEN 1 THEN @AlternatePressure ELSE ISNULL(@AlternatePressure,[AlternatePressure]) END)
		,	[ApplyVolumeCorrection]=(CASE ISNULL(@NullOverrideApplyVolumeCorrection,0) WHEN 1 THEN @ApplyVolumeCorrection ELSE ISNULL(@ApplyVolumeCorrection,[ApplyVolumeCorrection]) END)
		,	[VolumeUnitIndex]=(CASE ISNULL(@NullOverrideVolumeUnitIndex,0) WHEN 1 THEN @VolumeUnitIndex ELSE ISNULL(@VolumeUnitIndex,[VolumeUnitIndex]) END)
		,	[TemperatureUnitIndex]=(CASE ISNULL(@NullOverrideTemperatureUnitIndex,0) WHEN 1 THEN @TemperatureUnitIndex ELSE ISNULL(@TemperatureUnitIndex,[TemperatureUnitIndex]) END)
		,	[DensityUnitIndex]=(CASE ISNULL(@NullOverrideDensityUnitIndex,0) WHEN 1 THEN @DensityUnitIndex ELSE ISNULL(@DensityUnitIndex,[DensityUnitIndex]) END)
		,	[VolumeDecimalPlaces]=(CASE ISNULL(@NullOverrideVolumeDecimalPlaces,0) WHEN 1 THEN @VolumeDecimalPlaces ELSE ISNULL(@VolumeDecimalPlaces,[VolumeDecimalPlaces]) END)
		,	[TemperatureDecimalPlaces]=(CASE ISNULL(@NullOverrideTemperatureDecimalPlaces,0) WHEN 1 THEN @TemperatureDecimalPlaces ELSE ISNULL(@TemperatureDecimalPlaces,[TemperatureDecimalPlaces]) END)
		,	[DensityDecimalPlaces]=(CASE ISNULL(@NullOverrideDensityDecimalPlaces,0) WHEN 1 THEN @DensityDecimalPlaces ELSE ISNULL(@DensityDecimalPlaces,[DensityDecimalPlaces]) END)
		,	[Capitalize]=(CASE ISNULL(@NullOverrideCapitalize,0) WHEN 1 THEN @Capitalize ELSE ISNULL(@Capitalize,[Capitalize]) END)
		,	[OctaneNumber]=(CASE ISNULL(@NullOverrideOctaneNumber,0) WHEN 1 THEN @OctaneNumber ELSE ISNULL(@OctaneNumber,[OctaneNumber]) END)
		,	[ReidVaporPressure]=(CASE ISNULL(@NullOverrideReidVaporPressure,0) WHEN 1 THEN @ReidVaporPressure ELSE ISNULL(@ReidVaporPressure,[ReidVaporPressure]) END)
		,	[HazardousMaterial]=(CASE ISNULL(@NullOverrideHazardousMaterial,0) WHEN 1 THEN @HazardousMaterial ELSE ISNULL(@HazardousMaterial,[HazardousMaterial]) END)
		,	[RegulatoryClass]=(CASE ISNULL(@NullOverrideRegulatoryClass,0) WHEN 1 THEN @RegulatoryClass ELSE ISNULL(@RegulatoryClass,[RegulatoryClass]) END)
		,	[LoadRackDisplayText]=(CASE ISNULL(@NullOverrideLoadRackDisplayText,0) WHEN 1 THEN @LoadRackDisplayText ELSE ISNULL(@LoadRackDisplayText,[LoadRackDisplayText]) END)
		,	[ComponentTolerance]=(CASE ISNULL(@NullOverrideComponentTolerance,0) WHEN 1 THEN @ComponentTolerance ELSE ISNULL(@ComponentTolerance,[ComponentTolerance]) END)
		,	[VaporRecovery]=(CASE ISNULL(@NullOverrideVaporRecovery,0) WHEN 1 THEN @VaporRecovery ELSE ISNULL(@VaporRecovery,[VaporRecovery]) END)
		,	[LockedOut]=(CASE ISNULL(@NullOverrideLockedOut,0) WHEN 1 THEN @LockedOut ELSE ISNULL(@LockedOut,[LockedOut]) END)
		,	[LockedOutReason]=(CASE ISNULL(@NullOverrideLockedOutReason,0) WHEN 1 THEN @LockedOutReason ELSE ISNULL(@LockedOutReason,[LockedOutReason]) END)
		,	[LockedOutDate]=(CASE ISNULL(@NullOverrideLockedOutDate,0) WHEN 1 THEN @LockedOutDate ELSE ISNULL(@LockedOutDate,[LockedOutDate]) END)
		,	[VarianceTolerance]=(CASE ISNULL(@NullOverrideVarianceTolerance,0) WHEN 1 THEN @VarianceTolerance ELSE ISNULL(@VarianceTolerance,[VarianceTolerance]) END)
		,	[LoadByWeight]=(CASE ISNULL(@NullOverrideLoadByWeight,0) WHEN 1 THEN @LoadByWeight ELSE ISNULL(@LoadByWeight,[LoadByWeight]) END)
		,	[PIDXCode]=(CASE ISNULL(@NullOverridePIDXCode,0) WHEN 1 THEN @PIDXCode ELSE ISNULL(@PIDXCode,[PIDXCode]) END)
		,	[ContaminationPromptLoadRackText]=(CASE ISNULL(@NullOverrideContaminationPromptLoadRackText,0) WHEN 1 THEN @ContaminationPromptLoadRackText ELSE ISNULL(@ContaminationPromptLoadRackText,[ContaminationPromptLoadRackText]) END)
		,	[InhibitAccounting]=(CASE ISNULL(@NullOverrideInhibitAccounting,0) WHEN 1 THEN @InhibitAccounting ELSE ISNULL(@InhibitAccounting,[InhibitAccounting]) END)
		,	[UserData1]=(CASE ISNULL(@NullOverrideUserData1,0) WHEN 1 THEN @UserData1 ELSE ISNULL(@UserData1,[UserData1]) END)
		,	[UserData2]=(CASE ISNULL(@NullOverrideUserData2,0) WHEN 1 THEN @UserData2 ELSE ISNULL(@UserData2,[UserData2]) END)
		,	[UserData3]=(CASE ISNULL(@NullOverrideUserData3,0) WHEN 1 THEN @UserData3 ELSE ISNULL(@UserData3,[UserData3]) END)
		,	[UserData4]=(CASE ISNULL(@NullOverrideUserData4,0) WHEN 1 THEN @UserData4 ELSE ISNULL(@UserData4,[UserData4]) END)
		,	[UserData5]=(CASE ISNULL(@NullOverrideUserData5,0) WHEN 1 THEN @UserData5 ELSE ISNULL(@UserData5,[UserData5]) END)
		,	[UserData6]=(CASE ISNULL(@NullOverrideUserData6,0) WHEN 1 THEN @UserData6 ELSE ISNULL(@UserData6,[UserData6]) END)
		,	[UserData7]=(CASE ISNULL(@NullOverrideUserData7,0) WHEN 1 THEN @UserData7 ELSE ISNULL(@UserData7,[UserData7]) END)
		,	[UserData8]=(CASE ISNULL(@NullOverrideUserData8,0) WHEN 1 THEN @UserData8 ELSE ISNULL(@UserData8,[UserData8]) END)
		,	[UpdatedDate]=ISNULL(@UpdatedDate,SYSDATETIMEOFFSET())
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		,	[MassUnitIndex]=(CASE ISNULL(@NullOverrideMassUnitIndex,0) WHEN 1 THEN @MassUnitIndex ELSE ISNULL(@MassUnitIndex,[MassUnitIndex]) END)
		,	[LevelUnitIndex]=(CASE ISNULL(@NullOverrideLevelUnitIndex,0) WHEN 1 THEN @LevelUnitIndex ELSE ISNULL(@LevelUnitIndex,[LevelUnitIndex]) END)
		,	[FlowUnitIndex]=(CASE ISNULL(@NullOverrideFlowUnitIndex,0) WHEN 1 THEN @FlowUnitIndex ELSE ISNULL(@FlowUnitIndex,[FlowUnitIndex]) END)
		,	[PressureUnitIndex]=(CASE ISNULL(@NullOverridePressureUnitIndex,0) WHEN 1 THEN @PressureUnitIndex ELSE ISNULL(@PressureUnitIndex,[PressureUnitIndex]) END)
		,	[MassDecimalPlaces]=(CASE ISNULL(@NullOverrideMassDecimalPlaces,0) WHEN 1 THEN @MassDecimalPlaces ELSE ISNULL(@MassDecimalPlaces,[MassDecimalPlaces]) END)
		,	[LevelDecimalPlaces]=(CASE ISNULL(@NullOverrideLevelDecimalPlaces,0) WHEN 1 THEN @LevelDecimalPlaces ELSE ISNULL(@LevelDecimalPlaces,[LevelDecimalPlaces]) END)
		,	[FlowDecimalPlaces]=(CASE ISNULL(@NullOverrideFlowDecimalPlaces,0) WHEN 1 THEN @FlowDecimalPlaces ELSE ISNULL(@FlowDecimalPlaces,[FlowDecimalPlaces]) END)
		,	[PressureDecimalPlaces]=(CASE ISNULL(@NullOverridePressureDecimalPlaces,0) WHEN 1 THEN @PressureDecimalPlaces ELSE ISNULL(@PressureDecimalPlaces,[PressureDecimalPlaces]) END)
		,	[VolumePackageSize]=(CASE ISNULL(@NullOverrideVolumePackageSize,0) WHEN 1 THEN @VolumePackageSize ELSE ISNULL(@VolumePackageSize,[VolumePackageSize]) END)
		,	[MassPackageSize]=(CASE ISNULL(@NullOverrideMassPackageSize,0) WHEN 1 THEN @MassPackageSize ELSE ISNULL(@MassPackageSize,[MassPackageSize]) END)
		,	[SiteGuid]=(CASE ISNULL(@NullOverrideSiteGuid,0) WHEN 1 THEN @SiteGuid ELSE ISNULL(@SiteGuid,[SiteGuid]) END)
		,	[LookupProductTypeIndex]=(CASE ISNULL(@NullOverrideLookupProductTypeIndex,0) WHEN 1 THEN @LookupProductTypeIndex ELSE ISNULL(@LookupProductTypeIndex,[LookupProductTypeIndex]) END)
		,	[LookupMajorCorrectionMethodIndex]=(CASE ISNULL(@NullOverrideLookupMajorCorrectionMethodIndex,0) WHEN 1 THEN @LookupMajorCorrectionMethodIndex ELSE ISNULL(@LookupMajorCorrectionMethodIndex,[LookupMajorCorrectionMethodIndex]) END)
		,	[TrackingProductGuid]=(CASE ISNULL(@NullOverrideTrackingProductGuid,0) WHEN 1 THEN @TrackingProductGuid ELSE ISNULL(@TrackingProductGuid,[TrackingProductGuid]) END)
		,	[TaxCode]=(CASE ISNULL(@NullOverrideTaxCode,0) WHEN 1 THEN @TaxCode ELSE ISNULL(@TaxCode,[TaxCode]) END)
		,	[_MasterRecordGuid]=(CASE ISNULL(@NullOverride_MasterRecordGuid,0) WHEN 1 THEN @_MasterRecordGuid ELSE ISNULL(@_MasterRecordGuid,[_MasterRecordGuid]) END)
		WHERE	ProductGuid=@ProductGuid;
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblProducts]           
		WHERE ProductGuid=@ProductGuid;
	
 
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
						+ 'Procedure Name: gsp_ProductsUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
