CREATE TABLE [fmaudit].[map_tblModuleToPointTemplate](
	[ID] nvarchar (30) NULL
,	[Order] int NULL
,	[ModuleToPointTemplateData] xml NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[ModuleToPointTemplateGuid] uniqueidentifier NULL
,	[PointTemplateGuid] uniqueidentifier NULL
,	[ModuleGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_map_tblModuleToPointTemplate_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_map_tblModuleToPointTemplate_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_map_tblModuleToPointTemplate_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_map_tblModuleToPointTemplate_ClusterIdx] ON [fmaudit].[map_tblModuleToPointTemplate](_ClusterIdx ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_map_tblModuleToPointTemplate_AuditGUID] ON [fmaudit].[map_tblModuleToPointTemplate](_AuditGUID ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_map_tblModuleToPointTemplate_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[map_tblModuleToPointTemplate] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) 
GO