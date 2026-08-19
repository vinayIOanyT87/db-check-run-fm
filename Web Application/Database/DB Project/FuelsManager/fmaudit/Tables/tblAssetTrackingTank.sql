CREATE TABLE [fmaudit].[tblAssetTrackingTank](
	[AssetTrackingTankGuid] uniqueidentifier NULL
,	[AssetTrackingDetailGuid] uniqueidentifier NULL
,	[TankID] nvarchar (30) NULL
,	[Volume] float NULL
,	[Temperature] float NULL
,	[Density] float NULL
,	[Dielectric] float NULL
,	[ProductID] nvarchar (30) NULL
,	[Contaminated] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAssetTrackingTank_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAssetTrackingTank_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAssetTrackingTank_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblAssetTrackingTank_ClusterIdx] ON [fmaudit].[tblAssetTrackingTank](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingTank_AuditGUID] ON [fmaudit].[tblAssetTrackingTank](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingTank_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAssetTrackingTank] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)