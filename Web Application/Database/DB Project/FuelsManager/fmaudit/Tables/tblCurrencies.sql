CREATE TABLE [fmaudit].[tblCurrencies](
	[Country] nvarchar (50) NULL
,	[UnitDisplayName] nvarchar (50) NULL
,	[DisplayFlag] bit NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[CurrencyGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupCurrencyUnitIndex] int NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblCurrencies_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblCurrencies_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblCurrencies_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblCurrencies_AuditGUID] ON [fmaudit].[tblCurrencies](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblCurrencies_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblCurrencies] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblCurrencies_ClusterIdx] ON [fmaudit].[tblCurrencies](_ClusterIdx ASC)