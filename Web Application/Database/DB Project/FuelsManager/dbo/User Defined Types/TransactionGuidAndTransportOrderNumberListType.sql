CREATE TYPE [dbo].[TransactionGuidAndTransportOrderNumberListType] AS TABLE (
    [TransactionGuid]      UNIQUEIDENTIFIER NOT NULL,
    [TransportOrderNumber] NVARCHAR (50)    NOT NULL);

