CREATE TABLE [fmaudit].[tblTransactionAliases](
	[AliasName] nvarchar (32) NULL
,	[MeterCloseout] bit NULL
,	[BulkShipment] bit NULL
,	[DistributedImpact] bit NULL
,	[MultipleLineItems] bit NULL
,	[LimitSelectionsBasedOnHierarchy] bit NULL
,	[LineItemEditControl] bit NULL
,	[MultipleWeightReadings] bit NULL
,	[WeightReadingEditControl] bit NULL
,	[AssociatedReport] nvarchar (80) NULL
,	[AssociatedPreloadReport] nvarchar (80) NULL
,	[DestinationEquipmentTypes1] bigint NULL
,	[DestinationEquipmentTypes2] bigint NULL
,	[DestinationEquipmentTypes3] bigint NULL
,	[SourceEquipmentTypes1] bigint NULL
,	[SourceEquipmentTypes2] bigint NULL
,	[SourceEquipmentTypes3] bigint NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[ShowCompanyName] smallint NULL
,	[AggregateAssocTrans] bit NULL
,	[EnableTotalQuantityExceededWarning] bit NULL
,	[EnableQuantityToleranceExceededWarning] bit NULL
,	[EnableTotalValueExceededWarning] bit NULL
,	[EnableValueToleranceExceededWarning] bit NULL
,	[LevelUnitIndex] int NULL
,	[TemperatureUnitIndex] int NULL
,	[DensityUnitIndex] int NULL
,	[PressureUnitIndex] int NULL
,	[FlowUnitIndex] int NULL
,	[VolumeUnitIndex] int NULL
,	[MassUnitIndex] int NULL
,	[AdditiveVolumeUnitIndex] int NULL
,	[AdditiveProfileCycleAmountUnitIndex] int NULL
,	[AdditiveProfileRateUnitIndex] int NULL
,	[LevelDecimalPlaces] tinyint NULL
,	[TemperatureDecimalPlaces] tinyint NULL
,	[DensityDecimalPlaces] tinyint NULL
,	[PressureDecimalPlaces] tinyint NULL
,	[FlowDecimalPlaces] tinyint NULL
,	[VolumeDecimalPlaces] tinyint NULL
,	[MassDecimalPlaces] tinyint NULL
,	[AdditiveVolumeDecimalPlaces] tinyint NULL
,	[UseComboBoxControls] bit NULL
,	[MultipleTransportLineItems] bit NULL
,	[TransactionAliasGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupTransTypeIndex] smallint NULL
,	[LookupDefaultStatusIndex] int NULL
,	[AssociatedTransactionAliasGuid] uniqueidentifier NULL
,	[IncludeInDispatch] bit NULL
,	[_MasterRecordGuid] uniqueidentifier NULL
,	[EnableAutoCompleteControls] bit NULL
,	[PermitNonReferenceData] bit NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactionAliases_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactionAliases_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactionAliases_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
,	[UseTransactionDetailWithLayout] BIT NULL
,	[DefaultMeterToEquipmentID] BIT NULL
,	[LimitSourceEquipmentByProduct] BIT NULL
,	[RememberMeterEndForMeterID] BIT NULL
,	[PopulateCompaniesFromEquipment] BIT NULL
,	[PopulateGrossVolumeFromMeterValues] BIT NULL
,	[UseMeterAndCompressionFactorFromMeter] BIT NULL

)




GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionAliases_AuditGUID] ON [fmaudit].[tblTransactionAliases](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliases_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactionAliases] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTransactionAliases_ClusterIdx] ON [fmaudit].[tblTransactionAliases](_ClusterIdx ASC)