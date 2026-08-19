CREATE TABLE [fmaudit].[tblReportDetails](
	[ReportName] nvarchar (60) NULL
,	[ReportDescription] nvarchar (255) NULL
,	[ReportPath] nvarchar (200) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OrderNumber] int NULL
,	[PrintOnlyFlag] bit NULL
,	[PrimaryPrinterName] nvarchar (100) NULL
,	[SecondaryPrinterName] nvarchar (100) NULL
,	[PrintAtEndOfDay] bit NULL
,	[PrintAtEndOfMonth] bit NULL
,	[DWReportFlag] bit NULL
,	[ReportDetailGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ReportGroupGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblReportDetails_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblReportDetails_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblReportDetails_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblReportDetails_AuditGUID] ON [fmaudit].[tblReportDetails](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblReportDetails_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblReportDetails] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblReportDetails_ClusterIdx] ON [fmaudit].[tblReportDetails](_ClusterIdx ASC)