CREATE TABLE [fmaudit].[tblExternalGasboyStation](
	[ExternalStationGuid] uniqueidentifier NULL
,	[SiteCode] int NULL
,	[IPAddress] nvarchar (50) NULL
,	[UserName] nvarchar (50) NULL
,	[Password] varbinary (256) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblExternalGasboyStation_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblExternalGasboyStation_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblExternalGasboyStation_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)

GO
CREATE CLUSTERED INDEX [IX_tblExternalGasboyStation_ClusterIdx] ON [fmaudit].[tblExternalGasboyStation](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExternalGasboyStation_AuditGUID] ON [fmaudit].[tblExternalGasboyStation](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExternalGasboyStation_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExternalGasboyStation] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)