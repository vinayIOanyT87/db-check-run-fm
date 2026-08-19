CREATE TABLE [lookup].[tblSyncConflictType] (
    [SyncConflictTypeIndex] BIGINT             NOT NULL,
    [SyncConflictTypeCode]  NVARCHAR (80)      NOT NULL,
    [SyncConflictTypeName]  NVARCHAR (100)     NOT NULL,
    [SyncConflictTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]       NVARCHAR (1024)    NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncConflictType_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncConflictType_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncConflictType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncConflictType_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblSyncConflictType] PRIMARY KEY NONCLUSTERED ([SyncConflictTypeIndex] ASC)
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncConflictType_ClusterIdx]
    ON [lookup].[tblSyncConflictType]([_ClusterIdx] ASC);

