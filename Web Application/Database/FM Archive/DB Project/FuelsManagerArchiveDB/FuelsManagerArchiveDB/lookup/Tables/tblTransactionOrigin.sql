/*

	DROP TABLE [lookup].[tblTransactionOrigin]

*/
CREATE TABLE [lookup].[tblTransactionOrigin] (
    [TransactionOriginIndex] INT                NOT NULL,
    [TransactionOriginCode]  NVARCHAR (100)     NOT NULL,
    [TransactionOriginName]  NVARCHAR (100)     NULL,
    [TransactionOriginGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblTransactionOrigin] PRIMARY KEY NONCLUSTERED ([TransactionOriginIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblTransactionOrigin_TransactionOriginGuid]
    ON [lookup].[tblTransactionOrigin]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionOrigin_ClusterIdx]
    ON [lookup].[tblTransactionOrigin]([_ClusterIdx] ASC);