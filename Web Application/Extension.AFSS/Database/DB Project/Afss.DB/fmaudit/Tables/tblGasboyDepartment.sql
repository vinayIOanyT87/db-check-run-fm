CREATE TABLE [fmaudit].[tblGasboyDepartment](
	[GasboyDepartmentGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[DepartmentCode] bigint NULL
,	[DepartmentName] nvarchar (50) NULL
,	[GroupRuleName] nvarchar (50) NULL
,	[PriceListName] nvarchar (50) NULL
,	[LookupGasboyRecordStatusIndex] int NULL
,	[UsePINCodeFlag] bit NULL
,	[PINCode] varbinary (256) NULL
,	[AuthPINFrom] tinyint NULL
,	[PromptForVehiclePlateFlag] bit NULL
,	[LookupGasboyVehiclePlateCheckTypeIndex] int NULL
,	[AlwaysPromptForAdditionalValidationFlag] tinyint NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblGasboyDepartment_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblGasboyDepartment_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblGasboyDepartment_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL
,	[DepartmentID] bigint NOT NULL
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblGasboyDepartment_ClusterIdx] ON [fmaudit].[tblGasboyDepartment](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblGasboyDepartment_AuditGUID] ON [fmaudit].[tblGasboyDepartment](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblGasboyDepartment_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblGasboyDepartment] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)