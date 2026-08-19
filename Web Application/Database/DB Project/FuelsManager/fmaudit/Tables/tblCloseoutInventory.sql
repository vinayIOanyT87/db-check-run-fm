CREATE TABLE [fmaudit].[tblCloseoutInventory](
	[Site] nvarchar (30) NULL
,	[CloseoutDate] date NULL
,	[ProductName] nvarchar (30) NULL
,	[ManagerName] nvarchar (100) NULL
,	[GrossBookInventory] float NULL
,	[NetBookInventory] float NULL
,	[GrossPhysicalInventory] float NULL
,	[NetPhysicalInventory] float NULL
,	[GrossVariance] float NULL
,	[NetVariance] float NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[GrossBookPrice] float NULL
,	[NetBookPrice] float NULL
,	[GrossPhysicalPrice] float NULL
,	[NetPhysicalPrice] float NULL
,	[TransVersion] bigint NULL
,	[MassBookInventory] float NULL
,	[MassPhysicalInventory] float NULL
,	[MassVariance] float NULL
,	[MassBookPrice] float NULL
,	[MassPhysicalPrice] float NULL
,	[CloseoutInventoryGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ManagerCompanyGuid] uniqueidentifier NULL
,	[ProductGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblCloseoutInventory_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblCloseoutInventory_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblCloseoutInventory_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblCloseoutInventory_AuditGUID] ON [fmaudit].[tblCloseoutInventory](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblCloseoutInventory_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblCloseoutInventory] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblCloseoutInventory_ClusterIdx] ON [fmaudit].[tblCloseoutInventory](_ClusterIdx ASC)