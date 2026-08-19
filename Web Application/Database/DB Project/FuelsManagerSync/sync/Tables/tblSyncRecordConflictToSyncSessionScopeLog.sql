CREATE TABLE [sync].[tblSyncRecordConflictToSyncSessionScopeLog] (
    [SyncRecordConflictToSyncSessionScopeLogGuid] UNIQUEIDENTIFIER   NOT NULL,
    [SyncRecordConflictGuid]                      UNIQUEIDENTIFIER   NOT NULL,
    [SyncSessionScopeLogGuid]                     UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                                 DATETIMEOFFSET (7) CONSTRAINT [DF_SyncRecordConflictToSyncSessionScopeLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                                   [dbo].[udtUserID]  CONSTRAINT [DF_SyncRecordConflictToSyncSessionScopeLog_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                                 ROWVERSION         NOT NULL,
    CONSTRAINT [PK_tblSyncRecordConflictToSyncSessionScopeLog] PRIMARY KEY NONCLUSTERED ([SyncRecordConflictToSyncSessionScopeLogGuid] ASC) WITH ( ALLOW_PAGE_LOCKS = OFF ),
    CONSTRAINT [FK_tblSyncRecordConflictToSyncSessionScopeLog_tblSyncRecordConflict] FOREIGN KEY ([SyncRecordConflictGuid]) REFERENCES [sync].[tblSyncRecordConflict] ([SyncRecordConflictGuid]),
    CONSTRAINT [FK_tblSyncRecordConflictToSyncSessionScopeLog_tblSyncSessionScopeLog] FOREIGN KEY ([SyncSessionScopeLogGuid]) REFERENCES [sync].[tblSyncSessionScopeLog] ([SyncSessionScopeLogGuid])
);


GO
CREATE CLUSTERED INDEX [IX_tblSyncRecordConflictToSyncSessionScopeLog_CreatedDate]
    ON [sync].[tblSyncRecordConflictToSyncSessionScopeLog]([CreatedDate] ASC) 
	WITH ( ALLOW_PAGE_LOCKS = OFF );
GO


CREATE NONCLUSTERED INDEX [IX_tblSyncRecordConflictToSyncSessionScopeLog_SyncRCGuid_SyncSessionScopeLogGuid] ON [sync].[tblSyncRecordConflictToSyncSessionScopeLog] (
	[SyncRecordConflictGuid] ASC
	,[SyncSessionScopeLogGuid] ASC
	)
	WITH ( ALLOW_PAGE_LOCKS = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_tblSyncRecordConflictToSyncSessionScopeLog_SyncSessionScopeLogGuid] ON [sync].[tblSyncRecordConflictToSyncSessionScopeLog] (
	[SyncSessionScopeLogGuid] ASC
	) 
	INCLUDE ([SyncRecordConflictGuid])
WITH ( ALLOW_PAGE_LOCKS = OFF )

GO

