CREATE TABLE [fmaudit].[tblPointTag](
	[ID] nvarchar (50) NULL
,	[EngineeringUnitsType] int NULL
,	[EngineeringUnitsIndex] int NULL
,	[DecimalPlaces] tinyint NULL
,	[ServerEngineeringUnitsIndex] int NULL
,	[ValueType] nvarchar(max) NULL
,	[Status] bigint NULL
,	[Value] xml NULL
,	[ServerTimeStamp] datetimeoffset NULL
,	[SourceTimeStamp] datetimeoffset NULL
,	[Maximum] float NULL
,	[Minimum] float NULL
,	[PointTagInputOutputTypeIndex] int NULL
,	[LastPointTagInputOutputTypeIndex] INT NULL
,	[Input] bit NULL
,	[AlarmStatus] bit NULL
,	[ApplyPointEngineeringUnits] bit NULL
,	[ApplyPointDecimalPlaces] bit NULL
,	[ApplyPointMaximum] bit NULL
,	[ApplyPointMinimum] bit NULL
,	[OpcUaServerGuid] uniqueidentifier NULL
,	[OpcUaBrowsePath] nvarchar (250) NULL
,	[OpcUaNamespaceUri] nvarchar (250) NULL
,	[OpcUaPublishingInterval] int NULL
,	[OpcUaNodeId] nvarchar (250) NULL
,	[OpcUaIsReadable] bit NULL
,	[OpcUaServerDataType] int NULL
,	[OpcUaWriteHoldoffTime]	int NULL
,	[OpcUaWritePeriodicUpdateInterval] int	NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] binary(8) NULL
,	[PointTagGuid] uniqueidentifier NULL
,	[PointTemplateTagGuid] uniqueidentifier NULL
,	[PointGuid] uniqueidentifier NULL
,	[AlarmsEnabled] BIT NULL CONSTRAINT [DF_tblPointTag_AlarmsEnabled] DEFAULT 1
,	[InhibitInputOutputTypeConfiguration] BIT NULL CONSTRAINT [DF_tblPointTag_InhibitInputOutputTypeConfiguration] DEFAULT 0
,	[InhibitOverride] BIT NULL CONSTRAINT [DF_tblPointTag_InhibitOverride] DEFAULT 0
,	[Deadband] float(53) NULL
,	[Holdoff] int NULL
,	[Archived] BIT CONSTRAINT [DF_tblPointTag_Archived] DEFAULT (1) NOT NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblPointTag_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblPointTag_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblPointTag_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL
,	[_AuditContext] VARBINARY(128) NULL )
GO
CREATE CLUSTERED INDEX [IX_tblPointTag_ClusterIdx] ON [fmaudit].[tblPointTag](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblPointTag_AuditGUID] ON [fmaudit].[tblPointTag](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblPointTag_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblPointTag] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)