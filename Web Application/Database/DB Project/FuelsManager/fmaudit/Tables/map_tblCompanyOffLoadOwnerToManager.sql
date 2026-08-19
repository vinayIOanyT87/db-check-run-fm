CREATE TABLE [fmaudit].[map_tblCompanyOffLoadOwnerToManager](
	[CompanyOffLoadOwnerToManagerGuid] uniqueidentifier NULL
,	[CompanyGuid] uniqueidentifier NULL
,	[AssignedToCompanyGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ID] nvarchar (30) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_map_tblCompanyOffLoadOwnerToManager_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_map_tblCompanyOffLoadOwnerToManager_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_map_tblCompanyOffLoadOwnerToManager_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)


GO

CREATE NONCLUSTERED INDEX [IX_map_tblCompanyOffLoadOwnerToManager_AuditGUID] ON [fmaudit].[map_tblCompanyOffLoadOwnerToManager](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_map_tblCompanyOffLoadOwnerToManager_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[map_tblCompanyOffLoadOwnerToManager] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_map_tblCompanyOffLoadOwnerToManager_ClusterIdx] ON [fmaudit].[map_tblCompanyOffLoadOwnerToManager](_ClusterIdx ASC)