CREATE TABLE [fmaudit].[tblModule](
	[ID] nvarchar (30) NULL
,	[Description] nvarchar (50) NULL
,	[Standard] bit NULL
,	[ModuleCalculation] nvarchar (250) NULL
,	[ModuleTypeName] nvarchar (250) NULL
,	[ModuleData] xml NULL
,	[ModuleScript] nvarchar (max) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[ModuleGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblModule_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblModule_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblModule_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblModule_ClusterIdx] ON [fmaudit].[tblModule](_ClusterIdx ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblModule_AuditGUID] ON [fmaudit].[tblModule](_AuditGUID ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblModule_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblModule] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) 
GO
