CREATE TABLE [fmaudit].[tblAlarmTemplate](
	[AlarmTemplateGuid] uniqueidentifier NULL
,	[InputTemplateTagGuid] uniqueidentifier NULL
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
,	[OriginalRowVersion] binary(8) NULL
,	[AlarmStateTemplateTagGuid] uniqueidentifier NULL
,	[ExclusiveAlarm] bit NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblAlarmTemplate_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblAlarmTemplate_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblAlarmTemplate_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblAlarmTemplate_ClusterIdx] ON [fmaudit].[tblAlarmTemplate](_ClusterIdx ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmTemplate_AuditGUID] ON [fmaudit].[tblAlarmTemplate](_AuditGUID ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmTemplate_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAlarmTemplate] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) 
GO
