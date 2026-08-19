/*

	DROP TABLE [lookup].[tblSyncRequestType]

*/
CREATE TABLE [lookup].[tblSyncRequestType] (
    [SyncRequestTypeIndex] BIGINT             NOT NULL,
    [SyncRequestTypeCode]  NVARCHAR (80)      NOT NULL,
    [SyncRequestTypeName]  NVARCHAR (100)     NOT NULL,
    [SyncRequestTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]      NVARCHAR (1024)    NULL,
    [CreatedDate]          DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]            [dbo].[udtUserID]  NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) NULL,
    [UpdatedBy]            [dbo].[udtUserID]  NULL,
    [_RowVersion]          ROWVERSION         NOT NULL,
    [_ClusterIdx]          BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_sync_tblSyncRequestType] PRIMARY KEY NONCLUSTERED ([SyncRequestTypeIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncRequestType_ClusterIdx]
    ON [lookup].[tblSyncRequestType]([_ClusterIdx] ASC);