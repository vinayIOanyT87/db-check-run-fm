CREATE TABLE [fmaudit].[tblTransactionSubLineItems](
	[SequenceID] int NULL
,	[Product] nvarchar (30) NULL
,	[ProductCode] nvarchar (50) NULL
,	[ProductType] nvarchar (20) NULL
,	[GrossQuantity] float NULL
,	[DeliveredGrossQuantity] float NULL
,	[NetQuantity] float NULL
,	[DeliveredNetQuantity] float NULL
,  [Pressure] float NULL
,	[Vcf] float NULL
,	[Density] float NULL
,	[Temperature] float NULL
,	[Customs] nvarchar (20) NULL
,	[ArmNumber] int NULL
,	[LineNumber] int NULL
,	[BatchNumber] nvarchar (20) NULL
,	[LineFill] float NULL
,	[BottomVolume] float NULL
,	[NetCapacity] float NULL
,	[TankStatus] nvarchar (30) NULL
,	[MeterFactor] float NULL
,	[MeterStart] float NULL
,	[MeterStop] float NULL
,	[MeterStopDateTime] datetimeoffset NULL
,	[MeterStartDateTime] datetimeoffset NULL
,	[FreezePoint] float NULL
,	[DifferentialPressure] float NULL
,	[DosageRate] float NULL
,	[DeleteFlag] bit NULL
,	[PresetAmount] float NULL
,	[StorageLocationID] nvarchar (50) NULL
,	[MeterID] nvarchar (50) NULL
,	[COAID] nvarchar (40) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[TransactionInventoryDate] date NULL
,	[Tax1] float NULL
,	[Tax2] float NULL
,	[Tax3] float NULL
,	[Tax4] float NULL
,	[Tax5] float NULL
,	[TransVersion] bigint NULL
,	[ImproperAdditization] bit NULL
,	[BrokenBlend] bit NULL
,	[Flag01] bit NULL
,	[Flag02] bit NULL
,	[Flag03] bit NULL
,	[Flag04] bit NULL
,	[Flag05] bit NULL
,	[Flag06] bit NULL
,	[Number01] float NULL
,	[Number02] float NULL
,	[Number03] float NULL
,	[Number04] float NULL
,	[Number05] float NULL
,	[Number06] float NULL
,	[Date01] datetimeoffset NULL
,	[Date02] datetimeoffset NULL
,	[Date03] datetimeoffset NULL
,	[Date04] datetimeoffset NULL
,	[MassQuantity] float NULL
,	[NetManualValueFlag] bit NULL
,	[MassManualValueFlag] bit NULL
,	[GrossManualValueFlag] bit NULL
,	[VcfManualValueFlag] bit NULL
,	[DeliveredGrossManualValueFlag] bit NULL
,	[DeliveredNetManualValueFlag] bit NULL
,	[TransactionSubLineItemGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[LookupTransactionStatusIndex] int NULL
,	[LookupQualityIndex] int NULL
,	[TransactionLineItemGuid] uniqueidentifier NULL
,	[ProductGuid] uniqueidentifier NULL
,	[TransactionGuid] uniqueidentifier NULL
,	[StorageLocationTankGuid] uniqueidentifier NULL
,	[MeterGuid] uniqueidentifier NULL
,	[PackageManualValueFlag] bit NULL
,	[CleanLineItem] bit NULL
,	[CleanLineDeductItem] bit NULL
,	[CleanLineDeductQuantity] float NULL
,	[CleanLinePackQuantity] float NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactionSubLineItems_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactionSubLineItems_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactionSubLineItems_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)






GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_AuditGUID] ON [fmaudit].[tblTransactionSubLineItems](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactionSubLineItems] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTransactionSubLineItems_ClusterIdx] ON [fmaudit].[tblTransactionSubLineItems](_ClusterIdx ASC)