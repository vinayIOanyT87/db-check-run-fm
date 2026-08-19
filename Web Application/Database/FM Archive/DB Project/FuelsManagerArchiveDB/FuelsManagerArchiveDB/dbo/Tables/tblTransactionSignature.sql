/*

	DROP TABLE [dbo].[tblTransactionSignature]

*/
CREATE TABLE [dbo].[tblTransactionSignature] (
    [Signature]                VARBINARY (MAX)    NULL,
    [CreatedBy]                [dbo].[udtUserID]  NULL,
    [CreatedDate]              DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                [dbo].[udtUserID]  NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) NULL,
    [TransactionSignatureGuid] UNIQUEIDENTIFIER   NOT NULL,    
    [TransactionGuid]          UNIQUEIDENTIFIER   NOT NULL,
	[InventoryDateKey]	       INT                NOT NULL,
	[ArchiveDate]              DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]			   BIGINT			  NULL,
	[_RowVersion]              ROWVERSION         NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionSignature_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [TransactionSignatureGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionSignature_ClusterIdx] 
	ON [dbo].[tblTransactionSignature]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionSignature_CreatedDate]
    ON [dbo].[tblTransactionSignature]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionSignature_TransactionGuid]
    ON [dbo].[tblTransactionSignature]([TransactionGuid] ASC)
    INCLUDE([TransactionSignatureGuid])
	ON [AnnualPS]([InventoryDateKey]);
GO
