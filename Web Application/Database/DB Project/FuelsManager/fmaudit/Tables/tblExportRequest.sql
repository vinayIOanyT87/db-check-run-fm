CREATE TABLE [fmaudit].[tblExportRequest](
	[ExportRequestGuid] uniqueidentifier NULL
,	[RequestID] nvarchar (200) NULL
,	[InterfaceID] nvarchar (200) NULL
,	[OwnerCode] nvarchar (10) NULL
,	[UploadStagingFolder] nvarchar (200) NULL
,	[ArchiveFolder] nvarchar (200) NULL
,	[ConnectionInfo] nvarchar (max) NULL
,	[SendingCompanyCode] nvarchar (50) NULL
,	[SendViaFTP] bit NULL
,	[SendSecure] bit NULL
,	[CompanyNames] nvarchar (max) NULL
,	[LatestRowVersion] bigint NULL
,	[LastExportTime] datetimeoffset NULL
,	[ExportFrequency] int NULL
,	[BaselineDate] datetimeoffset NULL
,	[ExcludeEmptyFiles] bit NULL
,	[UseTimeOfDay] bit NULL
,	[NextExportTime] datetimeoffset NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblExportRequest_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblExportRequest_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblExportRequest_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
,	[SendMethod] INT CONSTRAINT [DF_tblExportRequest_SendMethod]  DEFAULT ((0)) NOT NULL
,	[WebServicePluginType] NVARCHAR(100) NULL
,	[WebServiceConfiguration] NVARCHAR(512) NULL
)




GO

CREATE NONCLUSTERED INDEX [IX_tblExportRequest_AuditGUID] ON [fmaudit].[tblExportRequest](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblExportRequest_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExportRequest] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblExportRequest_ClusterIdx] ON [fmaudit].[tblExportRequest](_ClusterIdx ASC)