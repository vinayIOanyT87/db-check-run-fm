CREATE TABLE [fmaudit].[tblAlarmPriorities](
	[ID] nvarchar (32) NULL
,	[BackgroundSteady] nvarchar (8) NULL
,	[BackgroundAlternate] nvarchar (8) NULL
,	[TextSteady] nvarchar (8) NULL
,	[TextAlternate] nvarchar (8) NULL
,	[SoundFile] nvarchar (50) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[AlarmPriorityGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[Priority] tinyint NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAlarmPriorities_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAlarmPriorities_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAlarmPriorities_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO


CREATE NONCLUSTERED INDEX [IX_tblAlarmPriorities_AuditGUID] ON [fmaudit].[tblAlarmPriorities](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmPriorities_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAlarmPriorities] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblAlarmPriorities_ClusterIdx] ON [fmaudit].[tblAlarmPriorities](_ClusterIdx ASC)