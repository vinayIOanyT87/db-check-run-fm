CREATE NONCLUSTERED INDEX [IX_tblTransactions_DocumentNumberUniquenessCheckSupport] ON tblTransactions
(
	[DocumentNumber] ASC,
	[TransID] ASC,
	[TransactionAliasGuid] ASC,
	[TransactionGuid] ASC,
	[SiteGuid] ASC,
	[ConjoinTransID] ASC,
	[ReversalType] ASC,
	[DeleteFlag] ASC
)