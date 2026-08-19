CREATE TABLE [fmaudit].[map_tblProductToLedgerView](
	[ProductToLedgerViewGuid] uniqueidentifier NULL
,	[ProductGuid] uniqueidentifier NULL
,	[AssignedToListViewGuid] uniqueidentifier NULL
,	[Sequence] int NULL
,	[BlendPercentage] float NULL
,	[AdditiveRate] float NULL
,	[Ratio] float NULL
,	[AdditiveCycleVolume] float NULL
,	[Tolerance] float NULL
,	[PresetNumber] int NULL
,	[AdditiveProfileGuid] uniqueidentifier NULL
,	[TankGuid] uniqueidentifier NULL
,	[MeterID] nvarchar (20) NULL
,	[ShipToProductID] nvarchar (30) NULL
,	[ShipToProductCode] nvarchar (15) NULL
,	[ShipToLoadRackDisplayText] nvarchar (10) NULL
,	[UnavailableInventoryGross] float NULL
,	[UnavailableInventoryNet] float NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_map_tblProductToLedgerView_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_map_tblProductToLedgerView_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_map_tblProductToLedgerView_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)


GO

CREATE NONCLUSTERED INDEX [IX_map_tblProductToLedgerView_AuditGUID] ON [fmaudit].[map_tblProductToLedgerView](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_map_tblProductToLedgerView_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[map_tblProductToLedgerView] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_map_tblProductToLedgerView_ClusterIdx] ON [fmaudit].[map_tblProductToLedgerView](_ClusterIdx ASC)