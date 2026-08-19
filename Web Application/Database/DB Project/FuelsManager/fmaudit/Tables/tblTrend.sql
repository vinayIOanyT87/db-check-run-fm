CREATE TABLE [fmaudit].[tblTrend](
	[ID]								NVARCHAR (30)	CONSTRAINT [DF_tblTrend_ID] DEFAULT ('') NOT NULL
,	[Description]					NVARCHAR (50)	NULL
,	[Mode]							NVARCHAR (10) CONSTRAINT [DF_tblTrend_Mode] DEFAULT ('Realtime') NOT NULL
,	[PeriodType]					NVARCHAR (7) CONSTRAINT [DF_tblTrend_PeriodType] DEFAULT('Minutes') NOT NULL
,	[Period]							FLOAT CONSTRAINT [DF_tblTrend_Period] DEFAULT(60) NOT NULL
,	[Start]							DATETIMEOFFSET (7) CONSTRAINT [DF_tblTrend_Start] DEFAULT (sysdatetimeoffset()) NOT NULL
,	[End]								DATETIMEOFFSET (7) CONSTRAINT [DF_tblTrend_End] DEFAULT (sysdatetimeoffset()) NOT NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[TrendGuid] uniqueidentifier NULL
,	[OriginalRowVersion] binary(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[PointTemplateGuid] uniqueidentifier NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblTrend_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblTrend_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblTrend_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL )
GO

CREATE CLUSTERED INDEX [IX_tblTrend_ClusterIdx] ON [fmaudit].[tblTrend](_ClusterIdx ASC)
GO

CREATE NONCLUSTERED INDEX [IX_tblTrend_AuditGUID] ON [fmaudit].[tblTrend](_AuditGUID ASC)
GO

CREATE NONCLUSTERED INDEX [IX_tblTrend_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTrend] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)