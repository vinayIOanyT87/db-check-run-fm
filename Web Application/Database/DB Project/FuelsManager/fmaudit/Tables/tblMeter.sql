CREATE TABLE [fmaudit].[tblMeter](
	[MeterGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[MeterID] nvarchar (30) NULL
,	[NumberOfDigits] tinyint NULL
,	[RotatesBackwardsFlag] bit NULL
,	[ReceiptMeterFlag] bit NULL
,	[MeterFactor] float NULL
,	[FuelCompressionFactor] float NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[DcuID] nvarchar (50) NULL
,	[DcuBatteryVoltage] float NULL
,	[DcuBatteryCurrent] float NULL
,	[DcuTemperature] float NULL
,	[DcuResets] int NULL
,	[DcuUpdateDate] datetimeoffset NULL
,	[DcuConfigurationDate] datetimeoffset NULL
,	[DcuFirmwareVersion] nvarchar (50) NULL
,	[DcuBluetoothAddress] nvarchar (50) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblMeter_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblMeter_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblMeter_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblMeter_AuditGUID] ON [fmaudit].[tblMeter](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblMeter_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblMeter] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblMeter_ClusterIdx] ON [fmaudit].[tblMeter](_ClusterIdx ASC)