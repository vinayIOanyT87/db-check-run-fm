/*

	DROP TABLE [lookup].[tblSyncSessionState]

*/
/*

	DROP TABLE [lookup].[tblSyncSessionState]

*/
CREATE TABLE [lookup].[tblSyncSessionState] (
    [SyncSessionStateIndex] BIGINT             NOT NULL,
    [SyncSessionStateCode]  NVARCHAR (80)      NOT NULL,
    [SyncSessionStateName]  NVARCHAR (100)     NOT NULL,
    [SyncSessionStateGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]       NVARCHAR (1024)    NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_sync_tblSyncSessionState] PRIMARY KEY NONCLUSTERED ([SyncSessionStateIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncSessionState_ClusterIdx]
    ON [lookup].[tblSyncSessionState]([_ClusterIdx] ASC);