CREATE TYPE [dbo].[TransactionGuidAndSubLineItemSequenceListType] AS TABLE (
    [TransactionLineItemGuid] UNIQUEIDENTIFIER NOT NULL,
    [SequenceID]              SMALLINT         NOT NULL);

