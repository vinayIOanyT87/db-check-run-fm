/*

	DROP TABLE [lookup].[tblTransactionTypes]

*/
CREATE TABLE [lookup].[tblTransactionTypes] (
    [TransactionTypesIndex] SMALLINT           NOT NULL,
    [TransactionTypesCode]  NVARCHAR (100)     NOT NULL,
    [TransactionTypesName]  NVARCHAR (100)     NULL,
    [TransactionTypesGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NULL,
    [CreatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblTransactionTypes] PRIMARY KEY NONCLUSTERED ([TransactionTypesIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblTransactionTypes_TransactionTypesGuid]
    ON [lookup].[tblTransactionTypes]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionTypes_ClusterIdx]
    ON [lookup].[tblTransactionTypes]([_ClusterIdx] ASC);