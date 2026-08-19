CREATE TABLE [sync].[tblSyncTableToScopeMapCommand] (
    [SyncTableToScopeMapCommandGuid] UNIQUEIDENTIFIER   NOT NULL,
    [SyncTableToScopeMapGuid]        UNIQUEIDENTIFIER   NOT NULL,
    [SelectIncrementalInserts]       NVARCHAR (512)     NULL,
    [ApplyIncrementalInserts]        NVARCHAR (512)     NULL,
    [SelectIncrementalUpdates]       NVARCHAR (512)     NULL,
    [ApplyIncrementalUpdates]        NVARCHAR (512)     NULL,
    [SelectIncrementalDeletes]       NVARCHAR (512)     NULL,
    [ApplyIncrementalDeletes]        NVARCHAR (512)     NULL,
    [SelectUpdateConflicts]          NVARCHAR (512)     NULL,
    [SelectDeleteConflicts]          NVARCHAR (512)     NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTableToScopeMapCommand_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                      [dbo].[udtUserID]  NULL,
    [UpdatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTableToScopeMapCommand_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                      [dbo].[udtUserID]  NULL,
    [_RowVersion]                    ROWVERSION         NOT NULL,
    [_ClusterIdx]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncTableToScopeMapCommand] PRIMARY KEY NONCLUSTERED ([SyncTableToScopeMapGuid] ASC),
    CONSTRAINT [FK_tblSyncTableToScopeMapCommand_tblSyncTableToScopeMap] FOREIGN KEY ([SyncTableToScopeMapGuid]) REFERENCES [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid])
);






GO
CREATE NONCLUSTERED INDEX [IX_tblSyncTableToScopeMapCommand_CreatedDate]
    ON [sync].[tblSyncTableToScopeMapCommand]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncTableToScopeMapCommand_ClusterIdx]
    ON [sync].[tblSyncTableToScopeMapCommand]([_ClusterIdx] ASC);

