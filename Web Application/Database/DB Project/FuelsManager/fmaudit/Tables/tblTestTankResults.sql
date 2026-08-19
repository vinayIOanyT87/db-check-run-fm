CREATE TABLE [fmaudit].[tblTestTankResults](
	[TestName] nvarchar (80) NULL
,	[Measurement] nvarchar (50) NULL
,	[TestDate] datetimeoffset NULL
,	[DeleteFlag] bit NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[PerformedBy] nvarchar (100) NULL
,	[Supervisor] nvarchar (100) NULL
,	[Flag01] bit NULL
,	[Flag02] bit NULL
,	[TestCode] nvarchar (5) NULL
,	[TestTankResultGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[LookupTestSetStatusIndex] int NULL
,	[TestSetTankResultGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTestTankResults_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTestTankResults_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTestTankResults_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblTestTankResults_AuditGUID] ON [fmaudit].[tblTestTankResults](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTestTankResults_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTestTankResults] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTestTankResults_ClusterIdx] ON [fmaudit].[tblTestTankResults](_ClusterIdx ASC)