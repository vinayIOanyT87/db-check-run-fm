CREATE TABLE [lookup].[tblSyncConflictResolutionStatus] (
    [SyncConflictResolutionStatusIndex] BIGINT             NOT NULL,
    [SyncConflictResolutionStatusGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [StatusCode]                        NVARCHAR (80)      NOT NULL,
    [StatusName]                        NVARCHAR (100)     NOT NULL,
    [LongDescription]                   NVARCHAR (1024)    NULL,
    [CreatedDate]                       DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncConflictResolutionStatus_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                         [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncConflictResolutionStatus_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]                       DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncConflictResolutionStatus_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                         [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncConflictResolutionStatus_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                       ROWVERSION         NOT NULL,
    [SequenceOrder]                     INT                NULL,
    [_ClusterIdx]                       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_SyncConflictResolutionStatus] PRIMARY KEY NONCLUSTERED ([SyncConflictResolutionStatusIndex] ASC)
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncConflictResolutionStatus_ClusterIdx]
    ON [lookup].[tblSyncConflictResolutionStatus]([_ClusterIdx] ASC);

