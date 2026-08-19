CREATE TABLE [fmaudit].[tblAssetTrackingMapConfiguration](
	[AssetTrackingMapConfigurationGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[MapName] nvarchar (100) NULL
,	[Zoom] int NULL
,	[Latitude] float NULL
,	[Longitude] float NULL
,	[LookupMapSourceIndex] int NULL
,	[Description] nvarchar (200) NULL
,	[Active] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAssetTrackingMapConfiguration_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAssetTrackingMapConfiguration_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAssetTrackingMapConfiguration_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingMapConfiguration_AuditGUID] ON [fmaudit].[tblAssetTrackingMapConfiguration](_AuditGUID ASC)
GO
CREATE CLUSTERED INDEX [IX_tblAssetTrackingMapConfiguration_ClusterIdx] ON [fmaudit].[tblAssetTrackingMapConfiguration](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingMapConfiguration_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAssetTrackingMapConfiguration] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)