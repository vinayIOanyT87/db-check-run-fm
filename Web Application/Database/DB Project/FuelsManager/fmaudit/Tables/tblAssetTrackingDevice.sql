CREATE TABLE [fmaudit].[tblAssetTrackingDevice](
	[AssetTrackingDeviceGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[DeviceID] nvarchar (30) NULL
,	[Description] nvarchar (50) NULL
,	[ModelNumber] nvarchar (50) NULL
,	[SerialNumber] nvarchar (50) NULL
,	[Active] bit NULL
,	[LookupAssetTrackingDeviceTypeIndex] int NULL
,	[LookupEngineeringUnitIndex] int NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAssetTrackingDevice_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAssetTrackingDevice_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAssetTrackingDevice_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingDevice_AuditGUID] ON [fmaudit].[tblAssetTrackingDevice](_AuditGUID ASC)
GO
CREATE CLUSTERED INDEX [IX_tblAssetTrackingDevice_ClusterIdx] ON [fmaudit].[tblAssetTrackingDevice](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingDevice_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAssetTrackingDevice] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)