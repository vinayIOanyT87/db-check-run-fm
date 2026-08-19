CREATE TABLE [fmaudit].[tblTestDefinitions](
	[TestName] nvarchar (80) NULL
,	[MeasurementUnit] nvarchar (32) NULL
,	[ValidationRule] nvarchar (64) NULL
,	[SampleSize] float NULL
,	[TestCode] nvarchar (5) NULL
,	[TestMethod] nvarchar (80) NULL
,	[ProductID] nvarchar (30) NULL
,	[DeleteFlag] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[TestDefinitionGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[OwnerSiteGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTestDefinitions_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTestDefinitions_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTestDefinitions_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblTestDefinitions_AuditGUID] ON [fmaudit].[tblTestDefinitions](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTestDefinitions_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTestDefinitions] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTestDefinitions_ClusterIdx] ON [fmaudit].[tblTestDefinitions](_ClusterIdx ASC)