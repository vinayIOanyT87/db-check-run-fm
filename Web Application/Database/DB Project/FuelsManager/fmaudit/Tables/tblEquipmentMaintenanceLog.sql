CREATE TABLE [fmaudit].[tblEquipmentMaintenanceLog](
	[EquipmentID] nvarchar (50) NULL
,	[EquipmentType] nvarchar (50) NULL
,	[OperatorID] nvarchar (50) NULL
,	[MaintenanceReason] nvarchar (50) NULL
,	[InServiceFlag] tinyint NULL
,	[ChangeDate] datetimeoffset NULL
,	[EstReturnToServiceDate] datetimeoffset NULL
,	[WorkOrder] nvarchar (20) NULL
,	[Memo] nvarchar (1000) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[EquipmentMaintenanceLogGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[EquipmentGuid] uniqueidentifier NULL
,	[MaintenanceReasonGuid] uniqueidentifier NULL
,	[OperatorPersonnelGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblEquipmentMaintenanceLog_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblEquipmentMaintenanceLog_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblEquipmentMaintenanceLog_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_AuditGUID] ON [fmaudit].[tblEquipmentMaintenanceLog](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblEquipmentMaintenanceLog] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_ClusterIdx] ON [fmaudit].[tblEquipmentMaintenanceLog](_ClusterIdx ASC)