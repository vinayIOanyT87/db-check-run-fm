CREATE TABLE [fmaudit].[tblProducts](
	[ProductID] nvarchar (30) NULL
,	[Description] nvarchar (50) NULL
,	[GenericType] nvarchar (10) NULL
,	[StockResetDate] datetimeoffset NULL
,	[StockTrack] bit NULL
,	[DensityHighLimit] float NULL
,	[DensityLowLimit] float NULL
,	[DensityDeadband] float NULL
,	[TemperatureHiHiLimit] float NULL
,	[TemperatureHighLimit] float NULL
,	[TemperatureLowLimit] float NULL
,	[TemperatureLoLoLimit] float NULL
,	[TemperatureDeadband] float NULL
,	[Bonded] bit NULL
,	[LowStockWarning] float NULL
,	[GroundFuel] bit NULL
,	[ProductCode] nvarchar (15) NULL
,	[Price] money NULL
,	[AviationFuelFlag] bit NULL
,	[StandardDensity] float NULL
,	[ApplyVolumeCorrection] bit NULL
,	[ApplyStandardDensity] bit NULL
,	[ApplyDensityLimits] bit NULL
,	[ApplyTemperatureLimits] bit NULL
,	[VolumeUnitIndex] int NULL
,	[TemperatureUnitIndex] int NULL
,	[DensityUnitIndex] int NULL
,	[VolumeDecimalPlaces] tinyint NULL
,	[TemperatureDecimalPlaces] tinyint NULL
,	[DensityDecimalPlaces] tinyint NULL
,	[Capitalize] bit NULL
,	[OctaneNumber] float NULL
,	[ReidVaporPressure] float NULL
,	[HazardousMaterial] bit NULL
,	[RegulatoryClass] int NULL
,	[LoadRackDisplayText] nvarchar (10) NULL
,	[ComponentTolerance] float NULL
,	[VaporRecovery] bit NULL
,	[LockedOut] bit NULL
,	[LockedOutReason] nvarchar (80) NULL
,	[LockedOutDate] datetimeoffset NULL
,	[VarianceTolerance] float NULL
,	[DielectricTolerance] float NULL
,	[LoadByWeight] bit NULL
,	[PIDXCode] nvarchar (4) NULL
,	[ContaminationPromptLoadRackText] nvarchar (10) NULL
,	[InhibitAccounting] bit NULL
,	[UserData1] nvarchar (60) NULL
,	[UserData2] nvarchar (60) NULL
,	[UserData3] nvarchar (60) NULL
,	[UserData4] nvarchar (60) NULL
,	[UserData5] nvarchar (60) NULL
,	[UserData6] nvarchar (60) NULL
,	[UserData7] nvarchar (60) NULL
,	[UserData8] nvarchar (60) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[MassUnitIndex] int NULL
,	[LevelUnitIndex] int NULL
,	[FlowUnitIndex] int NULL
,	[PressureUnitIndex] int NULL
,	[MassDecimalPlaces] tinyint NULL
,	[LevelDecimalPlaces] tinyint NULL
,	[FlowDecimalPlaces] tinyint NULL
,	[PressureDecimalPlaces] tinyint NULL
,	[VolumePackageSize] float NULL
,	[MassPackageSize] float NULL
,	[ProductGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupProductTypeIndex] int NULL
,	[TrackingProductGuid] uniqueidentifier NULL
,	[TaxCode] nvarchar (10) NULL
,	[VcfModuleSettings] xml NULL
,	[ProductColor] nvarchar(7) NULL
,	[PatternColor] nvarchar(7) NULL
,	[PatternNumber] INT NULL
,	[_MasterRecordGuid] uniqueidentifier NULL
,	[HiddenDate] datetimeoffset NULL
,	[AutomaticCloseout] bit NULL
,	[PIDXFamilyCode] nvarchar (4) NULL
,	[IsEthanol] bit NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblProducts_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblProducts_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblProducts_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblProducts_AuditGUID] ON [fmaudit].[tblProducts](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblProducts_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblProducts] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)

GO

CREATE CLUSTERED INDEX [IX_tblProducts_ClusterIdx] ON [fmaudit].[tblProducts](_ClusterIdx ASC)
GO

CREATE NONCLUSTERED INDEX [IX_fmaudit_tblProducts_ProductGuid__AuditEventType] ON [fmaudit].[tblProducts]
(
	[ProductGuid] ASC,
	[_AuditEventType] ASC
)
INCLUDE (ProductID)
GO