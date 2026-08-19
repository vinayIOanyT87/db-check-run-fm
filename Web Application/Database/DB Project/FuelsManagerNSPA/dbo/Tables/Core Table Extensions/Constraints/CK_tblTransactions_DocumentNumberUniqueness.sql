ALTER TABLE [dbo].[tblTransactions]
	ADD CONSTRAINT [CK_tblTransactions_DocumentNumberUniqueness]
	CHECK (([dbo].[udf_IsDocumentNumberUnique]([TransactionGuid],[SiteGuid],[TransactionAliasGuid],[ReversalType],[ConjoinTransID],[TransID],[DocumentNumber],[DeleteFlag])=(1)))
