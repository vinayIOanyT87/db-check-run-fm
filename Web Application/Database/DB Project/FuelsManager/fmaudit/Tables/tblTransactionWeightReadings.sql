CREATE TABLE [fmaudit].[tblTransactionWeightReadings](
	[CompartmentID] nvarchar (30) NULL
,	[BeginQuantityValue] float NULL
,	[RequestedQuantityValue] float NULL
,	[FinalQuantityValue] float NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[TransVersion] bigint NULL
,	[TransactionWeightReadingGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[TransactionGuid] uniqueidentifier NULL
,	[FuelsManagerVersionNumber] int NULL
,	[SourceVersionNumber] int NULL
,	[HistoricalFlag] bit NULL
,	[VolumetricTopOffFlag] bit NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactionWeightReadings_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactionWeightReadings_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactionWeightReadings_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)






GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionWeightReadings_AuditGUID] ON [fmaudit].[tblTransactionWeightReadings](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionWeightReadings_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactionWeightReadings] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTransactionWeightReadings_ClusterIdx] ON [fmaudit].[tblTransactionWeightReadings](_ClusterIdx ASC)