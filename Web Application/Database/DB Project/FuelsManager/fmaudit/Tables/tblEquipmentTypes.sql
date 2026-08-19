CREATE TABLE [fmaudit].[tblEquipmentTypes](
	[EqTypeName] nvarchar (50) NULL
,	[EqTypeDescription] nvarchar (50) NULL
,	[Capacity] float NULL
,	[SafeFill] float NULL
,	[Make] nvarchar (20) NULL
,	[Model] nvarchar (32) NULL
,	[Year] smallint NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[DeleteFlag] bit NULL
,	[IssPt] nvarchar (20) NULL
,	[MultiCompartment] bit NULL
,	[EquipmentTypeGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupEquipmentTypeIndex] int NULL
,	[ProductGuid] uniqueidentifier NULL
,	[CustomerDesignator] nvarchar (128) NULL
,	[ServiceTime] float NULL
,	[VolumeUnits] int NULL
,	[VolumeDecimalPlaces] smallint NULL
,	[MassUnits] int NULL
,	[MassDecimalPlaces] smallint NULL
,	[WingToWingToleranceType] smallint NULL
,	[WingToWingToleranceValue] float NULL
,	[TankToTankToleranceType] smallint NULL
,	[TankToTankToleranceValue] float NULL
,	[FuelServiceToleranceType] smallint NULL
,	[FuelServiceToleranceValue] float NULL
,	[FuelServiceToleranceMaxType] smallint NULL
,	[FuelServiceToleranceMaxValue] float NULL
,	[AllowFuelingByWeight] bit NULL
,	[LookupCompanyRoleIndex] int NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblEquipmentTypes_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblEquipmentTypes_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblEquipmentTypes_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblEquipmentTypes_AuditGUID] ON [fmaudit].[tblEquipmentTypes](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentTypes_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblEquipmentTypes] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblEquipmentTypes_ClusterIdx] ON [fmaudit].[tblEquipmentTypes](_ClusterIdx ASC)