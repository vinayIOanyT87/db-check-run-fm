/*

	DROP TABLE [lookup].[tblResetPeriod]

*/
CREATE TABLE [lookup].[tblResetPeriod] (
    [ResetPeriodIndex] INT                NOT NULL,
    [ResetPeriodCode]  NVARCHAR (100)     NOT NULL,
    [ResetPeriodName]  NVARCHAR (100)     NULL,
    [ResetPeriodGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]      DATETIMEOFFSET (7) NULL,
    [CreatedBy]        [dbo].[udtUserID]  NULL,
    [UpdatedDate]      DATETIMEOFFSET (7) NULL,
    [UpdatedBy]        [dbo].[udtUserID]  NULL,
    [_RowVersion]      ROWVERSION         NOT NULL,
    [_ClusterIdx]      BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblResetPeriod] PRIMARY KEY NONCLUSTERED ([ResetPeriodIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblResetPeriod_ResetPeriodGuid]
    ON [lookup].[tblResetPeriod]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblResetPeriod_ClusterIdx]
    ON [lookup].[tblResetPeriod]([_ClusterIdx] ASC);