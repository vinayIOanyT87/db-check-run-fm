CREATE TABLE [fmaudit].[tblExternalStationTransaction](
	[ExternalStationTransactionGuid] uniqueidentifier NULL
,	[ExternalStationGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[StationTransactionID] nvarchar (20) NULL
,	[RawTransactionData] nvarchar (max) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[LookupExternalStationTransactionStatusIndex] int NULL
,	[LookupExternalStationTransactionFailedStatusIndex] int NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblExternalStationTransaction_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblExternalStationTransaction_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblExternalStationTransaction_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblExternalStationTransaction_ClusterIdx] ON [fmaudit].[tblExternalStationTransaction](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExternalStationTransaction_AuditGUID] ON [fmaudit].[tblExternalStationTransaction](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExternalStationTransaction_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExternalStationTransaction] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)