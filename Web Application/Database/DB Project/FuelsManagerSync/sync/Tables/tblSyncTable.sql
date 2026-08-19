CREATE TABLE [sync].[tblSyncTable] (
    [SyncTableGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [TableName]                  NVARCHAR (256)     NULL,
    [SyncDependencyGroupGuid]    UNIQUEIDENTIFIER   NOT NULL,
    [LastSchemaDate]             DATETIMEOFFSET (7) NULL,
    [CreatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTable_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                  [dbo].[udtUserID]  NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTable_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  NULL,
    [_RowVersion]                ROWVERSION         NOT NULL,
    [IsSiteFilteredFlag]         BIT                CONSTRAINT [DF_tblSyncTable_IsSiteFilteredFlag] DEFAULT ((0)) NULL,
    [IsSiteFilteredOnDeleteFlag] BIT                CONSTRAINT [DF_tblSyncTable_IsSiteFilteredOnDeleteFlag] DEFAULT ((0)) NULL,
    [ParentSyncTableGuid]        UNIQUEIDENTIFIER   NULL,
    [ParentForeignKeyColumnName] NVARCHAR (512)     NULL,
    [_ClusterIdx]                BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncTable] PRIMARY KEY NONCLUSTERED ([SyncTableGuid] ASC),
    CONSTRAINT [FK_tblSyncTable_tblParentSyncTable] FOREIGN KEY ([ParentSyncTableGuid]) REFERENCES [sync].[tblSyncTable] ([SyncTableGuid]),
    CONSTRAINT [FK_tblSyncTable_tblSyncDependencyGroup] FOREIGN KEY ([SyncDependencyGroupGuid]) REFERENCES [sync].[tblSyncDependencyGroup] ([SyncDependencyGroupGuid])
);






GO
CREATE NONCLUSTERED INDEX [IX_tblSyncTable_CreatedDate]
    ON [sync].[tblSyncTable]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncTable_ClusterIdx]
    ON [sync].[tblSyncTable]([_ClusterIdx] ASC);

