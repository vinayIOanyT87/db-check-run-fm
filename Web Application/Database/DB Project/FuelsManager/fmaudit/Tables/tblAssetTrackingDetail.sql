CREATE TABLE [fmaudit].[tblAssetTrackingDetail]
(
	[AssetTrackingDetailGuid] UNIQUEIDENTIFIER NULL
,	[SiteGuid] UNIQUEIDENTIFIER NULL
,   [EquipmentID] NVARCHAR(30) NULL 
,   [ProductID] NVARCHAR(30) NULL
,   [ConvoyID] NVARCHAR(50) NULL
,   [AssetTrackingDeviceID] NVARCHAR(30) NULL 
,   [AssetSessionDateTime] DATETIME NULL
,   [AssetSessionStatus] INT NULL
,   [MOMSN] INT NULL
,   [MTMSN] INT NULL
,   [CDRReference] INT NULL
,   [Latitude] FLOAT NULL
,   [Longitude] FLOAT NULL
,   [CEPRadius] INT NULL
,	[ChecksumFlag] BIT NULL
,	[Contaminated] BIT NULL
,	[StartInvestigationDate] DATETIME NULL
,	[CompleteInvestigationDate] DATETIME NULL
,	[Remarks] NVARCHAR(4000) NULL
,	[LookupAssetTrackingPayloadTypeIndex] INT NULL
,	[LookupAssetTrackingMessageStateIndex] INT NULL
,   [CreatedDate] DATETIMEOFFSET NULL 
,   [CreatedBy] [dbo].[udtUserID] NULL
,   [UpdatedDate] DATETIMEOFFSET NULL 
,   [UpdatedBy] [dbo].[udtUserID] NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblAssetTrackingDetail_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAssetTrackingDetail_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAssetTrackingDetail_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)

GO
CREATE CLUSTERED INDEX [IX_tblAssetTrackingDetail_ClusterIdx] ON [fmaudit].[tblAssetTrackingDetail](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingDetail_AuditGUID] ON [fmaudit].[tblAssetTrackingDetail](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingDetail_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAssetTrackingDetail] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)