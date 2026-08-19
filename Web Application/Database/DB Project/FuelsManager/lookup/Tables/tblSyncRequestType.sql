CREATE TABLE [lookup].[tblSyncRequestType] (
    [SyncRequestTypeIndex] BIGINT             NOT NULL,
    [SyncRequestTypeCode]  NVARCHAR (80)      NOT NULL,
    [SyncRequestTypeName]  NVARCHAR (100)     NOT NULL,
    [SyncRequestTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]      NVARCHAR (1024)    NULL,
    [CreatedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncRequestType_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]            [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncRequestType_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncRequestType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]            [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncRequestType_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]          ROWVERSION         NOT NULL,
    [_ClusterIdx]          BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_sync_tblSyncRequestType] PRIMARY KEY NONCLUSTERED ([SyncRequestTypeIndex] ASC)
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncRequestType_ClusterIdx]
    ON [lookup].[tblSyncRequestType]([_ClusterIdx] ASC);

