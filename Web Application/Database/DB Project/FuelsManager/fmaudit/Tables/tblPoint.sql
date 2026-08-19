CREATE TABLE [fmaudit].[tblPoint](
	[ID] nvarchar (30) NULL
,	[Description] nvarchar (50) NULL
,	[Enabled] bit NULL
,	[Standard] bit NULL
,	[ExecutionInterval] int NULL
,	[LevelUnitIndex] int NULL
,	[TemperatureUnitIndex] int NULL
,	[DensityUnitIndex] int NULL
,	[PressureUnitIndex] int NULL
,	[FlowUnitIndex] int NULL
,	[VolumeUnitIndex] int NULL
,	[MassUnitIndex] int NULL
,	[VelocityUnitIndex] int NULL
,	[MassFlowUnitIndex] int NULL
,	[LevelDecimalPlaces] tinyint NULL
,	[TemperatureDecimalPlaces] tinyint NULL
,	[DensityDecimalPlaces] tinyint NULL
,	[PressureDecimalPlaces] tinyint NULL
,	[FlowDecimalPlaces] tinyint NULL
,	[VolumeDecimalPlaces] tinyint NULL
,	[MassDecimalPlaces] tinyint NULL
,	[VelocityDecimalPlaces] tinyint NULL
,	[MassFlowDecimalPlaces] tinyint NULL
,	[LevelMaximum] float NULL
,	[LevelMinimum] float NULL
,	[TemperatureMaximum] float NULL
,	[TemperatureMinimum] float NULL
,	[DensityMaximum] float NULL
,	[DensityMinimum] float NULL
,	[PressureMaximum] float NULL
,	[PressureMinimum] float NULL
,	[VolumetricFlowMaximum] float NULL
,	[VolumetricFlowMinimum] float NULL
,	[VolumeMaximum] float NULL
,	[VolumeMinimum] float NULL
,	[MassMaximum] float NULL
,	[MassMinimum] float NULL
,	[VelocityMaximum] float NULL
,	[VelocityMinimum] float NULL
,	[MassFlowMaximum] float NULL
,	[MassFlowMinimum] float NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[PointGuid] uniqueidentifier NULL
,	[OriginalRowVersion] binary(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[PointTemplateGuid] uniqueidentifier NULL
,	[ProfileImageGuid]	uniqueidentifier NULL
,	[ProductGuid]  uniqueidentifier NULL
,	[Notes]	NVARCHAR (255)		NULL
,  [OverrideDefaultDrawingGuid] uniqueidentifier NULL
,	[PointTemplateVersion] int NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblPoint_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblPoint_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblPoint_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL )
GO
CREATE CLUSTERED INDEX [IX_tblPoint_ClusterIdx] ON [fmaudit].[tblPoint](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblPoint_AuditGUID] ON [fmaudit].[tblPoint](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblPoint_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblPoint] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)