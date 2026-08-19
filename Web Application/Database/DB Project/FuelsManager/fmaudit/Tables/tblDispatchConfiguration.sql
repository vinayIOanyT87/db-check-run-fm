CREATE TABLE [fmaudit].[tblDispatchConfiguration](
	[DispatchConfigurationGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ID] nvarchar (50) NULL
,	[DisplayCurrentTime] bit NULL
,	[DispatchDataRefreshPeriod] int NULL
,	[TabularViewDisplayMilitaryDate] bit NULL
,	[QuantityNotZeroCheck] bit NULL
,	[ExactlyOneManagerCheck] bit NULL
,	[ExactlyOneOwnerCheck] bit NULL
,	[DispatchFuelAdditiveFlagCheck] bit NULL
,	[FastLogFuelAdditiveFlagCheck] bit NULL
,	[FillstandVolumeWithinToleranceCheck] bit NULL
,	[ReturnToBulkVolumeWithinToleranceCheck] bit NULL
,	[RecirculationVolumesGreaterThanZeroCheck] bit NULL
,	[OperatorIsInCheck] bit NULL
,	[OperatorNotAssignedCheck] bit NULL
,	[OperatorHasRequiredTrainingCheck] bit NULL
,	[OperatorTrainingNotExpiredCheck] bit NULL
,	[OperatorNotLockedOutCheck] bit NULL
,	[OperatorHasRequiredQualificationsCheck] bit NULL
,	[OperatorQualificationsNotExpiredCheck] bit NULL
,	[DefuelStatusCheck] bit NULL
,	[RefuelStatusCheck] bit NULL
,	[EquipmentFuelGradeCheck] bit NULL
,	[EquipmentNotLockedOutCheck] bit NULL
,	[EquipmentNotAssignedCheck] bit NULL
,	[EquipmentInServiceCheck] bit NULL
,	[TagLicenseNotExpiredCheck] bit NULL
,	[TestInspectionNotExpiredCheck] bit NULL
,	[QualityControlCheckupDateCheck] bit NULL
,	[CautionQualityTagCheck] bit NULL
,	[WarningQualityTagCheck] bit NULL
,	[DangerQualityTagCheck] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[EnableServiceRequests] bit NULL
,	[AutomaticRestartDelay] int NULL
,	[EquipmentRequired] bit NULL
,	[PersonnelRequired] bit NULL
,	[FillToActualOrStandard] int NULL
,	[OperationalWindowPastHours] int NULL
,	[OperationalWindowFutureHours] int NULL
,	[ShowGridLines] bit NULL
,	[StaticTimeDisplay] bit NULL
,	[UseArrivalTime] bit NULL
,	[UseStartTime] bit NULL
,	[UseStopTime] bit NULL
,	[FuelsManagerReportURL] nvarchar (max) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblDispatchConfiguration_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblDispatchConfiguration_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblDispatchConfiguration_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblDispatchConfiguration_AuditGUID] ON [fmaudit].[tblDispatchConfiguration](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblDispatchConfiguration_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblDispatchConfiguration] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblDispatchConfiguration_ClusterIdx] ON [fmaudit].[tblDispatchConfiguration](_ClusterIdx ASC)