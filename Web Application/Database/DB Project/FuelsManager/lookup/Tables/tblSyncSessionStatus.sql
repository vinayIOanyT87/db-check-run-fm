CREATE TABLE [lookup].[tblSyncSessionStatus] (
    [SyncSessionStatusIndex] BIGINT             NOT NULL,
    [SyncSessionStatusCode]  NVARCHAR (80)      NOT NULL,
    [SyncSessionStatusName]  NVARCHAR (100)     NOT NULL,
    [SyncSessionStatusGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]        NVARCHAR (1024)    NULL,
    [CreatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncSessionStatus_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncSessionStatus_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncSessionStatus_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncSessionStatus_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_sync_tblSyncSessionStatus] PRIMARY KEY NONCLUSTERED ([SyncSessionStatusIndex] ASC)
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncSessionStatus_ClusterIdx]
    ON [lookup].[tblSyncSessionStatus]([_ClusterIdx] ASC);

