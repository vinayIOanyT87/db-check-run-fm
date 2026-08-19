CREATE TABLE [fmaudit].[tblExStarsEndingInventory](
	[ManagerCompanyGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ProductGuid] uniqueidentifier NULL
,	[ReportYear] int NULL
,	[ReportMonth] int NULL
,	[ReportDay] int NULL
,	[PriorInventoryExists] bit NULL
,	[GrossVolume] float NULL
,	[NetVolume] float NULL
,	[EndingInventoryDate] datetimeoffset NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[ExStarsEndingInventoryGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblExStarsEndingInventory_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblExStarsEndingInventory_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblExStarsEndingInventory_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblExStarsEndingInventory_AuditGUID] ON [fmaudit].[tblExStarsEndingInventory](_AuditGUID ASC)
GO
CREATE CLUSTERED INDEX [IX_tblExStarsEndingInventory_ClusterIdx] ON [fmaudit].[tblExStarsEndingInventory](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExStarsEndingInventory_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExStarsEndingInventory] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)