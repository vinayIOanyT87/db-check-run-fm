CREATE TABLE [fmaudit].[tblAlarmTest](
	[AlarmTestGuid] uniqueidentifier NULL
,	[AlarmGuid] uniqueidentifier NULL
,	[ID] nvarchar (256) NULL
,	[LimitTagGuid] uniqueidentifier NULL
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
,	[AlarmTestTemplateGuid] uniqueidentifier NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblAlarmTest_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblAlarmTest_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblAlarmTest_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblAlarmTest_ClusterIdx] ON [fmaudit].[tblAlarmTest](_ClusterIdx ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmTest_AuditGUID] ON [fmaudit].[tblAlarmTest](_AuditGUID ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblAlarmTest_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAlarmTest] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) 
GO
