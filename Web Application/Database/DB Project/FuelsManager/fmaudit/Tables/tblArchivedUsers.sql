CREATE TABLE [fmaudit].[tblArchivedUsers](
	[UserID] nvarchar (100) NULL
,	[Password] varbinary (256) NULL
,	[LastLoginDate] datetimeoffset NULL
,	[LastLogoffDate] datetimeoffset NULL
,	[ChangePassword] bit NULL
,	[PasswordTimeStamp] datetimeoffset NULL
,	[Name] nvarchar (50) NULL
,	[EmailAddress] nvarchar (50) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[PasswordHistory1] varbinary (256) NULL
,	[PasswordHistory2] varbinary (256) NULL
,	[PasswordHistory3] varbinary (256) NULL
,	[PasswordHistory4] varbinary (256) NULL
,	[PasswordHistory5] varbinary (256) NULL
,	[PasswordHistory6] varbinary (256) NULL
,	[PasswordHistory7] varbinary (256) NULL
,	[PasswordHistory8] varbinary (256) NULL
,	[PasswordHistory9] varbinary (256) NULL
,	[PasswordHistory10] varbinary (256) NULL
,	[PasswordHistory11] varbinary (256) NULL
,	[PasswordHistory12] varbinary (256) NULL
,	[PasswordHistory13] varbinary (256) NULL
,	[PasswordHistory14] varbinary (256) NULL
,	[PasswordHistory15] varbinary (256) NULL
,	[PasswordHistory16] varbinary (256) NULL
,	[PasswordHistory17] varbinary (256) NULL
,	[PasswordHistory18] varbinary (256) NULL
,	[PasswordHistory19] varbinary (256) NULL
,	[PasswordHistory20] varbinary (256) NULL
,	[PasswordHistory21] varbinary (256) NULL
,	[PasswordHistory22] varbinary (256) NULL
,	[PasswordHistory23] varbinary (256) NULL
,	[PasswordHistory24] varbinary (256) NULL
,	[PasswordLockoutCount] int NULL
,	[InactivityLockout] int NULL
,	[InactivityLockoutDate] datetimeoffset NULL
,	[ArchivedDate] datetimeoffset NULL
,	[ArchivedUserGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[UserGuid] uniqueidentifier NULL
,	[PasswordHint] varchar (80) NULL
,	[UserData1] nvarchar (120) NULL
,	[UserData2] nvarchar (120) NULL
,	[UserData3] nvarchar (120) NULL
,	[UserData4] nvarchar (120) NULL
,	[UserData5] nvarchar (120) NULL
,	[UserData6] nvarchar (120) NULL
,	[UserData7] nvarchar (120) NULL
,	[UserData8] nvarchar (120) NULL
,	[PhoneNumber] nvarchar (20) NULL
,	[AccountExpirationDate] datetime NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblArchivedUsers_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblArchivedUsers_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblArchivedUsers_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblArchivedUsers_AuditGUID] ON [fmaudit].[tblArchivedUsers](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblArchivedUsers_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblArchivedUsers] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblArchivedUsers_ClusterIdx] ON [fmaudit].[tblArchivedUsers](_ClusterIdx ASC)