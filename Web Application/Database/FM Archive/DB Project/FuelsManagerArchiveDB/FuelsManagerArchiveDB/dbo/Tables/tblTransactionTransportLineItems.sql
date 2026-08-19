/*

	DROP TABLE [dbo].[tblTransactionTransportLineItems]

*/
CREATE TABLE [dbo].[tblTransactionTransportLineItems] (
    [TransportOrderNumber]             NVARCHAR (50)      NULL,
    [TransVersion]                     BIGINT             NULL,
    [LocationName]                     NVARCHAR (30)      NULL,
    [Address1]                         NVARCHAR (60)      NULL,
    [Address2]                         NVARCHAR (60)      NULL,
    [City]                             NVARCHAR (20)      NULL,
    [State]                            NVARCHAR (20)      NULL,
    [Zip]                              NVARCHAR (11)      NULL,
    [POCName]                          NVARCHAR (50)      NULL,
    [POCPhone]                         NVARCHAR (20)      NULL,
    [CreatedBy]                        [dbo].[udtUserID]  NULL,
    [CreatedDate]                      DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                        [dbo].[udtUserID]  NULL,
    [UpdatedDate]                      DATETIMEOFFSET (7) NULL,
    [TransactionTransportLineItemGuid] UNIQUEIDENTIFIER   NOT NULL,    
    [TransactionGuid]                  UNIQUEIDENTIFIER   NOT NULL,
	[InventoryDateKey]				   INT                NOT NULL,
	[ArchiveDate]					   DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]					   BIGINT			  NULL,
	[_RowVersion]                      ROWVERSION         NOT NULL,
    [_ClusterIdx]                      BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionTransportLineItems_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [TransactionTransportLineItemGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionTransportLineItems_ClusterIdx] 
	ON [dbo].[tblTransactionTransportLineItems]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionTransportLineItems_CreatedDate]
    ON [dbo].[tblTransactionTransportLineItems]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionTransportLineItems_TransactionGuid_TransportOrderNumber]
    ON [dbo].[tblTransactionTransportLineItems]([TransactionGuid] ASC, [TransportOrderNumber] ASC)
    INCLUDE([TransactionTransportLineItemGuid])
	ON [AnnualPS]([InventoryDateKey]);
GO
