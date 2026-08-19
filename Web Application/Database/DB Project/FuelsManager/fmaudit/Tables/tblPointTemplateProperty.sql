CREATE TABLE [fmaudit].[tblPointTemplateProperty]
(
	[ID] [nvarchar](50) NOT NULL
,	[ValueType] nvarchar(max) NULL
,	[Value] XML NULL
,	[CreatedDate] [datetimeoffset](7) NOT NULL
,	[CreatedBy] [dbo].[udtUserID] NOT NULL
,	[UpdatedDate] [datetimeoffset](7) NOT NULL
,	[UpdatedBy] [dbo].[udtUserID] NOT NULL
,	[OriginalRowVersion] binary(8) NULL
,	[PointTemplatePropertyGuid] [uniqueidentifier] NOT NULL
,  [PointTemplateGuid] [uniqueidentifier] NOT NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblPointTemplateProperty_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblPointTemplateProperty_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblPointTemplateProperty_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL )
GO
CREATE CLUSTERED INDEX [IX_tblPointTemplateProperty_ClusterIdx] ON [fmaudit].[tblPointTemplateProperty](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblPointTemplateProperty_AuditGUID] ON [fmaudit].[tblPointTemplateProperty](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblPointTemplateProperty_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblPointTemplateProperty] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)