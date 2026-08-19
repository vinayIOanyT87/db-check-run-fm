CREATE TABLE [fmaudit].[tblUserDataFieldSite](
	[UserDataFieldSiteGuid] uniqueidentifier NULL
,	[TransactionAliasGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[Number] tinyint NULL
,	[DisplayOrder] int NULL
,	[DisplayName] nvarchar (30) NULL
,	[LookupUserDataTypeIndex] int NULL
,	[Required] bit NULL
,	[UserGroupGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[DispatchField] bit NULL
,	[ClearOnNew] bit NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblUserDataFieldSite_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblUserDataFieldSite_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblUserDataFieldSite_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
,   [ReadOnly] BIT NULL
,   [Visibility] INT NULL
,	[DefaultValue] NVARCHAR(120) NULL
)






GO

CREATE NONCLUSTERED INDEX [IX_tblUserDataFieldSite_AuditGUID] ON [fmaudit].[tblUserDataFieldSite](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblUserDataFieldSite_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblUserDataFieldSite] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_tblUserDataFieldSite_ClusterIdx] ON [fmaudit].[tblUserDataFieldSite](_ClusterIdx ASC)