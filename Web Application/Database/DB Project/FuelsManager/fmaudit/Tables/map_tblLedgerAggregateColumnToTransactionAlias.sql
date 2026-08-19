CREATE TABLE [fmaudit].[map_tblLedgerAggregateColumnToTransactionAlias](
	[LedgerAggregateColumnToTransactionAliasGuid] uniqueidentifier NULL
,	[LedgerAggregateColumnGuid] uniqueidentifier NULL
,	[TransactionAliasGuid] uniqueidentifier NULL
,	[Symbol] nvarchar (1) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_map_tblLedgerAggregateColumnToTransactionAlias_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_map_tblLedgerAggregateColumnToTransactionAlias_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_map_tblLedgerAggregateColumnToTransactionAlias_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)


GO

CREATE NONCLUSTERED INDEX [IX_map_tblLedgerAggregateColumnToTransactionAlias_AuditGUID] ON [fmaudit].[map_tblLedgerAggregateColumnToTransactionAlias](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_map_tblLedgerAggregateColumnToTransactionAlias_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[map_tblLedgerAggregateColumnToTransactionAlias] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_map_tblLedgerAggregateColumnToTransactionAlias_ClusterIdx] ON [fmaudit].[map_tblLedgerAggregateColumnToTransactionAlias](_ClusterIdx ASC)