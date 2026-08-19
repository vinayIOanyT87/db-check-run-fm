/*

	DROP TABLE [lookup].[tblSyncSessionStatus]

*/
CREATE TABLE [lookup].[tblSyncSessionStatus] (
    [SyncSessionStatusIndex] BIGINT             NOT NULL,
    [SyncSessionStatusCode]  NVARCHAR (80)      NOT NULL,
    [SyncSessionStatusName]  NVARCHAR (100)     NOT NULL,
    [SyncSessionStatusGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]        NVARCHAR (1024)    NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_sync_tblSyncSessionStatus] PRIMARY KEY NONCLUSTERED ([SyncSessionStatusIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncSessionStatus_ClusterIdx]
    ON [lookup].[tblSyncSessionStatus]([_ClusterIdx] ASC);