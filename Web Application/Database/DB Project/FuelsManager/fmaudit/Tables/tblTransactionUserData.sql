CREATE TABLE [fmaudit].[tblTransactionUserData](
	[UserData1] nvarchar (255) NULL
,	[UserData2] nvarchar (255) NULL
,	[UserData3] nvarchar (255) NULL
,	[UserData4] nvarchar (255) NULL
,	[UserData5] nvarchar (255) NULL
,	[UserData6] nvarchar (255) NULL
,	[UserData7] nvarchar (255) NULL
,	[UserData8] nvarchar (255) NULL
,	[UserData9] nvarchar (255) NULL
,	[UserData10] nvarchar (255) NULL
,	[UserData11] nvarchar (255) NULL
,	[UserData12] nvarchar (255) NULL
,	[UserData13] nvarchar (255) NULL
,	[UserData14] nvarchar (255) NULL
,	[UserData15] nvarchar (255) NULL
,	[UserData16] nvarchar (255) NULL
,	[UserData17] nvarchar (255) NULL
,	[UserData18] nvarchar (255) NULL
,	[UserData19] nvarchar (255) NULL
,	[UserData20] nvarchar (255) NULL
,	[UserData21] nvarchar (255) NULL
,	[UserData22] nvarchar (255) NULL
,	[UserData23] nvarchar (255) NULL
,	[UserData24] nvarchar (255) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[TransactionUserDataGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[TransactionGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactionUserData_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactionUserData_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactionUserData_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)






GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionUserData_AuditGUID] ON [fmaudit].[tblTransactionUserData](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionUserData_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactionUserData] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_tblTransactionUserData_ClusterIdx] ON [fmaudit].[tblTransactionUserData](_ClusterIdx ASC)