CREATE TABLE [fmaudit].[tblTransactionAliasFields](
	[AliasID] int NULL
,	[DbName] nvarchar (50) NULL
,	[DisplayOrder] int NULL
,	[DisplayName] nvarchar (50) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[Required] bit NULL
,	[Virtual] bit NULL
,	[TransactionAliasFieldGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[LookupTransactionFieldTypeIndex] int NULL
,	[TransactionAliasGuid] uniqueidentifier NULL
,	[UserGroupGuid] uniqueidentifier NULL
,	[DispatchField] bit NULL
,	[ClearOnNew] bit NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactionAliasFields_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactionAliasFields_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactionAliasFields_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
,   [ReadOnly] BIT NULL
,   [Visibility] INT NULL
,	[DefaultValueType] NVARCHAR(MAX) NULL
,	[DefaultValue] XML NULL

)




GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionAliasFields_AuditGUID] ON [fmaudit].[tblTransactionAliasFields](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliasFields_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactionAliasFields] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_tblTransactionAliasFields_ClusterIdx] ON [fmaudit].[tblTransactionAliasFields](_ClusterIdx ASC)