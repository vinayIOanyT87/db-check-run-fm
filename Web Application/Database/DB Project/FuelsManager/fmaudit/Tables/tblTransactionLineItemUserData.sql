CREATE TABLE [fmaudit].[tblTransactionLineItemUserData](
	[UserData1] nvarchar (60) NULL
,	[UserData2] nvarchar (60) NULL
,	[UserData3] nvarchar (60) NULL
,	[UserData4] nvarchar (60) NULL
,	[UserData5] nvarchar (60) NULL
,	[UserData6] nvarchar (60) NULL
,	[UserData7] nvarchar (60) NULL
,	[UserData8] nvarchar (60) NULL
,	[UserData9] nvarchar (60) NULL
,	[UserData10] nvarchar (60) NULL
,	[UserData11] nvarchar (60) NULL
,	[UserData12] nvarchar (60) NULL
,	[UserData13] nvarchar (60) NULL
,	[UserData14] nvarchar (60) NULL
,	[UserData15] nvarchar (60) NULL
,	[UserData16] nvarchar (60) NULL
,	[UserData17] nvarchar (60) NULL
,	[UserData18] nvarchar (60) NULL
,	[UserData19] nvarchar (60) NULL
,	[UserData20] nvarchar (60) NULL
,	[UserData21] nvarchar (60) NULL
,	[UserData22] nvarchar (60) NULL
,	[UserData23] nvarchar (60) NULL
,	[UserData24] nvarchar (60) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[TransactionLineItemUserDataGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[TransactionLineItemGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactionLineItemUserData_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactionLineItemUserData_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactionLineItemUserData_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)






GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItemUserData_AuditGUID] ON [fmaudit].[tblTransactionLineItemUserData](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItemUserData_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactionLineItemUserData] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblTransactionLineItemUserData_ClusterIdx] ON [fmaudit].[tblTransactionLineItemUserData](_ClusterIdx ASC)