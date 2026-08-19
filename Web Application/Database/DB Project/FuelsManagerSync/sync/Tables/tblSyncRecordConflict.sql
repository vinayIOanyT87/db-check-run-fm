CREATE TABLE [sync].[tblSyncRecordConflict] (
	[SyncRecordConflictGuid]				UNIQUEIDENTIFIER	NOT NULL,
	[TargetNodeGuid]							UNIQUEIDENTIFIER	NOT NULL,
	[TargetNodeName]							NVARCHAR (256)	NULL,
	[TableName]									NVARCHAR (256)	NOT NULL,
	[RecordKey]									NVARCHAR (64)	NOT NULL,
	[RecordRowVersion]						BINARY (8) NULL,
	[ReSyncAnchorMin]							BINARY (8) NULL,
	[ReSyncAnchorMax]							BINARY (8) NULL,
	[SyncConflictTypeIndex]					BIGINT NOT NULL,
	[SyncConflictResolutionStatusIndex] BIGINT NOT NULL,
	[ResolvedDate]								DATETIMEOFFSET (7) NULL,
	[ResolvedBy]								[dbo].[udtUserID]  NULL,
	[CreatedDate]								DATETIMEOFFSET (7) CONSTRAINT [DF_SyncRecordConflict_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[CreatedBy]									[dbo].[udtUserID]  CONSTRAINT [DF_SyncRecordConflict_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
	[UpdatedDate]								DATETIMEOFFSET (7) CONSTRAINT [DF_SyncRecordConflict_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[UpdatedBy]									[dbo].[udtUserID]  CONSTRAINT [DF_SyncRecordConflict_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
	[_RowVersion]								ROWVERSION NOT NULL,
	[ConflictDescription]					NVARCHAR (4000) NOT NULL,
	[CommandText]								NVARCHAR (4000) NULL,
	[CommandType]								BIGINT NULL,
	[Parameters]								VARBINARY(MAX)	NULL,
	[Retrys]										INT NULL,
	CONSTRAINT [PK_tblSyncRecordConflict] PRIMARY KEY NONCLUSTERED ([SyncRecordConflictGuid] ASC) WITH ( ALLOW_PAGE_LOCKS = OFF ),
	CONSTRAINT [FK_tblSyncRecordConflict_tblSyncConflictResolutionStatus] FOREIGN KEY ([SyncConflictResolutionStatusIndex]) REFERENCES [lookup].[tblSyncConflictResolutionStatus] ([SyncConflictResolutionStatusIndex]),
	CONSTRAINT [FK_tblSyncRecordConflict_tblSyncConflictType] FOREIGN KEY ([SyncConflictTypeIndex]) REFERENCES [lookup].[tblSyncConflictType] ([SyncConflictTypeIndex])
);


GO
CREATE CLUSTERED INDEX [IX_tblSyncRecordConflict_CreatedDate] ON [sync].[tblSyncRecordConflict]
	( [CreatedDate] ASC) 
	WITH ( ALLOW_PAGE_LOCKS = OFF )
GO


CREATE NONCLUSTERED INDEX [IX_tblSyncRecordConflict_RecordKey] ON [sync].[tblSyncRecordConflict] 
	([RecordKey] ASC)
	WITH ( ALLOW_PAGE_LOCKS = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_tblSyncRecordConflict_RetryDesc] ON [sync].[tblSyncRecordConflict]
	([Retrys] DESC)
	INCLUDE ([_RowVersion])
	WITH ( ALLOW_PAGE_LOCKS = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_tblSyncRecordConflict_TargetNodeGuid_SyncConflictResolutionStatusIndex_UpdatedDate] ON [sync].[tblSyncRecordConflict] 
([TargetNodeGuid] ASC,[SyncConflictResolutionStatusIndex] ASC,[UpdatedDate] ASC	) 
INCLUDE ([SyncRecordConflictGuid],[CreatedDate])
	WITH ( ALLOW_PAGE_LOCKS = OFF )
GO

CREATE NONCLUSTERED INDEX IX_tblSyncRecordConflict_SyncRecordConflictGuid_TargetNodeGuid ON [sync].[tblSyncRecordConflict] 
([SyncRecordConflictGuid]) 
INCLUDE ([TargetNodeGuid])
	WITH ( ALLOW_PAGE_LOCKS = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_tblSyncRecordConflict_SyncConflictResolutionStatusIndex] ON [sync].[tblSyncRecordConflict]
(
	[SyncConflictResolutionStatusIndex] ASC
)
INCLUDE ([SyncRecordConflictGuid]
		,[TargetNodeGuid]
		,[TargetNodeName]
		,[TableName]
		,[RecordKey]
		,[SyncConflictTypeIndex]
		,[ResolvedDate]
		,[ResolvedBy]
		,[CommandText]
		,[CommandType]
		,[Retrys]
)
GO
