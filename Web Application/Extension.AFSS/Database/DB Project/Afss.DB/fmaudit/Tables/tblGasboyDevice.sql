CREATE TABLE [fmaudit].[tblGasboyDevice](
	[GasboyDeviceGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[GasboyDepartmentGuid] uniqueidentifier NULL
,	[DeviceCode] bigint NULL
,	[DeviceName] nvarchar (50) NULL
,	[CardNumber] nvarchar (50) NULL
,	[GroupRuleName] nvarchar (50) NULL
,	[LookupGasboyDeviceTypeIndex] int NULL
,	[LookupGasboyRecordStatusIndex] int NULL
,	[LookupGasboyHardwareTypeIndex] int NULL
,	[LookupGasboyAuthTypeIndex] int NULL
,	[LookupGasboyEmployeeTypeIndex] int NULL
,	[LookupGasboyTwoStageDriverValidationTypeIndex] int NULL
,	[UsePINCodeFlag] bit NULL
,	[PINCode] varbinary (256) NULL
,	[AuthPINFrom] tinyint NULL
,	[VehiclePlate] nvarchar (50) NULL
,	[PromptForVehiclePlateFlag] bit NULL
,	[LookupGasboyVehiclePlateCheckTypeIndex] int NULL
,	[AlwaysPromptForAdditionalValidationFlag] tinyint NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblGasboyDevice_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblGasboyDevice_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblGasboyDevice_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[DeviceID] bigint NULL
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblGasboyDevice_ClusterIdx] ON [fmaudit].[tblGasboyDevice](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblGasboyDevice_AuditGUID] ON [fmaudit].[tblGasboyDevice](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblGasboyDevice_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblGasboyDevice] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)