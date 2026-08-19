CREATE PROCEDURE [dbo].[gsp_EquipmentUpdateByPK]
(
		@EquipmentGuid uniqueidentifier
	,	@ID nvarchar(30)=NULL
	,	@Description nvarchar(50)=NULL
	,	@Make nvarchar(20)=NULL
	,	@Model nvarchar(50)=NULL
	,	@Year int=NULL
	,	@IssPtNum nvarchar(20)=NULL
	,	@Fixed bit=NULL
	,	@StorageType nvarchar(2)=NULL
	,	@InUse bit=NULL
	,	@FixedVolume bit=NULL
	,	@IntoPlane bit=NULL
	,	@Mobile bit=NULL
	,	@AttachedTo nvarchar(6)=NULL
	,	@MediaType char=NULL
	,	@Meters int=NULL
	,	@DefuelMeterForwards bit=NULL
	,	@PulseRatio float=NULL
	,	@Round bit=NULL
	,	@Xref nvarchar(10)=NULL
	,	@LowStockWarning float=NULL
	,	@StockTrack bit=NULL
	,	@Totalisor1 nvarchar(10)=NULL
	,	@Totalisor2 nvarchar(10)=NULL
	,	@FuelingState nvarchar(10)=NULL
	,	@Volume float=NULL
	,	@MeterReading float=NULL
	,	@Consecutive_OOS_Variance int=NULL
	,	@Notes nvarchar(1000)=NULL
	,	@Capacity float=NULL
	,	@SafeFill float=NULL
	,	@VolumeUnitIndex int=NULL
	,	@TemperatureUnitIndex int=NULL
	,	@DensityUnitIndex int=NULL
	,	@MassUnitIndex int=NULL
	,	@VolumeDecimalPlaces tinyint=NULL
	,	@TemperatureDecimalPlaces tinyint=NULL
	,	@DensityDecimalPlaces tinyint=NULL
	,	@MassDecimalPlaces tinyint=NULL
	,	@EquipmentSequence nvarchar(50)=NULL
	,	@LockedOut bit=NULL
	,	@LockedOutReason nvarchar(80)=NULL
	,	@LockedOutDate datetimeoffset(7)=NULL
	,	@SerialNumber nvarchar(30)=NULL
	,	@CompanyEquipmentID nvarchar(30)=NULL
	,	@TruckCardNumber nvarchar(32)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@RatedGPM float=NULL
	,	@ActualGPM float=NULL
	,	@FuelAdditiveFlag bit=NULL
	,	@ManufactureDate datetimeoffset(7)=NULL
	,	@InstallationDate datetimeoffset(7)=NULL
	,	@InspectionDate datetimeoffset(7)=NULL
	,	@CalibrationDate datetimeoffset(7)=NULL
	,	@QCDate datetimeoffset(7)=NULL
	,	@SecondaryStorageFlag bit=NULL
	,	@ManagedEquipmentFlag bit=NULL
	,	@FuelingType smallint=NULL
	,	@UserData1 nvarchar(60)=NULL
	,	@UserData2 nvarchar(60)=NULL
	,	@UserData3 nvarchar(60)=NULL
	,	@UserData4 nvarchar(60)=NULL
	,	@UserData5 nvarchar(60)=NULL
	,	@UserData6 nvarchar(60)=NULL
	,	@UserData7 nvarchar(60)=NULL
	,	@UserData8 nvarchar(60)=NULL
	,	@UserData9 nvarchar(60)=NULL
	,	@UserData10 nvarchar(60)=NULL
	,	@UserData11 nvarchar(60)=NULL
	,	@UserData12 nvarchar(60)=NULL
	,	@UserData13 nvarchar(60)=NULL
	,	@UserData14 nvarchar(60)=NULL
	,	@UserData15 nvarchar(60)=NULL
	,	@UserData16 nvarchar(60)=NULL
	,	@UserData17 nvarchar(60)=NULL
	,	@UserData18 nvarchar(60)=NULL
	,	@UserData19 nvarchar(60)=NULL
	,	@UserData20 nvarchar(60)=NULL
	,	@UserData21 nvarchar(60)=NULL
	,	@UserData22 nvarchar(60)=NULL
	,	@UserData23 nvarchar(60)=NULL
	,	@UserData24 nvarchar(60)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@CompanyGuid uniqueidentifier=NULL
	,	@ParentEquipmentGuid uniqueidentifier=NULL
	,	@EquipmentTypeGuid uniqueidentifier=NULL
	,	@FuelCardGuid uniqueidentifier=NULL
	,	@ProductGuid uniqueidentifier=NULL
	,	@AssignedToMeterGuid uniqueidentifier=NULL
	,	@_MasterRecordGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
	,	@NullOverrideID BIT=0 
	,	@NullOverrideDescription BIT=0 
	,	@NullOverrideMake BIT=0 
	,	@NullOverrideModel BIT=0 
	,	@NullOverrideYear BIT=0 
	,	@NullOverrideIssPtNum BIT=0 
	,	@NullOverrideFixed BIT=0 
	,	@NullOverrideStorageType BIT=0 
	,	@NullOverrideInUse BIT=0 
	,	@NullOverrideFixedVolume BIT=0 
	,	@NullOverrideIntoPlane BIT=0 
	,	@NullOverrideMobile BIT=0 
	,	@NullOverrideAttachedTo BIT=0 
	,	@NullOverrideMediaType BIT=0 
	,	@NullOverrideMeters BIT=0 
	,	@NullOverrideDefuelMeterForwards BIT=0 
	,	@NullOverridePulseRatio BIT=0 
	,	@NullOverrideRound BIT=0 
	,	@NullOverrideXref BIT=0 
	,	@NullOverrideLowStockWarning BIT=0 
	,	@NullOverrideStockTrack BIT=0 
	,	@NullOverrideTotalisor1 BIT=0 
	,	@NullOverrideTotalisor2 BIT=0 
	,	@NullOverrideFuelingState BIT=0 
	,	@NullOverrideVolume BIT=0 
	,	@NullOverrideMeterReading BIT=0 
	,	@NullOverrideConsecutive_OOS_Variance BIT=0 
	,	@NullOverrideNotes BIT=0 
	,	@NullOverrideCapacity BIT=0 
	,	@NullOverrideSafeFill BIT=0 
	,	@NullOverrideVolumeUnitIndex BIT=0 
	,	@NullOverrideTemperatureUnitIndex BIT=0 
	,	@NullOverrideDensityUnitIndex BIT=0 
	,	@NullOverrideMassUnitIndex BIT=0 
	,	@NullOverrideVolumeDecimalPlaces BIT=0 
	,	@NullOverrideTemperatureDecimalPlaces BIT=0 
	,	@NullOverrideDensityDecimalPlaces BIT=0 
	,	@NullOverrideMassDecimalPlaces BIT=0 
	,	@NullOverrideEquipmentSequence BIT=0 
	,	@NullOverrideLockedOut BIT=0 
	,	@NullOverrideLockedOutReason BIT=0 
	,	@NullOverrideLockedOutDate BIT=0 
	,	@NullOverrideSerialNumber BIT=0 
	,	@NullOverrideCompanyEquipmentID BIT=0 
	,	@NullOverrideTruckCardNumber BIT=0 
	,	@NullOverrideUpdatedDate BIT=0 
	,	@NullOverrideRatedGPM BIT=0 
	,	@NullOverrideActualGPM BIT=0 
	,	@NullOverrideFuelAdditiveFlag BIT=0 
	,	@NullOverrideManufactureDate BIT=0 
	,	@NullOverrideInstallationDate BIT=0 
	,	@NullOverrideInspectionDate BIT=0 
	,	@NullOverrideCalibrationDate BIT=0 
	,	@NullOverrideQCDate BIT=0 
	,	@NullOverrideSecondaryStorageFlag BIT=0 
	,	@NullOverrideManagedEquipmentFlag BIT=0 
	,	@NullOverrideFuelingType BIT=0 
	,	@NullOverrideUserData1 BIT=0 
	,	@NullOverrideUserData2 BIT=0 
	,	@NullOverrideUserData3 BIT=0 
	,	@NullOverrideUserData4 BIT=0 
	,	@NullOverrideUserData5 BIT=0 
	,	@NullOverrideUserData6 BIT=0 
	,	@NullOverrideUserData7 BIT=0 
	,	@NullOverrideUserData8 BIT=0 
	,	@NullOverrideUserData9 BIT=0 
	,	@NullOverrideUserData10 BIT=0 
	,	@NullOverrideUserData11 BIT=0 
	,	@NullOverrideUserData12 BIT=0 
	,	@NullOverrideUserData13 BIT=0 
	,	@NullOverrideUserData14 BIT=0 
	,	@NullOverrideUserData15 BIT=0 
	,	@NullOverrideUserData16 BIT=0 
	,	@NullOverrideUserData17 BIT=0 
	,	@NullOverrideUserData18 BIT=0 
	,	@NullOverrideUserData19 BIT=0 
	,	@NullOverrideUserData20 BIT=0 
	,	@NullOverrideUserData21 BIT=0 
	,	@NullOverrideUserData22 BIT=0 
	,	@NullOverrideUserData23 BIT=0 
	,	@NullOverrideUserData24 BIT=0 
	,	@NullOverrideSiteGuid BIT=0 
	,	@NullOverrideCompanyGuid BIT=0 
	,	@NullOverrideParentEquipmentGuid BIT=0 
	,	@NullOverrideEquipmentTypeGuid BIT=0 
	,	@NullOverrideFuelCardGuid BIT=0 
	,	@NullOverrideProductGuid BIT=0 
	,	@NullOverrideAssignedToMeterGuid BIT=0 
	,	@NullOverride_MasterRecordGuid BIT=0 
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_EquipmentUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.003 / 2014-02-05 16:52:29.4793951 -05:00
	-- Purpose: Update table [dbo].[tblEquipment]
	-- Notes:
	-- 1. @EquipmentGuid and @UpdatedBy are required parameter.
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
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblEquipment] WHERE EquipmentGuid=@EquipmentGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END
 
		UPDATE [dbo].[tblEquipment] SET
			[ID]=(CASE ISNULL(@NullOverrideID,0) WHEN 1 THEN @ID ELSE ISNULL(@ID,[ID]) END)
		,	[Description]=(CASE ISNULL(@NullOverrideDescription,0) WHEN 1 THEN @Description ELSE ISNULL(@Description,[Description]) END)
		,	[Make]=(CASE ISNULL(@NullOverrideMake,0) WHEN 1 THEN @Make ELSE ISNULL(@Make,[Make]) END)
		,	[Model]=(CASE ISNULL(@NullOverrideModel,0) WHEN 1 THEN @Model ELSE ISNULL(@Model,[Model]) END)
		,	[Year]=(CASE ISNULL(@NullOverrideYear,0) WHEN 1 THEN @Year ELSE ISNULL(@Year,[Year]) END)
		,	[IssPtNum]=(CASE ISNULL(@NullOverrideIssPtNum,0) WHEN 1 THEN @IssPtNum ELSE ISNULL(@IssPtNum,[IssPtNum]) END)
		,	[Fixed]=(CASE ISNULL(@NullOverrideFixed,0) WHEN 1 THEN @Fixed ELSE ISNULL(@Fixed,[Fixed]) END)
		,	[StorageType]=(CASE ISNULL(@NullOverrideStorageType,0) WHEN 1 THEN @StorageType ELSE ISNULL(@StorageType,[StorageType]) END)
		,	[InUse]=(CASE ISNULL(@NullOverrideInUse,0) WHEN 1 THEN @InUse ELSE ISNULL(@InUse,[InUse]) END)
		,	[FixedVolume]=(CASE ISNULL(@NullOverrideFixedVolume,0) WHEN 1 THEN @FixedVolume ELSE ISNULL(@FixedVolume,[FixedVolume]) END)
		,	[IntoPlane]=(CASE ISNULL(@NullOverrideIntoPlane,0) WHEN 1 THEN @IntoPlane ELSE ISNULL(@IntoPlane,[IntoPlane]) END)
		,	[Mobile]=(CASE ISNULL(@NullOverrideMobile,0) WHEN 1 THEN @Mobile ELSE ISNULL(@Mobile,[Mobile]) END)
		,	[AttachedTo]=(CASE ISNULL(@NullOverrideAttachedTo,0) WHEN 1 THEN @AttachedTo ELSE ISNULL(@AttachedTo,[AttachedTo]) END)
		,	[MediaType]=(CASE ISNULL(@NullOverrideMediaType,0) WHEN 1 THEN @MediaType ELSE ISNULL(@MediaType,[MediaType]) END)
		,	[Meters]=(CASE ISNULL(@NullOverrideMeters,0) WHEN 1 THEN @Meters ELSE ISNULL(@Meters,[Meters]) END)
		,	[DefuelMeterForwards]=(CASE ISNULL(@NullOverrideDefuelMeterForwards,0) WHEN 1 THEN @DefuelMeterForwards ELSE ISNULL(@DefuelMeterForwards,[DefuelMeterForwards]) END)
		,	[PulseRatio]=(CASE ISNULL(@NullOverridePulseRatio,0) WHEN 1 THEN @PulseRatio ELSE ISNULL(@PulseRatio,[PulseRatio]) END)
		,	[Round]=(CASE ISNULL(@NullOverrideRound,0) WHEN 1 THEN @Round ELSE ISNULL(@Round,[Round]) END)
		,	[Xref]=(CASE ISNULL(@NullOverrideXref,0) WHEN 1 THEN @Xref ELSE ISNULL(@Xref,[Xref]) END)
		,	[LowStockWarning]=(CASE ISNULL(@NullOverrideLowStockWarning,0) WHEN 1 THEN @LowStockWarning ELSE ISNULL(@LowStockWarning,[LowStockWarning]) END)
		,	[StockTrack]=(CASE ISNULL(@NullOverrideStockTrack,0) WHEN 1 THEN @StockTrack ELSE ISNULL(@StockTrack,[StockTrack]) END)
		,	[Totalisor1]=(CASE ISNULL(@NullOverrideTotalisor1,0) WHEN 1 THEN @Totalisor1 ELSE ISNULL(@Totalisor1,[Totalisor1]) END)
		,	[Totalisor2]=(CASE ISNULL(@NullOverrideTotalisor2,0) WHEN 1 THEN @Totalisor2 ELSE ISNULL(@Totalisor2,[Totalisor2]) END)
		,	[FuelingState]=(CASE ISNULL(@NullOverrideFuelingState,0) WHEN 1 THEN @FuelingState ELSE ISNULL(@FuelingState,[FuelingState]) END)
		,	[Volume]=(CASE ISNULL(@NullOverrideVolume,0) WHEN 1 THEN @Volume ELSE ISNULL(@Volume,[Volume]) END)
		,	[MeterReading]=(CASE ISNULL(@NullOverrideMeterReading,0) WHEN 1 THEN @MeterReading ELSE ISNULL(@MeterReading,[MeterReading]) END)
		,	[Consecutive_OOS_Variance]=(CASE ISNULL(@NullOverrideConsecutive_OOS_Variance,0) WHEN 1 THEN @Consecutive_OOS_Variance ELSE ISNULL(@Consecutive_OOS_Variance,[Consecutive_OOS_Variance]) END)
		,	[Notes]=(CASE ISNULL(@NullOverrideNotes,0) WHEN 1 THEN @Notes ELSE ISNULL(@Notes,[Notes]) END)
		,	[Capacity]=(CASE ISNULL(@NullOverrideCapacity,0) WHEN 1 THEN @Capacity ELSE ISNULL(@Capacity,[Capacity]) END)
		,	[SafeFill]=(CASE ISNULL(@NullOverrideSafeFill,0) WHEN 1 THEN @SafeFill ELSE ISNULL(@SafeFill,[SafeFill]) END)
		,	[VolumeUnitIndex]=(CASE ISNULL(@NullOverrideVolumeUnitIndex,0) WHEN 1 THEN @VolumeUnitIndex ELSE ISNULL(@VolumeUnitIndex,[VolumeUnitIndex]) END)
		,	[TemperatureUnitIndex]=(CASE ISNULL(@NullOverrideTemperatureUnitIndex,0) WHEN 1 THEN @TemperatureUnitIndex ELSE ISNULL(@TemperatureUnitIndex,[TemperatureUnitIndex]) END)
		,	[DensityUnitIndex]=(CASE ISNULL(@NullOverrideDensityUnitIndex,0) WHEN 1 THEN @DensityUnitIndex ELSE ISNULL(@DensityUnitIndex,[DensityUnitIndex]) END)
		,	[MassUnitIndex]=(CASE ISNULL(@NullOverrideMassUnitIndex,0) WHEN 1 THEN @MassUnitIndex ELSE ISNULL(@MassUnitIndex,[MassUnitIndex]) END)
		,	[VolumeDecimalPlaces]=(CASE ISNULL(@NullOverrideVolumeDecimalPlaces,0) WHEN 1 THEN @VolumeDecimalPlaces ELSE ISNULL(@VolumeDecimalPlaces,[VolumeDecimalPlaces]) END)
		,	[TemperatureDecimalPlaces]=(CASE ISNULL(@NullOverrideTemperatureDecimalPlaces,0) WHEN 1 THEN @TemperatureDecimalPlaces ELSE ISNULL(@TemperatureDecimalPlaces,[TemperatureDecimalPlaces]) END)
		,	[DensityDecimalPlaces]=(CASE ISNULL(@NullOverrideDensityDecimalPlaces,0) WHEN 1 THEN @DensityDecimalPlaces ELSE ISNULL(@DensityDecimalPlaces,[DensityDecimalPlaces]) END)
		,	[MassDecimalPlaces]=(CASE ISNULL(@NullOverrideMassDecimalPlaces,0) WHEN 1 THEN @MassDecimalPlaces ELSE ISNULL(@MassDecimalPlaces,[MassDecimalPlaces]) END)
		,	[EquipmentSequence]=(CASE ISNULL(@NullOverrideEquipmentSequence,0) WHEN 1 THEN @EquipmentSequence ELSE ISNULL(@EquipmentSequence,[EquipmentSequence]) END)
		,	[LockedOut]=(CASE ISNULL(@NullOverrideLockedOut,0) WHEN 1 THEN @LockedOut ELSE ISNULL(@LockedOut,[LockedOut]) END)
		,	[LockedOutReason]=(CASE ISNULL(@NullOverrideLockedOutReason,0) WHEN 1 THEN @LockedOutReason ELSE ISNULL(@LockedOutReason,[LockedOutReason]) END)
		,	[LockedOutDate]=(CASE ISNULL(@NullOverrideLockedOutDate,0) WHEN 1 THEN @LockedOutDate ELSE ISNULL(@LockedOutDate,[LockedOutDate]) END)
		,	[SerialNumber]=(CASE ISNULL(@NullOverrideSerialNumber,0) WHEN 1 THEN @SerialNumber ELSE ISNULL(@SerialNumber,[SerialNumber]) END)
		,	[CompanyEquipmentID]=(CASE ISNULL(@NullOverrideCompanyEquipmentID,0) WHEN 1 THEN @CompanyEquipmentID ELSE ISNULL(@CompanyEquipmentID,[CompanyEquipmentID]) END)
		,	[TruckCardNumber]=(CASE ISNULL(@NullOverrideTruckCardNumber,0) WHEN 1 THEN @TruckCardNumber ELSE ISNULL(@TruckCardNumber,[TruckCardNumber]) END)
		,	[UpdatedDate]=ISNULL(@UpdatedDate,SYSDATETIMEOFFSET())
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		,	[RatedGPM]=(CASE ISNULL(@NullOverrideRatedGPM,0) WHEN 1 THEN @RatedGPM ELSE ISNULL(@RatedGPM,[RatedGPM]) END)
		,	[ActualGPM]=(CASE ISNULL(@NullOverrideActualGPM,0) WHEN 1 THEN @ActualGPM ELSE ISNULL(@ActualGPM,[ActualGPM]) END)
		,	[FuelAdditiveFlag]=(CASE ISNULL(@NullOverrideFuelAdditiveFlag,0) WHEN 1 THEN @FuelAdditiveFlag ELSE ISNULL(@FuelAdditiveFlag,[FuelAdditiveFlag]) END)
		,	[ManufactureDate]=(CASE ISNULL(@NullOverrideManufactureDate,0) WHEN 1 THEN @ManufactureDate ELSE ISNULL(@ManufactureDate,[ManufactureDate]) END)
		,	[InstallationDate]=(CASE ISNULL(@NullOverrideInstallationDate,0) WHEN 1 THEN @InstallationDate ELSE ISNULL(@InstallationDate,[InstallationDate]) END)
		,	[InspectionDate]=(CASE ISNULL(@NullOverrideInspectionDate,0) WHEN 1 THEN @InspectionDate ELSE ISNULL(@InspectionDate,[InspectionDate]) END)
		,	[CalibrationDate]=(CASE ISNULL(@NullOverrideCalibrationDate,0) WHEN 1 THEN @CalibrationDate ELSE ISNULL(@CalibrationDate,[CalibrationDate]) END)
		,	[QCDate]=(CASE ISNULL(@NullOverrideQCDate,0) WHEN 1 THEN @QCDate ELSE ISNULL(@QCDate,[QCDate]) END)
		,	[SecondaryStorageFlag]=(CASE ISNULL(@NullOverrideSecondaryStorageFlag,0) WHEN 1 THEN @SecondaryStorageFlag ELSE ISNULL(@SecondaryStorageFlag,[SecondaryStorageFlag]) END)
		,	[ManagedEquipmentFlag]=(CASE ISNULL(@NullOverrideManagedEquipmentFlag,0) WHEN 1 THEN @ManagedEquipmentFlag ELSE ISNULL(@ManagedEquipmentFlag,[ManagedEquipmentFlag]) END)
		,	[FuelingType]=(CASE ISNULL(@NullOverrideFuelingType,0) WHEN 1 THEN @FuelingType ELSE ISNULL(@FuelingType,[FuelingType]) END)
		,	[UserData1]=(CASE ISNULL(@NullOverrideUserData1,0) WHEN 1 THEN @UserData1 ELSE ISNULL(@UserData1,[UserData1]) END)
		,	[UserData2]=(CASE ISNULL(@NullOverrideUserData2,0) WHEN 1 THEN @UserData2 ELSE ISNULL(@UserData2,[UserData2]) END)
		,	[UserData3]=(CASE ISNULL(@NullOverrideUserData3,0) WHEN 1 THEN @UserData3 ELSE ISNULL(@UserData3,[UserData3]) END)
		,	[UserData4]=(CASE ISNULL(@NullOverrideUserData4,0) WHEN 1 THEN @UserData4 ELSE ISNULL(@UserData4,[UserData4]) END)
		,	[UserData5]=(CASE ISNULL(@NullOverrideUserData5,0) WHEN 1 THEN @UserData5 ELSE ISNULL(@UserData5,[UserData5]) END)
		,	[UserData6]=(CASE ISNULL(@NullOverrideUserData6,0) WHEN 1 THEN @UserData6 ELSE ISNULL(@UserData6,[UserData6]) END)
		,	[UserData7]=(CASE ISNULL(@NullOverrideUserData7,0) WHEN 1 THEN @UserData7 ELSE ISNULL(@UserData7,[UserData7]) END)
		,	[UserData8]=(CASE ISNULL(@NullOverrideUserData8,0) WHEN 1 THEN @UserData8 ELSE ISNULL(@UserData8,[UserData8]) END)
		,	[UserData9]=(CASE ISNULL(@NullOverrideUserData9,0) WHEN 1 THEN @UserData9 ELSE ISNULL(@UserData9,[UserData9]) END)
		,	[UserData10]=(CASE ISNULL(@NullOverrideUserData10,0) WHEN 1 THEN @UserData10 ELSE ISNULL(@UserData10,[UserData10]) END)
		,	[UserData11]=(CASE ISNULL(@NullOverrideUserData11,0) WHEN 1 THEN @UserData11 ELSE ISNULL(@UserData11,[UserData11]) END)
		,	[UserData12]=(CASE ISNULL(@NullOverrideUserData12,0) WHEN 1 THEN @UserData12 ELSE ISNULL(@UserData12,[UserData12]) END)
		,	[UserData13]=(CASE ISNULL(@NullOverrideUserData13,0) WHEN 1 THEN @UserData13 ELSE ISNULL(@UserData13,[UserData13]) END)
		,	[UserData14]=(CASE ISNULL(@NullOverrideUserData14,0) WHEN 1 THEN @UserData14 ELSE ISNULL(@UserData14,[UserData14]) END)
		,	[UserData15]=(CASE ISNULL(@NullOverrideUserData15,0) WHEN 1 THEN @UserData15 ELSE ISNULL(@UserData15,[UserData15]) END)
		,	[UserData16]=(CASE ISNULL(@NullOverrideUserData16,0) WHEN 1 THEN @UserData16 ELSE ISNULL(@UserData16,[UserData16]) END)
		,	[UserData17]=(CASE ISNULL(@NullOverrideUserData17,0) WHEN 1 THEN @UserData17 ELSE ISNULL(@UserData17,[UserData17]) END)
		,	[UserData18]=(CASE ISNULL(@NullOverrideUserData18,0) WHEN 1 THEN @UserData18 ELSE ISNULL(@UserData18,[UserData18]) END)
		,	[UserData19]=(CASE ISNULL(@NullOverrideUserData19,0) WHEN 1 THEN @UserData19 ELSE ISNULL(@UserData19,[UserData19]) END)
		,	[UserData20]=(CASE ISNULL(@NullOverrideUserData20,0) WHEN 1 THEN @UserData20 ELSE ISNULL(@UserData20,[UserData20]) END)
		,	[UserData21]=(CASE ISNULL(@NullOverrideUserData21,0) WHEN 1 THEN @UserData21 ELSE ISNULL(@UserData21,[UserData21]) END)
		,	[UserData22]=(CASE ISNULL(@NullOverrideUserData22,0) WHEN 1 THEN @UserData22 ELSE ISNULL(@UserData22,[UserData22]) END)
		,	[UserData23]=(CASE ISNULL(@NullOverrideUserData23,0) WHEN 1 THEN @UserData23 ELSE ISNULL(@UserData23,[UserData23]) END)
		,	[UserData24]=(CASE ISNULL(@NullOverrideUserData24,0) WHEN 1 THEN @UserData24 ELSE ISNULL(@UserData24,[UserData24]) END)
		,	[SiteGuid]=(CASE ISNULL(@NullOverrideSiteGuid,0) WHEN 1 THEN @SiteGuid ELSE ISNULL(@SiteGuid,[SiteGuid]) END)
		,	[CompanyGuid]=(CASE ISNULL(@NullOverrideCompanyGuid,0) WHEN 1 THEN @CompanyGuid ELSE ISNULL(@CompanyGuid,[CompanyGuid]) END)
		,	[ParentEquipmentGuid]=(CASE ISNULL(@NullOverrideParentEquipmentGuid,0) WHEN 1 THEN @ParentEquipmentGuid ELSE ISNULL(@ParentEquipmentGuid,[ParentEquipmentGuid]) END)
		,	[EquipmentTypeGuid]=(CASE ISNULL(@NullOverrideEquipmentTypeGuid,0) WHEN 1 THEN @EquipmentTypeGuid ELSE ISNULL(@EquipmentTypeGuid,[EquipmentTypeGuid]) END)
		,	[FuelCardGuid]=(CASE ISNULL(@NullOverrideFuelCardGuid,0) WHEN 1 THEN @FuelCardGuid ELSE ISNULL(@FuelCardGuid,[FuelCardGuid]) END)
		,	[ProductGuid]=(CASE ISNULL(@NullOverrideProductGuid,0) WHEN 1 THEN @ProductGuid ELSE ISNULL(@ProductGuid,[ProductGuid]) END)
		,	[AssignedToMeterGuid]=(CASE ISNULL(@NullOverrideAssignedToMeterGuid,0) WHEN 1 THEN @AssignedToMeterGuid ELSE ISNULL(@AssignedToMeterGuid,[AssignedToMeterGuid]) END)
		,	[_MasterRecordGuid]=(CASE ISNULL(@NullOverride_MasterRecordGuid,0) WHEN 1 THEN @_MasterRecordGuid ELSE ISNULL(@_MasterRecordGuid,[_MasterRecordGuid]) END)
		WHERE	EquipmentGuid=@EquipmentGuid;
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblEquipment]           
		WHERE EquipmentGuid=@EquipmentGuid;
	
 
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
						+ 'Procedure Name: gsp_EquipmentUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
