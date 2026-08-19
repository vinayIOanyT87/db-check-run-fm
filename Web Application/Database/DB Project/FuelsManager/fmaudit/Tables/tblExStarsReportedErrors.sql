CREATE TABLE [fmaudit].[tblExStarsReportedErrors](
	[ManagerCompanyGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[SequenceNumber] nvarchar (20) NULL
,	[MustCorrect] bit NULL
,	[PBI01_Primary] nvarchar (10) NULL
,	[PBI01_Secondary] nvarchar (10) NULL
,	[PBI03_Primary] nvarchar (10) NULL
,	[PBI03_Secondary] nvarchar (10) NULL
,	[PBI04] nvarchar (10) NULL
,	[OriginalValue] nvarchar (max) NULL
,	[IRSErrorText] nvarchar (max) NULL
,	[ErrorCorrected] bit NULL
,	[ExStarsFilingsGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[ExStarsReportedErrorsGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblExStarsReportedErrors_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblExStarsReportedErrors_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblExStarsReportedErrors_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblExStarsReportedErrors_AuditGUID] ON [fmaudit].[tblExStarsReportedErrors](_AuditGUID ASC)
GO
CREATE CLUSTERED INDEX [IX_tblExStarsReportedErrors_ClusterIdx] ON [fmaudit].[tblExStarsReportedErrors](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExStarsReportedErrors_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExStarsReportedErrors] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)