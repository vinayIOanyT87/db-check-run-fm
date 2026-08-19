CREATE TABLE [fmaudit].[tblMobileDeviceProfile](
	[MobileDeviceProfileGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ProfileID] nvarchar (50) NULL
,	[Description] nvarchar (200) NULL
,	[ShowProductScreen] bit NULL
,	[GenerateTicketNumber] bit NULL
,	[ShowOperatorFieldInFlightList] bit NULL
,	[UseDefaultPrinter] bit NULL
,	[DefaultPrinter] nvarchar (50) NULL
,	[AdminPassword] varbinary (256) NULL
,	[ShutdownHotKey] nvarchar (50) NULL
,	[PrinterCOMPort] nvarchar (4) NULL
,	[SearchType] int NULL
,	[LoggingOption] bit NULL
,	[AllowableFailedLoginAttempts] int NULL
,	[FuelDistributionPrecision] int NULL
,	[MakeDefaultProfile] bit NULL
,	[VehicleID] nvarchar (50) NULL
,	[MonitorScreenTransitionTiming] bit NULL
,	[BypassFsrCheckOnScreenTrans] bit NULL
,	[ShowFuelUpdateCheckStatusWin] bit NULL
,	[RTDTemperatureRangeMin] float NULL
,	[RTDTemperatureRangeMax] float NULL
,	[DefaultTemperature] float NULL
,	[StrictUserValidation] bit NULL
,	[VerifyFuelingEquipment] bit NULL
,	[AllowEditRequiredFuelLoad] bit NULL
,	[AllowBackAfterArrivalScreen] bit NULL
,	[AllowBackAfterTicketPrinted] bit NULL
,	[RequirePrint] bit NULL
,	[TotalFuelLoadCheck] bit NULL
,	[VolumetricThresholdValidation] bit NULL
,	[ValidateShipNumber] bit NULL
,	[AllowVTOModification] bit NULL
,	[AllowFlightGateModification] bit NULL
,	[TankPositionBalanceVerification] int NULL
,	[TankPositionBalancePercentage] float NULL
,	[OverrideWingBalancePercentVar] bit NULL
,	[BypassDistributionTolerance] bit NULL
,	[VehicleIDCheck] bit NULL
,	[GSEFuelMustMatch] bit NULL
,	[AllowManualMeter] bit NULL
,	[UseValidLogicGATrans] bit NULL
,	[AllowShipNumberModification] bit NULL
,	[AllowAircraftTypeModification] bit NULL
,	[AllowDestinationModification] bit NULL
,	[TicketPrinting] int NULL
,	[AircraftTypeVerification] int NULL
,	[Destination] int NULL
,	[Gate] int NULL
,	[ShipNumber] int NULL
,	[MeterTotal] int NULL
,	[VolumePumped] int NULL
,	[TankCapacity] int NULL
,	[EAStrictUserValidation] bit NULL
,	[EAVerifyFuelingEquipment] bit NULL
,	[EAAllowEditOfRequiredFuelLoad] bit NULL
,	[EAAllowBackAfterArrivalScreen] bit NULL
,	[EAAllowBackAfterTicketPrinted] bit NULL
,	[EARequirePrint] bit NULL
,	[EATotalFuelLoad] bit NULL
,	[EAVolumetricThresholdValidation] bit NULL
,	[EAValidateShipNumber] bit NULL
,	[EAAllowVTOModification] bit NULL
,	[EAAllowFlightGateModification] bit NULL
,	[EATankDiffPercentage] bit NULL
,	[EAWingBalancePercentage] bit NULL
,	[EABypassDistributionTolerance] bit NULL
,	[EAVehicleIDCheck] bit NULL
,	[EAGSEFuelMustMatch] bit NULL
,	[EAAllowManualMeter] bit NULL
,	[EAUseValidationLogicGATrans] bit NULL
,	[EAAllowShipNumberModification] bit NULL
,	[EAAllowAircraftTypeModification] bit NULL
,	[EAAllowDestinationModification] bit NULL
,	[EADestination] bit NULL
,	[EATicketPrinting] bit NULL
,	[EAAircraftType] bit NULL
,	[EAShipNumber] bit NULL
,	[EAGateNumber] bit NULL
,	[EAMeterTotal] bit NULL
,	[EAVolumePumped] bit NULL
,	[EATankCapacity] bit NULL
,	[EquipmentType] int NULL
,	[ForeignKeyToMapEquipment] uniqueidentifier NULL
,	[IssueTransaction] uniqueidentifier NULL
,	[DefuelTransaction] uniqueidentifier NULL
,	[RotationTransaction] uniqueidentifier NULL
,	[MeterCloseout] uniqueidentifier NULL
,	[DeIceTransaction] uniqueidentifier NULL
,	[GSETransaction] uniqueidentifier NULL
,	[ManualConsumer] uniqueidentifier NULL
,	[ManualVendor] uniqueidentifier NULL
,	[ManualShipper] uniqueidentifier NULL
,	[ManualManager] uniqueidentifier NULL
,	[ManualSupplier] uniqueidentifier NULL
,	[ManualBillTo] uniqueidentifier NULL
,	[ManualProduct] uniqueidentifier NULL
,	[CloseoutConsumer] uniqueidentifier NULL
,	[CloseoutOwner] uniqueidentifier NULL
,	[CloseoutVendor] uniqueidentifier NULL
,	[ManualStationID] int NULL
,	[InhibitOverridingTemperature] bit NULL
,	[ManualTemperature] float NULL
,	[ManualDensity] float NULL
,	[HasDCU] bit NULL
,	[BluetoothDCU] bit NULL
,	[LogDCUActions] bit NULL
,	[HasAveryHardoll] bit NULL
,	[DCUComPort] nvarchar (4) NULL
,	[DCUReadRetry] int NULL
,	[DCUDisconnectDelay] int NULL
,	[DCUCommunicationFailRestart] int NULL
,	[AveryHardollComPort] nvarchar (4) NULL
,	[AveryHardollMeterID] nvarchar (4) NULL
,	[ConfirmFuelCaps] bit NULL
,	[VTOEnabled] bit NULL
,	[EnabledInOpGauges] bit NULL
,	[UseDispensingVehicleGSETrans] bit NULL
,	[GSEWaitMSecForGetMeter] int NULL
,	[GSEInactiveLogoutMinutes] int NULL
,	[GSEInactiveTimeout] int NULL
,	[BarcodeInvalidWarningSeconds] int NULL
,	[DeIceBlendDefault] float NULL
,	[CommunicationTimeoutSeconds] int NULL
,	[ConnectionRetries] int NULL
,	[ConnectionRetryTimeout] int NULL
,	[ConnectionType] int NULL
,	[UpdateInterval] int NULL
,	[PingVerificationIPAddress] bit NULL
,	[VehicleUpdateInterval] int NULL
,	[PresubmitDelay] int NULL
,	[VerificationIPAddress] nvarchar (15) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblMobileDeviceProfile_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblMobileDeviceProfile_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblMobileDeviceProfile_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblMobileDeviceProfile_AuditGUID] ON [fmaudit].[tblMobileDeviceProfile](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblMobileDeviceProfile_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblMobileDeviceProfile] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblMobileDeviceProfile_ClusterIdx] ON [fmaudit].[tblMobileDeviceProfile](_ClusterIdx ASC)