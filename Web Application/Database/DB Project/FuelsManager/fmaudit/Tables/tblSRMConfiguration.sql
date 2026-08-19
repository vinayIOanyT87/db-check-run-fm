CREATE TABLE [fmaudit].[tblSRMConfiguration](
	[SRMConfigurationGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[MessageRetryAttempts] tinyint NULL
,	[MessageRetryInterval] int NULL
,	[MessageRetentionTime] int NULL
,	[LogFailedMessages] bit NULL
,	[LogSuccessfulMessages] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblSRMConfiguration_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblSRMConfiguration_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblSRMConfiguration_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblSRMConfiguration_AuditGUID] ON [fmaudit].[tblSRMConfiguration](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblSRMConfiguration_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblSRMConfiguration] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblSRMConfiguration_ClusterIdx] ON [fmaudit].[tblSRMConfiguration](_ClusterIdx ASC)