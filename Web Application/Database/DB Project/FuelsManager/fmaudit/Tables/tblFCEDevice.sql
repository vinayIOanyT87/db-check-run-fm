CREATE TABLE [fmaudit].[tblFCEDevice]
(
    [FCEDeviceGuid] UNIQUEIDENTIFIER NULL
,	[SiteGuid] UNIQUEIDENTIFIER NULL
,	[ImeiNumber] [nchar](15) NULL
,	[FriendlyName] nchar(30) NULL
,	[HeartbeatTimeoutProcessed] Bit  NULL
,	[ConfigReady] Bit NULL
,	[MinTime]	int NULL
,	[MaxTime]	int NULL
,	[LevelDeadband] float NULL
,	[TempDeadband] float NULL
,	[Heartbeat] int NULL
,	[TLStanks] smallint NULL
,	[ModbusMap] smallint NULL
,	[MidnightOffset] int NULL
,	[ShortDeadband] float NULL
,	[ShortTime] int NULL
,	[LongDeadband] float NULL
,	[LongTime] int NULL
,	[SoftwareVersion] [nchar](32) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_map_tblFCEDevice_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblFCEDevice_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblFCEDevice_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblFCEEMapping_ClusterIdx] ON [fmaudit].[tblFCEDevice](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblFCEEMapping_AuditGUID] ON [fmaudit].[tblFCEDevice](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblFCEEMapping_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblFCEDevice] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)