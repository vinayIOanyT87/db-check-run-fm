CREATE TABLE [dbo].[tblTransactionSignature] (
    [Signature]                VARBINARY (MAX)    NULL,
    [CreatedBy]                [dbo].[udtUserID]  NULL,
    [CreatedDate]              DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) NULL,
    [TransactionSignatureGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionSignature_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [TransactionGuid]          UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTransactionSignature_GUID] PRIMARY KEY NONCLUSTERED ([TransactionSignatureGuid] ASC)
);


GO

CREATE CLUSTERED INDEX [IX_tblTransactionSignature_CreatedDate]
    ON [dbo].[tblTransactionSignature]([CreatedDate] ASC);

GO
