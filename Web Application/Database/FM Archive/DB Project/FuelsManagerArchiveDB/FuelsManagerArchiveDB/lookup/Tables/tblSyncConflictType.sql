/*

	DROP TABLE [lookup].[tblSyncConflictType]

*/
CREATE TABLE [lookup].[tblSyncConflictType] (
    [SyncConflictTypeIndex] BIGINT             NOT NULL,
    [SyncConflictTypeCode]  NVARCHAR (80)      NOT NULL,
    [SyncConflictTypeName]  NVARCHAR (100)     NOT NULL,
    [SyncConflictTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]       NVARCHAR (1024)    NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblSyncConflictType] PRIMARY KEY NONCLUSTERED ([SyncConflictTypeIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncConflictType_ClusterIdx]
    ON [lookup].[tblSyncConflictType]([_ClusterIdx] ASC);