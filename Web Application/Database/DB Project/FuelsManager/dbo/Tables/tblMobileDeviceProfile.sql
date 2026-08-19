CREATE TABLE [dbo].[tblMobileDeviceProfile] (
    [MobileDeviceProfileGuid]         UNIQUEIDENTIFIER   NOT NULL,
    [SiteGuid]                        UNIQUEIDENTIFIER   NOT NULL,
    [ProfileID]                       NVARCHAR (50)      NOT NULL,
    [Description]                     NVARCHAR (200)     NULL,
    [ShowProductScreen]               BIT                CONSTRAINT [DF_tblMobileDeviceProfile_ShowProductScreen] DEFAULT ((0)) NOT NULL,
    [GenerateTicketNumber]            BIT                CONSTRAINT [DF_tblMobileDeviceProfile_GenerateTicketNumber] DEFAULT ((0)) NOT NULL,
    [ShowOperatorFieldInFlightList]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_ShowOperatorFieldInFlightList] DEFAULT ((0)) NOT NULL,
    [UseDefaultPrinter]               BIT                CONSTRAINT [DF_tblMobileDeviceProfile_UseDefaultPrinter] DEFAULT ((0)) NOT NULL,
    [DefaultPrinter]                  NVARCHAR (50)      NULL,
    [AdminPassword]                   VARBINARY (256)    NULL,
    [ShutdownHotKey]                  NVARCHAR (50)      NULL,
    [PrinterCOMPort]                  NVARCHAR (4)       NULL,
    [SearchType]                      INT                NULL,
    [LoggingOption]                   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_LoggingOption] DEFAULT ((0)) NOT NULL,
    [AllowableFailedLoginAttempts]    INT                NULL,
    [FuelDistributionPrecision]       INT                NULL,
    [MakeDefaultProfile]              BIT                CONSTRAINT [DF_tblMobileDeviceProfile_MakeDefaultProfile] DEFAULT ((0)) NOT NULL,
    [VehicleID]                       NVARCHAR (50)      NULL,
    [MonitorScreenTransitionTiming]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_MonitorScreenTransitionTiming] DEFAULT ((0)) NULL,
    [BypassFsrCheckOnScreenTrans]     BIT                CONSTRAINT [DF_tblMobileDeviceProfile_BypassFsrCheckOnScreenTrans] DEFAULT ((0)) NOT NULL,
    [ShowFuelUpdateCheckStatusWin]    BIT                CONSTRAINT [DF_tblMobileDeviceProfile_ShowFuelUpdateCheckStatusWin] DEFAULT ((0)) NOT NULL,
    [RTDTemperatureRangeMin]          FLOAT (53)         NULL,
    [RTDTemperatureRangeMax]          FLOAT (53)         NULL,
    [DefaultTemperature]              FLOAT (53)         NULL,
    [StrictUserValidation]            BIT                CONSTRAINT [DF_tblMobileDeviceProfile_StrictUserValidation] DEFAULT ((0)) NOT NULL,
    [VerifyFuelingEquipment]          BIT                CONSTRAINT [DF_tblMobileDeviceProfile_VerifyFuelingEquipment] DEFAULT ((0)) NOT NULL,
    [AllowEditRequiredFuelLoad]       BIT                CONSTRAINT [DF_tblMobileDeviceProfile_AllowEditRequiredFuelLoad] DEFAULT ((0)) NOT NULL,
    [AllowBackAfterArrivalScreen]     BIT                CONSTRAINT [DF_tblMobileDeviceProfile_AllowBackAfterArrivalScreen] DEFAULT ((0)) NOT NULL,
    [AllowBackAfterTicketPrinted]     BIT                CONSTRAINT [DF_tblMobileDeviceProfile_AllowBackAfterTicketPrinted] DEFAULT ((0)) NOT NULL,
    [RequirePrint]                    BIT                CONSTRAINT [DF_tblMobileDeviceProfile_RequirePrint] DEFAULT ((0)) NOT NULL,
    [TotalFuelLoadCheck]              BIT                CONSTRAINT [DF_tblMobileDeviceProfile_TotalFuelLoadCheck] DEFAULT ((0)) NOT NULL,
    [VolumetricThresholdValidation]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_VolumetricThresholdValidation] DEFAULT ((0)) NOT NULL,
    [ValidateShipNumber]              BIT                CONSTRAINT [DF_tblMobileDeviceProfile_ValidateShipNumber] DEFAULT ((0)) NOT NULL,
    [AllowVTOModification]            BIT                CONSTRAINT [DF_tblMobileDeviceProfile_AllowVTOModification] DEFAULT ((0)) NOT NULL,
    [AllowFlightGateModification]     BIT                CONSTRAINT [DF_tblMobileDeviceProfile_AllowFlightGateModification] DEFAULT ((0)) NOT NULL,
    [TankPositionBalanceVerification] INT                CONSTRAINT [DF_tblMobileDeviceProfile_TankPositionBalanceVerification] DEFAULT ((0)) NULL,
    [TankPositionBalancePercentage]   FLOAT (53)         NULL,
    [OverrideWingBalancePercentVar]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_OverrideWingBalancePercentVar] DEFAULT ((0)) NOT NULL,
    [BypassDistributionTolerance]     BIT                CONSTRAINT [DF_tblMobileDeviceProfile_BypassDistributionTolerance] DEFAULT ((0)) NOT NULL,
    [VehicleIDCheck]                  BIT                CONSTRAINT [DF_tblMobileDeviceProfile_VehicleIDCheck] DEFAULT ((0)) NOT NULL,
    [GSEFuelMustMatch]                BIT                CONSTRAINT [DF_tblMobileDeviceProfile_GSEFuelMustMatch] DEFAULT ((0)) NOT NULL,
    [AllowManualMeter]                BIT                CONSTRAINT [DF_tblMobileDeviceProfile_AllowManualMeter] DEFAULT ((0)) NOT NULL,
    [UseValidLogicGATrans]            BIT                CONSTRAINT [DF_tblMobileDeviceProfile_UseValidLogicGATrans] DEFAULT ((0)) NOT NULL,
    [AllowShipNumberModification]     BIT                CONSTRAINT [DF_tblMobileDeviceProfile_AllowShipNumberModification] DEFAULT ((0)) NOT NULL,
    [AllowAircraftTypeModification]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_AllowAircraftTypeModification] DEFAULT ((0)) NOT NULL,
    [AllowDestinationModification]    BIT                CONSTRAINT [DF_tblMobileDeviceProfile_AllowDestinationModification] DEFAULT ((0)) NOT NULL,
    [TicketPrinting]                  INT                NULL,
    [AircraftTypeVerification]        INT                NULL,
    [Destination]                     INT                NULL,
    [Gate]                            INT                NULL,
    [ShipNumber]                      INT                NULL,
    [MeterTotal]                      INT                NULL,
    [VolumePumped]                    INT                NULL,
    [TankCapacity]                    INT                NULL,
    [EAStrictUserValidation]          BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAStrictUserValidation] DEFAULT ((0)) NOT NULL,
    [EAVerifyFuelingEquipment]        BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAVerifyFuelingEquipment] DEFAULT ((0)) NOT NULL,
    [EAAllowEditOfRequiredFuelLoad]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAllowEditOfRequiredFuelLoad] DEFAULT ((0)) NOT NULL,
    [EAAllowBackAfterArrivalScreen]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAllowBackAfterArrivalScreen] DEFAULT ((0)) NOT NULL,
    [EAAllowBackAfterTicketPrinted]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAllowBackAfterTicketPrinted] DEFAULT ((0)) NOT NULL,
    [EARequirePrint]                  BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EARequirePrint] DEFAULT ((0)) NOT NULL,
    [EATotalFuelLoad]                 BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EATotalFuelLoad] DEFAULT ((0)) NOT NULL,
    [EAVolumetricThresholdValidation] BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAVolumetricThresholdValidation] DEFAULT ((0)) NOT NULL,
    [EAValidateShipNumber]            BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAValidateShipNumber] DEFAULT ((0)) NOT NULL,
    [EAAllowVTOModification]          BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAllowVTOModification] DEFAULT ((0)) NOT NULL,
    [EAAllowFlightGateModification]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAllowFlightGateModification] DEFAULT ((0)) NOT NULL,
    [EATankDiffPercentage]            BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EATankDiffPercentage] DEFAULT ((0)) NOT NULL,
    [EAWingBalancePercentage]         BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAWingBalancePercentage] DEFAULT ((0)) NOT NULL,
    [EABypassDistributionTolerance]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EABypassDistributionTolerance] DEFAULT ((0)) NOT NULL,
    [EAVehicleIDCheck]                BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAVehicleIDCheck] DEFAULT ((0)) NOT NULL,
    [EAGSEFuelMustMatch]              BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAGSEFuelMustMatch] DEFAULT ((0)) NOT NULL,
    [EAAllowManualMeter]              BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAllowManualMeter] DEFAULT ((0)) NOT NULL,
    [EAUseValidationLogicGATrans]     BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAUseValidationLogicGATrans] DEFAULT ((0)) NOT NULL,
    [EAAllowShipNumberModification]   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAllowShipNumberModification] DEFAULT ((0)) NOT NULL,
    [EAAllowAircraftTypeModification] BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAllowAircraftTypeModification] DEFAULT ((0)) NOT NULL,
    [EAAllowDestinationModification]  BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAllowDestinationModification] DEFAULT ((0)) NOT NULL,
    [EADestination]                   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EADestination] DEFAULT ((0)) NOT NULL,
    [EATicketPrinting]                BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EATicketPrinting] DEFAULT ((0)) NOT NULL,
    [EAAircraftType]                  BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAAircraftType] DEFAULT ((0)) NOT NULL,
    [EAShipNumber]                    BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAShipNumber] DEFAULT ((0)) NOT NULL,
    [EAGateNumber]                    BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAGateNumber] DEFAULT ((0)) NOT NULL,
    [EAMeterTotal]                    BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAMeterTotal] DEFAULT ((0)) NOT NULL,
    [EAVolumePumped]                  BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EAVolumePumped] DEFAULT ((0)) NOT NULL,
    [EATankCapacity]                  BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EATankCapacity] DEFAULT ((0)) NOT NULL,
    [EquipmentType]                   INT                NULL,
    [ForeignKeyToMapEquipment]        UNIQUEIDENTIFIER   NULL,
    [IssueTransaction]                UNIQUEIDENTIFIER   NULL,
    [DefuelTransaction]               UNIQUEIDENTIFIER   NULL,
    [RotationTransaction]             UNIQUEIDENTIFIER   NULL,
    [MeterCloseout]                   UNIQUEIDENTIFIER   NULL,
    [DeIceTransaction]                UNIQUEIDENTIFIER   NULL,
    [GSETransaction]                  UNIQUEIDENTIFIER   NULL,
    [ManualConsumer]                  UNIQUEIDENTIFIER   NULL,
    [ManualVendor]                    UNIQUEIDENTIFIER   NULL,
    [ManualShipper]                   UNIQUEIDENTIFIER   NULL,
    [ManualManager]                   UNIQUEIDENTIFIER   NULL,
    [ManualSupplier]                  UNIQUEIDENTIFIER   NULL,
    [ManualBillTo]                    UNIQUEIDENTIFIER   NULL,
    [ManualProduct]                   UNIQUEIDENTIFIER   NULL,
    [CloseoutConsumer]                UNIQUEIDENTIFIER   NULL,
    [CloseoutOwner]                   UNIQUEIDENTIFIER   NULL,
    [CloseoutVendor]                  UNIQUEIDENTIFIER   NULL,
    [ManualStationID]                 INT                NULL,
    [InhibitOverridingTemperature]    BIT                CONSTRAINT [DF_tblMobileDeviceProfile_InhibitOverridingTemperature] DEFAULT ((0)) NOT NULL,
    [ManualTemperature]               FLOAT (53)         NULL,
    [ManualDensity]                   FLOAT (53)         NULL,
    [HasDCU]                          BIT                CONSTRAINT [DF_tblMobileDeviceProfile_HasDCU] DEFAULT ((0)) NOT NULL,
    [BluetoothDCU]                    BIT                CONSTRAINT [DF_tblMobileDeviceProfile_BluetoothDCU] DEFAULT ((0)) NOT NULL,
    [LogDCUActions]                   BIT                CONSTRAINT [DF_tblMobileDeviceProfile_LogDCUActions] DEFAULT ((0)) NOT NULL,
    [HasAveryHardoll]                 BIT                CONSTRAINT [DF_tblMobileDeviceProfile_HasAveryHardoll] DEFAULT ((0)) NOT NULL,
    [DCUComPort]                      NVARCHAR (4)       NULL,
    [DCUReadRetry]                    INT                NULL,
    [DCUDisconnectDelay]              INT                NULL,
    [DCUCommunicationFailRestart]     INT                NULL,
    [AveryHardollComPort]             NVARCHAR (4)       NULL,
    [AveryHardollMeterID]             NVARCHAR (4)       NULL,
    [ConfirmFuelCaps]                 BIT                CONSTRAINT [DF_tblMobileDeviceProfile_ConfirmFuelCaps] DEFAULT ((0)) NOT NULL,
    [VTOEnabled]                      BIT                CONSTRAINT [DF_tblMobileDeviceProfile_VTOEnabled] DEFAULT ((0)) NOT NULL,
    [EnabledInOpGauges]               BIT                CONSTRAINT [DF_tblMobileDeviceProfile_EnabledInOpGauges] DEFAULT ((0)) NOT NULL,
    [UseDispensingVehicleGSETrans]    BIT                CONSTRAINT [DF_tblMobileDeviceProfile_UseDispensingVehicleGSETrans] DEFAULT ((0)) NOT NULL,
    [GSEWaitMSecForGetMeter]          INT                NULL,
    [GSEInactiveLogoutMinutes]        INT                NULL,
    [GSEInactiveTimeout]              INT                NULL,
    [BarcodeInvalidWarningSeconds]    INT                NULL,
    [DeIceBlendDefault]               FLOAT (53)         NULL,
    [CommunicationTimeoutSeconds]     INT                NULL,
    [ConnectionRetries]               INT                NULL,
    [ConnectionRetryTimeout]          INT                NULL,
    [ConnectionType]                  INT                NULL,
    [UpdateInterval]                  INT                NULL,
    [PingVerificationIPAddress]       BIT                CONSTRAINT [DF_tblMobileDeviceProfile_PingVerificationIPAddress] DEFAULT ((0)) NOT NULL,
    [VehicleUpdateInterval]           INT                NULL,
    [PresubmitDelay]                  INT                NULL,
    [VerificationIPAddress]           NVARCHAR (15)      NULL,
    [CreatedDate]                     DATETIMEOFFSET (7) CONSTRAINT [DF_tblMobileDeviceProfile_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                       [dbo].[udtUserID]  CONSTRAINT [DF_tblMobileDeviceProfile_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]                     DATETIMEOFFSET (7) CONSTRAINT [DF_tblMobileDeviceProfile_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                       [dbo].[udtUserID]  CONSTRAINT [DF_tblMobileDeviceProfile_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                     ROWVERSION         NOT NULL,
    [_ClusterIdx]                     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblMobileDeviceProfile] PRIMARY KEY NONCLUSTERED ([MobileDeviceProfileGuid] ASC),
    CONSTRAINT [CK_tblMobileDeviceProfile_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessMobileDeviceProfile]([MobileDeviceProfileGuid],[SiteGuid],[ProfileID])=(1)),
    CONSTRAINT [FK_tblMobileDeviceProfile_tblSites] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblMobileDeviceProfile_CreatedDate]
    ON [dbo].[tblMobileDeviceProfile]([CreatedDate] ASC);




GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblMobileDeviceProfile] ON [dbo].[tblMobileDeviceProfile] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMobileDeviceProfile','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'U' -- For Updates 
	SET @_AuditEventSequence= 1 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
		,	@_AuditContext=s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID 

	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
	-- Treat the change as a local change so it can be synchronized back to the remote system. 
	IF ((SELECT trigger_nestlevel()) > 1) 
	BEGIN 
		SET @_AuditContext = NULL 
	END 

	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
	IF (@_AuditContext IS NOT NULL) 
	BEGIN 
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
 
	DECLARE @AuditGuidList TABLE
	(
	MobileDeviceProfileGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblMobileDeviceProfile (
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
	,	[OriginalRowVersion]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	OUTPUT inserted.[MobileDeviceProfileGuid] AS 'MobileDeviceProfileGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[MobileDeviceProfileGuid]
	,	d.[SiteGuid]
	,	d.[ProfileID]
	,	d.[Description]
	,	d.[ShowProductScreen]
	,	d.[GenerateTicketNumber]
	,	d.[ShowOperatorFieldInFlightList]
	,	d.[UseDefaultPrinter]
	,	d.[DefaultPrinter]
	,	d.[AdminPassword]
	,	d.[ShutdownHotKey]
	,	d.[PrinterCOMPort]
	,	d.[SearchType]
	,	d.[LoggingOption]
	,	d.[AllowableFailedLoginAttempts]
	,	d.[FuelDistributionPrecision]
	,	d.[MakeDefaultProfile]
	,	d.[VehicleID]
	,	d.[MonitorScreenTransitionTiming]
	,	d.[BypassFsrCheckOnScreenTrans]
	,	d.[ShowFuelUpdateCheckStatusWin]
	,	d.[RTDTemperatureRangeMin]
	,	d.[RTDTemperatureRangeMax]
	,	d.[DefaultTemperature]
	,	d.[StrictUserValidation]
	,	d.[VerifyFuelingEquipment]
	,	d.[AllowEditRequiredFuelLoad]
	,	d.[AllowBackAfterArrivalScreen]
	,	d.[AllowBackAfterTicketPrinted]
	,	d.[RequirePrint]
	,	d.[TotalFuelLoadCheck]
	,	d.[VolumetricThresholdValidation]
	,	d.[ValidateShipNumber]
	,	d.[AllowVTOModification]
	,	d.[AllowFlightGateModification]
	,	d.[TankPositionBalanceVerification]
	,	d.[TankPositionBalancePercentage]
	,	d.[OverrideWingBalancePercentVar]
	,	d.[BypassDistributionTolerance]
	,	d.[VehicleIDCheck]
	,	d.[GSEFuelMustMatch]
	,	d.[AllowManualMeter]
	,	d.[UseValidLogicGATrans]
	,	d.[AllowShipNumberModification]
	,	d.[AllowAircraftTypeModification]
	,	d.[AllowDestinationModification]
	,	d.[TicketPrinting]
	,	d.[AircraftTypeVerification]
	,	d.[Destination]
	,	d.[Gate]
	,	d.[ShipNumber]
	,	d.[MeterTotal]
	,	d.[VolumePumped]
	,	d.[TankCapacity]
	,	d.[EAStrictUserValidation]
	,	d.[EAVerifyFuelingEquipment]
	,	d.[EAAllowEditOfRequiredFuelLoad]
	,	d.[EAAllowBackAfterArrivalScreen]
	,	d.[EAAllowBackAfterTicketPrinted]
	,	d.[EARequirePrint]
	,	d.[EATotalFuelLoad]
	,	d.[EAVolumetricThresholdValidation]
	,	d.[EAValidateShipNumber]
	,	d.[EAAllowVTOModification]
	,	d.[EAAllowFlightGateModification]
	,	d.[EATankDiffPercentage]
	,	d.[EAWingBalancePercentage]
	,	d.[EABypassDistributionTolerance]
	,	d.[EAVehicleIDCheck]
	,	d.[EAGSEFuelMustMatch]
	,	d.[EAAllowManualMeter]
	,	d.[EAUseValidationLogicGATrans]
	,	d.[EAAllowShipNumberModification]
	,	d.[EAAllowAircraftTypeModification]
	,	d.[EAAllowDestinationModification]
	,	d.[EADestination]
	,	d.[EATicketPrinting]
	,	d.[EAAircraftType]
	,	d.[EAShipNumber]
	,	d.[EAGateNumber]
	,	d.[EAMeterTotal]
	,	d.[EAVolumePumped]
	,	d.[EATankCapacity]
	,	d.[EquipmentType]
	,	d.[ForeignKeyToMapEquipment]
	,	d.[IssueTransaction]
	,	d.[DefuelTransaction]
	,	d.[RotationTransaction]
	,	d.[MeterCloseout]
	,	d.[DeIceTransaction]
	,	d.[GSETransaction]
	,	d.[ManualConsumer]
	,	d.[ManualVendor]
	,	d.[ManualShipper]
	,	d.[ManualManager]
	,	d.[ManualSupplier]
	,	d.[ManualBillTo]
	,	d.[ManualProduct]
	,	d.[CloseoutConsumer]
	,	d.[CloseoutOwner]
	,	d.[CloseoutVendor]
	,	d.[ManualStationID]
	,	d.[InhibitOverridingTemperature]
	,	d.[ManualTemperature]
	,	d.[ManualDensity]
	,	d.[HasDCU]
	,	d.[BluetoothDCU]
	,	d.[LogDCUActions]
	,	d.[HasAveryHardoll]
	,	d.[DCUComPort]
	,	d.[DCUReadRetry]
	,	d.[DCUDisconnectDelay]
	,	d.[DCUCommunicationFailRestart]
	,	d.[AveryHardollComPort]
	,	d.[AveryHardollMeterID]
	,	d.[ConfirmFuelCaps]
	,	d.[VTOEnabled]
	,	d.[EnabledInOpGauges]
	,	d.[UseDispensingVehicleGSETrans]
	,	d.[GSEWaitMSecForGetMeter]
	,	d.[GSEInactiveLogoutMinutes]
	,	d.[GSEInactiveTimeout]
	,	d.[BarcodeInvalidWarningSeconds]
	,	d.[DeIceBlendDefault]
	,	d.[CommunicationTimeoutSeconds]
	,	d.[ConnectionRetries]
	,	d.[ConnectionRetryTimeout]
	,	d.[ConnectionType]
	,	d.[UpdateInterval]
	,	d.[PingVerificationIPAddress]
	,	d.[VehicleUpdateInterval]
	,	d.[PresubmitDelay]
	,	d.[VerificationIPAddress]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM deleted d
 
	INSERT INTO [fmaudit].tblMobileDeviceProfile (
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
	,	[OriginalRowVersion]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	SELECT 
		i.[MobileDeviceProfileGuid]
	,	i.[SiteGuid]
	,	i.[ProfileID]
	,	i.[Description]
	,	i.[ShowProductScreen]
	,	i.[GenerateTicketNumber]
	,	i.[ShowOperatorFieldInFlightList]
	,	i.[UseDefaultPrinter]
	,	i.[DefaultPrinter]
	,	i.[AdminPassword]
	,	i.[ShutdownHotKey]
	,	i.[PrinterCOMPort]
	,	i.[SearchType]
	,	i.[LoggingOption]
	,	i.[AllowableFailedLoginAttempts]
	,	i.[FuelDistributionPrecision]
	,	i.[MakeDefaultProfile]
	,	i.[VehicleID]
	,	i.[MonitorScreenTransitionTiming]
	,	i.[BypassFsrCheckOnScreenTrans]
	,	i.[ShowFuelUpdateCheckStatusWin]
	,	i.[RTDTemperatureRangeMin]
	,	i.[RTDTemperatureRangeMax]
	,	i.[DefaultTemperature]
	,	i.[StrictUserValidation]
	,	i.[VerifyFuelingEquipment]
	,	i.[AllowEditRequiredFuelLoad]
	,	i.[AllowBackAfterArrivalScreen]
	,	i.[AllowBackAfterTicketPrinted]
	,	i.[RequirePrint]
	,	i.[TotalFuelLoadCheck]
	,	i.[VolumetricThresholdValidation]
	,	i.[ValidateShipNumber]
	,	i.[AllowVTOModification]
	,	i.[AllowFlightGateModification]
	,	i.[TankPositionBalanceVerification]
	,	i.[TankPositionBalancePercentage]
	,	i.[OverrideWingBalancePercentVar]
	,	i.[BypassDistributionTolerance]
	,	i.[VehicleIDCheck]
	,	i.[GSEFuelMustMatch]
	,	i.[AllowManualMeter]
	,	i.[UseValidLogicGATrans]
	,	i.[AllowShipNumberModification]
	,	i.[AllowAircraftTypeModification]
	,	i.[AllowDestinationModification]
	,	i.[TicketPrinting]
	,	i.[AircraftTypeVerification]
	,	i.[Destination]
	,	i.[Gate]
	,	i.[ShipNumber]
	,	i.[MeterTotal]
	,	i.[VolumePumped]
	,	i.[TankCapacity]
	,	i.[EAStrictUserValidation]
	,	i.[EAVerifyFuelingEquipment]
	,	i.[EAAllowEditOfRequiredFuelLoad]
	,	i.[EAAllowBackAfterArrivalScreen]
	,	i.[EAAllowBackAfterTicketPrinted]
	,	i.[EARequirePrint]
	,	i.[EATotalFuelLoad]
	,	i.[EAVolumetricThresholdValidation]
	,	i.[EAValidateShipNumber]
	,	i.[EAAllowVTOModification]
	,	i.[EAAllowFlightGateModification]
	,	i.[EATankDiffPercentage]
	,	i.[EAWingBalancePercentage]
	,	i.[EABypassDistributionTolerance]
	,	i.[EAVehicleIDCheck]
	,	i.[EAGSEFuelMustMatch]
	,	i.[EAAllowManualMeter]
	,	i.[EAUseValidationLogicGATrans]
	,	i.[EAAllowShipNumberModification]
	,	i.[EAAllowAircraftTypeModification]
	,	i.[EAAllowDestinationModification]
	,	i.[EADestination]
	,	i.[EATicketPrinting]
	,	i.[EAAircraftType]
	,	i.[EAShipNumber]
	,	i.[EAGateNumber]
	,	i.[EAMeterTotal]
	,	i.[EAVolumePumped]
	,	i.[EATankCapacity]
	,	i.[EquipmentType]
	,	i.[ForeignKeyToMapEquipment]
	,	i.[IssueTransaction]
	,	i.[DefuelTransaction]
	,	i.[RotationTransaction]
	,	i.[MeterCloseout]
	,	i.[DeIceTransaction]
	,	i.[GSETransaction]
	,	i.[ManualConsumer]
	,	i.[ManualVendor]
	,	i.[ManualShipper]
	,	i.[ManualManager]
	,	i.[ManualSupplier]
	,	i.[ManualBillTo]
	,	i.[ManualProduct]
	,	i.[CloseoutConsumer]
	,	i.[CloseoutOwner]
	,	i.[CloseoutVendor]
	,	i.[ManualStationID]
	,	i.[InhibitOverridingTemperature]
	,	i.[ManualTemperature]
	,	i.[ManualDensity]
	,	i.[HasDCU]
	,	i.[BluetoothDCU]
	,	i.[LogDCUActions]
	,	i.[HasAveryHardoll]
	,	i.[DCUComPort]
	,	i.[DCUReadRetry]
	,	i.[DCUDisconnectDelay]
	,	i.[DCUCommunicationFailRestart]
	,	i.[AveryHardollComPort]
	,	i.[AveryHardollMeterID]
	,	i.[ConfirmFuelCaps]
	,	i.[VTOEnabled]
	,	i.[EnabledInOpGauges]
	,	i.[UseDispensingVehicleGSETrans]
	,	i.[GSEWaitMSecForGetMeter]
	,	i.[GSEInactiveLogoutMinutes]
	,	i.[GSEInactiveTimeout]
	,	i.[BarcodeInvalidWarningSeconds]
	,	i.[DeIceBlendDefault]
	,	i.[CommunicationTimeoutSeconds]
	,	i.[ConnectionRetries]
	,	i.[ConnectionRetryTimeout]
	,	i.[ConnectionType]
	,	i.[UpdateInterval]
	,	i.[PingVerificationIPAddress]
	,	i.[VehicleUpdateInterval]
	,	i.[PresubmitDelay]
	,	i.[VerificationIPAddress]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	@_AuditEventType
	,	2
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	agl._AuditGUID
	,	@_UserId
	,	@_AuditContext
	FROM inserted i 
	INNER JOIN	@AuditGuidList agl ON
		(
			agl.[MobileDeviceProfileGuid]=i.[MobileDeviceProfileGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblMobileDeviceProfile
CREATE TRIGGER dbo.trg_insupd_tblMobileDeviceProfile_ForSync 
   ON dbo.tblMobileDeviceProfile
   AFTER INSERT, UPDATE 
AS 
BEGIN 
	-- SET NOCOUNT ON added to prevent extra result sets from 
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 
 
    DECLARE @changeContextName nvarchar(100); 
    DECLARE @bypassTrackingFlags int; 
    DECLARE @bypassReason nvarchar(512); 
 
    SELECT @changeContextName = ContextName 
            ,@bypassTrackingFlags = BypassTrackingFlags 
            ,@bypassReason = BypassReason 
        FROM [track].[udf_GetChangeTrackingSessionDetails](); 
 
	-- Get the synchronization context.  This will be NULL if this trigger was fired
	-- due to a normal application insert or update.
    DECLARE @syncContext varbinary(128); 
    DECLARE @currentDateTimeOffset datetimeoffset(7); 
 
    SET @currentDateTimeOffset = sysdatetimeoffset(); 
 
   IF (([track].[udf_IsInsertChangeTrackingEnabled](@bypassTrackingFlags) = 1) OR ([track].[udf_IsUpdateChangeTrackingEnabled](@bypassTrackingFlags) = 1))
   BEGIN 
       SET @syncContext = dbo.udf_GetSyncContext(); 
 
       -- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
       -- Treat the change as a local change so it can be synchronized back to the remote system. 
       IF ((SELECT trigger_nestlevel()) > 1) 
       BEGIN 
           SET @syncContext = NULL 
       END 
 
       SELECT @syncContext AS ChangeContext 
                    ,d.MobileDeviceProfileGuid AS Deleted_PK_MobileDeviceProfileGuid
                    ,i.MobileDeviceProfileGuid AS Inserted_PK_MobileDeviceProfileGuid
                    ,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    ,CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
                    ,i.SiteGuid AS CurrentSiteGuid 
                    ,d.SiteGuid AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.MobileDeviceProfileGuid = i.MobileDeviceProfileGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblMobileDeviceProfile As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_MobileDeviceProfileGuid = currentTrackingData.PK_MobileDeviceProfileGuid
 
 
		    INSERT track.tblMobileDeviceProfile (InsertedDate 
 			    	,InsertedContext 
 				    ,InsertedRowVersion 
 				    ,UpdatedDate 
 				    ,UpdatedContext 
 				    ,UpdatedRowVersion 
 				    ,DeletedDate 
 				    ,DeletedContext 
 				    ,DeletedRowVersion 
 				    ,CurrentSiteGuid 
 				    ,PreviousSiteGuid 
				    ,PK_MobileDeviceProfileGuid
				    ,FK_ParentPK 
		    )
		    SELECT CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
		                 WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate 
		                 ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
			    	,entityChanges.ChangeContext 
				    ,entityChanges.Inserted_RowVersion 
    				,entityChanges.Inserted_CreatedDate 
	    			,entityChanges.ChangeContext 
		    		,entityChanges.Inserted_RowVersion 
			    	,NULL 
    				,NULL 
	    			,NULL 
		    		,entityChanges.CurrentSiteGuid 
			    	,CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), entityChanges.PreviousSiteGuid),'')) THEN entityChanges.PreviousSiteGuid ELSE NULL END
				    ,entityChanges.Inserted_PK_MobileDeviceProfileGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblMobileDeviceProfile As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_MobileDeviceProfileGuid = currentTrackingData.PK_MobileDeviceProfileGuid
)
    END
END 

GO
--Creating Delete Trigger for tblMobileDeviceProfile
CREATE TRIGGER dbo.trg_del_tblMobileDeviceProfile_ForSync 
   ON dbo.tblMobileDeviceProfile
   AFTER DELETE 
AS 
BEGIN 
	-- SET NOCOUNT ON added to prevent extra result sets from 
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 

    DECLARE @changeContextName nvarchar(100); 
    DECLARE @bypassTrackingFlags int; 
    DECLARE @bypassReason nvarchar(512); 

    SELECT @changeContextName = ContextName 
            ,@bypassTrackingFlags = BypassTrackingFlags 
            ,@bypassReason = BypassReason 
        FROM [track].[udf_GetChangeTrackingSessionDetails](); 

	-- Get the synchronization context.  This will be NULL if this trigger was fired
	-- due to a normal application delete.
    DECLARE @syncContext varbinary(128); 
    DECLARE @currentDateTimeOffset datetimeoffset(7); 

    SET @currentDateTimeOffset = sysdatetimeoffset(); 

    IF ([track].[udf_IsDeleteChangeTrackingEnabled](@bypassTrackingFlags) = 1)
    BEGIN
       SET @syncContext = dbo.udf_GetSyncContext(); 

       -- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
       -- Treat the change as a local change so it can be synchronized back to the remote system. 
       IF ((SELECT trigger_nestlevel()) > 1) 
       BEGIN 
           SET @syncContext = NULL 
       END 

		  ; WITH ChangeList AS ( 
				SELECT @syncContext AS ChangeContext 
						,d.MobileDeviceProfileGuid AS Deleted_PK_MobileDeviceProfileGuid
                        ,d.MobileDeviceProfileGuid AS Inserted_PK_MobileDeviceProfileGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblMobileDeviceProfile As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_MobileDeviceProfileGuid = currentTrackingData.PK_MobileDeviceProfileGuid
				WHEN Matched 
				THEN 
					UPDATE SET DeletedDate = @currentDateTimeOffset 
								,DeletedContext = entityChanges.ChangeContext 
                             ,DeletedRowVersion = entityChanges.Deleted_RowVersion 
				WHEN Not Matched 
				THEN 
				INSERT (InsertedDate
				    	,InsertedContext
				    	,InsertedRowVersion
				    	,UpdatedDate
				    	,UpdatedContext
				    	,UpdatedRowVersion
				    	,CurrentSiteGuid
				    	,PreviousSiteGuid
				    	,DeletedDate
				    	,DeletedContext
				    	,DeletedRowVersion
						,PK_MobileDeviceProfileGuid
				        ,FK_ParentPK 
				)
				VALUES (CASE WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
						,entityChanges.ChangeContext 
						,entityChanges.Inserted_RowVersion 
						,NULL 
						,NULL 
						,NULL 
						,entityChanges.CurrentSiteGuid 
						,NULL 
						,@currentDateTimeOffset 
						,entityChanges.ChangeContext 
						,entityChanges.Deleted_RowVersion
						,entityChanges.Deleted_PK_MobileDeviceProfileGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblMobileDeviceProfile] ON [dbo].[tblMobileDeviceProfile] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMobileDeviceProfile','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'D'; -- For Deletes 
	SET @_AuditEventSequence= 1; 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
		,	@_AuditContext=s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID;

	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
	-- Treat the change as a local change so it can be synchronized back to the remote system. 
	IF ((SELECT trigger_nestlevel()) > 1) 
	BEGIN 
		SET @_AuditContext = NULL 
	END 

	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
	IF (@_AuditContext IS NOT NULL) 
	BEGIN 
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblMobileDeviceProfile (
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
	,	[OriginalRowVersion]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	SELECT 
		d.[MobileDeviceProfileGuid]
	,	d.[SiteGuid]
	,	d.[ProfileID]
	,	d.[Description]
	,	d.[ShowProductScreen]
	,	d.[GenerateTicketNumber]
	,	d.[ShowOperatorFieldInFlightList]
	,	d.[UseDefaultPrinter]
	,	d.[DefaultPrinter]
	,	d.[AdminPassword]
	,	d.[ShutdownHotKey]
	,	d.[PrinterCOMPort]
	,	d.[SearchType]
	,	d.[LoggingOption]
	,	d.[AllowableFailedLoginAttempts]
	,	d.[FuelDistributionPrecision]
	,	d.[MakeDefaultProfile]
	,	d.[VehicleID]
	,	d.[MonitorScreenTransitionTiming]
	,	d.[BypassFsrCheckOnScreenTrans]
	,	d.[ShowFuelUpdateCheckStatusWin]
	,	d.[RTDTemperatureRangeMin]
	,	d.[RTDTemperatureRangeMax]
	,	d.[DefaultTemperature]
	,	d.[StrictUserValidation]
	,	d.[VerifyFuelingEquipment]
	,	d.[AllowEditRequiredFuelLoad]
	,	d.[AllowBackAfterArrivalScreen]
	,	d.[AllowBackAfterTicketPrinted]
	,	d.[RequirePrint]
	,	d.[TotalFuelLoadCheck]
	,	d.[VolumetricThresholdValidation]
	,	d.[ValidateShipNumber]
	,	d.[AllowVTOModification]
	,	d.[AllowFlightGateModification]
	,	d.[TankPositionBalanceVerification]
	,	d.[TankPositionBalancePercentage]
	,	d.[OverrideWingBalancePercentVar]
	,	d.[BypassDistributionTolerance]
	,	d.[VehicleIDCheck]
	,	d.[GSEFuelMustMatch]
	,	d.[AllowManualMeter]
	,	d.[UseValidLogicGATrans]
	,	d.[AllowShipNumberModification]
	,	d.[AllowAircraftTypeModification]
	,	d.[AllowDestinationModification]
	,	d.[TicketPrinting]
	,	d.[AircraftTypeVerification]
	,	d.[Destination]
	,	d.[Gate]
	,	d.[ShipNumber]
	,	d.[MeterTotal]
	,	d.[VolumePumped]
	,	d.[TankCapacity]
	,	d.[EAStrictUserValidation]
	,	d.[EAVerifyFuelingEquipment]
	,	d.[EAAllowEditOfRequiredFuelLoad]
	,	d.[EAAllowBackAfterArrivalScreen]
	,	d.[EAAllowBackAfterTicketPrinted]
	,	d.[EARequirePrint]
	,	d.[EATotalFuelLoad]
	,	d.[EAVolumetricThresholdValidation]
	,	d.[EAValidateShipNumber]
	,	d.[EAAllowVTOModification]
	,	d.[EAAllowFlightGateModification]
	,	d.[EATankDiffPercentage]
	,	d.[EAWingBalancePercentage]
	,	d.[EABypassDistributionTolerance]
	,	d.[EAVehicleIDCheck]
	,	d.[EAGSEFuelMustMatch]
	,	d.[EAAllowManualMeter]
	,	d.[EAUseValidationLogicGATrans]
	,	d.[EAAllowShipNumberModification]
	,	d.[EAAllowAircraftTypeModification]
	,	d.[EAAllowDestinationModification]
	,	d.[EADestination]
	,	d.[EATicketPrinting]
	,	d.[EAAircraftType]
	,	d.[EAShipNumber]
	,	d.[EAGateNumber]
	,	d.[EAMeterTotal]
	,	d.[EAVolumePumped]
	,	d.[EATankCapacity]
	,	d.[EquipmentType]
	,	d.[ForeignKeyToMapEquipment]
	,	d.[IssueTransaction]
	,	d.[DefuelTransaction]
	,	d.[RotationTransaction]
	,	d.[MeterCloseout]
	,	d.[DeIceTransaction]
	,	d.[GSETransaction]
	,	d.[ManualConsumer]
	,	d.[ManualVendor]
	,	d.[ManualShipper]
	,	d.[ManualManager]
	,	d.[ManualSupplier]
	,	d.[ManualBillTo]
	,	d.[ManualProduct]
	,	d.[CloseoutConsumer]
	,	d.[CloseoutOwner]
	,	d.[CloseoutVendor]
	,	d.[ManualStationID]
	,	d.[InhibitOverridingTemperature]
	,	d.[ManualTemperature]
	,	d.[ManualDensity]
	,	d.[HasDCU]
	,	d.[BluetoothDCU]
	,	d.[LogDCUActions]
	,	d.[HasAveryHardoll]
	,	d.[DCUComPort]
	,	d.[DCUReadRetry]
	,	d.[DCUDisconnectDelay]
	,	d.[DCUCommunicationFailRestart]
	,	d.[AveryHardollComPort]
	,	d.[AveryHardollMeterID]
	,	d.[ConfirmFuelCaps]
	,	d.[VTOEnabled]
	,	d.[EnabledInOpGauges]
	,	d.[UseDispensingVehicleGSETrans]
	,	d.[GSEWaitMSecForGetMeter]
	,	d.[GSEInactiveLogoutMinutes]
	,	d.[GSEInactiveTimeout]
	,	d.[BarcodeInvalidWarningSeconds]
	,	d.[DeIceBlendDefault]
	,	d.[CommunicationTimeoutSeconds]
	,	d.[ConnectionRetries]
	,	d.[ConnectionRetryTimeout]
	,	d.[ConnectionType]
	,	d.[UpdateInterval]
	,	d.[PingVerificationIPAddress]
	,	d.[VehicleUpdateInterval]
	,	d.[PresubmitDelay]
	,	d.[VerificationIPAddress]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM deleted d
END

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblMobileDeviceProfile] ON [dbo].[tblMobileDeviceProfile] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMobileDeviceProfile','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'I' -- For Inserts 
	SET @_AuditEventSequence= 1 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
		,	@_AuditContext=s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID 

	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
	-- Treat the change as a local change so it can be synchronized back to the remote system. 
	IF ((SELECT trigger_nestlevel()) > 1) 
	BEGIN 
		SET @_AuditContext = NULL 
	END 

	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
	IF (@_AuditContext IS NOT NULL) 
	BEGIN 
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblMobileDeviceProfile (
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
	,	[OriginalRowVersion]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	SELECT 
		i.[MobileDeviceProfileGuid]
	,	i.[SiteGuid]
	,	i.[ProfileID]
	,	i.[Description]
	,	i.[ShowProductScreen]
	,	i.[GenerateTicketNumber]
	,	i.[ShowOperatorFieldInFlightList]
	,	i.[UseDefaultPrinter]
	,	i.[DefaultPrinter]
	,	i.[AdminPassword]
	,	i.[ShutdownHotKey]
	,	i.[PrinterCOMPort]
	,	i.[SearchType]
	,	i.[LoggingOption]
	,	i.[AllowableFailedLoginAttempts]
	,	i.[FuelDistributionPrecision]
	,	i.[MakeDefaultProfile]
	,	i.[VehicleID]
	,	i.[MonitorScreenTransitionTiming]
	,	i.[BypassFsrCheckOnScreenTrans]
	,	i.[ShowFuelUpdateCheckStatusWin]
	,	i.[RTDTemperatureRangeMin]
	,	i.[RTDTemperatureRangeMax]
	,	i.[DefaultTemperature]
	,	i.[StrictUserValidation]
	,	i.[VerifyFuelingEquipment]
	,	i.[AllowEditRequiredFuelLoad]
	,	i.[AllowBackAfterArrivalScreen]
	,	i.[AllowBackAfterTicketPrinted]
	,	i.[RequirePrint]
	,	i.[TotalFuelLoadCheck]
	,	i.[VolumetricThresholdValidation]
	,	i.[ValidateShipNumber]
	,	i.[AllowVTOModification]
	,	i.[AllowFlightGateModification]
	,	i.[TankPositionBalanceVerification]
	,	i.[TankPositionBalancePercentage]
	,	i.[OverrideWingBalancePercentVar]
	,	i.[BypassDistributionTolerance]
	,	i.[VehicleIDCheck]
	,	i.[GSEFuelMustMatch]
	,	i.[AllowManualMeter]
	,	i.[UseValidLogicGATrans]
	,	i.[AllowShipNumberModification]
	,	i.[AllowAircraftTypeModification]
	,	i.[AllowDestinationModification]
	,	i.[TicketPrinting]
	,	i.[AircraftTypeVerification]
	,	i.[Destination]
	,	i.[Gate]
	,	i.[ShipNumber]
	,	i.[MeterTotal]
	,	i.[VolumePumped]
	,	i.[TankCapacity]
	,	i.[EAStrictUserValidation]
	,	i.[EAVerifyFuelingEquipment]
	,	i.[EAAllowEditOfRequiredFuelLoad]
	,	i.[EAAllowBackAfterArrivalScreen]
	,	i.[EAAllowBackAfterTicketPrinted]
	,	i.[EARequirePrint]
	,	i.[EATotalFuelLoad]
	,	i.[EAVolumetricThresholdValidation]
	,	i.[EAValidateShipNumber]
	,	i.[EAAllowVTOModification]
	,	i.[EAAllowFlightGateModification]
	,	i.[EATankDiffPercentage]
	,	i.[EAWingBalancePercentage]
	,	i.[EABypassDistributionTolerance]
	,	i.[EAVehicleIDCheck]
	,	i.[EAGSEFuelMustMatch]
	,	i.[EAAllowManualMeter]
	,	i.[EAUseValidationLogicGATrans]
	,	i.[EAAllowShipNumberModification]
	,	i.[EAAllowAircraftTypeModification]
	,	i.[EAAllowDestinationModification]
	,	i.[EADestination]
	,	i.[EATicketPrinting]
	,	i.[EAAircraftType]
	,	i.[EAShipNumber]
	,	i.[EAGateNumber]
	,	i.[EAMeterTotal]
	,	i.[EAVolumePumped]
	,	i.[EATankCapacity]
	,	i.[EquipmentType]
	,	i.[ForeignKeyToMapEquipment]
	,	i.[IssueTransaction]
	,	i.[DefuelTransaction]
	,	i.[RotationTransaction]
	,	i.[MeterCloseout]
	,	i.[DeIceTransaction]
	,	i.[GSETransaction]
	,	i.[ManualConsumer]
	,	i.[ManualVendor]
	,	i.[ManualShipper]
	,	i.[ManualManager]
	,	i.[ManualSupplier]
	,	i.[ManualBillTo]
	,	i.[ManualProduct]
	,	i.[CloseoutConsumer]
	,	i.[CloseoutOwner]
	,	i.[CloseoutVendor]
	,	i.[ManualStationID]
	,	i.[InhibitOverridingTemperature]
	,	i.[ManualTemperature]
	,	i.[ManualDensity]
	,	i.[HasDCU]
	,	i.[BluetoothDCU]
	,	i.[LogDCUActions]
	,	i.[HasAveryHardoll]
	,	i.[DCUComPort]
	,	i.[DCUReadRetry]
	,	i.[DCUDisconnectDelay]
	,	i.[DCUCommunicationFailRestart]
	,	i.[AveryHardollComPort]
	,	i.[AveryHardollMeterID]
	,	i.[ConfirmFuelCaps]
	,	i.[VTOEnabled]
	,	i.[EnabledInOpGauges]
	,	i.[UseDispensingVehicleGSETrans]
	,	i.[GSEWaitMSecForGetMeter]
	,	i.[GSEInactiveLogoutMinutes]
	,	i.[GSEInactiveTimeout]
	,	i.[BarcodeInvalidWarningSeconds]
	,	i.[DeIceBlendDefault]
	,	i.[CommunicationTimeoutSeconds]
	,	i.[ConnectionRetries]
	,	i.[ConnectionRetryTimeout]
	,	i.[ConnectionType]
	,	i.[UpdateInterval]
	,	i.[PingVerificationIPAddress]
	,	i.[VehicleUpdateInterval]
	,	i.[PresubmitDelay]
	,	i.[VerificationIPAddress]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM inserted i
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMobileDeviceProfile_ClusterIdx]
    ON [dbo].[tblMobileDeviceProfile]([_ClusterIdx] ASC);

