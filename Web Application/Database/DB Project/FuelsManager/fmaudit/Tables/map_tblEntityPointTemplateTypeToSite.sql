CREATE TABLE [fmaudit].[map_tblEntityPointTemplateTypeToSite](
	[PointTemplateTypeToSiteGuid] uniqueidentifier NULL
,	[ApplicationStringGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] binary(8) NULL
,	[AssignedFromSiteGuid] uniqueidentifier NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_map_tblEntityPointTemplateTypeToSite_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_map_tblEntityPointTemplateTypeToSite_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_map_tblEntityPointTemplateTypeToSite_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL)
GO
CREATE CLUSTERED INDEX [IX_map_tblEntityPointTemplateTypeToSite_ClusterIdx] ON [fmaudit].[map_tblEntityPointTemplateTypeToSite](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_map_tblEntityPointTemplateTypeToSite_AuditCreatedDate] ON [fmaudit].[map_tblEntityPointTemplateTypeToSite](_AuditCreatedDate ASC)
GO
CREATE NONCLUSTERED INDEX [IX_map_tblEntityPointTemplateTypeToSite_AuditGUID] ON [fmaudit].[map_tblEntityPointTemplateTypeToSite](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_map_tblEntityPointTemplateTypeToSite_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[map_tblEntityPointTemplateTypeToSite] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)