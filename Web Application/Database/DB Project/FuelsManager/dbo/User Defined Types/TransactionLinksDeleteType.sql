CREATE TYPE [dbo].[TransactionLinksDeleteType] AS TABLE
(
	OriginalTransID NVARCHAR(64) NULL,
	LinkedTransID NVARCHAR(64) NOT NULL,
	TransactionLineItemGuid UNIQUEIDENTIFIER NULL,
	LinkedTransactionLineItemGuid UNIQUEIDENTIFIER NULL
)
