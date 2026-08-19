CREATE TABLE [fmaudit].[map_tblQualificationPersonTrainingToEquipmentType](
	[QualificationPersonTrainingToEquipmentTypeGuid] uniqueidentifier NULL
,	[QualificationGuid] uniqueidentifier NULL
,	[EquipmentTypeGuid] uniqueidentifier NULL
,	[Sequence] int NULL
,	[Instructor] nvarchar (50) NULL
,	[DateCompleted] datetimeoffset NULL
,	[DateDue] datetimeoffset NULL
,	[ExpirationDate] datetimeoffset NULL
,	[ID] varchar (50) NULL
,	[Rating] nvarchar (20) NULL
,	[HistoricalRecord] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_map_tblQualificationPersonTrainingToEquipmentType_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_map_tblQualificationPersonTrainingToEquipmentType_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_map_tblQualificationPersonTrainingToEquipmentType_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
,    [SiteGuid] UNIQUEIDENTIFIER NULL 
)


GO

CREATE NONCLUSTERED INDEX [IX_map_tblQualificationPersonTrainingToEquipmentType_AuditGUID] ON [fmaudit].[map_tblQualificationPersonTrainingToEquipmentType](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_map_tblQualificationPersonTrainingToEquipmentType_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[map_tblQualificationPersonTrainingToEquipmentType] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_map_tblQualificationPersonTrainingToEquipmentType_ClusterIdx] ON [fmaudit].[map_tblQualificationPersonTrainingToEquipmentType](_ClusterIdx ASC)