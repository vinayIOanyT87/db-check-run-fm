CREATE TABLE [fmaudit].[tblSystemSettings](
	[ReportServerURL] nvarchar (80) NULL
,	[StationMessageTimeout] int NULL
,	[StationPromptTimeout] int NULL
,	[ProhibitUpdatingLinkedEquipment] bit NULL
,	[UserDataListDefaultToFirstValue] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[SystemSettingGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[ReportServerUserName] nvarchar (50) NULL
,	[ReportServerPassword] varbinary (256) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblSystemSettings_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblSystemSettings_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblSystemSettings_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblSystemSettings_AuditGUID] ON [fmaudit].[tblSystemSettings](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblSystemSettings_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblSystemSettings] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_tblSystemSettings_ClusterIdx] ON [fmaudit].[tblSystemSettings](_ClusterIdx ASC)