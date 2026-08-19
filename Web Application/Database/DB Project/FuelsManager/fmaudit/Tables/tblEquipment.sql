CREATE TABLE [fmaudit].[tblEquipment](
	[ID] nvarchar (30) NULL
,	[Description] nvarchar (50) NULL
,	[Make] nvarchar (20) NULL
,	[Model] nvarchar (50) NULL
,	[Year] int NULL
,	[IssPtNum] nvarchar (20) NULL
,	[Fixed] bit NULL
,	[StorageType] nvarchar (2) NULL
,	[InUse] bit NULL
,	[FixedVolume] bit NULL
,	[IntoPlane] bit NULL
,	[Mobile] bit NULL
,	[AttachedTo] nvarchar (6) NULL
,	[MediaType] char (1) NULL
,	[Meters] int NULL
,	[DefuelMeterForwards] bit NULL
,	[PulseRatio] float NULL
,	[Round] bit NULL
,	[Xref] nvarchar (10) NULL
,	[LowStockWarning] float NULL
,	[StockTrack] bit NULL
,	[Totalisor1] nvarchar (10) NULL
,	[Totalisor2] nvarchar (10) NULL
,	[FuelingState] nvarchar (10) NULL
,	[Volume] float NULL
,	[MeterReading] float NULL
,	[Consecutive_OOS_Variance] int NULL
,	[Notes] nvarchar (1000) NULL
,	[Capacity] float NULL
,	[SafeFill] float NULL
,	[VolumeUnitIndex] int NULL
,	[TemperatureUnitIndex] int NULL
,	[DensityUnitIndex] int NULL
,	[MassUnitIndex] int NULL
,	[VolumeDecimalPlaces] tinyint NULL
,	[TemperatureDecimalPlaces] tinyint NULL
,	[DensityDecimalPlaces] tinyint NULL
,	[MassDecimalPlaces] tinyint NULL
,	[EquipmentSequence] nvarchar (50) NULL
,	[LockedOut] bit NULL
,	[LockedOutReason] nvarchar (80) NULL
,	[LockedOutDate] datetimeoffset NULL
,	[SerialNumber] nvarchar (30) NULL
,	[CompanyEquipmentID] nvarchar (30) NULL
,	[TruckCardNumber] nvarchar (32) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[RatedGPM] float NULL
,	[ActualGPM] float NULL
,	[FuelAdditiveFlag] bit NULL
,	[ManufactureDate] datetimeoffset NULL
,	[InstallationDate] datetimeoffset NULL
,	[InspectionDate] datetimeoffset NULL
,	[CalibrationDate] datetimeoffset NULL
,	[QCDate] datetimeoffset NULL
,	[SecondaryStorageFlag] bit NULL
,	[ManagedEquipmentFlag] bit NULL
,	[FuelingType] smallint NULL
,	[UserData1] nvarchar (60) NULL
,	[UserData2] nvarchar (60) NULL
,	[UserData3] nvarchar (60) NULL
,	[UserData4] nvarchar (60) NULL
,	[UserData5] nvarchar (60) NULL
,	[UserData6] nvarchar (60) NULL
,	[UserData7] nvarchar (60) NULL
,	[UserData8] nvarchar (60) NULL
,	[UserData9] nvarchar (60) NULL
,	[UserData10] nvarchar (60) NULL
,	[UserData11] nvarchar (60) NULL
,	[UserData12] nvarchar (60) NULL
,	[UserData13] nvarchar (60) NULL
,	[UserData14] nvarchar (60) NULL
,	[UserData15] nvarchar (60) NULL
,	[UserData16] nvarchar (60) NULL
,	[UserData17] nvarchar (60) NULL
,	[UserData18] nvarchar (60) NULL
,	[UserData19] nvarchar (60) NULL
,	[UserData20] nvarchar (60) NULL
,	[UserData21] nvarchar (60) NULL
,	[UserData22] nvarchar (60) NULL
,	[UserData23] nvarchar (60) NULL
,	[UserData24] nvarchar (60) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[EquipmentGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[CompanyGuid] uniqueidentifier NULL
,	[ParentEquipmentGuid] uniqueidentifier NULL
,	[EquipmentTypeGuid] uniqueidentifier NULL
,	[FuelCardGuid] uniqueidentifier NULL
,	[ProductGuid] uniqueidentifier NULL
,	[AssignedToMeterGuid] uniqueidentifier NULL
,	[AssetTrackingDeviceGuid] uniqueidentifier NULL
,	[_MasterRecordGuid] uniqueidentifier NULL
,	[HiddenDate] datetimeoffset NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblEquipment_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblEquipment_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblEquipment_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL, 
    [ScullyRequired] BIT CONSTRAINT [DF_tblEquipment_ScullyRequired] DEFAULT ((0)) NULL
)

GO


CREATE NONCLUSTERED INDEX [IX_tblEquipment_AuditGUID] ON [fmaudit].[tblEquipment](_AuditGUID ASC)

GO

CREATE NONCLUSTERED INDEX [IX_tblEquipment_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblEquipment] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblEquipment_ClusterIdx] ON [fmaudit].[tblEquipment](_ClusterIdx ASC)