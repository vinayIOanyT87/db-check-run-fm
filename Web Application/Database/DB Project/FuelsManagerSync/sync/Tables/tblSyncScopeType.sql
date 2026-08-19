CREATE TABLE [sync].[tblSyncScopeType] (
    [SyncScopeTypeIndex] BIGINT             NOT NULL,
    [ID]                 NVARCHAR (80)      NOT NULL,
    [FriendlyName]       NVARCHAR (100)     NOT NULL,
    [LongDescription]    NVARCHAR (1024)    NULL,
    [CreatedDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncScopeType_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL,
    [UpdatedDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncScopeType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]          [dbo].[udtUserID]  NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    [_ClusterIdx]        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncScopeType] PRIMARY KEY NONCLUSTERED ([SyncScopeTypeIndex] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_tblSyncScopeType_CreatedDate]
    ON [sync].[tblSyncScopeType]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncScopeType_ClusterIdx]
    ON [sync].[tblSyncScopeType]([_ClusterIdx] ASC);

