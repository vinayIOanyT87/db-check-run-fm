CREATE TABLE [fmaudit].[tblGasboyFleet](
	[GasboyFleetGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[FleetCode] bigint NULL
,	[FleetName] nvarchar (50) NULL
,	[GroupRuleName] nvarchar (50) NULL
,	[PriceListName] nvarchar (50) NULL
,	[LookupGasboyRecordStatusIndex] int NULL
,	[UsePINCodeFlag] bit NULL
,	[PINCode] varbinary (256) NULL
,	[AuthPINFrom] tinyint NULL
,	[PromptForVehiclePlateFlag] bit NULL
,	[LookupGasboyVehiclePlateCheckTypeIndex] int NULL
,	[AlwaysPromptForAdditionalValidationFlag] tinyint NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblGasboyFleet_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblGasboyFleet_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblGasboyFleet_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[FleetID] bigint NOT NULL
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblGasboyFleet_ClusterIdx] ON [fmaudit].[tblGasboyFleet](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblGasboyFleet_AuditGUID] ON [fmaudit].[tblGasboyFleet](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblGasboyFleet_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblGasboyFleet] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)