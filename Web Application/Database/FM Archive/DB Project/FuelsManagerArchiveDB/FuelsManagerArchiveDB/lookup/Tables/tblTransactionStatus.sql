/*

	DROP TABLE [lookup].[tblTransactionStatus]

*/
CREATE TABLE [lookup].[tblTransactionStatus] (
    [TransactionStatusIndex] INT                NOT NULL,
    [TransactionStatusCode]  NVARCHAR (100)     NOT NULL,
    [TransactionStatusName]  NVARCHAR (100)     NULL,
    [TransactionStatusGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblTransactionStatus] PRIMARY KEY NONCLUSTERED ([TransactionStatusIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblTransactionStatus_TransactionStatusGuid]
    ON [lookup].[tblTransactionStatus]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionStatus_ClusterIdx]
    ON [lookup].[tblTransactionStatus]([_ClusterIdx] ASC);