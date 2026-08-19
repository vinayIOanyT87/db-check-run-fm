CREATE TABLE [fmaudit].[map_tblEntityAutoDistributionRuleToSite](
	[AutoDistributionRuleToSiteGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[AutoDistributionRuleGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[AssignedFromSiteGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_map_tblEntityAutoDistributionRuleToSite_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_map_tblEntityAutoDistributionRuleToSite_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_map_tblEntityAutoDistributionRuleToSite_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)


GO

CREATE NONCLUSTERED INDEX [IX_map_tblEntityAutoDistributionRuleToSite_AuditGUID] ON [fmaudit].[map_tblEntityAutoDistributionRuleToSite](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_map_tblEntityAutoDistributionRuleToSite_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[map_tblEntityAutoDistributionRuleToSite] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_map_tblEntityAutoDistributionRuleToSite_ClusterIdx] ON [fmaudit].[map_tblEntityAutoDistributionRuleToSite](_ClusterIdx ASC)