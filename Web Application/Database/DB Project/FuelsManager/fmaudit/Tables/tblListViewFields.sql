CREATE TABLE [fmaudit].[tblListViewFields](
	[ColumnOrder] int NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[ListViewID] nvarchar (50) NULL
,	[ListViewFieldGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[LookupListViewFieldTypeIndex] int NULL
,	[LookupStandardFieldTypeIndex] int NULL
,	[ListViewGuid] uniqueidentifier NULL
,	[TransactionAliasGuid] uniqueidentifier NULL
,	[TransactionAliasFieldGuid] uniqueidentifier NULL
,	[UserDataFieldTransactionAliasGuid] uniqueidentifier NULL
,	[UserDataFieldTransactionAliasLineItemGuid] uniqueidentifier NULL
,	[LedgerAggregateColumnGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblListViewFields_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblListViewFields_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblListViewFields_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblListViewFields_AuditGUID] ON [fmaudit].[tblListViewFields](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblListViewFields_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblListViewFields] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblListViewFields_ClusterIdx] ON [fmaudit].[tblListViewFields](_ClusterIdx ASC)