CREATE TABLE [fmaudit].[tblMobileDeviceProfileAnalogInput](
	[MobileDeviceProfileAnalogInputGuid] uniqueidentifier NULL
,	[MobileDeviceProfileGuid] uniqueidentifier NULL
,	[LowLimit] float NULL
,	[HighLimit] float NULL
,	[ParameterA] nvarchar (20) NULL
,	[ParameterB] nvarchar (20) NULL
,	[ParameterC] nvarchar (20) NULL
,	[AnalogFormula] nvarchar (50) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblMobileDeviceProfileAnalogInput_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblMobileDeviceProfileAnalogInput_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblMobileDeviceProfileAnalogInput_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblMobileDeviceProfileAnalogInput_AuditGUID] ON [fmaudit].[tblMobileDeviceProfileAnalogInput](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblMobileDeviceProfileAnalogInput_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblMobileDeviceProfileAnalogInput] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblMobileDeviceProfileAnalogInput_ClusterIdx] ON [fmaudit].[tblMobileDeviceProfileAnalogInput](_ClusterIdx ASC)