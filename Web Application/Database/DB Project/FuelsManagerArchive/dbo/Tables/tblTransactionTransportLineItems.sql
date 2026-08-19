CREATE TABLE [dbo].[tblTransactionTransportLineItems] (
    [TransportOrderNumber]             NVARCHAR (50)      CONSTRAINT [DF_tblTransactionTransportLineItems_TransportOrderNumber] DEFAULT ('') NOT NULL,
    [TransVersion]                     BIGINT             NULL,
    [LocationName]                     NVARCHAR (30)      NULL,
    [Address1]                         NVARCHAR (60)      NULL,
    [Address2]                         NVARCHAR (60)      NULL,
    [City]                             NVARCHAR (20)      NULL,
    [State]                            NVARCHAR (20)      NULL,
    [Zip]                              NVARCHAR (11)      NULL,
    [POCName]                          NVARCHAR (50)      NULL,
    [POCPhone]                         NVARCHAR (20)      NULL,
    [CreatedBy]                        [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionTransportLineItems_CreatedBy] DEFAULT ('') NOT NULL,
    [CreatedDate]                      DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionTransportLineItems_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                        [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionTransportLineItems_UpdatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                      DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionTransportLineItems_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TransactionTransportLineItemGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionTransportLineItems_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                      ROWVERSION         NOT NULL,
    [TransactionGuid]                  UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTransactionTransportLineItems_GUID] PRIMARY KEY NONCLUSTERED ([TransactionTransportLineItemGuid] ASC)
);


GO

CREATE CLUSTERED INDEX [IX_tblTransactionTransportLineItems_CreatedDate]
    ON [dbo].[tblTransactionTransportLineItems]([CreatedDate] ASC);

GO
