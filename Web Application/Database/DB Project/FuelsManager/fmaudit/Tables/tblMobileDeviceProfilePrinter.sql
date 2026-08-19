CREATE TABLE [fmaudit].[tblMobileDeviceProfilePrinter](
	[MobileDeviceProfilePrinterGUID] uniqueidentifier NULL
,	[MobileDeviceProfileGUID] uniqueidentifier NULL
,	[PrinterID] nvarchar (30) NULL
,	[BaudRate] nvarchar (8) NULL
,	[COMPort] nvarchar (4) NULL
,	[DataBits] nvarchar (8) NULL
,	[StopBits] nvarchar (8) NULL
,	[UseXonXoff] nvarchar (8) NULL
,	[XonChar] nvarchar (8) NULL
,	[XoffChar] nvarchar (8) NULL
,	[BufferSize] nvarchar (8) NULL
,	[Parity] nvarchar (12) NULL
,	[CreatedBy] nvarchar (50) NULL
,	[UpdatedBy] nvarchar (50) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblMobileDeviceProfilePrinter_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblMobileDeviceProfilePrinter_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblMobileDeviceProfilePrinter_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblMobileDeviceProfilePrinter_AuditGUID] ON [fmaudit].[tblMobileDeviceProfilePrinter](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblMobileDeviceProfilePrinter_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblMobileDeviceProfilePrinter] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblMobileDeviceProfilePrinter_ClusterIdx] ON [fmaudit].[tblMobileDeviceProfilePrinter](_ClusterIdx ASC)