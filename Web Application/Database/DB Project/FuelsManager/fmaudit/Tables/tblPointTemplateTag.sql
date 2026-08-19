CREATE TABLE [fmaudit].[tblPointTemplateTag](
	[ID] nvarchar (50) NULL
,	[EngineeringUnitsType] int NULL
,	[EngineeringUnitsIndex] int NULL
,	[DecimalPlaces] tinyint NULL
,	[ServerEngineeringUnitsIndex] int NULL
,	[ValueType] nvarchar(max) NULL
,	[Value] XML NULL
,	[Maximum] float NULL
,	[Minimum] float NULL
,	[PointTagInputOutputTypeIndex] int NULL
,	[Input] bit NULL
,	[AlarmStatus] bit NULL
,	[ApplyPointTemplateEngineeringUnits] bit NULL
,	[ApplyPointTemplateDecimalPlaces] bit NULL
,	[ApplyPointTemplateMaximum] bit NULL
,	[ApplyPointTemplateMinimum] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] binary(8) NULL
,	[PointTemplateTagGuid] uniqueidentifier NULL
,	[PointTemplateGuid] uniqueidentifier NULL
,	[WellKnownIdentityGuid]	UNIQUEIDENTIFIER	NULL
,	[AlarmsEnabled] BIT NULL CONSTRAINT [DF_tblPointTemplateTag_AlarmsEnabled] DEFAULT 0
,	[InhibitInputOutputTypeConfiguration] BIT NULL CONSTRAINT [DF_tblPointTemplateTag_InhibitInputOutputTypeConfiguration] DEFAULT 0
,	[InhibitOverride] BIT NULL CONSTRAINT [DF_tblPointTemplateTag_InhibitOverride] DEFAULT 0
,	[Module] BIT NULL CONSTRAINT [DF_tblPointTemplateTag_Module] DEFAULT 0
,	[Archived] BIT NULL CONSTRAINT [DF_tblPointTemplateTag_Archived] DEFAULT 1
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblPointTemplateTag_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblPointTemplateTag_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblPointTemplateTag_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL )
GO
CREATE CLUSTERED INDEX [IX_tblPointTemplateTag_ClusterIdx] ON [fmaudit].[tblPointTemplateTag](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblPointTemplateTag_AuditGUID] ON [fmaudit].[tblPointTemplateTag](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblPointTemplateTag_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblPointTemplateTag] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)