/*

	DROP TABLE [lookup].[tblFuelCardLimitPeriod]

*/
CREATE TABLE [lookup].[tblFuelCardLimitPeriod] (
    [FuelCardLimitPeriodIndex] INT                NOT NULL,
    [FuelCardLimitPeriodCode]  NVARCHAR (100)     NULL,
    [FuelCardLimitPeriodName]  NVARCHAR (100)     NULL,
    [FuelCardLimitPeriodGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedBy]                [dbo].[udtUserID]  NULL,
    [CreatedDate]              DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblFuelCardLimitPeriod] PRIMARY KEY NONCLUSTERED ([FuelCardLimitPeriodIndex] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_tblFuelCardLimitPeriod_FuelCardLimitPeriodGuid] ON [lookup].[tblFuelCardLimitPeriod] (FuelCardLimitPeriodGuid)
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblFuelCardLimitPeriod_ClusterIdx]
    ON [lookup].[tblFuelCardLimitPeriod]([_ClusterIdx] ASC);