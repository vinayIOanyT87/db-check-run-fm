CREATE TABLE [fmaudit].[tblAlarm](
	[AlarmGuid] uniqueidentifier NULL
,	[InputTagGuid] uniqueidentifier NULL
,	[ID] nvarchar (256) NULL
,	[Enabled] bit NULL
,	[AlarmCategoryApplicationStringGuid] uniqueidentifier NULL
,	[Order] int NULL
,	[NotAlarmState] nvarchar (100) NULL
,	[Comment] nvarchar (256) NULL
,	[ShelvedStartTimeStamp] datetimeoffset NULL
,	[ShelvedEndTimeStamp] datetimeoffset NULL
,	[ShelvedOneShot] bit NULL
,	[ShelvedBy] nvarchar (100) NULL
,	[Suppressed] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[AlarmStateTagGuid] uniqueidentifier NULL
,	[ExclusiveAlarm] bit NULL
,	[AlarmTemplateGuid] uniqueidentifier NULL
,	[Notify] bit NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAlarm_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAlarm_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAlarm_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblAlarm_ClusterIdx] ON [fmaudit].[tblAlarm](_ClusterIdx ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarm_AuditGUID] ON [fmaudit].[tblAlarm](_AuditGUID ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarm_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAlarm] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) 
GO
 