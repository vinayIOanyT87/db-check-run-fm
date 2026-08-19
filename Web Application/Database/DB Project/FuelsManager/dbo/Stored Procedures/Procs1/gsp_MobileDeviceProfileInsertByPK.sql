CREATE PROCEDURE [dbo].[gsp_MobileDeviceProfileInsertByPK]
(
		@MobileDeviceProfileGuid uniqueidentifier=NULL OUTPUT
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
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_MobileDeviceProfileInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2852767 -05:00
	-- Purpose: Insert into table [dbo].[tblMobileDeviceProfile]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @MobileDeviceProfileGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblMobileDeviceProfile] 
		(
			[MobileDeviceProfileGuid]
		,	[SiteGuid]
		,	[ProfileID]
		,	[Description]
		,	[ShowProductScreen]
		,	[GenerateTicketNumber]
		,	[ShowOperatorFieldInFlightList]
		,	[UseDefaultPrinter]
		,	[DefaultPrinter]
		,	[AdminPassword]
		,	[ShutdownHotKey]
		,	[PrinterCOMPort]
		,	[SearchType]
		,	[LoggingOption]
		,	[AllowableFailedLoginAttempts]
		,	[FuelDistributionPrecision]
		,	[MakeDefaultProfile]
		,	[VehicleID]
		,	[MonitorScreenTransitionTiming]
		,	[BypassFsrCheckOnScreenTrans]
		,	[ShowFuelUpdateCheckStatusWin]
		,	[RTDTemperatureRangeMin]
		,	[RTDTemperatureRangeMax]
		,	[DefaultTemperature]
		,	[StrictUserValidation]
		,	[VerifyFuelingEquipment]
		,	[AllowEditRequiredFuelLoad]
		,	[AllowBackAfterArrivalScreen]
		,	[AllowBackAfterTicketPrinted]
		,	[RequirePrint]
		,	[TotalFuelLoadCheck]
		,	[VolumetricThresholdValidation]
		,	[ValidateShipNumber]
		,	[AllowVTOModification]
		,	[AllowFlightGateModification]
		,	[TankPositionBalanceVerification]
		,	[TankPositionBalancePercentage]
		,	[OverrideWingBalancePercentVar]
		,	[BypassDistributionTolerance]
		,	[VehicleIDCheck]
		,	[GSEFuelMustMatch]
		,	[AllowManualMeter]
		,	[UseValidLogicGATrans]
		,	[AllowShipNumberModification]
		,	[AllowAircraftTypeModification]
		,	[AllowDestinationModification]
		,	[TicketPrinting]
		,	[AircraftTypeVerification]
		,	[Destination]
		,	[Gate]
		,	[ShipNumber]
		,	[MeterTotal]
		,	[VolumePumped]
		,	[TankCapacity]
		,	[EAStrictUserValidation]
		,	[EAVerifyFuelingEquipment]
		,	[EAAllowEditOfRequiredFuelLoad]
		,	[EAAllowBackAfterArrivalScreen]
		,	[EAAllowBackAfterTicketPrinted]
		,	[EARequirePrint]
		,	[EATotalFuelLoad]
		,	[EAVolumetricThresholdValidation]
		,	[EAValidateShipNumber]
		,	[EAAllowVTOModification]
		,	[EAAllowFlightGateModification]
		,	[EATankDiffPercentage]
		,	[EAWingBalancePercentage]
		,	[EABypassDistributionTolerance]
		,	[EAVehicleIDCheck]
		,	[EAGSEFuelMustMatch]
		,	[EAAllowManualMeter]
		,	[EAUseValidationLogicGATrans]
		,	[EAAllowShipNumberModification]
		,	[EAAllowAircraftTypeModification]
		,	[EAAllowDestinationModification]
		,	[EADestination]
		,	[EATicketPrinting]
		,	[EAAircraftType]
		,	[EAShipNumber]
		,	[EAGateNumber]
		,	[EAMeterTotal]
		,	[EAVolumePumped]
		,	[EATankCapacity]
		,	[EquipmentType]
		,	[ForeignKeyToMapEquipment]
		,	[IssueTransaction]
		,	[DefuelTransaction]
		,	[RotationTransaction]
		,	[MeterCloseout]
		,	[DeIceTransaction]
		,	[GSETransaction]
		,	[ManualConsumer]
		,	[ManualVendor]
		,	[ManualShipper]
		,	[ManualManager]
		,	[ManualSupplier]
		,	[ManualBillTo]
		,	[ManualProduct]
		,	[CloseoutConsumer]
		,	[CloseoutOwner]
		,	[CloseoutVendor]
		,	[ManualStationID]
		,	[InhibitOverridingTemperature]
		,	[ManualTemperature]
		,	[ManualDensity]
		,	[HasDCU]
		,	[BluetoothDCU]
		,	[LogDCUActions]
		,	[HasAveryHardoll]
		,	[DCUComPort]
		,	[DCUReadRetry]
		,	[DCUDisconnectDelay]
		,	[DCUCommunicationFailRestart]
		,	[AveryHardollComPort]
		,	[AveryHardollMeterID]
		,	[ConfirmFuelCaps]
		,	[VTOEnabled]
		,	[EnabledInOpGauges]
		,	[UseDispensingVehicleGSETrans]
		,	[GSEWaitMSecForGetMeter]
		,	[GSEInactiveLogoutMinutes]
		,	[GSEInactiveTimeout]
		,	[BarcodeInvalidWarningSeconds]
		,	[DeIceBlendDefault]
		,	[CommunicationTimeoutSeconds]
		,	[ConnectionRetries]
		,	[ConnectionRetryTimeout]
		,	[ConnectionType]
		,	[UpdateInterval]
		,	[PingVerificationIPAddress]
		,	[VehicleUpdateInterval]
		,	[PresubmitDelay]
		,	[VerificationIPAddress]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@MobileDeviceProfileGuid
		,	@SiteGuid
		,	@ProfileID
		,	@Description
		,	@ShowProductScreen
		,	@GenerateTicketNumber
		,	@ShowOperatorFieldInFlightList
		,	@UseDefaultPrinter
		,	@DefaultPrinter
		,	@AdminPassword
		,	@ShutdownHotKey
		,	@PrinterCOMPort
		,	@SearchType
		,	@LoggingOption
		,	@AllowableFailedLoginAttempts
		,	@FuelDistributionPrecision
		,	@MakeDefaultProfile
		,	@VehicleID
		,	@MonitorScreenTransitionTiming
		,	@BypassFsrCheckOnScreenTrans
		,	@ShowFuelUpdateCheckStatusWin
		,	@RTDTemperatureRangeMin
		,	@RTDTemperatureRangeMax
		,	@DefaultTemperature
		,	@StrictUserValidation
		,	@VerifyFuelingEquipment
		,	@AllowEditRequiredFuelLoad
		,	@AllowBackAfterArrivalScreen
		,	@AllowBackAfterTicketPrinted
		,	@RequirePrint
		,	@TotalFuelLoadCheck
		,	@VolumetricThresholdValidation
		,	@ValidateShipNumber
		,	@AllowVTOModification
		,	@AllowFlightGateModification
		,	@TankPositionBalanceVerification
		,	@TankPositionBalancePercentage
		,	@OverrideWingBalancePercentVar
		,	@BypassDistributionTolerance
		,	@VehicleIDCheck
		,	@GSEFuelMustMatch
		,	@AllowManualMeter
		,	@UseValidLogicGATrans
		,	@AllowShipNumberModification
		,	@AllowAircraftTypeModification
		,	@AllowDestinationModification
		,	@TicketPrinting
		,	@AircraftTypeVerification
		,	@Destination
		,	@Gate
		,	@ShipNumber
		,	@MeterTotal
		,	@VolumePumped
		,	@TankCapacity
		,	@EAStrictUserValidation
		,	@EAVerifyFuelingEquipment
		,	@EAAllowEditOfRequiredFuelLoad
		,	@EAAllowBackAfterArrivalScreen
		,	@EAAllowBackAfterTicketPrinted
		,	@EARequirePrint
		,	@EATotalFuelLoad
		,	@EAVolumetricThresholdValidation
		,	@EAValidateShipNumber
		,	@EAAllowVTOModification
		,	@EAAllowFlightGateModification
		,	@EATankDiffPercentage
		,	@EAWingBalancePercentage
		,	@EABypassDistributionTolerance
		,	@EAVehicleIDCheck
		,	@EAGSEFuelMustMatch
		,	@EAAllowManualMeter
		,	@EAUseValidationLogicGATrans
		,	@EAAllowShipNumberModification
		,	@EAAllowAircraftTypeModification
		,	@EAAllowDestinationModification
		,	@EADestination
		,	@EATicketPrinting
		,	@EAAircraftType
		,	@EAShipNumber
		,	@EAGateNumber
		,	@EAMeterTotal
		,	@EAVolumePumped
		,	@EATankCapacity
		,	@EquipmentType
		,	@ForeignKeyToMapEquipment
		,	@IssueTransaction
		,	@DefuelTransaction
		,	@RotationTransaction
		,	@MeterCloseout
		,	@DeIceTransaction
		,	@GSETransaction
		,	@ManualConsumer
		,	@ManualVendor
		,	@ManualShipper
		,	@ManualManager
		,	@ManualSupplier
		,	@ManualBillTo
		,	@ManualProduct
		,	@CloseoutConsumer
		,	@CloseoutOwner
		,	@CloseoutVendor
		,	@ManualStationID
		,	@InhibitOverridingTemperature
		,	@ManualTemperature
		,	@ManualDensity
		,	@HasDCU
		,	@BluetoothDCU
		,	@LogDCUActions
		,	@HasAveryHardoll
		,	@DCUComPort
		,	@DCUReadRetry
		,	@DCUDisconnectDelay
		,	@DCUCommunicationFailRestart
		,	@AveryHardollComPort
		,	@AveryHardollMeterID
		,	@ConfirmFuelCaps
		,	@VTOEnabled
		,	@EnabledInOpGauges
		,	@UseDispensingVehicleGSETrans
		,	@GSEWaitMSecForGetMeter
		,	@GSEInactiveLogoutMinutes
		,	@GSEInactiveTimeout
		,	@BarcodeInvalidWarningSeconds
		,	@DeIceBlendDefault
		,	@CommunicationTimeoutSeconds
		,	@ConnectionRetries
		,	@ConnectionRetryTimeout
		,	@ConnectionType
		,	@UpdateInterval
		,	@PingVerificationIPAddress
		,	@VehicleUpdateInterval
		,	@PresubmitDelay
		,	@VerificationIPAddress
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
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
						+ 'Procedure Name: gsp_MobileDeviceProfileInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
