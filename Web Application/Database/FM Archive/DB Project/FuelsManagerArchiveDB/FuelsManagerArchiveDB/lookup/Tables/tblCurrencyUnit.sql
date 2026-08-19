/*

	DROP TABLE [lookup].[tblCurrencyUnit]

*/
/*

	DROP TABLE [lookup].[tblCurrencyUnit]

*/
CREATE TABLE [lookup].[tblCurrencyUnit] (
    [CurrencyUnitIndex] INT                NOT NULL,
    [CurrencyUnitCode]  NVARCHAR (100)     NOT NULL,
    [CurrencyUnitName]  NVARCHAR (100)     NULL,
    [CurrencyUnitGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) NULL,
    [CreatedBy]         [dbo].[udtUserID]  NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
    [_ClusterIdx]       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblCurrencyUnit] PRIMARY KEY NONCLUSTERED ([CurrencyUnitIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblCurrencyUnit_CurrencyUnitGuid]
    ON [lookup].[tblCurrencyUnit]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblCurrencyUnit_ClusterIdx]
    ON [lookup].[tblCurrencyUnit]([_ClusterIdx] ASC);