CREATE TABLE [fmaudit].[tblAutoDistributionReasonCodes](
	[AutoDistributionReasonCodeGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ReasonCode] nvarchar (50) NULL
,	[Description] nvarchar (255) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (50) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (50) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAutoDistributionReasonCodes_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAutoDistributionReasonCodes_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAutoDistributionReasonCodes_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblAutoDistributionReasonCodes_AuditGUID] ON [fmaudit].[tblAutoDistributionReasonCodes](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblAutoDistributionReasonCodes_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAutoDistributionReasonCodes] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblAutoDistributionReasonCodes_ClusterIdx] ON [fmaudit].[tblAutoDistributionReasonCodes](_ClusterIdx ASC)