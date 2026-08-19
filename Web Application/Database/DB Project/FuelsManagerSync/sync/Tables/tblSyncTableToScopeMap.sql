CREATE TABLE [sync].[tblSyncTableToScopeMap] (
    [SyncTableToScopeMapGuid]     UNIQUEIDENTIFIER   NOT NULL,
    [ID]                          NVARCHAR (80)      NOT NULL,
    [SyncScopeGuid]               UNIQUEIDENTIFIER   NOT NULL,
    [SyncTableGuid]               UNIQUEIDENTIFIER   NOT NULL,
    [SyncOrder]                   INT                NOT NULL,
    [SyncDirection]               INT                NOT NULL,
    [MaxBatchSegmentRowCount]     INT                CONSTRAINT [DF_tblSyncTableToScopeMap_MaxBatchSegmentRowCount] DEFAULT ((0)) NULL,
    [MaxTransferSegmentKB]        INT                CONSTRAINT [DF_tblSyncTableToScopeMap_MaxTransferSegmentKB] DEFAULT ((0)) NULL,
    [AdditionalFilterJoinClause]  NVARCHAR (1024)    NULL,
    [AdditionalFilterWhereClause] NVARCHAR (512)     NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTableToScopeMap_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                   [dbo].[udtUserID]  NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTableToScopeMap_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  NULL,
    [_RowVersion]                 ROWVERSION         NOT NULL,
    [ClientTableNameOverride]     NVARCHAR (256)     NULL,
    [FirstTimeSyncOption]         INT                CONSTRAINT [DF_tblSyncTableToScopeMap_FirstTimeSyncOption] DEFAULT ((0)) NULL,
    [_ClusterIdx]                 BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncTableToScopeMap] PRIMARY KEY NONCLUSTERED ([SyncTableToScopeMapGuid] ASC),
    CONSTRAINT [FK_tblSyncTableToScopeMap_tblSyncScope] FOREIGN KEY ([SyncScopeGuid]) REFERENCES [sync].[tblSyncScope] ([SyncScopeGuid]),
    CONSTRAINT [FK_tblSyncTableToScopeMap_tblSyncTable] FOREIGN KEY ([SyncTableGuid]) REFERENCES [sync].[tblSyncTable] ([SyncTableGuid])
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncTableToScopeMap_ClusterIdx]
    ON [sync].[tblSyncTableToScopeMap]([_ClusterIdx] ASC);

GO

CREATE INDEX [IX_IXNC_tblSyncTableToScopeMapColumn_SyncTableToScopeMapGuid] 
ON [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapGuid])
GO
