CREATE TABLE [fmaudit].[tblAlarmAndEvents](
	[Source] nvarchar (120) NULL
,	[Alarm] bit NULL
,	[ID] nvarchar (120) NULL
,	[CategoryIndex] int NULL
,	[PriorityIndex] int NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[Enabled] bit NULL
,	[AlarmAndEventGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[CategoryGuid] uniqueidentifier NULL
,	[PriorityGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAlarmAndEvents_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAlarmAndEvents_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAlarmAndEvents_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO


CREATE NONCLUSTERED INDEX [IX_tblAlarmAndEvents_AuditGUID] ON [fmaudit].[tblAlarmAndEvents](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmAndEvents_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAlarmAndEvents] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblAlarmAndEvents_ClusterIdx] ON [fmaudit].[tblAlarmAndEvents](_ClusterIdx ASC)