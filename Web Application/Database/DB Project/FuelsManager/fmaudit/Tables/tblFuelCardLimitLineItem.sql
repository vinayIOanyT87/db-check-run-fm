CREATE TABLE [fmaudit].[tblFuelCardLimitLineItem](
	[FuelCardLimitLineItemGuid] uniqueidentifier NULL
,	[FuelCardLimitGuid] uniqueidentifier NULL
,	[Limit] float NULL
,	[Period] int NULL
,	[ProductGuid] uniqueidentifier NULL
,	[ProductGroupApplicationStringGuid] uniqueidentifier NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblFuelCardLimitLineItem_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblFuelCardLimitLineItem_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblFuelCardLimitLineItem_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)


GO

CREATE NONCLUSTERED INDEX [IX_tblFuelCardLimitLineItem_AuditGUID] ON [fmaudit].[tblFuelCardLimitLineItem](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblFuelCardLimitLineItem_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblFuelCardLimitLineItem] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblFuelCardLimitLineItem_ClusterIdx] ON [fmaudit].[tblFuelCardLimitLineItem](_ClusterIdx ASC)