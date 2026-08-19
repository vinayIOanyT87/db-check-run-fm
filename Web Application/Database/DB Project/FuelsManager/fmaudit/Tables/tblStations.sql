CREATE TABLE [fmaudit].[tblStations](
	[ID] nvarchar (50) NULL
,	[SwingArmPosition] bit NULL
,	[VaporRecovery] bit NULL
,	[Enabled] bit NULL
,	[BOLPrinter] nvarchar (80) NULL
,	[PreloadPrinter] nvarchar (80) NULL
,	[BOLAgeInMinutes] int NULL
,	[CardReader] bit NULL
,	[ThirtyFiveBitCardSupport] bit NULL
,	[NumberOfCopies] int NULL
,	[NumberOfPreloadCopies] int NULL
,	[InhibitLoadingByLoadID] bit NULL
,	[InhibitOperatingModePrompt] bit NULL
,	[SynchronizeReferenceDensity] bit NULL
,	[SignatureDevice] nvarchar (20) NULL
,	[SetDefaultPresetToZero] bit NULL
,	[ArmsServiced] nvarchar (100) NULL
,	[InhibitSettingRecipeNames] bit NULL
,	[SignatureDevicePort] int NULL
,	[SignatureDeviceBaudRate] int NULL
,	[MeterRecircCardNumber] nvarchar (30) NULL
,	[TouchKeyReader] bit NULL
,	[OffLoadByOffLoadID] bit NULL
,	[UseManualMeterData] bit NULL
,	[PromptForBOLNumber] bit NULL
,	[QueryForTrailers] bit NULL
,	[PromptForGravityCaptured] bit NULL
,	[PromptForTemperatureCaptured] bit NULL
,	[LastTransactionNumber] int NULL
,	[LastTransactionNumberDateTime] datetimeoffset NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[StationGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupStationTypeIndex] int NULL
,	[LookupStationInterfaceTypeIndex] int NULL
,	[TankGuid] uniqueidentifier NULL
,	[IssueByVolumeTransactionAliasGuid] uniqueidentifier NULL
,	[IssueByWeightTransactionAliasGuid] uniqueidentifier NULL
,	[ReceiptByVolumeTransactionAliasGuid] uniqueidentifier NULL
,	[ReceiptByWeightTransactionAliasGuid] uniqueidentifier NULL
,	[RecircTransactionAliasGuid] uniqueidentifier NULL
,	[LogCommunications] bit NULL
,	[LogCommPath] nvarchar (255) NULL
,	[EnableScully] bit NULL
,	[EnableEquipmentValidate] bit NULL
,	[StationPromptTimeout] int NULL
,	[StationMessageTimeout] int NULL
,	[AssignedMeterGuid] uniqueidentifier NULL
,  [EnableDynamicRecipes]               BIT                NULL
,	[EthanolExcess] bit NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblStations_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblStations_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblStations_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblStations_ClusterIdx] ON [fmaudit].[tblStations](_ClusterIdx ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblStations_AuditGUID] ON [fmaudit].[tblStations](_AuditGUID ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblStations_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblStations] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) 
GO
