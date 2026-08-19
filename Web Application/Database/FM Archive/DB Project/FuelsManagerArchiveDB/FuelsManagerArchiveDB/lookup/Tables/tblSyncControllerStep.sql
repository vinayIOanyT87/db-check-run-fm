/*

	DROP TABLE [lookup].[tblSyncControllerStep]

*/
CREATE TABLE [lookup].[tblSyncControllerStep] (
    [SyncControllerStepIndex] BIGINT             NOT NULL,
    [SyncControllerStepCode]  NVARCHAR (80)      NOT NULL,
    [SyncControllerStepName]  NVARCHAR (100)     NOT NULL,
    [SyncControllerStepGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [LongDescription]       NVARCHAR (1024)    NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    CONSTRAINT [PK_sync.tblSyncControllerStep] PRIMARY KEY CLUSTERED ([SyncControllerStepIndex] ASC)
);