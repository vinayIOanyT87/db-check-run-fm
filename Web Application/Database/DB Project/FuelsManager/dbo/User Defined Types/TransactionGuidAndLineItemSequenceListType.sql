CREATE TYPE [dbo].[TransactionGuidAndLineItemSequenceListType] AS TABLE (
    [TransactionGuid] UNIQUEIDENTIFIER NOT NULL,
    [SequenceID]      SMALLINT         NOT NULL);

