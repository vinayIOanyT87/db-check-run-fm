CREATE TABLE [fmaudit].[tblGasboyStationEvent](
	[GasboyStationEventGuid] uniqueidentifier NULL
,	[ExternalStationLogGuid] uniqueidentifier NULL
,	[EventID] int NULL
,	[LookupGasboyEventErrorClassCodeIndex] int NULL
,	[ErrorCode] int NULL
,	[FleetID] int NULL
,	[ObjectID] int NULL
,	[LookupGasboyEventObjectTypeIndex] int NULL
,	[DeviceName] nvarchar (100) NULL
,	[Field1] nvarchar (100) NULL
,	[Field2] nvarchar (100) NULL
,	[Field3] nvarchar (100) NULL
,	[Field4] nvarchar (100) NULL
,	[Field5] nvarchar (100) NULL
,	[Field6] nvarchar (100) NULL
,	[Field7] nvarchar (100) NULL
,	[Field8] nvarchar (100) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblGasboyStationEvent_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblGasboyStationEvent_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblGasboyStationEvent_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblGasboyStationEvent_ClusterIdx] ON [fmaudit].[tblGasboyStationEvent](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblGasboyStationEvent_AuditGUID] ON [fmaudit].[tblGasboyStationEvent](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblGasboyStationEvent_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblGasboyStationEvent] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)