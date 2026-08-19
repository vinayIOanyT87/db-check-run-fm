CREATE TABLE [fmaudit].[tblAppointmentEquipment](
	[AppointmentEquipmentGuid] uniqueidentifier NULL
,	[EquipmentGuid] uniqueidentifier NULL
,	[TestSetDefinitionGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[AssetText] nvarchar (100) NULL
,	[AppointmentCategory] nvarchar (50) NULL
,	[AppointmentIsSingle] bit NULL
,	[ScheduleOnWeekends] bit NULL
,	[ScheduleOnHolidays] bit NULL
,	[StartDate] datetimeoffset NULL
,	[Duration] int NULL
,	[AppointmentPeriod] int NULL
,	[AppointmentPeriodText] nvarchar (50) NULL
,	[Description] nvarchar (50) NULL
,	[AppointmentTimeInterval] int NULL
,	[AppointmentDayOfTheWeekText] nvarchar (20) NULL
,	[AppointmentDayOfTheWeek] int NULL
,	[AppointmentReoccuranceInterval] int NULL
,	[AppointmentOption2Selected] bit NULL
,	[AppointmentTimeOptionSelectionText] nvarchar (20) NULL
,	[AppointmentTimeOptionSelection] int NULL
,	[AppointmentMonthSelectionText] nvarchar (20) NULL
,	[AppointmentMonthSelection] int NULL
,	[AppointmentDayOfTheMonth] int NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAppointmentEquipment_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAppointmentEquipment_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAppointmentEquipment_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblAppointmentEquipment_AuditGUID] ON [fmaudit].[tblAppointmentEquipment](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblAppointmentEquipment_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAppointmentEquipment] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblAppointmentEquipment_ClusterIdx] ON [fmaudit].[tblAppointmentEquipment](_ClusterIdx ASC)