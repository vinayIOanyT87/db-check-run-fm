CREATE TABLE [fmaudit].[tblExcise](
	[ExciseRate] float NULL
,	[ExciseCode] nvarchar (50) NULL
,	[ExciseDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[ExciseGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[ProductGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblExcise_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblExcise_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblExcise_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblExcise_AuditGUID] ON [fmaudit].[tblExcise](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblExcise_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblExcise] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblExcise_ClusterIdx] ON [fmaudit].[tblExcise](_ClusterIdx ASC)