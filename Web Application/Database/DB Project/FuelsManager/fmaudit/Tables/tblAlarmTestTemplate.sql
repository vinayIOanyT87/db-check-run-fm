CREATE TABLE [fmaudit].[tblAlarmTestTemplate](
	[AlarmTestTemplateGuid] uniqueidentifier NULL
,	[AlarmTemplateGuid] uniqueidentifier NULL
,	[ID] nvarchar (256) NULL
,	[LimitTemplateTagGuid] uniqueidentifier NULL
,	[TagField] int NULL
,	[AlarmPriorityGuid] uniqueidentifier NULL
,	[NormalUnacknowledgedAlarmPriorityGuid] uniqueidentifier NULL
,	[TestType] int NULL
,	[BitMask] bigint NULL
,	[Enabled] bit NULL
,	[Order] int NULL
,	[AlarmState] nvarchar (100) NULL
,	[Holdoff] float NULL
,	[AlarmText] nvarchar (256) NULL
,	[HelpFile] nvarchar (max) NULL
,	[DrawingGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] binary(8) NULL
,	[BitwiseOperator] int NULL
,	[TimedHoldOffInSeconds] int NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblAlarmTestTemplate_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblAlarmTestTemplate_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblAlarmTestTemplate_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblAlarmTestTemplate_ClusterIdx] ON [fmaudit].[tblAlarmTestTemplate](_ClusterIdx ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmTestTemplate_AuditGUID] ON [fmaudit].[tblAlarmTestTemplate](_AuditGUID ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmTestTemplate_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAlarmTestTemplate] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) 
GO

