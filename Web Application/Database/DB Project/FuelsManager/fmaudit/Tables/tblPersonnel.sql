CREATE TABLE [fmaudit].[tblPersonnel](
	[PersonID] nvarchar (50) NULL
,	[CardNumber] nvarchar (30) NULL
,	[FirstName] nvarchar (20) NULL
,	[MiddleName] nvarchar (20) NULL
,	[LastName] nvarchar (30) NULL
,	[Title] nvarchar (50) NULL
,	[Department] nvarchar (20) NULL
,	[Address1] nvarchar (50) NULL
,	[Address2] nvarchar (50) NULL
,	[City] nvarchar (60) NULL
,	[State] nvarchar (20) NULL
,	[Zip] nvarchar (10) NULL
,	[Country] nvarchar (20) NULL
,	[Phone1] nvarchar (50) NULL
,	[Phone2] nvarchar (50) NULL
,	[AssignmentDate] datetimeoffset NULL
,	[SupervisionDate] datetimeoffset NULL
,	[SSAN] nvarchar (11) NULL
,	[BirthDate] datetimeoffset NULL
,	[PayRate] money NULL
,	[LaborRate1] float NULL
,	[LaborRate2] float NULL
,	[LaborRate3] float NULL
,	[LaborRate4] float NULL
,	[Status] smallint NULL
,	[Email] nvarchar (50) NULL
,	[ResponsibleOfficer] bit NULL
,	[Shift] smallint NULL
,	[PINNumber] varbinary (256) NULL
,	[PINRequired] bit NULL
,	[LockedOut] bit NULL
,	[LockedOutReason] nvarchar (80) NULL
,	[LockedOutDate] datetimeoffset NULL
,	[LastActivityDate] datetimeoffset NULL
,	[CardedIn] bit NULL
,	[ShortCardNumber] nvarchar (6) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OnFileSignature] varbinary(max) NULL
,	[UserData1] nvarchar (60) NULL
,	[UserData2] nvarchar (60) NULL
,	[UserData3] nvarchar (60) NULL
,	[UserData4] nvarchar (60) NULL
,	[UserData5] nvarchar (60) NULL
,	[UserData6] nvarchar (60) NULL
,	[UserData7] nvarchar (60) NULL
,	[UserData8] nvarchar (60) NULL
,	[UserData9] nvarchar (60) NULL
,	[UserData10] nvarchar (60) NULL
,	[UserData11] nvarchar (60) NULL
,	[UserData12] nvarchar (60) NULL
,	[UserData13] nvarchar (60) NULL
,	[UserData14] nvarchar (60) NULL
,	[UserData15] nvarchar (60) NULL
,	[UserData16] nvarchar (60) NULL
,	[UserData17] nvarchar (60) NULL
,	[UserData18] nvarchar (60) NULL
,	[UserData19] nvarchar (60) NULL
,	[UserData20] nvarchar (60) NULL
,	[UserData21] nvarchar (60) NULL
,	[UserData22] nvarchar (60) NULL
,	[UserData23] nvarchar (60) NULL
,	[UserData24] nvarchar (60) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[PersonnelGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[CompanyGuid] uniqueidentifier NULL
,	[SupervisorPersonnelGuid] uniqueidentifier NULL
,	[UserGuid] uniqueidentifier NULL
,	[AssignedEquipmentGuid] uniqueidentifier NULL
,	[InhibitInactivityLockout] BIT NULL
,	[_MasterRecordGuid] uniqueidentifier NULL
,	[HiddenDate] datetimeoffset NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblPersonnel_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblPersonnel_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblPersonnel_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblPersonnel_AuditGUID] ON [fmaudit].[tblPersonnel](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblPersonnel_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblPersonnel] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblPersonnel_ClusterIdx] ON [fmaudit].[tblPersonnel](_ClusterIdx ASC)