CREATE TABLE [lookup].[tblSyncTransferType] (
    [SyncTransferTypeIndex] BIGINT             NOT NULL,
    [SyncTransferTypeCode]  NVARCHAR (80)      NOT NULL,
    [SyncTransferTypeName]  NVARCHAR (100)     NOT NULL,
    [SyncTransferTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]       NVARCHAR (1024)    NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTransferType_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncTransferType_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncTransferType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncTransferType_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_sync_tblSyncTransferType] PRIMARY KEY NONCLUSTERED ([SyncTransferTypeIndex] ASC)
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncTransferType_ClusterIdx]
    ON [lookup].[tblSyncTransferType]([_ClusterIdx] ASC);

