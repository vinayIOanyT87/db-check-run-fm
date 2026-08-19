CREATE TABLE [fmaudit].[tblTransactionTransportLineItems](
	[TransportOrderNumber] nvarchar (50) NULL
,	[TransVersion] bigint NULL
,	[LocationName] nvarchar (30) NULL
,	[Address1] nvarchar (60) NULL
,	[Address2] nvarchar (60) NULL
,	[City] nvarchar (60) NULL
,	[State] nvarchar (20) NULL
,	[Zip] nvarchar (11) NULL
,	[POCName] nvarchar (50) NULL
,	[POCPhone] nvarchar (20) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[TransactionTransportLineItemGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[TransactionGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactionTransportLineItems_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactionTransportLineItems_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactionTransportLineItems_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)






GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionTransportLineItems_AuditGUID] ON [fmaudit].[tblTransactionTransportLineItems](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionTransportLineItems_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactionTransportLineItems] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_tblTransactionTransportLineItems_ClusterIdx] ON [fmaudit].[tblTransactionTransportLineItems](_ClusterIdx ASC)