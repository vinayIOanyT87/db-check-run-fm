CREATE TABLE [fmaudit].[tblVRUThresholds](
	[VRUThresholdGuid] uniqueidentifier NULL
,	[ID] nvarchar (60) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[Interval] int NULL
,	[IntervalType] int NULL
,	[Limit] float NULL
,	[Tolerance] decimal NULL
,	[Enabled] bit NULL
,	[ResetDate] datetimeoffset NULL
,	[CurrentValue] float NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[LastCalculationDate] datetimeoffset NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblVRUThresholds_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblVRUThresholds_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblVRUThresholds_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblVRUThresholds_ClusterIdx] ON [fmaudit].[tblVRUThresholds](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblVRUThresholds_AuditGUID] ON [fmaudit].[tblVRUThresholds](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblVRUThresholds_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblVRUThresholds] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)