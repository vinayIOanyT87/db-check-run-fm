CREATE TABLE [fmaudit].[tblExStarsSiteConfig](
	[SiteGuid] uniqueidentifier NULL
,	[ManagerCompanyGuid] uniqueidentifier NULL
,	[InterchangeSenderId] nvarchar (15) NULL
,	[ApplicationSendersCode] nvarchar (15) NULL
,	[AuthorizationCode] nvarchar (10) NULL
,	[FeinCode] nvarchar (18) NULL
,	[SecurityCode] nvarchar (10) NULL
,	[InfoProviderName] nvarchar (18) NULL
,	[AbbreviatedProviderName] nvarchar (18) NULL
,	[GroupControlNumber] nvarchar (9) NULL
,	[IRS_637Registration] nvarchar (18) NULL
,	[TerminalControlNumber] nvarchar (9) NULL
,	[ISA05Qualifier] nvarchar (2) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[ExStarsSiteConfigdGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblExStarsSiteConfig_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblExStarsSiteConfig_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblExStarsSiteConfig_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblExStarsSiteConfig_AuditGUID] ON [fmaudit].[tblExStarsSiteConfig](_AuditGUID ASC)
GO
CREATE CLUSTERED INDEX [IX_tblExStarsSiteConfig_ClusterIdx] ON [fmaudit].[tblExStarsSiteConfig](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExStarsSiteConfig_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExStarsSiteConfig] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)