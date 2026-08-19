CREATE TABLE [fmaudit].[tblExternalStation](
	[ExternalStationGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ID] nvarchar (50) NULL
,	[LookupExternalStationTypeIndex] int NULL
,	[BillingID] nvarchar (50) NULL
,	[DownloadTransactionsAutomatically] bit NULL
,	[LookupExternalStationStatusIndex] int NULL
,	[LastSuccessfulConnection] datetimeoffset NULL
,	[LastConnectionAttempt] datetimeoffset NULL
,	[LastTransactionID] bigint NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] binary(8) NULL
,	[LastDeviceCount] int NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblExternalStation_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblExternalStation_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblExternalStation_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)

GO
CREATE CLUSTERED INDEX [IX_tblExternalStation_ClusterIdx] ON [fmaudit].[tblExternalStation](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExternalStation_AuditGUID] ON [fmaudit].[tblExternalStation](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExternalStation_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExternalStation] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)