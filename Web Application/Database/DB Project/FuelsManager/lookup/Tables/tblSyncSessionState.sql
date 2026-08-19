CREATE TABLE [lookup].[tblSyncSessionState] (
    [SyncSessionStateIndex] BIGINT             NOT NULL,
    [SyncSessionStateCode]  NVARCHAR (80)      NOT NULL,
    [SyncSessionStateName]  NVARCHAR (100)     NOT NULL,
    [SyncSessionStateGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]       NVARCHAR (1024)    NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncSessionState_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncSessionState_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncSessionState_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncSessionState_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_sync_tblSyncSessionState] PRIMARY KEY NONCLUSTERED ([SyncSessionStateIndex] ASC)
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncSessionState_ClusterIdx]
    ON [lookup].[tblSyncSessionState]([_ClusterIdx] ASC);

