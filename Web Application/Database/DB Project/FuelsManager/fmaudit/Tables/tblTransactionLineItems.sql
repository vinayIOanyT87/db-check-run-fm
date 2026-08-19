CREATE TABLE [fmaudit].[tblTransactionLineItems](
	[SequenceID] smallint NULL
,	[MeterStart] float NULL
,	[MeterStop] float NULL
,	[GrossQuantity] float NULL
,	[DeliveredGrossQuantity] float NULL
,	[Temperature] float NULL
,	[Vcf] float NULL
,	[Density] float NULL
,	[Product] nvarchar (30) NULL
,	[ProductCode] nvarchar (30) NULL
,	[ProductType] nvarchar (20) NULL
,	[ProductPrice] float NULL
,	[CLIN] nvarchar (10) NULL
,	[NetQuantity] float NULL
,  [DeliveredNetQuantity] float NULL
,  [Pressure] float NULL
,	[ContractNumber] nvarchar (30) NULL
,	[DestinationRegistrationID] nvarchar (30) NULL
,	[DestinationSerialNumber] nvarchar (10) NULL
,	[DestinationEquipmentType] nvarchar (50) NULL
,	[DestinationEquipmentModel] nvarchar (20) NULL
,	[DestinationCompanyEquipmentID] nvarchar (30) NULL
,	[DestinationCompartmentID] nvarchar (50) NULL
,	[SourceRegistrationID] nvarchar (30) NULL
,	[SourceSerialNumber] nvarchar (10) NULL
,	[SourceEquipmentType] nvarchar (50) NULL
,	[SourceEquipmentModel] nvarchar (20) NULL
,	[SourceCompanyEquipmentID] nvarchar (30) NULL
,	[SourceCompartmentID] nvarchar (50) NULL
,	[MeterFactor] float NULL
,	[LineItemSequenceNumber] nvarchar (5) NULL
,	[BatchNumber] nvarchar (20) NULL
,	[DocumentNumber] nvarchar (30) NULL
,	[LineFill] float NULL
,	[BottomVolume] float NULL
,	[NetCapacity] float NULL
,	[Customs] nvarchar (20) NULL
,	[ArmNumber] int NULL
,	[LineNumber] int NULL
,	[OperatorID] nvarchar (50) NULL
,	[TankStatus] nvarchar (30) NULL
,	[MeterStartDateTime] datetimeoffset NULL
,	[MeterStopDateTime] datetimeoffset NULL
,	[Pit] nvarchar (10) NULL
,	[RequestedDateTime] datetimeoffset NULL
,	[DispatchedDateTime] datetimeoffset NULL
,	[AcknowledgedDateTime] datetimeoffset NULL
,	[OnLocationTime] datetimeoffset NULL
,	[ValidationDateTime] datetimeoffset NULL
,	[CompletionDateTime] datetimeoffset NULL
,	[ReceiptVariance] float NULL
,	[DifferentialPressure] float NULL
,	[LoadRackVariance] float NULL
,	[RequestedBy] nvarchar (50) NULL
,	[FreezePoint] float NULL
,	[DeleteFlag] bit NULL
,	[StorageLocationID] nvarchar (50) NULL
,	[MeterID] nvarchar (50) NULL
,	[AdditiveProfileID] nvarchar (50) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[PresetAmount] float NULL
,	[EngineeringUnitsIndex] int NULL
,	[CustomerProductName] nvarchar (50) NULL
,	[CustomerProductCode] nvarchar (20) NULL
,	[TransactionInventoryDate] date NULL
,	[COAWaiver] bit NULL
,	[COANote] nvarchar (50) NULL
,	[COAID] nvarchar (40) NULL
,	[Tax1] float NULL
,	[Tax2] float NULL
,	[Tax3] float NULL
,	[Tax4] float NULL
,	[Tax5] float NULL
,	[TransVersion] bigint NULL
,	[LoadingLocationID] nvarchar (30) NULL
,	[ImproperAdditization] bit NULL
,	[BrokenBlend] bit NULL
,	[ContaminatePrompt] bit NULL
,	[CompartmentsPreviouslyLoaded] bit NULL
,	[CompartmentsEmpty] bit NULL
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
,	[OdometerHours] float NULL
,	[EndDeliveryDate] datetimeoffset NULL
,	[RequestedDeliveryDate] datetimeoffset NULL
,	[InvoiceNumber] nvarchar (50) NULL
,	[InvoiceLineNumber] nvarchar (50) NULL
,	[AlternativeGrossVolume] float NULL
,	[AlternativeNetVolume] float NULL
,	[AlternativeUnits] int NULL
,	[TankLevel] float NULL
,	[TankLevelUnits] int NULL
,	[Date01] datetimeoffset NULL
,	[Date02] datetimeoffset NULL
,	[Date03] datetimeoffset NULL
,	[Date04] datetimeoffset NULL
,	[NonDomesticPrice] float NULL
,	[CurrencyUnit] int NULL
,	[ExchangeRate] float NULL
,	[QualityTestNumber] nvarchar (50) NULL
,	[Odometer] float NULL
,	[DeliveryLocation] nvarchar (50) NULL
,	[Variance] float NULL
,	[PartialFill] bit NULL
,	[MassQuantity] float NULL
,	[NetManualValueFlag] bit NULL
,	[MassManualValueFlag] bit NULL
,	[GrossManualValueFlag] bit NULL
,	[VcfManualValueFlag] bit NULL
,	[DeliveredGrossManualValueFlag] bit NULL
,	[DeliveredNetManualValueFlag] bit NULL
,	[TransactionLineItemGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[LookupTransactionStatusIndex] int NULL
,	[LookupQualityIndex] int NULL
,	[StorageLocationTankGuid] uniqueidentifier NULL
,	[AdditiveProfileGuid] uniqueidentifier NULL
,	[DestinationCompartmentEquipmentGuid] uniqueidentifier NULL
,	[DestinationEquipmentGuid] uniqueidentifier NULL
,	[OperatorPersonnelGuid] uniqueidentifier NULL
,	[ProductGuid] uniqueidentifier NULL
,	[SourceCompartmentEquipmentGuid] uniqueidentifier NULL
,	[SourceEquipmentGuid] uniqueidentifier NULL
,	[TransactionGuid] uniqueidentifier NULL
,	[CurrencyGuid] uniqueidentifier NULL
,	[OrderReferenceTransactionLineItemGuid] uniqueidentifier NULL
,	[LoadingLocationStationGuid] uniqueidentifier NULL
,	[MeterGuid] uniqueidentifier NULL
,	[PackageManualValueFlag] bit NULL
,	[CleanLineItem] bit NULL
,	[CleanLineDeductItem] bit NULL
,	[CleanLineDeductQuantity] float NULL
,	[CleanLinePackQuantity] float NULL
,	[DualFuelingModeFlag] bit NULL
,	[DualFuelingPrimaryFlag] bit NULL
,	[EngineRunTime] float NULL
,	[FlowRate] float NULL
,	[FuelCompressionFactor] float NULL
,	[HydrantPressure] float NULL
,	[MobileDeviceID] nvarchar (50) NULL
,	[MobileDeviceGuid] uniqueidentifier NULL
,	[TemperatureQualityStatus] nvarchar (50) NULL
,	[MeterStartObtainedAutomaticallyFlag] bit NULL
,	[MeterStopObtainedAutomaticallyFlag] bit NULL
,	[NetVolumeIndicator] BIT NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactionLineItems_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactionLineItems_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactionLineItems_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)





GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItems_AuditGUID] ON [fmaudit].[tblTransactionLineItems](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItems_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactionLineItems] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTransactionLineItems_ClusterIdx] ON [fmaudit].[tblTransactionLineItems](_ClusterIdx ASC)
GO
