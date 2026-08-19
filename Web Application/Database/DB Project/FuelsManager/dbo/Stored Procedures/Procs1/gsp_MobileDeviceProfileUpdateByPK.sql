CREATE PROCEDURE [dbo].[gsp_MobileDeviceProfileUpdateByPK]
(
		@MobileDeviceProfileGuid uniqueidentifier
	,	@SiteGuid uniqueidentifier=NULL
	,	@ProfileID nvarchar(50)=NULL
	,	@Description nvarchar(200)=NULL
	,	@ShowProductScreen bit=NULL
	,	@GenerateTicketNumber bit=NULL
	,	@ShowOperatorFieldInFlightList bit=NULL
	,	@UseDefaultPrinter bit=NULL
	,	@DefaultPrinter nvarchar(50)=NULL
	,	@AdminPassword varbinary=NULL
	,	@ShutdownHotKey nvarchar(50)=NULL
	,	@PrinterCOMPort nvarchar(4)=NULL
	,	@SearchType int=NULL
	,	@LoggingOption bit=NULL
	,	@AllowableFailedLoginAttempts int=NULL
	,	@FuelDistributionPrecision int=NULL
	,	@MakeDefaultProfile bit=NULL
	,	@VehicleID nvarchar(50)=NULL
	,	@MonitorScreenTransitionTiming bit=NULL
	,	@BypassFsrCheckOnScreenTrans bit=NULL
	,	@ShowFuelUpdateCheckStatusWin bit=NULL
	,	@RTDTemperatureRangeMin float=NULL
	,	@RTDTemperatureRangeMax float=NULL
	,	@DefaultTemperature float=NULL
	,	@StrictUserValidation bit=NULL
	,	@VerifyFuelingEquipment bit=NULL
	,	@AllowEditRequiredFuelLoad bit=NULL
	,	@AllowBackAfterArrivalScreen bit=NULL
	,	@AllowBackAfterTicketPrinted bit=NULL
	,	@RequirePrint bit=NULL
	,	@TotalFuelLoadCheck bit=NULL
	,	@VolumetricThresholdValidation bit=NULL
	,	@ValidateShipNumber bit=NULL
	,	@AllowVTOModification bit=NULL
	,	@AllowFlightGateModification bit=NULL
	,	@TankPositionBalanceVerification int=NULL
	,	@TankPositionBalancePercentage float=NULL
	,	@OverrideWingBalancePercentVar bit=NULL
	,	@BypassDistributionTolerance bit=NULL
	,	@VehicleIDCheck bit=NULL
	,	@GSEFuelMustMatch bit=NULL
	,	@AllowManualMeter bit=NULL
	,	@UseValidLogicGATrans bit=NULL
	,	@AllowShipNumberModification bit=NULL
	,	@AllowAircraftTypeModification bit=NULL
	,	@AllowDestinationModification bit=NULL
	,	@TicketPrinting int=NULL
	,	@AircraftTypeVerification int=NULL
	,	@Destination int=NULL
	,	@Gate int=NULL
	,	@ShipNumber int=NULL
	,	@MeterTotal int=NULL
	,	@VolumePumped int=NULL
	,	@TankCapacity int=NULL
	,	@EAStrictUserValidation bit=NULL
	,	@EAVerifyFuelingEquipment bit=NULL
	,	@EAAllowEditOfRequiredFuelLoad bit=NULL
	,	@EAAllowBackAfterArrivalScreen bit=NULL
	,	@EAAllowBackAfterTicketPrinted bit=NULL
	,	@EARequirePrint bit=NULL
	,	@EATotalFuelLoad bit=NULL
	,	@EAVolumetricThresholdValidation bit=NULL
	,	@EAValidateShipNumber bit=NULL
	,	@EAAllowVTOModification bit=NULL
	,	@EAAllowFlightGateModification bit=NULL
	,	@EATankDiffPercentage bit=NULL
	,	@EAWingBalancePercentage bit=NULL
	,	@EABypassDistributionTolerance bit=NULL
	,	@EAVehicleIDCheck bit=NULL
	,	@EAGSEFuelMustMatch bit=NULL
	,	@EAAllowManualMeter bit=NULL
	,	@EAUseValidationLogicGATrans bit=NULL
	,	@EAAllowShipNumberModification bit=NULL
	,	@EAAllowAircraftTypeModification bit=NULL
	,	@EAAllowDestinationModification bit=NULL
	,	@EADestination bit=NULL
	,	@EATicketPrinting bit=NULL
	,	@EAAircraftType bit=NULL
	,	@EAShipNumber bit=NULL
	,	@EAGateNumber bit=NULL
	,	@EAMeterTotal bit=NULL
	,	@EAVolumePumped bit=NULL
	,	@EATankCapacity bit=NULL
	,	@EquipmentType int=NULL
	,	@ForeignKeyToMapEquipment uniqueidentifier=NULL
	,	@IssueTransaction uniqueidentifier=NULL
	,	@DefuelTransaction uniqueidentifier=NULL
	,	@RotationTransaction uniqueidentifier=NULL
	,	@MeterCloseout uniqueidentifier=NULL
	,	@DeIceTransaction uniqueidentifier=NULL
	,	@GSETransaction uniqueidentifier=NULL
	,	@ManualConsumer uniqueidentifier=NULL
	,	@ManualVendor uniqueidentifier=NULL
	,	@ManualShipper uniqueidentifier=NULL
	,	@ManualManager uniqueidentifier=NULL
	,	@ManualSupplier uniqueidentifier=NULL
	,	@ManualBillTo uniqueidentifier=NULL
	,	@ManualProduct uniqueidentifier=NULL
	,	@CloseoutConsumer uniqueidentifier=NULL
	,	@CloseoutOwner uniqueidentifier=NULL
	,	@CloseoutVendor uniqueidentifier=NULL
	,	@ManualStationID int=NULL
	,	@InhibitOverridingTemperature bit=NULL
	,	@ManualTemperature float=NULL
	,	@ManualDensity float=NULL
	,	@HasDCU bit=NULL
	,	@BluetoothDCU bit=NULL
	,	@LogDCUActions bit=NULL
	,	@HasAveryHardoll bit=NULL
	,	@DCUComPort nvarchar(4)=NULL
	,	@DCUReadRetry int=NULL
	,	@DCUDisconnectDelay int=NULL
	,	@DCUCommunicationFailRestart int=NULL
	,	@AveryHardollComPort nvarchar(4)=NULL
	,	@AveryHardollMeterID nvarchar(4)=NULL
	,	@ConfirmFuelCaps bit=NULL
	,	@VTOEnabled bit=NULL
	,	@EnabledInOpGauges bit=NULL
	,	@UseDispensingVehicleGSETrans bit=NULL
	,	@GSEWaitMSecForGetMeter int=NULL
	,	@GSEInactiveLogoutMinutes int=NULL
	,	@GSEInactiveTimeout int=NULL
	,	@BarcodeInvalidWarningSeconds int=NULL
	,	@DeIceBlendDefault float=NULL
	,	@CommunicationTimeoutSeconds int=NULL
	,	@ConnectionRetries int=NULL
	,	@ConnectionRetryTimeout int=NULL
	,	@ConnectionType int=NULL
	,	@UpdateInterval int=NULL
	,	@PingVerificationIPAddress bit=NULL
	,	@VehicleUpdateInterval int=NULL
	,	@PresubmitDelay int=NULL
	,	@VerificationIPAddress nvarchar(15)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
	,	@NullOverrideSiteGuid BIT=0 
	,	@NullOverrideProfileID BIT=0 
	,	@NullOverrideDescription BIT=0 
	,	@NullOverrideShowProductScreen BIT=0 
	,	@NullOverrideGenerateTicketNumber BIT=0 
	,	@NullOverrideShowOperatorFieldInFlightList BIT=0 
	,	@NullOverrideUseDefaultPrinter BIT=0 
	,	@NullOverrideDefaultPrinter BIT=0 
	,	@NullOverrideAdminPassword BIT=0 
	,	@NullOverrideShutdownHotKey BIT=0 
	,	@NullOverridePrinterCOMPort BIT=0 
	,	@NullOverrideSearchType BIT=0 
	,	@NullOverrideLoggingOption BIT=0 
	,	@NullOverrideAllowableFailedLoginAttempts BIT=0 
	,	@NullOverrideFuelDistributionPrecision BIT=0 
	,	@NullOverrideMakeDefaultProfile BIT=0 
	,	@NullOverrideVehicleID BIT=0 
	,	@NullOverrideMonitorScreenTransitionTiming BIT=0 
	,	@NullOverrideBypassFsrCheckOnScreenTrans BIT=0 
	,	@NullOverrideShowFuelUpdateCheckStatusWin BIT=0 
	,	@NullOverrideRTDTemperatureRangeMin BIT=0 
	,	@NullOverrideRTDTemperatureRangeMax BIT=0 
	,	@NullOverrideDefaultTemperature BIT=0 
	,	@NullOverrideStrictUserValidation BIT=0 
	,	@NullOverrideVerifyFuelingEquipment BIT=0 
	,	@NullOverrideAllowEditRequiredFuelLoad BIT=0 
	,	@NullOverrideAllowBackAfterArrivalScreen BIT=0 
	,	@NullOverrideAllowBackAfterTicketPrinted BIT=0 
	,	@NullOverrideRequirePrint BIT=0 
	,	@NullOverrideTotalFuelLoadCheck BIT=0 
	,	@NullOverrideVolumetricThresholdValidation BIT=0 
	,	@NullOverrideValidateShipNumber BIT=0 
	,	@NullOverrideAllowVTOModification BIT=0 
	,	@NullOverrideAllowFlightGateModification BIT=0 
	,	@NullOverrideTankPositionBalanceVerification BIT=0 
	,	@NullOverrideTankPositionBalancePercentage BIT=0 
	,	@NullOverrideOverrideWingBalancePercentVar BIT=0 
	,	@NullOverrideBypassDistributionTolerance BIT=0 
	,	@NullOverrideVehicleIDCheck BIT=0 
	,	@NullOverrideGSEFuelMustMatch BIT=0 
	,	@NullOverrideAllowManualMeter BIT=0 
	,	@NullOverrideUseValidLogicGATrans BIT=0 
	,	@NullOverrideAllowShipNumberModification BIT=0 
	,	@NullOverrideAllowAircraftTypeModification BIT=0 
	,	@NullOverrideAllowDestinationModification BIT=0 
	,	@NullOverrideTicketPrinting BIT=0 
	,	@NullOverrideAircraftTypeVerification BIT=0 
	,	@NullOverrideDestination BIT=0 
	,	@NullOverrideGate BIT=0 
	,	@NullOverrideShipNumber BIT=0 
	,	@NullOverrideMeterTotal BIT=0 
	,	@NullOverrideVolumePumped BIT=0 
	,	@NullOverrideTankCapacity BIT=0 
	,	@NullOverrideEAStrictUserValidation BIT=0 
	,	@NullOverrideEAVerifyFuelingEquipment BIT=0 
	,	@NullOverrideEAAllowEditOfRequiredFuelLoad BIT=0 
	,	@NullOverrideEAAllowBackAfterArrivalScreen BIT=0 
	,	@NullOverrideEAAllowBackAfterTicketPrinted BIT=0 
	,	@NullOverrideEARequirePrint BIT=0 
	,	@NullOverrideEATotalFuelLoad BIT=0 
	,	@NullOverrideEAVolumetricThresholdValidation BIT=0 
	,	@NullOverrideEAValidateShipNumber BIT=0 
	,	@NullOverrideEAAllowVTOModification BIT=0 
	,	@NullOverrideEAAllowFlightGateModification BIT=0 
	,	@NullOverrideEATankDiffPercentage BIT=0 
	,	@NullOverrideEAWingBalancePercentage BIT=0 
	,	@NullOverrideEABypassDistributionTolerance BIT=0 
	,	@NullOverrideEAVehicleIDCheck BIT=0 
	,	@NullOverrideEAGSEFuelMustMatch BIT=0 
	,	@NullOverrideEAAllowManualMeter BIT=0 
	,	@NullOverrideEAUseValidationLogicGATrans BIT=0 
	,	@NullOverrideEAAllowShipNumberModification BIT=0 
	,	@NullOverrideEAAllowAircraftTypeModification BIT=0 
	,	@NullOverrideEAAllowDestinationModification BIT=0 
	,	@NullOverrideEADestination BIT=0 
	,	@NullOverrideEATicketPrinting BIT=0 
	,	@NullOverrideEAAircraftType BIT=0 
	,	@NullOverrideEAShipNumber BIT=0 
	,	@NullOverrideEAGateNumber BIT=0 
	,	@NullOverrideEAMeterTotal BIT=0 
	,	@NullOverrideEAVolumePumped BIT=0 
	,	@NullOverrideEATankCapacity BIT=0 
	,	@NullOverrideEquipmentType BIT=0 
	,	@NullOverrideForeignKeyToMapEquipment BIT=0 
	,	@NullOverrideIssueTransaction BIT=0 
	,	@NullOverrideDefuelTransaction BIT=0 
	,	@NullOverrideRotationTransaction BIT=0 
	,	@NullOverrideMeterCloseout BIT=0 
	,	@NullOverrideDeIceTransaction BIT=0 
	,	@NullOverrideGSETransaction BIT=0 
	,	@NullOverrideManualConsumer BIT=0 
	,	@NullOverrideManualVendor BIT=0 
	,	@NullOverrideManualShipper BIT=0 
	,	@NullOverrideManualManager BIT=0 
	,	@NullOverrideManualSupplier BIT=0 
	,	@NullOverrideManualBillTo BIT=0 
	,	@NullOverrideManualProduct BIT=0 
	,	@NullOverrideCloseoutConsumer BIT=0 
	,	@NullOverrideCloseoutOwner BIT=0 
	,	@NullOverrideCloseoutVendor BIT=0 
	,	@NullOverrideManualStationID BIT=0 
	,	@NullOverrideInhibitOverridingTemperature BIT=0 
	,	@NullOverrideManualTemperature BIT=0 
	,	@NullOverrideManualDensity BIT=0 
	,	@NullOverrideHasDCU BIT=0 
	,	@NullOverrideBluetoothDCU BIT=0 
	,	@NullOverrideLogDCUActions BIT=0 
	,	@NullOverrideHasAveryHardoll BIT=0 
	,	@NullOverrideDCUComPort BIT=0 
	,	@NullOverrideDCUReadRetry BIT=0 
	,	@NullOverrideDCUDisconnectDelay BIT=0 
	,	@NullOverrideDCUCommunicationFailRestart BIT=0 
	,	@NullOverrideAveryHardollComPort BIT=0 
	,	@NullOverrideAveryHardollMeterID BIT=0 
	,	@NullOverrideConfirmFuelCaps BIT=0 
	,	@NullOverrideVTOEnabled BIT=0 
	,	@NullOverrideEnabledInOpGauges BIT=0 
	,	@NullOverrideUseDispensingVehicleGSETrans BIT=0 
	,	@NullOverrideGSEWaitMSecForGetMeter BIT=0 
	,	@NullOverrideGSEInactiveLogoutMinutes BIT=0 
	,	@NullOverrideGSEInactiveTimeout BIT=0 
	,	@NullOverrideBarcodeInvalidWarningSeconds BIT=0 
	,	@NullOverrideDeIceBlendDefault BIT=0 
	,	@NullOverrideCommunicationTimeoutSeconds BIT=0 
	,	@NullOverrideConnectionRetries BIT=0 
	,	@NullOverrideConnectionRetryTimeout BIT=0 
	,	@NullOverrideConnectionType BIT=0 
	,	@NullOverrideUpdateInterval BIT=0 
	,	@NullOverridePingVerificationIPAddress BIT=0 
	,	@NullOverrideVehicleUpdateInterval BIT=0 
	,	@NullOverridePresubmitDelay BIT=0 
	,	@NullOverrideVerificationIPAddress BIT=0 
	,	@NullOverrideUpdatedDate BIT=0 
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_MobileDeviceProfileUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.003 / 2014-02-05 16:52:29.5754047 -05:00
	-- Purpose: Update table [dbo].[tblMobileDeviceProfile]
	-- Notes:
	-- 1. @MobileDeviceProfileGuid and @UpdatedBy are required parameter.
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
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblMobileDeviceProfile] WHERE MobileDeviceProfileGuid=@MobileDeviceProfileGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END
 
		UPDATE [dbo].[tblMobileDeviceProfile] SET
			[SiteGuid]=(CASE ISNULL(@NullOverrideSiteGuid,0) WHEN 1 THEN @SiteGuid ELSE ISNULL(@SiteGuid,[SiteGuid]) END)
		,	[ProfileID]=(CASE ISNULL(@NullOverrideProfileID,0) WHEN 1 THEN @ProfileID ELSE ISNULL(@ProfileID,[ProfileID]) END)
		,	[Description]=(CASE ISNULL(@NullOverrideDescription,0) WHEN 1 THEN @Description ELSE ISNULL(@Description,[Description]) END)
		,	[ShowProductScreen]=(CASE ISNULL(@NullOverrideShowProductScreen,0) WHEN 1 THEN @ShowProductScreen ELSE ISNULL(@ShowProductScreen,[ShowProductScreen]) END)
		,	[GenerateTicketNumber]=(CASE ISNULL(@NullOverrideGenerateTicketNumber,0) WHEN 1 THEN @GenerateTicketNumber ELSE ISNULL(@GenerateTicketNumber,[GenerateTicketNumber]) END)
		,	[ShowOperatorFieldInFlightList]=(CASE ISNULL(@NullOverrideShowOperatorFieldInFlightList,0) WHEN 1 THEN @ShowOperatorFieldInFlightList ELSE ISNULL(@ShowOperatorFieldInFlightList,[ShowOperatorFieldInFlightList]) END)
		,	[UseDefaultPrinter]=(CASE ISNULL(@NullOverrideUseDefaultPrinter,0) WHEN 1 THEN @UseDefaultPrinter ELSE ISNULL(@UseDefaultPrinter,[UseDefaultPrinter]) END)
		,	[DefaultPrinter]=(CASE ISNULL(@NullOverrideDefaultPrinter,0) WHEN 1 THEN @DefaultPrinter ELSE ISNULL(@DefaultPrinter,[DefaultPrinter]) END)
		,	[AdminPassword]=(CASE ISNULL(@NullOverrideAdminPassword,0) WHEN 1 THEN @AdminPassword ELSE ISNULL(@AdminPassword,[AdminPassword]) END)
		,	[ShutdownHotKey]=(CASE ISNULL(@NullOverrideShutdownHotKey,0) WHEN 1 THEN @ShutdownHotKey ELSE ISNULL(@ShutdownHotKey,[ShutdownHotKey]) END)
		,	[PrinterCOMPort]=(CASE ISNULL(@NullOverridePrinterCOMPort,0) WHEN 1 THEN @PrinterCOMPort ELSE ISNULL(@PrinterCOMPort,[PrinterCOMPort]) END)
		,	[SearchType]=(CASE ISNULL(@NullOverrideSearchType,0) WHEN 1 THEN @SearchType ELSE ISNULL(@SearchType,[SearchType]) END)
		,	[LoggingOption]=(CASE ISNULL(@NullOverrideLoggingOption,0) WHEN 1 THEN @LoggingOption ELSE ISNULL(@LoggingOption,[LoggingOption]) END)
		,	[AllowableFailedLoginAttempts]=(CASE ISNULL(@NullOverrideAllowableFailedLoginAttempts,0) WHEN 1 THEN @AllowableFailedLoginAttempts ELSE ISNULL(@AllowableFailedLoginAttempts,[AllowableFailedLoginAttempts]) END)
		,	[FuelDistributionPrecision]=(CASE ISNULL(@NullOverrideFuelDistributionPrecision,0) WHEN 1 THEN @FuelDistributionPrecision ELSE ISNULL(@FuelDistributionPrecision,[FuelDistributionPrecision]) END)
		,	[MakeDefaultProfile]=(CASE ISNULL(@NullOverrideMakeDefaultProfile,0) WHEN 1 THEN @MakeDefaultProfile ELSE ISNULL(@MakeDefaultProfile,[MakeDefaultProfile]) END)
		,	[VehicleID]=(CASE ISNULL(@NullOverrideVehicleID,0) WHEN 1 THEN @VehicleID ELSE ISNULL(@VehicleID,[VehicleID]) END)
		,	[MonitorScreenTransitionTiming]=(CASE ISNULL(@NullOverrideMonitorScreenTransitionTiming,0) WHEN 1 THEN @MonitorScreenTransitionTiming ELSE ISNULL(@MonitorScreenTransitionTiming,[MonitorScreenTransitionTiming]) END)
		,	[BypassFsrCheckOnScreenTrans]=(CASE ISNULL(@NullOverrideBypassFsrCheckOnScreenTrans,0) WHEN 1 THEN @BypassFsrCheckOnScreenTrans ELSE ISNULL(@BypassFsrCheckOnScreenTrans,[BypassFsrCheckOnScreenTrans]) END)
		,	[ShowFuelUpdateCheckStatusWin]=(CASE ISNULL(@NullOverrideShowFuelUpdateCheckStatusWin,0) WHEN 1 THEN @ShowFuelUpdateCheckStatusWin ELSE ISNULL(@ShowFuelUpdateCheckStatusWin,[ShowFuelUpdateCheckStatusWin]) END)
		,	[RTDTemperatureRangeMin]=(CASE ISNULL(@NullOverrideRTDTemperatureRangeMin,0) WHEN 1 THEN @RTDTemperatureRangeMin ELSE ISNULL(@RTDTemperatureRangeMin,[RTDTemperatureRangeMin]) END)
		,	[RTDTemperatureRangeMax]=(CASE ISNULL(@NullOverrideRTDTemperatureRangeMax,0) WHEN 1 THEN @RTDTemperatureRangeMax ELSE ISNULL(@RTDTemperatureRangeMax,[RTDTemperatureRangeMax]) END)
		,	[DefaultTemperature]=(CASE ISNULL(@NullOverrideDefaultTemperature,0) WHEN 1 THEN @DefaultTemperature ELSE ISNULL(@DefaultTemperature,[DefaultTemperature]) END)
		,	[StrictUserValidation]=(CASE ISNULL(@NullOverrideStrictUserValidation,0) WHEN 1 THEN @StrictUserValidation ELSE ISNULL(@StrictUserValidation,[StrictUserValidation]) END)
		,	[VerifyFuelingEquipment]=(CASE ISNULL(@NullOverrideVerifyFuelingEquipment,0) WHEN 1 THEN @VerifyFuelingEquipment ELSE ISNULL(@VerifyFuelingEquipment,[VerifyFuelingEquipment]) END)
		,	[AllowEditRequiredFuelLoad]=(CASE ISNULL(@NullOverrideAllowEditRequiredFuelLoad,0) WHEN 1 THEN @AllowEditRequiredFuelLoad ELSE ISNULL(@AllowEditRequiredFuelLoad,[AllowEditRequiredFuelLoad]) END)
		,	[AllowBackAfterArrivalScreen]=(CASE ISNULL(@NullOverrideAllowBackAfterArrivalScreen,0) WHEN 1 THEN @AllowBackAfterArrivalScreen ELSE ISNULL(@AllowBackAfterArrivalScreen,[AllowBackAfterArrivalScreen]) END)
		,	[AllowBackAfterTicketPrinted]=(CASE ISNULL(@NullOverrideAllowBackAfterTicketPrinted,0) WHEN 1 THEN @AllowBackAfterTicketPrinted ELSE ISNULL(@AllowBackAfterTicketPrinted,[AllowBackAfterTicketPrinted]) END)
		,	[RequirePrint]=(CASE ISNULL(@NullOverrideRequirePrint,0) WHEN 1 THEN @RequirePrint ELSE ISNULL(@RequirePrint,[RequirePrint]) END)
		,	[TotalFuelLoadCheck]=(CASE ISNULL(@NullOverrideTotalFuelLoadCheck,0) WHEN 1 THEN @TotalFuelLoadCheck ELSE ISNULL(@TotalFuelLoadCheck,[TotalFuelLoadCheck]) END)
		,	[VolumetricThresholdValidation]=(CASE ISNULL(@NullOverrideVolumetricThresholdValidation,0) WHEN 1 THEN @VolumetricThresholdValidation ELSE ISNULL(@VolumetricThresholdValidation,[VolumetricThresholdValidation]) END)
		,	[ValidateShipNumber]=(CASE ISNULL(@NullOverrideValidateShipNumber,0) WHEN 1 THEN @ValidateShipNumber ELSE ISNULL(@ValidateShipNumber,[ValidateShipNumber]) END)
		,	[AllowVTOModification]=(CASE ISNULL(@NullOverrideAllowVTOModification,0) WHEN 1 THEN @AllowVTOModification ELSE ISNULL(@AllowVTOModification,[AllowVTOModification]) END)
		,	[AllowFlightGateModification]=(CASE ISNULL(@NullOverrideAllowFlightGateModification,0) WHEN 1 THEN @AllowFlightGateModification ELSE ISNULL(@AllowFlightGateModification,[AllowFlightGateModification]) END)
		,	[TankPositionBalanceVerification]=(CASE ISNULL(@NullOverrideTankPositionBalanceVerification,0) WHEN 1 THEN @TankPositionBalanceVerification ELSE ISNULL(@TankPositionBalanceVerification,[TankPositionBalanceVerification]) END)
		,	[TankPositionBalancePercentage]=(CASE ISNULL(@NullOverrideTankPositionBalancePercentage,0) WHEN 1 THEN @TankPositionBalancePercentage ELSE ISNULL(@TankPositionBalancePercentage,[TankPositionBalancePercentage]) END)
		,	[OverrideWingBalancePercentVar]=(CASE ISNULL(@NullOverrideOverrideWingBalancePercentVar,0) WHEN 1 THEN @OverrideWingBalancePercentVar ELSE ISNULL(@OverrideWingBalancePercentVar,[OverrideWingBalancePercentVar]) END)
		,	[BypassDistributionTolerance]=(CASE ISNULL(@NullOverrideBypassDistributionTolerance,0) WHEN 1 THEN @BypassDistributionTolerance ELSE ISNULL(@BypassDistributionTolerance,[BypassDistributionTolerance]) END)
		,	[VehicleIDCheck]=(CASE ISNULL(@NullOverrideVehicleIDCheck,0) WHEN 1 THEN @VehicleIDCheck ELSE ISNULL(@VehicleIDCheck,[VehicleIDCheck]) END)
		,	[GSEFuelMustMatch]=(CASE ISNULL(@NullOverrideGSEFuelMustMatch,0) WHEN 1 THEN @GSEFuelMustMatch ELSE ISNULL(@GSEFuelMustMatch,[GSEFuelMustMatch]) END)
		,	[AllowManualMeter]=(CASE ISNULL(@NullOverrideAllowManualMeter,0) WHEN 1 THEN @AllowManualMeter ELSE ISNULL(@AllowManualMeter,[AllowManualMeter]) END)
		,	[UseValidLogicGATrans]=(CASE ISNULL(@NullOverrideUseValidLogicGATrans,0) WHEN 1 THEN @UseValidLogicGATrans ELSE ISNULL(@UseValidLogicGATrans,[UseValidLogicGATrans]) END)
		,	[AllowShipNumberModification]=(CASE ISNULL(@NullOverrideAllowShipNumberModification,0) WHEN 1 THEN @AllowShipNumberModification ELSE ISNULL(@AllowShipNumberModification,[AllowShipNumberModification]) END)
		,	[AllowAircraftTypeModification]=(CASE ISNULL(@NullOverrideAllowAircraftTypeModification,0) WHEN 1 THEN @AllowAircraftTypeModification ELSE ISNULL(@AllowAircraftTypeModification,[AllowAircraftTypeModification]) END)
		,	[AllowDestinationModification]=(CASE ISNULL(@NullOverrideAllowDestinationModification,0) WHEN 1 THEN @AllowDestinationModification ELSE ISNULL(@AllowDestinationModification,[AllowDestinationModification]) END)
		,	[TicketPrinting]=(CASE ISNULL(@NullOverrideTicketPrinting,0) WHEN 1 THEN @TicketPrinting ELSE ISNULL(@TicketPrinting,[TicketPrinting]) END)
		,	[AircraftTypeVerification]=(CASE ISNULL(@NullOverrideAircraftTypeVerification,0) WHEN 1 THEN @AircraftTypeVerification ELSE ISNULL(@AircraftTypeVerification,[AircraftTypeVerification]) END)
		,	[Destination]=(CASE ISNULL(@NullOverrideDestination,0) WHEN 1 THEN @Destination ELSE ISNULL(@Destination,[Destination]) END)
		,	[Gate]=(CASE ISNULL(@NullOverrideGate,0) WHEN 1 THEN @Gate ELSE ISNULL(@Gate,[Gate]) END)
		,	[ShipNumber]=(CASE ISNULL(@NullOverrideShipNumber,0) WHEN 1 THEN @ShipNumber ELSE ISNULL(@ShipNumber,[ShipNumber]) END)
		,	[MeterTotal]=(CASE ISNULL(@NullOverrideMeterTotal,0) WHEN 1 THEN @MeterTotal ELSE ISNULL(@MeterTotal,[MeterTotal]) END)
		,	[VolumePumped]=(CASE ISNULL(@NullOverrideVolumePumped,0) WHEN 1 THEN @VolumePumped ELSE ISNULL(@VolumePumped,[VolumePumped]) END)
		,	[TankCapacity]=(CASE ISNULL(@NullOverrideTankCapacity,0) WHEN 1 THEN @TankCapacity ELSE ISNULL(@TankCapacity,[TankCapacity]) END)
		,	[EAStrictUserValidation]=(CASE ISNULL(@NullOverrideEAStrictUserValidation,0) WHEN 1 THEN @EAStrictUserValidation ELSE ISNULL(@EAStrictUserValidation,[EAStrictUserValidation]) END)
		,	[EAVerifyFuelingEquipment]=(CASE ISNULL(@NullOverrideEAVerifyFuelingEquipment,0) WHEN 1 THEN @EAVerifyFuelingEquipment ELSE ISNULL(@EAVerifyFuelingEquipment,[EAVerifyFuelingEquipment]) END)
		,	[EAAllowEditOfRequiredFuelLoad]=(CASE ISNULL(@NullOverrideEAAllowEditOfRequiredFuelLoad,0) WHEN 1 THEN @EAAllowEditOfRequiredFuelLoad ELSE ISNULL(@EAAllowEditOfRequiredFuelLoad,[EAAllowEditOfRequiredFuelLoad]) END)
		,	[EAAllowBackAfterArrivalScreen]=(CASE ISNULL(@NullOverrideEAAllowBackAfterArrivalScreen,0) WHEN 1 THEN @EAAllowBackAfterArrivalScreen ELSE ISNULL(@EAAllowBackAfterArrivalScreen,[EAAllowBackAfterArrivalScreen]) END)
		,	[EAAllowBackAfterTicketPrinted]=(CASE ISNULL(@NullOverrideEAAllowBackAfterTicketPrinted,0) WHEN 1 THEN @EAAllowBackAfterTicketPrinted ELSE ISNULL(@EAAllowBackAfterTicketPrinted,[EAAllowBackAfterTicketPrinted]) END)
		,	[EARequirePrint]=(CASE ISNULL(@NullOverrideEARequirePrint,0) WHEN 1 THEN @EARequirePrint ELSE ISNULL(@EARequirePrint,[EARequirePrint]) END)
		,	[EATotalFuelLoad]=(CASE ISNULL(@NullOverrideEATotalFuelLoad,0) WHEN 1 THEN @EATotalFuelLoad ELSE ISNULL(@EATotalFuelLoad,[EATotalFuelLoad]) END)
		,	[EAVolumetricThresholdValidation]=(CASE ISNULL(@NullOverrideEAVolumetricThresholdValidation,0) WHEN 1 THEN @EAVolumetricThresholdValidation ELSE ISNULL(@EAVolumetricThresholdValidation,[EAVolumetricThresholdValidation]) END)
		,	[EAValidateShipNumber]=(CASE ISNULL(@NullOverrideEAValidateShipNumber,0) WHEN 1 THEN @EAValidateShipNumber ELSE ISNULL(@EAValidateShipNumber,[EAValidateShipNumber]) END)
		,	[EAAllowVTOModification]=(CASE ISNULL(@NullOverrideEAAllowVTOModification,0) WHEN 1 THEN @EAAllowVTOModification ELSE ISNULL(@EAAllowVTOModification,[EAAllowVTOModification]) END)
		,	[EAAllowFlightGateModification]=(CASE ISNULL(@NullOverrideEAAllowFlightGateModification,0) WHEN 1 THEN @EAAllowFlightGateModification ELSE ISNULL(@EAAllowFlightGateModification,[EAAllowFlightGateModification]) END)
		,	[EATankDiffPercentage]=(CASE ISNULL(@NullOverrideEATankDiffPercentage,0) WHEN 1 THEN @EATankDiffPercentage ELSE ISNULL(@EATankDiffPercentage,[EATankDiffPercentage]) END)
		,	[EAWingBalancePercentage]=(CASE ISNULL(@NullOverrideEAWingBalancePercentage,0) WHEN 1 THEN @EAWingBalancePercentage ELSE ISNULL(@EAWingBalancePercentage,[EAWingBalancePercentage]) END)
		,	[EABypassDistributionTolerance]=(CASE ISNULL(@NullOverrideEABypassDistributionTolerance,0) WHEN 1 THEN @EABypassDistributionTolerance ELSE ISNULL(@EABypassDistributionTolerance,[EABypassDistributionTolerance]) END)
		,	[EAVehicleIDCheck]=(CASE ISNULL(@NullOverrideEAVehicleIDCheck,0) WHEN 1 THEN @EAVehicleIDCheck ELSE ISNULL(@EAVehicleIDCheck,[EAVehicleIDCheck]) END)
		,	[EAGSEFuelMustMatch]=(CASE ISNULL(@NullOverrideEAGSEFuelMustMatch,0) WHEN 1 THEN @EAGSEFuelMustMatch ELSE ISNULL(@EAGSEFuelMustMatch,[EAGSEFuelMustMatch]) END)
		,	[EAAllowManualMeter]=(CASE ISNULL(@NullOverrideEAAllowManualMeter,0) WHEN 1 THEN @EAAllowManualMeter ELSE ISNULL(@EAAllowManualMeter,[EAAllowManualMeter]) END)
		,	[EAUseValidationLogicGATrans]=(CASE ISNULL(@NullOverrideEAUseValidationLogicGATrans,0) WHEN 1 THEN @EAUseValidationLogicGATrans ELSE ISNULL(@EAUseValidationLogicGATrans,[EAUseValidationLogicGATrans]) END)
		,	[EAAllowShipNumberModification]=(CASE ISNULL(@NullOverrideEAAllowShipNumberModification,0) WHEN 1 THEN @EAAllowShipNumberModification ELSE ISNULL(@EAAllowShipNumberModification,[EAAllowShipNumberModification]) END)
		,	[EAAllowAircraftTypeModification]=(CASE ISNULL(@NullOverrideEAAllowAircraftTypeModification,0) WHEN 1 THEN @EAAllowAircraftTypeModification ELSE ISNULL(@EAAllowAircraftTypeModification,[EAAllowAircraftTypeModification]) END)
		,	[EAAllowDestinationModification]=(CASE ISNULL(@NullOverrideEAAllowDestinationModification,0) WHEN 1 THEN @EAAllowDestinationModification ELSE ISNULL(@EAAllowDestinationModification,[EAAllowDestinationModification]) END)
		,	[EADestination]=(CASE ISNULL(@NullOverrideEADestination,0) WHEN 1 THEN @EADestination ELSE ISNULL(@EADestination,[EADestination]) END)
		,	[EATicketPrinting]=(CASE ISNULL(@NullOverrideEATicketPrinting,0) WHEN 1 THEN @EATicketPrinting ELSE ISNULL(@EATicketPrinting,[EATicketPrinting]) END)
		,	[EAAircraftType]=(CASE ISNULL(@NullOverrideEAAircraftType,0) WHEN 1 THEN @EAAircraftType ELSE ISNULL(@EAAircraftType,[EAAircraftType]) END)
		,	[EAShipNumber]=(CASE ISNULL(@NullOverrideEAShipNumber,0) WHEN 1 THEN @EAShipNumber ELSE ISNULL(@EAShipNumber,[EAShipNumber]) END)
		,	[EAGateNumber]=(CASE ISNULL(@NullOverrideEAGateNumber,0) WHEN 1 THEN @EAGateNumber ELSE ISNULL(@EAGateNumber,[EAGateNumber]) END)
		,	[EAMeterTotal]=(CASE ISNULL(@NullOverrideEAMeterTotal,0) WHEN 1 THEN @EAMeterTotal ELSE ISNULL(@EAMeterTotal,[EAMeterTotal]) END)
		,	[EAVolumePumped]=(CASE ISNULL(@NullOverrideEAVolumePumped,0) WHEN 1 THEN @EAVolumePumped ELSE ISNULL(@EAVolumePumped,[EAVolumePumped]) END)
		,	[EATankCapacity]=(CASE ISNULL(@NullOverrideEATankCapacity,0) WHEN 1 THEN @EATankCapacity ELSE ISNULL(@EATankCapacity,[EATankCapacity]) END)
		,	[EquipmentType]=(CASE ISNULL(@NullOverrideEquipmentType,0) WHEN 1 THEN @EquipmentType ELSE ISNULL(@EquipmentType,[EquipmentType]) END)
		,	[ForeignKeyToMapEquipment]=(CASE ISNULL(@NullOverrideForeignKeyToMapEquipment,0) WHEN 1 THEN @ForeignKeyToMapEquipment ELSE ISNULL(@ForeignKeyToMapEquipment,[ForeignKeyToMapEquipment]) END)
		,	[IssueTransaction]=(CASE ISNULL(@NullOverrideIssueTransaction,0) WHEN 1 THEN @IssueTransaction ELSE ISNULL(@IssueTransaction,[IssueTransaction]) END)
		,	[DefuelTransaction]=(CASE ISNULL(@NullOverrideDefuelTransaction,0) WHEN 1 THEN @DefuelTransaction ELSE ISNULL(@DefuelTransaction,[DefuelTransaction]) END)
		,	[RotationTransaction]=(CASE ISNULL(@NullOverrideRotationTransaction,0) WHEN 1 THEN @RotationTransaction ELSE ISNULL(@RotationTransaction,[RotationTransaction]) END)
		,	[MeterCloseout]=(CASE ISNULL(@NullOverrideMeterCloseout,0) WHEN 1 THEN @MeterCloseout ELSE ISNULL(@MeterCloseout,[MeterCloseout]) END)
		,	[DeIceTransaction]=(CASE ISNULL(@NullOverrideDeIceTransaction,0) WHEN 1 THEN @DeIceTransaction ELSE ISNULL(@DeIceTransaction,[DeIceTransaction]) END)
		,	[GSETransaction]=(CASE ISNULL(@NullOverrideGSETransaction,0) WHEN 1 THEN @GSETransaction ELSE ISNULL(@GSETransaction,[GSETransaction]) END)
		,	[ManualConsumer]=(CASE ISNULL(@NullOverrideManualConsumer,0) WHEN 1 THEN @ManualConsumer ELSE ISNULL(@ManualConsumer,[ManualConsumer]) END)
		,	[ManualVendor]=(CASE ISNULL(@NullOverrideManualVendor,0) WHEN 1 THEN @ManualVendor ELSE ISNULL(@ManualVendor,[ManualVendor]) END)
		,	[ManualShipper]=(CASE ISNULL(@NullOverrideManualShipper,0) WHEN 1 THEN @ManualShipper ELSE ISNULL(@ManualShipper,[ManualShipper]) END)
		,	[ManualManager]=(CASE ISNULL(@NullOverrideManualManager,0) WHEN 1 THEN @ManualManager ELSE ISNULL(@ManualManager,[ManualManager]) END)
		,	[ManualSupplier]=(CASE ISNULL(@NullOverrideManualSupplier,0) WHEN 1 THEN @ManualSupplier ELSE ISNULL(@ManualSupplier,[ManualSupplier]) END)
		,	[ManualBillTo]=(CASE ISNULL(@NullOverrideManualBillTo,0) WHEN 1 THEN @ManualBillTo ELSE ISNULL(@ManualBillTo,[ManualBillTo]) END)
		,	[ManualProduct]=(CASE ISNULL(@NullOverrideManualProduct,0) WHEN 1 THEN @ManualProduct ELSE ISNULL(@ManualProduct,[ManualProduct]) END)
		,	[CloseoutConsumer]=(CASE ISNULL(@NullOverrideCloseoutConsumer,0) WHEN 1 THEN @CloseoutConsumer ELSE ISNULL(@CloseoutConsumer,[CloseoutConsumer]) END)
		,	[CloseoutOwner]=(CASE ISNULL(@NullOverrideCloseoutOwner,0) WHEN 1 THEN @CloseoutOwner ELSE ISNULL(@CloseoutOwner,[CloseoutOwner]) END)
		,	[CloseoutVendor]=(CASE ISNULL(@NullOverrideCloseoutVendor,0) WHEN 1 THEN @CloseoutVendor ELSE ISNULL(@CloseoutVendor,[CloseoutVendor]) END)
		,	[ManualStationID]=(CASE ISNULL(@NullOverrideManualStationID,0) WHEN 1 THEN @ManualStationID ELSE ISNULL(@ManualStationID,[ManualStationID]) END)
		,	[InhibitOverridingTemperature]=(CASE ISNULL(@NullOverrideInhibitOverridingTemperature,0) WHEN 1 THEN @InhibitOverridingTemperature ELSE ISNULL(@InhibitOverridingTemperature,[InhibitOverridingTemperature]) END)
		,	[ManualTemperature]=(CASE ISNULL(@NullOverrideManualTemperature,0) WHEN 1 THEN @ManualTemperature ELSE ISNULL(@ManualTemperature,[ManualTemperature]) END)
		,	[ManualDensity]=(CASE ISNULL(@NullOverrideManualDensity,0) WHEN 1 THEN @ManualDensity ELSE ISNULL(@ManualDensity,[ManualDensity]) END)
		,	[HasDCU]=(CASE ISNULL(@NullOverrideHasDCU,0) WHEN 1 THEN @HasDCU ELSE ISNULL(@HasDCU,[HasDCU]) END)
		,	[BluetoothDCU]=(CASE ISNULL(@NullOverrideBluetoothDCU,0) WHEN 1 THEN @BluetoothDCU ELSE ISNULL(@BluetoothDCU,[BluetoothDCU]) END)
		,	[LogDCUActions]=(CASE ISNULL(@NullOverrideLogDCUActions,0) WHEN 1 THEN @LogDCUActions ELSE ISNULL(@LogDCUActions,[LogDCUActions]) END)
		,	[HasAveryHardoll]=(CASE ISNULL(@NullOverrideHasAveryHardoll,0) WHEN 1 THEN @HasAveryHardoll ELSE ISNULL(@HasAveryHardoll,[HasAveryHardoll]) END)
		,	[DCUComPort]=(CASE ISNULL(@NullOverrideDCUComPort,0) WHEN 1 THEN @DCUComPort ELSE ISNULL(@DCUComPort,[DCUComPort]) END)
		,	[DCUReadRetry]=(CASE ISNULL(@NullOverrideDCUReadRetry,0) WHEN 1 THEN @DCUReadRetry ELSE ISNULL(@DCUReadRetry,[DCUReadRetry]) END)
		,	[DCUDisconnectDelay]=(CASE ISNULL(@NullOverrideDCUDisconnectDelay,0) WHEN 1 THEN @DCUDisconnectDelay ELSE ISNULL(@DCUDisconnectDelay,[DCUDisconnectDelay]) END)
		,	[DCUCommunicationFailRestart]=(CASE ISNULL(@NullOverrideDCUCommunicationFailRestart,0) WHEN 1 THEN @DCUCommunicationFailRestart ELSE ISNULL(@DCUCommunicationFailRestart,[DCUCommunicationFailRestart]) END)
		,	[AveryHardollComPort]=(CASE ISNULL(@NullOverrideAveryHardollComPort,0) WHEN 1 THEN @AveryHardollComPort ELSE ISNULL(@AveryHardollComPort,[AveryHardollComPort]) END)
		,	[AveryHardollMeterID]=(CASE ISNULL(@NullOverrideAveryHardollMeterID,0) WHEN 1 THEN @AveryHardollMeterID ELSE ISNULL(@AveryHardollMeterID,[AveryHardollMeterID]) END)
		,	[ConfirmFuelCaps]=(CASE ISNULL(@NullOverrideConfirmFuelCaps,0) WHEN 1 THEN @ConfirmFuelCaps ELSE ISNULL(@ConfirmFuelCaps,[ConfirmFuelCaps]) END)
		,	[VTOEnabled]=(CASE ISNULL(@NullOverrideVTOEnabled,0) WHEN 1 THEN @VTOEnabled ELSE ISNULL(@VTOEnabled,[VTOEnabled]) END)
		,	[EnabledInOpGauges]=(CASE ISNULL(@NullOverrideEnabledInOpGauges,0) WHEN 1 THEN @EnabledInOpGauges ELSE ISNULL(@EnabledInOpGauges,[EnabledInOpGauges]) END)
		,	[UseDispensingVehicleGSETrans]=(CASE ISNULL(@NullOverrideUseDispensingVehicleGSETrans,0) WHEN 1 THEN @UseDispensingVehicleGSETrans ELSE ISNULL(@UseDispensingVehicleGSETrans,[UseDispensingVehicleGSETrans]) END)
		,	[GSEWaitMSecForGetMeter]=(CASE ISNULL(@NullOverrideGSEWaitMSecForGetMeter,0) WHEN 1 THEN @GSEWaitMSecForGetMeter ELSE ISNULL(@GSEWaitMSecForGetMeter,[GSEWaitMSecForGetMeter]) END)
		,	[GSEInactiveLogoutMinutes]=(CASE ISNULL(@NullOverrideGSEInactiveLogoutMinutes,0) WHEN 1 THEN @GSEInactiveLogoutMinutes ELSE ISNULL(@GSEInactiveLogoutMinutes,[GSEInactiveLogoutMinutes]) END)
		,	[GSEInactiveTimeout]=(CASE ISNULL(@NullOverrideGSEInactiveTimeout,0) WHEN 1 THEN @GSEInactiveTimeout ELSE ISNULL(@GSEInactiveTimeout,[GSEInactiveTimeout]) END)
		,	[BarcodeInvalidWarningSeconds]=(CASE ISNULL(@NullOverrideBarcodeInvalidWarningSeconds,0) WHEN 1 THEN @BarcodeInvalidWarningSeconds ELSE ISNULL(@BarcodeInvalidWarningSeconds,[BarcodeInvalidWarningSeconds]) END)
		,	[DeIceBlendDefault]=(CASE ISNULL(@NullOverrideDeIceBlendDefault,0) WHEN 1 THEN @DeIceBlendDefault ELSE ISNULL(@DeIceBlendDefault,[DeIceBlendDefault]) END)
		,	[CommunicationTimeoutSeconds]=(CASE ISNULL(@NullOverrideCommunicationTimeoutSeconds,0) WHEN 1 THEN @CommunicationTimeoutSeconds ELSE ISNULL(@CommunicationTimeoutSeconds,[CommunicationTimeoutSeconds]) END)
		,	[ConnectionRetries]=(CASE ISNULL(@NullOverrideConnectionRetries,0) WHEN 1 THEN @ConnectionRetries ELSE ISNULL(@ConnectionRetries,[ConnectionRetries]) END)
		,	[ConnectionRetryTimeout]=(CASE ISNULL(@NullOverrideConnectionRetryTimeout,0) WHEN 1 THEN @ConnectionRetryTimeout ELSE ISNULL(@ConnectionRetryTimeout,[ConnectionRetryTimeout]) END)
		,	[ConnectionType]=(CASE ISNULL(@NullOverrideConnectionType,0) WHEN 1 THEN @ConnectionType ELSE ISNULL(@ConnectionType,[ConnectionType]) END)
		,	[UpdateInterval]=(CASE ISNULL(@NullOverrideUpdateInterval,0) WHEN 1 THEN @UpdateInterval ELSE ISNULL(@UpdateInterval,[UpdateInterval]) END)
		,	[PingVerificationIPAddress]=(CASE ISNULL(@NullOverridePingVerificationIPAddress,0) WHEN 1 THEN @PingVerificationIPAddress ELSE ISNULL(@PingVerificationIPAddress,[PingVerificationIPAddress]) END)
		,	[VehicleUpdateInterval]=(CASE ISNULL(@NullOverrideVehicleUpdateInterval,0) WHEN 1 THEN @VehicleUpdateInterval ELSE ISNULL(@VehicleUpdateInterval,[VehicleUpdateInterval]) END)
		,	[PresubmitDelay]=(CASE ISNULL(@NullOverridePresubmitDelay,0) WHEN 1 THEN @PresubmitDelay ELSE ISNULL(@PresubmitDelay,[PresubmitDelay]) END)
		,	[VerificationIPAddress]=(CASE ISNULL(@NullOverrideVerificationIPAddress,0) WHEN 1 THEN @VerificationIPAddress ELSE ISNULL(@VerificationIPAddress,[VerificationIPAddress]) END)
		,	[UpdatedDate]=ISNULL(@UpdatedDate,SYSDATETIMEOFFSET())
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		WHERE	MobileDeviceProfileGuid=@MobileDeviceProfileGuid;
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblMobileDeviceProfile]           
		WHERE MobileDeviceProfileGuid=@MobileDeviceProfileGuid;
	
 
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
						+ 'Procedure Name: gsp_MobileDeviceProfileUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
