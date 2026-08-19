CREATE TABLE [fmaudit].[tblAutoDistributionRule](
	[AutoDistributionRuleGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[RuleID] nvarchar (50) NULL
,	[RuleDescription] nvarchar (255) NULL
,	[RuleEnabled] bit NULL
,	[DefaultEOM] bit NULL
,	[TransactionAliasGuid] uniqueidentifier NULL
,	[DefaultReasonCodeGuid] uniqueidentifier NULL
,	[DefaultNotes] nvarchar (1000) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAutoDistributionRule_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAutoDistributionRule_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAutoDistributionRule_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblAutoDistributionRule_AuditGUID] ON [fmaudit].[tblAutoDistributionRule](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblAutoDistributionRule_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAutoDistributionRule] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblAutoDistributionRule_ClusterIdx] ON [fmaudit].[tblAutoDistributionRule](_ClusterIdx ASC)