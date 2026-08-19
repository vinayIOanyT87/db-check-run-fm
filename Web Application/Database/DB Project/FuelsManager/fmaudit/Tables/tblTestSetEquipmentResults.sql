CREATE TABLE [fmaudit].[tblTestSetEquipmentResults](
	[ResultTimeStamp] datetimeoffset NULL
,	[TestSetName] nvarchar (80) NULL
,	[Inspector] nvarchar (100) NULL
,	[Supervisor] nvarchar (100) NULL
,	[EquipmentID] nvarchar (50) NULL
,	[SampleNumber] int NULL
,	[SampleSize] float NULL
,	[IsRetest] bit NULL
,	[PreviousSampleNumber] int NULL
,	[DocumentNumber] nvarchar (50) NULL
,	[Memo] nvarchar (1000) NULL
,	[GallonsRepresented] float NULL
,	[Override] bit NULL
,	[DeleteFlag] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[TestSetEquipmentResultGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupTestSetStatusIndex] int NULL
,	[EquipmentGuid] uniqueidentifier NULL
,	[Flag01] bit NULL
,	[Flag02] bit NULL
,	[UserData01] nvarchar (60) NULL
,	[UserData02] nvarchar (60) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTestSetEquipmentResults_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTestSetEquipmentResults_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTestSetEquipmentResults_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblTestSetEquipmentResults_AuditGUID] ON [fmaudit].[tblTestSetEquipmentResults](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTestSetEquipmentResults_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTestSetEquipmentResults] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTestSetEquipmentResults_ClusterIdx] ON [fmaudit].[tblTestSetEquipmentResults](_ClusterIdx ASC)