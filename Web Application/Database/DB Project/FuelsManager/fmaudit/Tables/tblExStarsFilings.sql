CREATE TABLE [fmaudit].[tblExStarsFilings](
	[FilingStartDate] date NULL
,	[FilingEndDate] date NULL
,	[ManagerCompanyGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ReportType] nvarchar (30) NULL
,	[Modifier] nvarchar (30) NULL
,	[ControlNumber] nvarchar (9) NULL
,	[TransSetControlNumber] nvarchar (9) NULL
,	[OriginalControlNumber] nchar (9) NULL
,	[FilingStatus] nvarchar (30) NULL
,	[FilingCreated] datetimeoffset NULL
,	[FilingSent] datetimeoffset NULL
,	[ResponseLoaded] datetimeoffset NULL
,	[RawDataFileName] nvarchar (max) NULL
,	[EasyReadFileName] nvarchar (max) NULL
,	[EdiReport] nvarchar (max) NULL
,	[EasyReadReport] nvarchar (max) NULL
,	[SerializedData] nvarchar (max) NULL
,	[Acknowledgement] nvarchar (max) NULL
,	[AckEasyRead] nvarchar (max) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[ExStarsFilingsGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblExStarsFilings_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblExStarsFilings_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblExStarsFilings_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblExStarsFilings_AuditGUID] ON [fmaudit].[tblExStarsFilings](_AuditGUID ASC)
GO
CREATE CLUSTERED INDEX [IX_tblExStarsFilings_ClusterIdx] ON [fmaudit].[tblExStarsFilings](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExStarsFilings_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExStarsFilings] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)