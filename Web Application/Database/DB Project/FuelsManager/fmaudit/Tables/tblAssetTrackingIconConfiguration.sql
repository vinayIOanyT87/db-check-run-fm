CREATE TABLE [fmaudit].[tblAssetTrackingIconConfiguration](
	[AssetTrackingIconConfigurationGuid] UNIQUEIDENTIFIER NULL
,	[SiteGuid] UNIQUEIDENTIFIER NULL
,   [IconConfigurationID] NVARCHAR(20) NULL
,	[EquipmentIconName] NVARCHAR(50) NULL
,	[EquipmentVarianceIconName] NVARCHAR(50) NULL
,	[EquipmentInvestigationIconName] NVARCHAR(50) NULL
,	[EquipmentCompleteInvestigationFailedIconName] NVARCHAR(50) NULL
,	[EquipmentCompleteInvestigationPassedIconName] NVARCHAR(50) NULL
,	[TankIconName] NVARCHAR(50) NULL
,	[FacilityIconName] NVARCHAR(50) NULL
,	[DeliveryLocationIconName] NVARCHAR(50) NULL
,	[BreadcrumbIconName] NVARCHAR(50) NULL
,	[BreadcrumbVarianceIconName] NVARCHAR(50) NULL
,	[BreadcrumbInvestigationIconName] NVARCHAR(50) NULL 
,	[BreadcrumbCompleteInvestigationFailedIconName] NVARCHAR(50) NULL 
,	[BreadcrumbCompleteInvestigationPassedIconName] NVARCHAR(50) NULL 
,	[MapPinIconName] NVARCHAR(50) NULL
,	[CreatedDate] DATETIMEOFFSET NULL
,   [CreatedBy] [dbo].[udtUserID] NULL
,   [UpdatedDate] DATETIMEOFFSET NULL 
,   [UpdatedBy] [dbo].[udtUserID] NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblAssetTrackingIconConfiguration_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAssetTrackingIconConfiguration_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAssetTrackingIconConfiguration_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingIconConfiguration_AuditGUID] ON [fmaudit].[tblAssetTrackingIconConfiguration](_AuditGUID ASC)
GO
CREATE CLUSTERED INDEX [IX_tblAssetTrackingIconConfiguration_ClusterIdx] ON [fmaudit].[tblAssetTrackingIconConfiguration](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblAssetTrackingIconConfiguration_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAssetTrackingIconConfiguration] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)