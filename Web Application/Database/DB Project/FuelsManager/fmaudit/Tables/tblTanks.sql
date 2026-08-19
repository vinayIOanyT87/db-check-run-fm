CREATE TABLE [fmaudit].[tblTanks](
	[TankID] nvarchar (50) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[TankGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupVesselTypeIndex] int NULL
,	[ManagerCompanyGuid] uniqueidentifier NULL
,	[ProductGuid] uniqueidentifier NULL
,	[HiddenDate] datetimeoffset NULL
,	[AssetTrackingDeviceGuid] uniqueidentifier NULL
,	[LookupDeviceTankTypeIndex] int NULL
,	[Latitude] float NULL
,	[Longitude] float NULL
,	[TankConfigurationNumber] int NULL
,	[Zoom] int NULL
,	[OwnerCompanyGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTanks_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTanks_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTanks_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)

GO

CREATE NONCLUSTERED INDEX [IX_tblTanks_AuditGUID] ON [fmaudit].[tblTanks](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTanks_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTanks] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTanks_ClusterIdx] ON [fmaudit].[tblTanks](_ClusterIdx ASC)