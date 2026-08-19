CREATE TABLE [fmaudit].[tblExStarsIrsErrorCodes](
	[CodeGroup] nvarchar (25) NULL
,	[Code] nvarchar (10) NULL
,	[Description] nvarchar (1000) NULL
,	[ElementId] nvarchar (10) NULL
,	[ExStarsIrsErrorCodesGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblExStarsIrsErrorCodes_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblExStarsIrsErrorCodes_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblExStarsIrsErrorCodes_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblExStarsIrsErrorCodes_AuditGUID] ON [fmaudit].[tblExStarsIrsErrorCodes](_AuditGUID ASC)
GO
CREATE CLUSTERED INDEX [IX_tblExStarsIrsErrorCodes_ClusterIdx] ON [fmaudit].[tblExStarsIrsErrorCodes](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblExStarsIrsErrorCodes_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExStarsIrsErrorCodes] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)