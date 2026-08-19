CREATE TABLE [fmaudit].[tblOpcUaServer](
	[OpcUaServerGuid] uniqueidentifier NULL
,	[ServerEndPoint] nvarchar (250) NULL
,	[SecurityMode] nvarchar (50) NULL
,	[SecurityPolicy] nvarchar (50) NULL
,	[MessageEncoding] nvarchar (50) NULL
,	[UserIdentityMethod] nvarchar (50) NULL
,	[UserId] nvarchar (250) NULL
,	[UserPassword] nvarchar (250) NULL
,	[UserCertificatePath] nvarchar (250) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblOpcUaServer_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblOpcUaServer_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblOpcUaServer_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblOpcUaServer_ClusterIdx] ON [fmaudit].[tblOpcUaServer](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblOpcUaServer_AuditGUID] ON [fmaudit].[tblOpcUaServer](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblOpcUaServer_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblOpcUaServer] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)