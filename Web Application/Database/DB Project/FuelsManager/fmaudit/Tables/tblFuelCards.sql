CREATE TABLE [fmaudit].[tblFuelCards](
	[ID] nvarchar (50) NULL
,	[Provider] nvarchar (50) NULL
,	[ActivationStatus] int NULL
,	[InactivityPeriod] int NULL
,	[Notes] nvarchar (max) NULL
,	[StatusModifiedDate] datetimeoffset NULL
,	[StatusModifiedBy] nvarchar (50) NULL
,	[UserData1] nvarchar (60) NULL
,	[UserData2] nvarchar (60) NULL
,	[UserData3] nvarchar (60) NULL
,	[UserData4] nvarchar (60) NULL
,	[UserData5] nvarchar (60) NULL
,	[UserData6] nvarchar (60) NULL
,	[UserData7] nvarchar (60) NULL
,	[UserData8] nvarchar (60) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[FuelCardGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[BillToCompanyGuid] uniqueidentifier NULL
,	[ManagerCompanyGuid] uniqueidentifier NULL
,	[OwnerCompanyGuid] uniqueidentifier NULL
,	[ShipperCompanyGuid] uniqueidentifier NULL
,	[ShipToCompanyGuid] uniqueidentifier NULL
,	[ExpirationDate] datetimeoffset NULL
,	[TransientCardFlag] bit NULL
,	[PIN] varbinary (256) NULL
,	[ProviderID] nvarchar (60) NULL
,	[FuelCardTypeApplicationStringGuid] uniqueidentifier NULL
,	[HiddenDate] datetimeoffset NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblFuelCards_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblFuelCards_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblFuelCards_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblFuelCards_AuditGUID] ON [fmaudit].[tblFuelCards](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblFuelCards_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblFuelCards] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblFuelCards_ClusterIdx] ON [fmaudit].[tblFuelCards](_ClusterIdx ASC)