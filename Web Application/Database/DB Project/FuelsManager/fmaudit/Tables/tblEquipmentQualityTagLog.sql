CREATE TABLE [fmaudit].[tblEquipmentQualityTagLog](
	[QualityTagName] nvarchar (50) NULL
,	[EquipmentID] nvarchar (50) NULL
,	[EquipmentType] nvarchar (50) NULL
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
,	[EquipmentQualityTagLogGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[EquipmentGuid] uniqueidentifier NULL
,	[QualityTagGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblEquipmentQualityTagLog_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblEquipmentQualityTagLog_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblEquipmentQualityTagLog_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblEquipmentQualityTagLog_AuditGUID] ON [fmaudit].[tblEquipmentQualityTagLog](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentQualityTagLog_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblEquipmentQualityTagLog] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblEquipmentQualityTagLog_ClusterIdx] ON [fmaudit].[tblEquipmentQualityTagLog](_ClusterIdx ASC)