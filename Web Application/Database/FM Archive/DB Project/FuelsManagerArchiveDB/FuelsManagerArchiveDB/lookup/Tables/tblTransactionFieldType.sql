/*

	DROP TABLE [lookup].[tblTransactionFieldType]

*/
CREATE TABLE [lookup].[tblTransactionFieldType] (
    [TransactionFieldTypeIndex] INT                NOT NULL,
    [TransactionFieldTypeCode]  NVARCHAR (100)     NOT NULL,
    [TransactionFieldTypeName]  NVARCHAR (100)     NULL,
    [TransactionFieldTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]               DATETIMEOFFSET (7) NULL,
    [CreatedBy]                 [dbo].[udtUserID]  NULL,
    [UpdatedDate]               DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                 [dbo].[udtUserID]  NULL,
    [_RowVersion]               ROWVERSION         NOT NULL,
    [_ClusterIdx]               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblTransactionFieldType] PRIMARY KEY NONCLUSTERED ([TransactionFieldTypeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblTransactionFieldType_TransactionFieldTypeGuid]
    ON [lookup].[tblTransactionFieldType]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionFieldType_ClusterIdx]
    ON [lookup].[tblTransactionFieldType]([_ClusterIdx] ASC);