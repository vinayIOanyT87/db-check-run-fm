CREATE TABLE [fmaudit].[tblCurrencyLineItems](
	[Date] datetimeoffset NULL
,	[Rate] float NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[CurrencyLineItemGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[CurrencyGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblCurrencyLineItems_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblCurrencyLineItems_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblCurrencyLineItems_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblCurrencyLineItems_AuditGUID] ON [fmaudit].[tblCurrencyLineItems](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblCurrencyLineItems_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblCurrencyLineItems] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblCurrencyLineItems_ClusterIdx] ON [fmaudit].[tblCurrencyLineItems](_ClusterIdx ASC)