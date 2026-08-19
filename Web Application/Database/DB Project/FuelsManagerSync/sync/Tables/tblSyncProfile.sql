CREATE TABLE [sync].[tblSyncProfile] (
    [SyncProfileGuid] UNIQUEIDENTIFIER   NOT NULL,
    [ID]              NVARCHAR (80)      NOT NULL,
    [FriendlyName]    NVARCHAR (100)     NOT NULL,
    [LongDescription] NVARCHAR (1024)    NULL,
    [CreatedDate]     DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncProfile_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]       [dbo].[udtUserID]  NULL,
    [UpdatedDate]     DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncProfile_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]       [dbo].[udtUserID]  NULL,
    [_RowVersion]     ROWVERSION         NOT NULL,
    [_ClusterIdx]     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncProfile] PRIMARY KEY NONCLUSTERED ([SyncProfileGuid] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_tblSyncProfile_CreatedDate]
    ON [sync].[tblSyncProfile]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncProfile_ClusterIdx]
    ON [sync].[tblSyncProfile]([_ClusterIdx] ASC);

