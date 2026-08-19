CREATE TABLE [sync].[tblSyncAnchor] (
    [SyncAnchorGuid]                UNIQUEIDENTIFIER   CONSTRAINT [DF_tblSyncAnchor_SyncAnchorGuid] DEFAULT (newid()) NOT NULL,
    [SiteID]                        NVARCHAR (30)      NULL,
    [TableName]                     NVARCHAR (256)     NOT NULL,
    [LastReceivedAnchor]            BINARY (8)         NULL,
    [LastSentAnchor1]               BINARY (8)         NULL,
    [LastSentAnchor2]               BINARY (8)         NULL,
    [CurrentBatchSegment]           BIGINT             NULL,
    [MaxBatchSegment]               BIGINT             NULL,
    [LastDateRangeStart]            DATETIMEOFFSET (7) NULL,
    [LastDateRangeEnd]              DATETIMEOFFSET (7) NULL,
    [LastDateRangeDateSynchronized] DATETIMEOFFSET (7) NULL,
    [LastSynchronizedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncAnchor_LastSynchronizedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [LastReceivedAnchor2]           BINARY (8)         NULL,
    [_ClusterIdx]                   BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncAnchor] PRIMARY KEY NONCLUSTERED ([SyncAnchorGuid] ASC)
);




GO



GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_tblSyncAnchor_SiteID_TableName]
    ON [sync].[tblSyncAnchor]([SiteID] ASC, [TableName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblSyncAnchor_SiteID]
    ON [sync].[tblSyncAnchor]([SiteID] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncAnchor_ClusterIdx]
    ON [sync].[tblSyncAnchor]([_ClusterIdx] ASC);

