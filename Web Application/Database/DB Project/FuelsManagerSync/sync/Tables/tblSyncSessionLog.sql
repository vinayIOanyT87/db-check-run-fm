CREATE TABLE [sync].[tblSyncSessionLog] (
    [SyncSessionLogGuid]     UNIQUEIDENTIFIER   NOT NULL,
    [SyncProfileID]          NVARCHAR (80)      NOT NULL,
    [SyncSessionStatusIndex] BIGINT             NOT NULL,
    [SyncSessionStateIndex]  BIGINT             NOT NULL,
    [SyncTransferTypeIndex]  BIGINT             NOT NULL,
    [SyncRequestTypeIndex]   BIGINT             NOT NULL,
    [SyncDateRangeStart]     DATETIMEOFFSET (7) NULL,
    [SyncDateRangeEnd]       DATETIMEOFFSET (7) NULL,
    [StartDate]              DATETIMEOFFSET (7) NULL,
    [EndDate]                DATETIMEOFFSET (7) NULL,
    [RemoteNodeGuid]         UNIQUEIDENTIFIER   NOT NULL,
    [RemoteNodeMachineName]  NVARCHAR (256)     NOT NULL,
    [SyncAnchorMax]          BINARY (8)         NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]              [dbo].[udtUserID]  NOT NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NOT NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NOT NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_sync_tblSyncSessionLog] PRIMARY KEY NONCLUSTERED ([SyncSessionLogGuid] ASC),
    CONSTRAINT [FK_tblSyncSessionLog_tblSyncRequestType] FOREIGN KEY ([SyncRequestTypeIndex]) REFERENCES [lookup].[tblSyncRequestType] ([SyncRequestTypeIndex]),
    CONSTRAINT [FK_tblSyncSessionLog_tblSyncSessionState] FOREIGN KEY ([SyncSessionStateIndex]) REFERENCES [lookup].[tblSyncSessionState] ([SyncSessionStateIndex]),
    CONSTRAINT [FK_tblSyncSessionLog_tblSyncSessionStatus] FOREIGN KEY ([SyncSessionStatusIndex]) REFERENCES [lookup].[tblSyncSessionStatus] ([SyncSessionStatusIndex]),
    CONSTRAINT [FK_tblSyncSessionLog_tblSyncTransferType] FOREIGN KEY ([SyncTransferTypeIndex]) REFERENCES [lookup].[tblSyncTransferType] ([SyncTransferTypeIndex])
);

GO

/****** Object:  Index [IX_tblSyncSessionLog_EndDateStartDateDesc]    Script Date: 4/16/2015 6:57:39 PM ******/
/****** Object:  Index [IX_tblSyncSessionLog_EndDateStartDateDesc]    Script Date: 4/16/2015 6:57:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblSyncSessionLog_EndDateStartDateDesc] ON [sync].[tblSyncSessionLog]
(
	[EndDate] DESC,
	[StartDate] DESC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_tblSyncSessionLog_StartDateDesc]    Script Date: 4/16/2015 6:57:39 PM ******/
/****** Object:  Index [IX_tblSyncSessionLog_StartDateDesc]    Script Date: 4/16/2015 6:57:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblSyncSessionLog_StartDateDesc] ON [sync].[tblSyncSessionLog]
(
	[StartDate] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_tblSyncSessionLog_StartEndNode]    Script Date: 4/16/2015 6:57:39 PM ******/
/****** Object:  Index [IX_tblSyncSessionLog_StartEndNode]    Script Date: 4/16/2015 6:57:39 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblSyncSessionLog_StartEndNode] ON [sync].[tblSyncSessionLog]
(
	[SyncSessionLogGuid] ASC,
	[RemoteNodeGuid] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE NONCLUSTERED INDEX [IX_tblSyncSessionLog_CreatedDate]
    ON [sync].[tblSyncSessionLog]([CreatedDate] ASC);
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncSessionLog_ClusterIdx]
    ON [sync].[tblSyncSessionLog]([_ClusterIdx] ASC);
GO

CREATE INDEX [IX_tblSyncSessionScopeLog_SyncSessionLogGuid_ScopeID]
 ON [sync].[tblSyncSessionScopeLog] 
([SyncSessionLogGuid],[ScopeID])
GO

