CREATE TABLE [sync].[tblSyncSessionScopeLog] (
    [SyncSessionScopeLogGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [SyncSessionLogGuid]       UNIQUEIDENTIFIER   NOT NULL,
    [SiteGuid]                 UNIQUEIDENTIFIER   NULL,
    [SiteTypeIndex]            BIGINT             NULL,
    [ScopeID]                  NVARCHAR (80)      NOT NULL,
    [SyncSessionStatusIndex]   BIGINT             NOT NULL,
    [SyncSessionStateIndex]    BIGINT             NOT NULL,
    [StartDate]                DATETIMEOFFSET (7) NULL,
    [EndDate]                  DATETIMEOFFSET (7) NULL,
    [TableCount]               INT                NULL,
    [TableSuccessCount]        INT                NULL,
    [TableErrorCount]          INT                NULL,
    [TotalChangesCount]        INT                NULL,
    [TotalChangesAppliedCount] INT                NULL,
    [TotalChangesFailedCount]  INT                NULL,
    [TotalChangesPendingCount] INT                NULL,
    [TotalDeleteCount]         INT                NULL,
    [TotalInsertCount]         INT                NULL,
    [TotalUpdateCount]         INT                NULL,
    [BatchFileName]            NVARCHAR (384)     NULL,
    [CreatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncSessionScopeLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncSessionScopeLog_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncSessionScopeLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncSessionScopeLog_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    CONSTRAINT [PK_tblSyncSessionScopeLog] PRIMARY KEY NONCLUSTERED ([SyncSessionScopeLogGuid] ASC),
    CONSTRAINT [FK_tblSyncSessionScopeLog_tblSiteType] FOREIGN KEY ([SiteTypeIndex]) REFERENCES [sync].[tblSiteType] ([SiteTypeIndex]),
    CONSTRAINT [FK_tblSyncSessionScopeLog_tblSyncSessionLog] FOREIGN KEY ([SyncSessionLogGuid]) REFERENCES [sync].[tblSyncSessionLog] ([SyncSessionLogGuid]),
    CONSTRAINT [FK_tblSyncSessionScopeLog_tblSyncSessionState] FOREIGN KEY ([SyncSessionStateIndex]) REFERENCES [lookup].[tblSyncSessionState] ([SyncSessionStateIndex]),
    CONSTRAINT [FK_tblSyncSessionScopeLog_tblSyncSessionStatus] FOREIGN KEY ([SyncSessionStatusIndex]) REFERENCES [lookup].[tblSyncSessionStatus] ([SyncSessionStatusIndex])
);


GO
CREATE CLUSTERED INDEX [IX_tblSyncSessionScopeLog_CreatedDate]
    ON [sync].[tblSyncSessionScopeLog]([CreatedDate] ASC);
GO

CREATE INDEX [IX_tblSyncSessionScopeLog_SyncSessionLogGuid_SiteGuid_ScopeID] ON [sync].[tblSyncSessionScopeLog] 
([SyncSessionLogGuid],[SiteGuid],[ScopeID])
GO

CREATE NONCLUSTERED INDEX [IX_tblSyncSessionScopeLog_SyncSessionLogGuid_SyncSessionScopeLogGuid_SiteGuid_ScopeID] ON [sync].[tblSyncSessionScopeLog] (
[SyncSessionLogGuid] ASC
,[SyncSessionScopeLogGuid] ASC
,[SiteGuid] ASC
,[ScopeID] ASC
) INCLUDE ([TotalChangesCount])
GO