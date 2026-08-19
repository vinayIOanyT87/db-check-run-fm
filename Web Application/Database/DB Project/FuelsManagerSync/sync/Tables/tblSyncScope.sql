CREATE TABLE [sync].[tblSyncScope] (
    [SyncScopeGuid]      UNIQUEIDENTIFIER   NOT NULL,
    [ID]                 NVARCHAR (80)      NOT NULL,
    [SyncScopeTypeIndex] BIGINT             NOT NULL,
    [FriendlyName]       NVARCHAR (100)     NOT NULL,
    [LongDescription]    NVARCHAR (1024)    NULL,
    [SyncProfileGuid]    UNIQUEIDENTIFIER   NOT NULL,
    [SyncOrder]          INT                NOT NULL,
    [CreatedDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncScope_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL,
    [UpdatedDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncScope_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]          [dbo].[udtUserID]  NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    [SyncSinglePass]     BIT                CONSTRAINT [DF_tblSyncScope_SyncSinglePass] DEFAULT ((0)) NOT NULL,
    [_ClusterIdx]        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncScope] PRIMARY KEY NONCLUSTERED ([SyncScopeGuid] ASC),
    CONSTRAINT [FK_tblSyncScope_tblSyncProfile] FOREIGN KEY ([SyncProfileGuid]) REFERENCES [sync].[tblSyncProfile] ([SyncProfileGuid]),
    CONSTRAINT [FK_tblSyncScope_tblSyncScopeType] FOREIGN KEY ([SyncScopeTypeIndex]) REFERENCES [sync].[tblSyncScopeType] ([SyncScopeTypeIndex])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblSyncScope_SyncProfileGuid]
    ON [sync].[tblSyncScope]([SyncProfileGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblSyncScope_SyncOrder]
    ON [sync].[tblSyncScope]([SyncOrder] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncScope_ClusterIdx]
    ON [sync].[tblSyncScope]([_ClusterIdx] ASC);

