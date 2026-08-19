/*

	DROP TABLE [lookup].[tblTransactionQuality]

*/
CREATE TABLE [lookup].[tblTransactionQuality] (
    [TransactionQualityIndex] INT                NOT NULL,
    [TransactionQualityCode]  NVARCHAR (100)     NOT NULL,
    [TransactionQualityName]  NVARCHAR (100)     NULL,
    [TransactionQualityGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]             DATETIMEOFFSET (7) NULL,
    [CreatedBy]               [dbo].[udtUserID]  NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) NULL,
    [UpdatedBy]               [dbo].[udtUserID]  NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblTransactionQuality] PRIMARY KEY NONCLUSTERED ([TransactionQualityIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblTransactionQuality_TransactionQualityGuid]
    ON [lookup].[tblTransactionQuality]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionQuality_ClusterIdx]
    ON [lookup].[tblTransactionQuality]([_ClusterIdx] ASC);