CREATE TABLE [fmaudit].[tblUserDataFieldUser](
	[UserDataFieldUserGuid] uniqueidentifier NULL
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
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblUserDataFieldUser_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblUserDataFieldUser_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblUserDataFieldUser_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
,   [ReadOnly] BIT NULL
,   [Visibility] INT NULL
,	[DefaultValue] NVARCHAR(120) NULL
)
GO
CREATE CLUSTERED INDEX [IX_tblUserDataFieldUser_ClusterIdx] ON [fmaudit].[tblUserDataFieldUser](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblUserDataFieldUser_AuditGUID] ON [fmaudit].[tblUserDataFieldUser](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblUserDataFieldUser_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblUserDataFieldUser] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)