CREATE TABLE [lookup].[tblSyncControllerStep] (
    [SyncControllerStepIndex] BIGINT             NOT NULL,
    [SyncControllerStepCode]  NVARCHAR (80)      NOT NULL,
    [SyncControllerStepName]  NVARCHAR (100)     NOT NULL,
    [SyncControllerStepGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]       NVARCHAR (1024)    NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncControllerStep_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncControllerStep_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncControllerStep_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncControllerStep_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    CONSTRAINT [PK_sync.tblSyncControllerStep] PRIMARY KEY CLUSTERED ([SyncControllerStepIndex] ASC)
);

