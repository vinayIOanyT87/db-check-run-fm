CREATE TABLE [fmaudit].[tblAirplaneTank](
	[Alias] nvarchar (50) NULL
,	[AirlineTankId] nvarchar (50) NULL
,	[Description] nvarchar (250) NULL
,	[Capacity] float NULL
,	[Position] int NULL
,	[Location] int NULL
,	[DisplayOrder] int NULL
,	[TankGuid] uniqueidentifier NULL
,	[EquipmentTypeGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAirplaneTank_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAirplaneTank_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAirplaneTank_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO


CREATE NONCLUSTERED INDEX [IX_tblAirplaneTank_AuditGUID] ON [fmaudit].[tblAirplaneTank](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblAirplaneTank_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAirplaneTank] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblAirplaneTank_ClusterIdx] ON [fmaudit].[tblAirplaneTank](_ClusterIdx ASC)