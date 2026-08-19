/*

	DROP TABLE [staging].[tblTransactionTransportLineItems]

*/
CREATE TABLE [staging].[tblTransactionTransportLineItems] (
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
    [TransactionTransportLineItemGuid] UNIQUEIDENTIFIER   NULL,    
    [TransactionGuid]                  UNIQUEIDENTIFIER   NULL,
	[InventoryDateKey]				   INT                NULL,
	[ArchiveDate]					   DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]					   BIGINT			  NULL,
	[SourceClusterIdx]				   BIGINT			  NULL,
	[SourceRowVersion]				   BIGINT		      NULL,
	[IgnoreRecord]					   BIT				  NOT NULL,
	[IsProcessed]					   BIT				  NOT NULL,
	[_RowVersion]					   ROWVERSION	      NOT NULL,
	[SKey]							   INT				  IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblTransactionTransportLineItems_SKey] PRIMARY KEY CLUSTERED ([SKey] ASC)
);
GO


ALTER TABLE [staging].[tblTransactionTransportLineItems] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblTransactionTransportLineItems] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO
