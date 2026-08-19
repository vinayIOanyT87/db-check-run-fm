CREATE TABLE [fmaudit].[tblTankQualityTagLog](
	[TankID] nvarchar (50) NULL
,	[VesselType] nvarchar (50) NULL
,	[QualityTagName] nvarchar (50) NULL
,	[TaggedDate] datetimeoffset NULL
,	[TaggedBy] nvarchar (50) NULL
,	[Memo] nvarchar (1000) NULL
,	[RemovedDate] datetimeoffset NULL
,	[RemovedBy] nvarchar (255) NULL
,	[DeleteFlag] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[TagNumber] int NULL
,	[TankQualityTagLogGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupVesselTypeIndex] int NULL
,	[QualityTagGuid] uniqueidentifier NULL
,	[TankGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTankQualityTagLog_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTankQualityTagLog_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTankQualityTagLog_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblTankQualityTagLog_AuditGUID] ON [fmaudit].[tblTankQualityTagLog](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTankQualityTagLog_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTankQualityTagLog] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTankQualityTagLog_ClusterIdx] ON [fmaudit].[tblTankQualityTagLog](_ClusterIdx ASC)