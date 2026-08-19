CREATE TYPE [dbo].[TransactionGuidAndTransVersionListType] AS TABLE
(
	TransactionGuid UNIQUEIDENTIFIER NOT NULL,
	TransVersion BIGINT NOT NULL
)
