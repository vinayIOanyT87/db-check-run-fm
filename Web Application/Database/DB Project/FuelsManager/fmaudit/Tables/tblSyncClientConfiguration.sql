CREATE TABLE [fmaudit].[tblSyncClientConfiguration](
	[SyncClientConfigurationGuid] uniqueidentifier NULL
,	[RootSiteID] nvarchar (30) NULL
,	[EnterpriseURL] nvarchar (1024) NULL
,	[SuspendSynchronizationFlag] bit NULL
,	[ServerAuthUserName] nvarchar (256) NULL
,	[ServerAuthPassword] varbinary (256) NULL
,	[ServerAuthDomain] nvarchar (256) NULL
,	[ServerAuthClientCertificate] nvarchar (768) NULL
,	[FMAuthUserName] nvarchar (100) NULL
,	[FMAuthPassword] varbinary (256) NULL
,	[FMAuthClientCertificate] nvarchar (768) NULL
,	[MessageSecuritySigningCertificate] nvarchar (768) NULL
,	[MessageSecurityOfflineEncryptionCertificate] nvarchar (768) NULL
,	[MessageSecurityOfflineDecryptionCertificate] nvarchar (768) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[ServiceMaximumRetryAttempts] int NULL
,	[ServiceRetryWaitTime] int NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblSyncClientConfiguration_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblSyncClientConfiguration_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblSyncClientConfiguration_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblSyncClientConfiguration_AuditGUID] ON [fmaudit].[tblSyncClientConfiguration](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblSyncClientConfiguration_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblSyncClientConfiguration] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_tblSyncClientConfiguration_ClusterIdx] ON [fmaudit].[tblSyncClientConfiguration](_ClusterIdx ASC)