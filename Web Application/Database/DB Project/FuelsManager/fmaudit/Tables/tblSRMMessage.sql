CREATE TABLE [fmaudit].[tblSRMMessage](
	[SRMMessageGuid] uniqueidentifier NULL
,	[SRMAdaptorGuid] uniqueidentifier NULL
,	[ReceiptDateTime] datetimeoffset NULL
,	[ExternalSourceIdentifier] nvarchar (100) NULL
,	[FlightNumber] nvarchar (10) NULL
,	[FlightOriginationDate] datetimeoffset NULL
,	[OriginIATACode] nvarchar (10) NULL
,	[DestinationIATACode] nvarchar (10) NULL
,	[MessageText] nvarchar (max) NULL
,	[ConvertedMessageXML] nvarchar (max) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[AirlineIATACode] nvarchar (10) NULL
,	[TimesLegFlown] nvarchar (10) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblSRMMessage_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblSRMMessage_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblSRMMessage_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblSRMMessage_AuditGUID] ON [fmaudit].[tblSRMMessage](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblSRMMessage_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblSRMMessage] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblSRMMessage_ClusterIdx] ON [fmaudit].[tblSRMMessage](_ClusterIdx ASC)