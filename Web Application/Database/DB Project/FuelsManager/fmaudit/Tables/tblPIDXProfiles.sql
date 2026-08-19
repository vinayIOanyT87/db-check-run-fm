CREATE TABLE [fmaudit].[tblPIDXProfiles](
	[Type] tinyint NULL
,	[ID] nvarchar (30) NULL
,	[IPAddress] nvarchar (60) NULL
,	[Port] int NULL
,	[TerminalID] nvarchar (30) NULL
,	[UserID] nvarchar (30) NULL
,	[Password] nvarchar (30) NULL
,	[Enabled] bit NULL
,	[LoggingEnabled] bit NULL
,	[LogFilePath] nvarchar (255) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[PIDXProfileGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[Version] int NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblPIDXProfiles_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblPIDXProfiles_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblPIDXProfiles_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblPIDXProfiles_AuditGUID] ON [fmaudit].[tblPIDXProfiles](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblPIDXProfiles_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblPIDXProfiles] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblPIDXProfiles_ClusterIdx] ON [fmaudit].[tblPIDXProfiles](_ClusterIdx ASC)