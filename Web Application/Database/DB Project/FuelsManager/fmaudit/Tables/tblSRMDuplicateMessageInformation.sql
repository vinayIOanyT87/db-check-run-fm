CREATE TABLE [fmaudit].[tblSRMDuplicateMessageInformation](
	[SRMDuplicateMessageInformationGuid] uniqueidentifier NULL
,	[MessageSequenceNumber] nvarchar (100) NULL
,	[FlightNumber] nvarchar (10) NULL
,	[FlightOriginationDate] datetimeoffset NULL
,	[OriginIATACode] nvarchar (10) NULL
,	[DestinationIATACode] nvarchar (10) NULL
,	[AirlineIATACode] nvarchar (10) NULL
,	[TimesLegFlown] nvarchar (10) NULL
,	[HashValue] nvarchar (32) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblSRMDuplicateMessageInformation_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblSRMDuplicateMessageInformation_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblSRMDuplicateMessageInformation_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblSRMDuplicateMessageInformation_AuditGUID] ON [fmaudit].[tblSRMDuplicateMessageInformation](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblSRMDuplicateMessageInformation_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblSRMDuplicateMessageInformation] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblSRMDuplicateMessageInformation_ClusterIdx] ON [fmaudit].[tblSRMDuplicateMessageInformation](_ClusterIdx ASC)