CREATE TABLE [fmaudit].[tblSiteCloseoutTime](
	[SiteCloseoutTimeGuid] uniqueidentifier NULL
,	[EffectiveDate] datetimeoffset NULL
,	[ExpirationDate] datetimeoffset NULL
,	[CloseoutTime] time NULL
,  [PointTagRefDataAsXML] XML
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_ClusterIdx] bigint NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblSiteCloseoutTime_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblSiteCloseoutTime_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblSiteCloseoutTime_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_AuditContext] [varbinary](128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblSiteCloseoutTime_AuditCreatedDate] ON [fmaudit].[tblSiteCloseoutTime](_AuditCreatedDate ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblSiteCloseoutTime_AuditGUID] ON [fmaudit].[tblSiteCloseoutTime](_AuditGUID ASC) 
GO