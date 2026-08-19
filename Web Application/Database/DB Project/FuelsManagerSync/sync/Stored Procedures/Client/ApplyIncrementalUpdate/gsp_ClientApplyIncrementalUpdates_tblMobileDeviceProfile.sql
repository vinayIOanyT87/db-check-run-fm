-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMobileDeviceProfile
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblMobileDeviceProfile]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@MobileDeviceProfileGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ProfileID nvarchar(50),
@Description nvarchar(200),
@ShowProductScreen bit,
@GenerateTicketNumber bit,
@ShowOperatorFieldInFlightList bit,
@UseDefaultPrinter bit,
@DefaultPrinter nvarchar(50),
@AdminPassword varbinary(256),
@ShutdownHotKey nvarchar(50),
@PrinterCOMPort nvarchar(4),
@SearchType int,
@LoggingOption bit,
@AllowableFailedLoginAttempts int,
@FuelDistributionPrecision int,
@MakeDefaultProfile bit,
@VehicleID nvarchar(50),
@MonitorScreenTransitionTiming bit,
@BypassFsrCheckOnScreenTrans bit,
@ShowFuelUpdateCheckStatusWin bit,
@RTDTemperatureRangeMin float,
@RTDTemperatureRangeMax float,
@DefaultTemperature float,
@StrictUserValidation bit,
@VerifyFuelingEquipment bit,
@AllowEditRequiredFuelLoad bit,
@AllowBackAfterArrivalScreen bit,
@AllowBackAfterTicketPrinted bit,
@RequirePrint bit,
@TotalFuelLoadCheck bit,
@VolumetricThresholdValidation bit,
@ValidateShipNumber bit,
@AllowVTOModification bit,
@AllowFlightGateModification bit,
@TankPositionBalanceVerification int,
@TankPositionBalancePercentage float,
@OverrideWingBalancePercentVar bit,
@BypassDistributionTolerance bit,
@VehicleIDCheck bit,
@GSEFuelMustMatch bit,
@AllowManualMeter bit,
@UseValidLogicGATrans bit,
@AllowShipNumberModification bit,
@AllowAircraftTypeModification bit,
@AllowDestinationModification bit,
@TicketPrinting int,
@AircraftTypeVerification int,
@Destination int,
@Gate int,
@ShipNumber int,
@MeterTotal int,
@VolumePumped int,
@TankCapacity int,
@EAStrictUserValidation bit,
@EAVerifyFuelingEquipment bit,
@EAAllowEditOfRequiredFuelLoad bit,
@EAAllowBackAfterArrivalScreen bit,
@EAAllowBackAfterTicketPrinted bit,
@EARequirePrint bit,
@EATotalFuelLoad bit,
@EAVolumetricThresholdValidation bit,
@EAValidateShipNumber bit,
@EAAllowVTOModification bit,
@EAAllowFlightGateModification bit,
@EATankDiffPercentage bit,
@EAWingBalancePercentage bit,
@EABypassDistributionTolerance bit,
@EAVehicleIDCheck bit,
@EAGSEFuelMustMatch bit,
@EAAllowManualMeter bit,
@EAUseValidationLogicGATrans bit,
@EAAllowShipNumberModification bit,
@EAAllowAircraftTypeModification bit,
@EAAllowDestinationModification bit,
@EADestination bit,
@EATicketPrinting bit,
@EAAircraftType bit,
@EAShipNumber bit,
@EAGateNumber bit,
@EAMeterTotal bit,
@EAVolumePumped bit,
@EATankCapacity bit,
@EquipmentType int,
@ForeignKeyToMapEquipment uniqueidentifier,
@IssueTransaction uniqueidentifier,
@DefuelTransaction uniqueidentifier,
@RotationTransaction uniqueidentifier,
@MeterCloseout uniqueidentifier,
@DeIceTransaction uniqueidentifier,
@GSETransaction uniqueidentifier,
@ManualConsumer uniqueidentifier,
@ManualVendor uniqueidentifier,
@ManualShipper uniqueidentifier,
@ManualManager uniqueidentifier,
@ManualSupplier uniqueidentifier,
@ManualBillTo uniqueidentifier,
@ManualProduct uniqueidentifier,
@CloseoutConsumer uniqueidentifier,
@CloseoutOwner uniqueidentifier,
@CloseoutVendor uniqueidentifier,
@ManualStationID int,
@InhibitOverridingTemperature bit,
@ManualTemperature float,
@ManualDensity float,
@HasDCU bit,
@BluetoothDCU bit,
@LogDCUActions bit,
@HasAveryHardoll bit,
@DCUComPort nvarchar(4),
@DCUReadRetry int,
@DCUDisconnectDelay int,
@DCUCommunicationFailRestart int,
@AveryHardollComPort nvarchar(4),
@AveryHardollMeterID nvarchar(4),
@ConfirmFuelCaps bit,
@VTOEnabled bit,
@EnabledInOpGauges bit,
@UseDispensingVehicleGSETrans bit,
@GSEWaitMSecForGetMeter int,
@GSEInactiveLogoutMinutes int,
@GSEInactiveTimeout int,
@BarcodeInvalidWarningSeconds int,
@DeIceBlendDefault float,
@CommunicationTimeoutSeconds int,
@ConnectionRetries int,
@ConnectionRetryTimeout int,
@ConnectionType int,
@UpdateInterval int,
@PingVerificationIPAddress bit,
@VehicleUpdateInterval int,
@PresubmitDelay int,
@VerificationIPAddress nvarchar(15),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblMobileDeviceProfile] CT
                        WHERE CT.PK_MobileDeviceProfileGuid = @MobileDeviceProfileGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblMobileDeviceProfile].[MobileDeviceProfileGuid],[dbo].[tblMobileDeviceProfile].[SiteGuid],[dbo].[tblMobileDeviceProfile].[ProfileID],[dbo].[tblMobileDeviceProfile].[Description],[dbo].[tblMobileDeviceProfile].[ShowProductScreen],[dbo].[tblMobileDeviceProfile].[GenerateTicketNumber],[dbo].[tblMobileDeviceProfile].[ShowOperatorFieldInFlightList],[dbo].[tblMobileDeviceProfile].[UseDefaultPrinter],[dbo].[tblMobileDeviceProfile].[DefaultPrinter],[dbo].[tblMobileDeviceProfile].[AdminPassword],[dbo].[tblMobileDeviceProfile].[ShutdownHotKey],[dbo].[tblMobileDeviceProfile].[PrinterCOMPort],[dbo].[tblMobileDeviceProfile].[SearchType],[dbo].[tblMobileDeviceProfile].[LoggingOption],[dbo].[tblMobileDeviceProfile].[AllowableFailedLoginAttempts],[dbo].[tblMobileDeviceProfile].[FuelDistributionPrecision],[dbo].[tblMobileDeviceProfile].[MakeDefaultProfile],[dbo].[tblMobileDeviceProfile].[VehicleID],[dbo].[tblMobileDeviceProfile].[MonitorScreenTransitionTiming],[dbo].[tblMobileDeviceProfile].[BypassFsrCheckOnScreenTrans],[dbo].[tblMobileDeviceProfile].[ShowFuelUpdateCheckStatusWin],[dbo].[tblMobileDeviceProfile].[RTDTemperatureRangeMin],[dbo].[tblMobileDeviceProfile].[RTDTemperatureRangeMax],[dbo].[tblMobileDeviceProfile].[DefaultTemperature],[dbo].[tblMobileDeviceProfile].[StrictUserValidation],[dbo].[tblMobileDeviceProfile].[VerifyFuelingEquipment],[dbo].[tblMobileDeviceProfile].[AllowEditRequiredFuelLoad],[dbo].[tblMobileDeviceProfile].[AllowBackAfterArrivalScreen],[dbo].[tblMobileDeviceProfile].[AllowBackAfterTicketPrinted],[dbo].[tblMobileDeviceProfile].[RequirePrint],[dbo].[tblMobileDeviceProfile].[TotalFuelLoadCheck],[dbo].[tblMobileDeviceProfile].[VolumetricThresholdValidation],[dbo].[tblMobileDeviceProfile].[ValidateShipNumber],[dbo].[tblMobileDeviceProfile].[AllowVTOModification],[dbo].[tblMobileDeviceProfile].[AllowFlightGateModification],[dbo].[tblMobileDeviceProfile].[TankPositionBalanceVerification],[dbo].[tblMobileDeviceProfile].[TankPositionBalancePercentage],[dbo].[tblMobileDeviceProfile].[OverrideWingBalancePercentVar],[dbo].[tblMobileDeviceProfile].[BypassDistributionTolerance],[dbo].[tblMobileDeviceProfile].[VehicleIDCheck],[dbo].[tblMobileDeviceProfile].[GSEFuelMustMatch],[dbo].[tblMobileDeviceProfile].[AllowManualMeter],[dbo].[tblMobileDeviceProfile].[UseValidLogicGATrans],[dbo].[tblMobileDeviceProfile].[AllowShipNumberModification],[dbo].[tblMobileDeviceProfile].[AllowAircraftTypeModification],[dbo].[tblMobileDeviceProfile].[AllowDestinationModification],[dbo].[tblMobileDeviceProfile].[TicketPrinting],[dbo].[tblMobileDeviceProfile].[AircraftTypeVerification],[dbo].[tblMobileDeviceProfile].[Destination],[dbo].[tblMobileDeviceProfile].[Gate],[dbo].[tblMobileDeviceProfile].[ShipNumber],[dbo].[tblMobileDeviceProfile].[MeterTotal],[dbo].[tblMobileDeviceProfile].[VolumePumped],[dbo].[tblMobileDeviceProfile].[TankCapacity],[dbo].[tblMobileDeviceProfile].[EAStrictUserValidation],[dbo].[tblMobileDeviceProfile].[EAVerifyFuelingEquipment],[dbo].[tblMobileDeviceProfile].[EAAllowEditOfRequiredFuelLoad],[dbo].[tblMobileDeviceProfile].[EAAllowBackAfterArrivalScreen],[dbo].[tblMobileDeviceProfile].[EAAllowBackAfterTicketPrinted],[dbo].[tblMobileDeviceProfile].[EARequirePrint],[dbo].[tblMobileDeviceProfile].[EATotalFuelLoad],[dbo].[tblMobileDeviceProfile].[EAVolumetricThresholdValidation],[dbo].[tblMobileDeviceProfile].[EAValidateShipNumber],[dbo].[tblMobileDeviceProfile].[EAAllowVTOModification],[dbo].[tblMobileDeviceProfile].[EAAllowFlightGateModification],[dbo].[tblMobileDeviceProfile].[EATankDiffPercentage],[dbo].[tblMobileDeviceProfile].[EAWingBalancePercentage],[dbo].[tblMobileDeviceProfile].[EABypassDistributionTolerance],[dbo].[tblMobileDeviceProfile].[EAVehicleIDCheck],[dbo].[tblMobileDeviceProfile].[EAGSEFuelMustMatch],[dbo].[tblMobileDeviceProfile].[EAAllowManualMeter],[dbo].[tblMobileDeviceProfile].[EAUseValidationLogicGATrans],[dbo].[tblMobileDeviceProfile].[EAAllowShipNumberModification],[dbo].[tblMobileDeviceProfile].[EAAllowAircraftTypeModification],[dbo].[tblMobileDeviceProfile].[EAAllowDestinationModification],[dbo].[tblMobileDeviceProfile].[EADestination],[dbo].[tblMobileDeviceProfile].[EATicketPrinting],[dbo].[tblMobileDeviceProfile].[EAAircraftType],[dbo].[tblMobileDeviceProfile].[EAShipNumber],[dbo].[tblMobileDeviceProfile].[EAGateNumber],[dbo].[tblMobileDeviceProfile].[EAMeterTotal],[dbo].[tblMobileDeviceProfile].[EAVolumePumped],[dbo].[tblMobileDeviceProfile].[EATankCapacity],[dbo].[tblMobileDeviceProfile].[EquipmentType],[dbo].[tblMobileDeviceProfile].[ForeignKeyToMapEquipment],[dbo].[tblMobileDeviceProfile].[IssueTransaction],[dbo].[tblMobileDeviceProfile].[DefuelTransaction],[dbo].[tblMobileDeviceProfile].[RotationTransaction],[dbo].[tblMobileDeviceProfile].[MeterCloseout],[dbo].[tblMobileDeviceProfile].[DeIceTransaction],[dbo].[tblMobileDeviceProfile].[GSETransaction],[dbo].[tblMobileDeviceProfile].[ManualConsumer],[dbo].[tblMobileDeviceProfile].[ManualVendor],[dbo].[tblMobileDeviceProfile].[ManualShipper],[dbo].[tblMobileDeviceProfile].[ManualManager],[dbo].[tblMobileDeviceProfile].[ManualSupplier],[dbo].[tblMobileDeviceProfile].[ManualBillTo],[dbo].[tblMobileDeviceProfile].[ManualProduct],[dbo].[tblMobileDeviceProfile].[CloseoutConsumer],[dbo].[tblMobileDeviceProfile].[CloseoutOwner],[dbo].[tblMobileDeviceProfile].[CloseoutVendor],[dbo].[tblMobileDeviceProfile].[ManualStationID],[dbo].[tblMobileDeviceProfile].[InhibitOverridingTemperature],[dbo].[tblMobileDeviceProfile].[ManualTemperature],[dbo].[tblMobileDeviceProfile].[ManualDensity],[dbo].[tblMobileDeviceProfile].[HasDCU],[dbo].[tblMobileDeviceProfile].[BluetoothDCU],[dbo].[tblMobileDeviceProfile].[LogDCUActions],[dbo].[tblMobileDeviceProfile].[HasAveryHardoll],[dbo].[tblMobileDeviceProfile].[DCUComPort],[dbo].[tblMobileDeviceProfile].[DCUReadRetry],[dbo].[tblMobileDeviceProfile].[DCUDisconnectDelay],[dbo].[tblMobileDeviceProfile].[DCUCommunicationFailRestart],[dbo].[tblMobileDeviceProfile].[AveryHardollComPort],[dbo].[tblMobileDeviceProfile].[AveryHardollMeterID],[dbo].[tblMobileDeviceProfile].[ConfirmFuelCaps],[dbo].[tblMobileDeviceProfile].[VTOEnabled],[dbo].[tblMobileDeviceProfile].[EnabledInOpGauges],[dbo].[tblMobileDeviceProfile].[UseDispensingVehicleGSETrans],[dbo].[tblMobileDeviceProfile].[GSEWaitMSecForGetMeter],[dbo].[tblMobileDeviceProfile].[GSEInactiveLogoutMinutes],[dbo].[tblMobileDeviceProfile].[GSEInactiveTimeout],[dbo].[tblMobileDeviceProfile].[BarcodeInvalidWarningSeconds],[dbo].[tblMobileDeviceProfile].[DeIceBlendDefault],[dbo].[tblMobileDeviceProfile].[CommunicationTimeoutSeconds],[dbo].[tblMobileDeviceProfile].[ConnectionRetries],[dbo].[tblMobileDeviceProfile].[ConnectionRetryTimeout],[dbo].[tblMobileDeviceProfile].[ConnectionType],[dbo].[tblMobileDeviceProfile].[UpdateInterval],[dbo].[tblMobileDeviceProfile].[PingVerificationIPAddress],[dbo].[tblMobileDeviceProfile].[VehicleUpdateInterval],[dbo].[tblMobileDeviceProfile].[PresubmitDelay],[dbo].[tblMobileDeviceProfile].[VerificationIPAddress],[dbo].[tblMobileDeviceProfile].[CreatedDate],[dbo].[tblMobileDeviceProfile].[CreatedBy],[dbo].[tblMobileDeviceProfile].[UpdatedDate],[dbo].[tblMobileDeviceProfile].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblMobileDeviceProfile]
                        INNER JOIN [track].[tblMobileDeviceProfile] CT
                            ON CT.PK_MobileDeviceProfileGuid = [dbo].[tblMobileDeviceProfile].[MobileDeviceProfileGuid] 
                    WHERE CT.PK_MobileDeviceProfileGuid = @MobileDeviceProfileGuid
            ) MERGE existingData
            USING (SELECT @MobileDeviceProfileGuid,@SiteGuid,@ProfileID,@Description,@ShowProductScreen,@GenerateTicketNumber,@ShowOperatorFieldInFlightList,@UseDefaultPrinter,@DefaultPrinter,@AdminPassword,@ShutdownHotKey,@PrinterCOMPort,@SearchType,@LoggingOption,@AllowableFailedLoginAttempts,@FuelDistributionPrecision,@MakeDefaultProfile,@VehicleID,@MonitorScreenTransitionTiming,@BypassFsrCheckOnScreenTrans,@ShowFuelUpdateCheckStatusWin,@RTDTemperatureRangeMin,@RTDTemperatureRangeMax,@DefaultTemperature,@StrictUserValidation,@VerifyFuelingEquipment,@AllowEditRequiredFuelLoad,@AllowBackAfterArrivalScreen,@AllowBackAfterTicketPrinted,@RequirePrint,@TotalFuelLoadCheck,@VolumetricThresholdValidation,@ValidateShipNumber,@AllowVTOModification,@AllowFlightGateModification,@TankPositionBalanceVerification,@TankPositionBalancePercentage,@OverrideWingBalancePercentVar,@BypassDistributionTolerance,@VehicleIDCheck,@GSEFuelMustMatch,@AllowManualMeter,@UseValidLogicGATrans,@AllowShipNumberModification,@AllowAircraftTypeModification,@AllowDestinationModification,@TicketPrinting,@AircraftTypeVerification,@Destination,@Gate,@ShipNumber,@MeterTotal,@VolumePumped,@TankCapacity,@EAStrictUserValidation,@EAVerifyFuelingEquipment,@EAAllowEditOfRequiredFuelLoad,@EAAllowBackAfterArrivalScreen,@EAAllowBackAfterTicketPrinted,@EARequirePrint,@EATotalFuelLoad,@EAVolumetricThresholdValidation,@EAValidateShipNumber,@EAAllowVTOModification,@EAAllowFlightGateModification,@EATankDiffPercentage,@EAWingBalancePercentage,@EABypassDistributionTolerance,@EAVehicleIDCheck,@EAGSEFuelMustMatch,@EAAllowManualMeter,@EAUseValidationLogicGATrans,@EAAllowShipNumberModification,@EAAllowAircraftTypeModification,@EAAllowDestinationModification,@EADestination,@EATicketPrinting,@EAAircraftType,@EAShipNumber,@EAGateNumber,@EAMeterTotal,@EAVolumePumped,@EATankCapacity,@EquipmentType,@ForeignKeyToMapEquipment,@IssueTransaction,@DefuelTransaction,@RotationTransaction,@MeterCloseout,@DeIceTransaction,@GSETransaction,@ManualConsumer,@ManualVendor,@ManualShipper,@ManualManager,@ManualSupplier,@ManualBillTo,@ManualProduct,@CloseoutConsumer,@CloseoutOwner,@CloseoutVendor,@ManualStationID,@InhibitOverridingTemperature,@ManualTemperature,@ManualDensity,@HasDCU,@BluetoothDCU,@LogDCUActions,@HasAveryHardoll,@DCUComPort,@DCUReadRetry,@DCUDisconnectDelay,@DCUCommunicationFailRestart,@AveryHardollComPort,@AveryHardollMeterID,@ConfirmFuelCaps,@VTOEnabled,@EnabledInOpGauges,@UseDispensingVehicleGSETrans,@GSEWaitMSecForGetMeter,@GSEInactiveLogoutMinutes,@GSEInactiveTimeout,@BarcodeInvalidWarningSeconds,@DeIceBlendDefault,@CommunicationTimeoutSeconds,@ConnectionRetries,@ConnectionRetryTimeout,@ConnectionType,@UpdateInterval,@PingVerificationIPAddress,@VehicleUpdateInterval,@PresubmitDelay,@VerificationIPAddress,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([MobileDeviceProfileGuid],[SiteGuid],[ProfileID],[Description],[ShowProductScreen],[GenerateTicketNumber],[ShowOperatorFieldInFlightList],[UseDefaultPrinter],[DefaultPrinter],[AdminPassword],[ShutdownHotKey],[PrinterCOMPort],[SearchType],[LoggingOption],[AllowableFailedLoginAttempts],[FuelDistributionPrecision],[MakeDefaultProfile],[VehicleID],[MonitorScreenTransitionTiming],[BypassFsrCheckOnScreenTrans],[ShowFuelUpdateCheckStatusWin],[RTDTemperatureRangeMin],[RTDTemperatureRangeMax],[DefaultTemperature],[StrictUserValidation],[VerifyFuelingEquipment],[AllowEditRequiredFuelLoad],[AllowBackAfterArrivalScreen],[AllowBackAfterTicketPrinted],[RequirePrint],[TotalFuelLoadCheck],[VolumetricThresholdValidation],[ValidateShipNumber],[AllowVTOModification],[AllowFlightGateModification],[TankPositionBalanceVerification],[TankPositionBalancePercentage],[OverrideWingBalancePercentVar],[BypassDistributionTolerance],[VehicleIDCheck],[GSEFuelMustMatch],[AllowManualMeter],[UseValidLogicGATrans],[AllowShipNumberModification],[AllowAircraftTypeModification],[AllowDestinationModification],[TicketPrinting],[AircraftTypeVerification],[Destination],[Gate],[ShipNumber],[MeterTotal],[VolumePumped],[TankCapacity],[EAStrictUserValidation],[EAVerifyFuelingEquipment],[EAAllowEditOfRequiredFuelLoad],[EAAllowBackAfterArrivalScreen],[EAAllowBackAfterTicketPrinted],[EARequirePrint],[EATotalFuelLoad],[EAVolumetricThresholdValidation],[EAValidateShipNumber],[EAAllowVTOModification],[EAAllowFlightGateModification],[EATankDiffPercentage],[EAWingBalancePercentage],[EABypassDistributionTolerance],[EAVehicleIDCheck],[EAGSEFuelMustMatch],[EAAllowManualMeter],[EAUseValidationLogicGATrans],[EAAllowShipNumberModification],[EAAllowAircraftTypeModification],[EAAllowDestinationModification],[EADestination],[EATicketPrinting],[EAAircraftType],[EAShipNumber],[EAGateNumber],[EAMeterTotal],[EAVolumePumped],[EATankCapacity],[EquipmentType],[ForeignKeyToMapEquipment],[IssueTransaction],[DefuelTransaction],[RotationTransaction],[MeterCloseout],[DeIceTransaction],[GSETransaction],[ManualConsumer],[ManualVendor],[ManualShipper],[ManualManager],[ManualSupplier],[ManualBillTo],[ManualProduct],[CloseoutConsumer],[CloseoutOwner],[CloseoutVendor],[ManualStationID],[InhibitOverridingTemperature],[ManualTemperature],[ManualDensity],[HasDCU],[BluetoothDCU],[LogDCUActions],[HasAveryHardoll],[DCUComPort],[DCUReadRetry],[DCUDisconnectDelay],[DCUCommunicationFailRestart],[AveryHardollComPort],[AveryHardollMeterID],[ConfirmFuelCaps],[VTOEnabled],[EnabledInOpGauges],[UseDispensingVehicleGSETrans],[GSEWaitMSecForGetMeter],[GSEInactiveLogoutMinutes],[GSEInactiveTimeout],[BarcodeInvalidWarningSeconds],[DeIceBlendDefault],[CommunicationTimeoutSeconds],[ConnectionRetries],[ConnectionRetryTimeout],[ConnectionType],[UpdateInterval],[PingVerificationIPAddress],[VehicleUpdateInterval],[PresubmitDelay],[VerificationIPAddress],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[MobileDeviceProfileGuid] = remoteChanges.[MobileDeviceProfileGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [SiteGuid] = remoteChanges.[SiteGuid]
                       ,[ProfileID] = remoteChanges.[ProfileID]
                       ,[Description] = remoteChanges.[Description]
                       ,[ShowProductScreen] = remoteChanges.[ShowProductScreen]
                       ,[GenerateTicketNumber] = remoteChanges.[GenerateTicketNumber]
                       ,[ShowOperatorFieldInFlightList] = remoteChanges.[ShowOperatorFieldInFlightList]
                       ,[UseDefaultPrinter] = remoteChanges.[UseDefaultPrinter]
                       ,[DefaultPrinter] = remoteChanges.[DefaultPrinter]
                       ,[AdminPassword] = remoteChanges.[AdminPassword]
                       ,[ShutdownHotKey] = remoteChanges.[ShutdownHotKey]
                       ,[PrinterCOMPort] = remoteChanges.[PrinterCOMPort]
                       ,[SearchType] = remoteChanges.[SearchType]
                       ,[LoggingOption] = remoteChanges.[LoggingOption]
                       ,[AllowableFailedLoginAttempts] = remoteChanges.[AllowableFailedLoginAttempts]
                       ,[FuelDistributionPrecision] = remoteChanges.[FuelDistributionPrecision]
                       ,[MakeDefaultProfile] = remoteChanges.[MakeDefaultProfile]
                       ,[VehicleID] = remoteChanges.[VehicleID]
                       ,[MonitorScreenTransitionTiming] = remoteChanges.[MonitorScreenTransitionTiming]
                       ,[BypassFsrCheckOnScreenTrans] = remoteChanges.[BypassFsrCheckOnScreenTrans]
                       ,[ShowFuelUpdateCheckStatusWin] = remoteChanges.[ShowFuelUpdateCheckStatusWin]
                       ,[RTDTemperatureRangeMin] = remoteChanges.[RTDTemperatureRangeMin]
                       ,[RTDTemperatureRangeMax] = remoteChanges.[RTDTemperatureRangeMax]
                       ,[DefaultTemperature] = remoteChanges.[DefaultTemperature]
                       ,[StrictUserValidation] = remoteChanges.[StrictUserValidation]
                       ,[VerifyFuelingEquipment] = remoteChanges.[VerifyFuelingEquipment]
                       ,[AllowEditRequiredFuelLoad] = remoteChanges.[AllowEditRequiredFuelLoad]
                       ,[AllowBackAfterArrivalScreen] = remoteChanges.[AllowBackAfterArrivalScreen]
                       ,[AllowBackAfterTicketPrinted] = remoteChanges.[AllowBackAfterTicketPrinted]
                       ,[RequirePrint] = remoteChanges.[RequirePrint]
                       ,[TotalFuelLoadCheck] = remoteChanges.[TotalFuelLoadCheck]
                       ,[VolumetricThresholdValidation] = remoteChanges.[VolumetricThresholdValidation]
                       ,[ValidateShipNumber] = remoteChanges.[ValidateShipNumber]
                       ,[AllowVTOModification] = remoteChanges.[AllowVTOModification]
                       ,[AllowFlightGateModification] = remoteChanges.[AllowFlightGateModification]
                       ,[TankPositionBalanceVerification] = remoteChanges.[TankPositionBalanceVerification]
                       ,[TankPositionBalancePercentage] = remoteChanges.[TankPositionBalancePercentage]
                       ,[OverrideWingBalancePercentVar] = remoteChanges.[OverrideWingBalancePercentVar]
                       ,[BypassDistributionTolerance] = remoteChanges.[BypassDistributionTolerance]
                       ,[VehicleIDCheck] = remoteChanges.[VehicleIDCheck]
                       ,[GSEFuelMustMatch] = remoteChanges.[GSEFuelMustMatch]
                       ,[AllowManualMeter] = remoteChanges.[AllowManualMeter]
                       ,[UseValidLogicGATrans] = remoteChanges.[UseValidLogicGATrans]
                       ,[AllowShipNumberModification] = remoteChanges.[AllowShipNumberModification]
                       ,[AllowAircraftTypeModification] = remoteChanges.[AllowAircraftTypeModification]
                       ,[AllowDestinationModification] = remoteChanges.[AllowDestinationModification]
                       ,[TicketPrinting] = remoteChanges.[TicketPrinting]
                       ,[AircraftTypeVerification] = remoteChanges.[AircraftTypeVerification]
                       ,[Destination] = remoteChanges.[Destination]
                       ,[Gate] = remoteChanges.[Gate]
                       ,[ShipNumber] = remoteChanges.[ShipNumber]
                       ,[MeterTotal] = remoteChanges.[MeterTotal]
                       ,[VolumePumped] = remoteChanges.[VolumePumped]
                       ,[TankCapacity] = remoteChanges.[TankCapacity]
                       ,[EAStrictUserValidation] = remoteChanges.[EAStrictUserValidation]
                       ,[EAVerifyFuelingEquipment] = remoteChanges.[EAVerifyFuelingEquipment]
                       ,[EAAllowEditOfRequiredFuelLoad] = remoteChanges.[EAAllowEditOfRequiredFuelLoad]
                       ,[EAAllowBackAfterArrivalScreen] = remoteChanges.[EAAllowBackAfterArrivalScreen]
                       ,[EAAllowBackAfterTicketPrinted] = remoteChanges.[EAAllowBackAfterTicketPrinted]
                       ,[EARequirePrint] = remoteChanges.[EARequirePrint]
                       ,[EATotalFuelLoad] = remoteChanges.[EATotalFuelLoad]
                       ,[EAVolumetricThresholdValidation] = remoteChanges.[EAVolumetricThresholdValidation]
                       ,[EAValidateShipNumber] = remoteChanges.[EAValidateShipNumber]
                       ,[EAAllowVTOModification] = remoteChanges.[EAAllowVTOModification]
                       ,[EAAllowFlightGateModification] = remoteChanges.[EAAllowFlightGateModification]
                       ,[EATankDiffPercentage] = remoteChanges.[EATankDiffPercentage]
                       ,[EAWingBalancePercentage] = remoteChanges.[EAWingBalancePercentage]
                       ,[EABypassDistributionTolerance] = remoteChanges.[EABypassDistributionTolerance]
                       ,[EAVehicleIDCheck] = remoteChanges.[EAVehicleIDCheck]
                       ,[EAGSEFuelMustMatch] = remoteChanges.[EAGSEFuelMustMatch]
                       ,[EAAllowManualMeter] = remoteChanges.[EAAllowManualMeter]
                       ,[EAUseValidationLogicGATrans] = remoteChanges.[EAUseValidationLogicGATrans]
                       ,[EAAllowShipNumberModification] = remoteChanges.[EAAllowShipNumberModification]
                       ,[EAAllowAircraftTypeModification] = remoteChanges.[EAAllowAircraftTypeModification]
                       ,[EAAllowDestinationModification] = remoteChanges.[EAAllowDestinationModification]
                       ,[EADestination] = remoteChanges.[EADestination]
                       ,[EATicketPrinting] = remoteChanges.[EATicketPrinting]
                       ,[EAAircraftType] = remoteChanges.[EAAircraftType]
                       ,[EAShipNumber] = remoteChanges.[EAShipNumber]
                       ,[EAGateNumber] = remoteChanges.[EAGateNumber]
                       ,[EAMeterTotal] = remoteChanges.[EAMeterTotal]
                       ,[EAVolumePumped] = remoteChanges.[EAVolumePumped]
                       ,[EATankCapacity] = remoteChanges.[EATankCapacity]
                       ,[EquipmentType] = remoteChanges.[EquipmentType]
                       ,[ForeignKeyToMapEquipment] = remoteChanges.[ForeignKeyToMapEquipment]
                       ,[IssueTransaction] = remoteChanges.[IssueTransaction]
                       ,[DefuelTransaction] = remoteChanges.[DefuelTransaction]
                       ,[RotationTransaction] = remoteChanges.[RotationTransaction]
                       ,[MeterCloseout] = remoteChanges.[MeterCloseout]
                       ,[DeIceTransaction] = remoteChanges.[DeIceTransaction]
                       ,[GSETransaction] = remoteChanges.[GSETransaction]
                       ,[ManualConsumer] = remoteChanges.[ManualConsumer]
                       ,[ManualVendor] = remoteChanges.[ManualVendor]
                       ,[ManualShipper] = remoteChanges.[ManualShipper]
                       ,[ManualManager] = remoteChanges.[ManualManager]
                       ,[ManualSupplier] = remoteChanges.[ManualSupplier]
                       ,[ManualBillTo] = remoteChanges.[ManualBillTo]
                       ,[ManualProduct] = remoteChanges.[ManualProduct]
                       ,[CloseoutConsumer] = remoteChanges.[CloseoutConsumer]
                       ,[CloseoutOwner] = remoteChanges.[CloseoutOwner]
                       ,[CloseoutVendor] = remoteChanges.[CloseoutVendor]
                       ,[ManualStationID] = remoteChanges.[ManualStationID]
                       ,[InhibitOverridingTemperature] = remoteChanges.[InhibitOverridingTemperature]
                       ,[ManualTemperature] = remoteChanges.[ManualTemperature]
                       ,[ManualDensity] = remoteChanges.[ManualDensity]
                       ,[HasDCU] = remoteChanges.[HasDCU]
                       ,[BluetoothDCU] = remoteChanges.[BluetoothDCU]
                       ,[LogDCUActions] = remoteChanges.[LogDCUActions]
                       ,[HasAveryHardoll] = remoteChanges.[HasAveryHardoll]
                       ,[DCUComPort] = remoteChanges.[DCUComPort]
                       ,[DCUReadRetry] = remoteChanges.[DCUReadRetry]
                       ,[DCUDisconnectDelay] = remoteChanges.[DCUDisconnectDelay]
                       ,[DCUCommunicationFailRestart] = remoteChanges.[DCUCommunicationFailRestart]
                       ,[AveryHardollComPort] = remoteChanges.[AveryHardollComPort]
                       ,[AveryHardollMeterID] = remoteChanges.[AveryHardollMeterID]
                       ,[ConfirmFuelCaps] = remoteChanges.[ConfirmFuelCaps]
                       ,[VTOEnabled] = remoteChanges.[VTOEnabled]
                       ,[EnabledInOpGauges] = remoteChanges.[EnabledInOpGauges]
                       ,[UseDispensingVehicleGSETrans] = remoteChanges.[UseDispensingVehicleGSETrans]
                       ,[GSEWaitMSecForGetMeter] = remoteChanges.[GSEWaitMSecForGetMeter]
                       ,[GSEInactiveLogoutMinutes] = remoteChanges.[GSEInactiveLogoutMinutes]
                       ,[GSEInactiveTimeout] = remoteChanges.[GSEInactiveTimeout]
                       ,[BarcodeInvalidWarningSeconds] = remoteChanges.[BarcodeInvalidWarningSeconds]
                       ,[DeIceBlendDefault] = remoteChanges.[DeIceBlendDefault]
                       ,[CommunicationTimeoutSeconds] = remoteChanges.[CommunicationTimeoutSeconds]
                       ,[ConnectionRetries] = remoteChanges.[ConnectionRetries]
                       ,[ConnectionRetryTimeout] = remoteChanges.[ConnectionRetryTimeout]
                       ,[ConnectionType] = remoteChanges.[ConnectionType]
                       ,[UpdateInterval] = remoteChanges.[UpdateInterval]
                       ,[PingVerificationIPAddress] = remoteChanges.[PingVerificationIPAddress]
                       ,[VehicleUpdateInterval] = remoteChanges.[VehicleUpdateInterval]
                       ,[PresubmitDelay] = remoteChanges.[PresubmitDelay]
                       ,[VerificationIPAddress] = remoteChanges.[VerificationIPAddress]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]

            WHEN NOT MATCHED THEN
                INSERT ([MobileDeviceProfileGuid],[SiteGuid],[ProfileID],[Description],[ShowProductScreen],[GenerateTicketNumber],[ShowOperatorFieldInFlightList],[UseDefaultPrinter],[DefaultPrinter],[AdminPassword],[ShutdownHotKey],[PrinterCOMPort],[SearchType],[LoggingOption],[AllowableFailedLoginAttempts],[FuelDistributionPrecision],[MakeDefaultProfile],[VehicleID],[MonitorScreenTransitionTiming],[BypassFsrCheckOnScreenTrans],[ShowFuelUpdateCheckStatusWin],[RTDTemperatureRangeMin],[RTDTemperatureRangeMax],[DefaultTemperature],[StrictUserValidation],[VerifyFuelingEquipment],[AllowEditRequiredFuelLoad],[AllowBackAfterArrivalScreen],[AllowBackAfterTicketPrinted],[RequirePrint],[TotalFuelLoadCheck],[VolumetricThresholdValidation],[ValidateShipNumber],[AllowVTOModification],[AllowFlightGateModification],[TankPositionBalanceVerification],[TankPositionBalancePercentage],[OverrideWingBalancePercentVar],[BypassDistributionTolerance],[VehicleIDCheck],[GSEFuelMustMatch],[AllowManualMeter],[UseValidLogicGATrans],[AllowShipNumberModification],[AllowAircraftTypeModification],[AllowDestinationModification],[TicketPrinting],[AircraftTypeVerification],[Destination],[Gate],[ShipNumber],[MeterTotal],[VolumePumped],[TankCapacity],[EAStrictUserValidation],[EAVerifyFuelingEquipment],[EAAllowEditOfRequiredFuelLoad],[EAAllowBackAfterArrivalScreen],[EAAllowBackAfterTicketPrinted],[EARequirePrint],[EATotalFuelLoad],[EAVolumetricThresholdValidation],[EAValidateShipNumber],[EAAllowVTOModification],[EAAllowFlightGateModification],[EATankDiffPercentage],[EAWingBalancePercentage],[EABypassDistributionTolerance],[EAVehicleIDCheck],[EAGSEFuelMustMatch],[EAAllowManualMeter],[EAUseValidationLogicGATrans],[EAAllowShipNumberModification],[EAAllowAircraftTypeModification],[EAAllowDestinationModification],[EADestination],[EATicketPrinting],[EAAircraftType],[EAShipNumber],[EAGateNumber],[EAMeterTotal],[EAVolumePumped],[EATankCapacity],[EquipmentType],[ForeignKeyToMapEquipment],[IssueTransaction],[DefuelTransaction],[RotationTransaction],[MeterCloseout],[DeIceTransaction],[GSETransaction],[ManualConsumer],[ManualVendor],[ManualShipper],[ManualManager],[ManualSupplier],[ManualBillTo],[ManualProduct],[CloseoutConsumer],[CloseoutOwner],[CloseoutVendor],[ManualStationID],[InhibitOverridingTemperature],[ManualTemperature],[ManualDensity],[HasDCU],[BluetoothDCU],[LogDCUActions],[HasAveryHardoll],[DCUComPort],[DCUReadRetry],[DCUDisconnectDelay],[DCUCommunicationFailRestart],[AveryHardollComPort],[AveryHardollMeterID],[ConfirmFuelCaps],[VTOEnabled],[EnabledInOpGauges],[UseDispensingVehicleGSETrans],[GSEWaitMSecForGetMeter],[GSEInactiveLogoutMinutes],[GSEInactiveTimeout],[BarcodeInvalidWarningSeconds],[DeIceBlendDefault],[CommunicationTimeoutSeconds],[ConnectionRetries],[ConnectionRetryTimeout],[ConnectionType],[UpdateInterval],[PingVerificationIPAddress],[VehicleUpdateInterval],[PresubmitDelay],[VerificationIPAddress],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@MobileDeviceProfileGuid,@SiteGuid,@ProfileID,@Description,@ShowProductScreen,@GenerateTicketNumber,@ShowOperatorFieldInFlightList,@UseDefaultPrinter,@DefaultPrinter,@AdminPassword,@ShutdownHotKey,@PrinterCOMPort,@SearchType,@LoggingOption,@AllowableFailedLoginAttempts,@FuelDistributionPrecision,@MakeDefaultProfile,@VehicleID,@MonitorScreenTransitionTiming,@BypassFsrCheckOnScreenTrans,@ShowFuelUpdateCheckStatusWin,@RTDTemperatureRangeMin,@RTDTemperatureRangeMax,@DefaultTemperature,@StrictUserValidation,@VerifyFuelingEquipment,@AllowEditRequiredFuelLoad,@AllowBackAfterArrivalScreen,@AllowBackAfterTicketPrinted,@RequirePrint,@TotalFuelLoadCheck,@VolumetricThresholdValidation,@ValidateShipNumber,@AllowVTOModification,@AllowFlightGateModification,@TankPositionBalanceVerification,@TankPositionBalancePercentage,@OverrideWingBalancePercentVar,@BypassDistributionTolerance,@VehicleIDCheck,@GSEFuelMustMatch,@AllowManualMeter,@UseValidLogicGATrans,@AllowShipNumberModification,@AllowAircraftTypeModification,@AllowDestinationModification,@TicketPrinting,@AircraftTypeVerification,@Destination,@Gate,@ShipNumber,@MeterTotal,@VolumePumped,@TankCapacity,@EAStrictUserValidation,@EAVerifyFuelingEquipment,@EAAllowEditOfRequiredFuelLoad,@EAAllowBackAfterArrivalScreen,@EAAllowBackAfterTicketPrinted,@EARequirePrint,@EATotalFuelLoad,@EAVolumetricThresholdValidation,@EAValidateShipNumber,@EAAllowVTOModification,@EAAllowFlightGateModification,@EATankDiffPercentage,@EAWingBalancePercentage,@EABypassDistributionTolerance,@EAVehicleIDCheck,@EAGSEFuelMustMatch,@EAAllowManualMeter,@EAUseValidationLogicGATrans,@EAAllowShipNumberModification,@EAAllowAircraftTypeModification,@EAAllowDestinationModification,@EADestination,@EATicketPrinting,@EAAircraftType,@EAShipNumber,@EAGateNumber,@EAMeterTotal,@EAVolumePumped,@EATankCapacity,@EquipmentType,@ForeignKeyToMapEquipment,@IssueTransaction,@DefuelTransaction,@RotationTransaction,@MeterCloseout,@DeIceTransaction,@GSETransaction,@ManualConsumer,@ManualVendor,@ManualShipper,@ManualManager,@ManualSupplier,@ManualBillTo,@ManualProduct,@CloseoutConsumer,@CloseoutOwner,@CloseoutVendor,@ManualStationID,@InhibitOverridingTemperature,@ManualTemperature,@ManualDensity,@HasDCU,@BluetoothDCU,@LogDCUActions,@HasAveryHardoll,@DCUComPort,@DCUReadRetry,@DCUDisconnectDelay,@DCUCommunicationFailRestart,@AveryHardollComPort,@AveryHardollMeterID,@ConfirmFuelCaps,@VTOEnabled,@EnabledInOpGauges,@UseDispensingVehicleGSETrans,@GSEWaitMSecForGetMeter,@GSEInactiveLogoutMinutes,@GSEInactiveTimeout,@BarcodeInvalidWarningSeconds,@DeIceBlendDefault,@CommunicationTimeoutSeconds,@ConnectionRetries,@ConnectionRetryTimeout,@ConnectionType,@UpdateInterval,@PingVerificationIPAddress,@VehicleUpdateInterval,@PresubmitDelay,@VerificationIPAddress,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
            ;
         SET @sync_row_count = @@rowcount;
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MobileDeviceProfileGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MobileDeviceProfileGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MobileDeviceProfileGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblMobileDeviceProfile] WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
