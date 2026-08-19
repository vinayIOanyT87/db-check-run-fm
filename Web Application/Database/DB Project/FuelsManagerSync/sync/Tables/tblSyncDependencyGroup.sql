CREATE TABLE [sync].[tblSyncDependencyGroup] (
    [SyncDependencyGroupGuid] UNIQUEIDENTIFIER   NOT NULL,
    [ID]                      NVARCHAR (80)      NOT NULL,
    [FriendlyName]            NVARCHAR (100)     NOT NULL,
    [LongDescription]         NVARCHAR (1024)    NULL,
    [DependencyLevel]         INT                NOT NULL,
    [CreatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncDependencyGroup_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]               [dbo].[udtUserID]  NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncDependencyGroup_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]               [dbo].[udtUserID]  NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncDependencyGroup] PRIMARY KEY NONCLUSTERED ([SyncDependencyGroupGuid] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblSyncDependencyGroup_DependencyLevel]
    ON [sync].[tblSyncDependencyGroup]([DependencyLevel] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncDependencyGroup_ClusterIdx]
    ON [sync].[tblSyncDependencyGroup]([_ClusterIdx] ASC);

