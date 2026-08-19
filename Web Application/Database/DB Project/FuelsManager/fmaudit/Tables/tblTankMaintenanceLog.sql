CREATE TABLE [fmaudit].[tblTankMaintenanceLog](
	[TankID] nvarchar (50) NULL
,	[VesselType] nvarchar (50) NULL
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
,	[TankMaintenanceLogGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupVesselTypeIndex] int NULL
,	[MaintenanceReasonGuid] uniqueidentifier NULL
,	[OperatorPersonnelGuid] uniqueidentifier NULL
,	[TankGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTankMaintenanceLog_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTankMaintenanceLog_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTankMaintenanceLog_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblTankMaintenanceLog_AuditGUID] ON [fmaudit].[tblTankMaintenanceLog](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTankMaintenanceLog_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTankMaintenanceLog] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTankMaintenanceLog_ClusterIdx] ON [fmaudit].[tblTankMaintenanceLog](_ClusterIdx ASC)