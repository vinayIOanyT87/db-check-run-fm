CREATE TABLE [fmaudit].[tblDispatchGridColumn](
	[DispatchGridColumnGuid] uniqueidentifier NULL
,	[DispatchGridGuid] uniqueidentifier NULL
,	[DispatchGridID] nvarchar (50) NULL
,	[LookupDispatchGridColumnTypeIndex] int NULL
,	[ID] nvarchar (50) NULL
,	[ColumnOrder] int NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[UserDataFieldTransactionAliasGuid] uniqueidentifier NULL
,	[UserDataFieldTransactionAliasLineItemGuid] uniqueidentifier NULL
,	[AliasName] nvarchar (50) NULL
,	[UserDataNumber] int NULL
,	[UserGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblDispatchGridColumn_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblDispatchGridColumn_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblDispatchGridColumn_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblDispatchGridColumn_AuditGUID] ON [fmaudit].[tblDispatchGridColumn](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblDispatchGridColumn_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblDispatchGridColumn] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblDispatchGridColumn_ClusterIdx] ON [fmaudit].[tblDispatchGridColumn](_ClusterIdx ASC)