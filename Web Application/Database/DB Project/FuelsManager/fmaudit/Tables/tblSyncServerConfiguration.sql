CREATE TABLE [fmaudit].[tblSyncServerConfiguration](
	[SyncServerConfigurationGuid] uniqueidentifier NULL
,	[AllowSynchronizationFlag] bit NULL
,	[AcceptFMUserAuthenticationFlag] bit NULL
,	[AcceptClientCertificateAuthenticationFlag] bit NULL
,	[ClientSignatureRequiredForMessagesFlag] bit NULL
,	[ClientEncryptionRequiredForMessagesFlag] bit NULL
,	[NodeHealthCriticalThresholdHours] int NULL
,	[NodeHealthCautionThresholdHours] int NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblSyncServerConfiguration_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblSyncServerConfiguration_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblSyncServerConfiguration_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblSyncServerConfiguration_AuditGUID] ON [fmaudit].[tblSyncServerConfiguration](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblSyncServerConfiguration_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblSyncServerConfiguration] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_tblSyncServerConfiguration_ClusterIdx] ON [fmaudit].[tblSyncServerConfiguration](_ClusterIdx ASC)