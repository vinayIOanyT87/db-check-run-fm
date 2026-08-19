CREATE TABLE [fmaudit].[tblTransactionPIDX](
	[AuthorizationNumber] nvarchar (8) NULL
,	[SentFlag] bit NULL
,	[DateSent] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[BrokenBlend] bit NULL
,	[TransactionPIDXGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[PIDXProfileGuid] uniqueidentifier NULL
,	[TransactionGuid] uniqueidentifier NULL
,	[CompanyPersonnelToShipToBillToGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactionPIDX_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactionPIDX_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactionPIDX_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL, 
    [BOLVersion] INT NULL 
)






GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionPIDX_AuditGUID] ON [fmaudit].[tblTransactionPIDX](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionPIDX_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactionPIDX] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_tblTransactionPIDX_ClusterIdx] ON [fmaudit].[tblTransactionPIDX](_ClusterIdx ASC)