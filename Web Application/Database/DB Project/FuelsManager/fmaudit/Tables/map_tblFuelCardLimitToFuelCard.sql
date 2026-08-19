CREATE TABLE [fmaudit].[map_tblFuelCardLimitToFuelCard](
	[FuelCardLimitToFuelCardGuid] uniqueidentifier NULL
,	[FuelCardLimitGuid] uniqueidentifier NULL
,	[FuelCardGuid] uniqueidentifier NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_map_tblFuelCardLimitToFuelCard_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_map_tblFuelCardLimitToFuelCard_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_map_tblFuelCardLimitToFuelCard_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_map_tblFuelCardLimitToFuelCard_AuditGUID] ON [fmaudit].[map_tblFuelCardLimitToFuelCard](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_map_tblFuelCardLimitToFuelCard_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[map_tblFuelCardLimitToFuelCard] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_map_tblFuelCardLimitToFuelCard_ClusterIdx] ON [fmaudit].[map_tblFuelCardLimitToFuelCard](_ClusterIdx ASC)