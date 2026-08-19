CREATE TYPE [dbo].[TransactionLinksType] AS TABLE
(
	SiteGuid UNIQUEIDENTIFIER NOT NULL,
	OriginalTransID NVARCHAR(64) NOT NULL,
	LinkedTransID NVARCHAR(64) NOT NULL,
	[Level] INT NOT NULL,		
	TransactionLineItemGuid UNIQUEIDENTIFIER NOT NULL,
	LinkedTransactionLineItemGuid UNIQUEIDENTIFIER NOT NULL,
	CreatedUpdatedBy udtUserID NOT NULL
)
