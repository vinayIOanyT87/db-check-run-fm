/*

	DROP TABLE [dbo].[tblTransactionLinks]

*/
CREATE TABLE [dbo].[tblTransactionLinks] (
    [OriginalTransID]               NVARCHAR (64)      NULL,
    [LinkedTransID]                 NVARCHAR (64)      NULL,
    [Level]                         INT                NULL,
    [CreatedBy]                     [dbo].[udtUserID]  NULL,
    [CreatedDate]                   DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                     [dbo].[udtUserID]  NULL,
    [UpdatedDate]                   DATETIMEOFFSET (7) NULL,
    [TransactionLinkGuid]           UNIQUEIDENTIFIER   NOT NULL,    
    [SiteGuid]                      UNIQUEIDENTIFIER   NOT NULL,
    [LinkedTransactionLineItemGuid] UNIQUEIDENTIFIER   NULL,
    [TransactionLineItemGuid]       UNIQUEIDENTIFIER   NOT NULL,
	[InventoryDateKey]			    INT                NOT NULL,
	[ArchiveDate]                   DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]					BIGINT			   NULL,
	[_RowVersion]                   ROWVERSION         NOT NULL,
    [_ClusterIdx]                   BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionLinks_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [TransactionLinkGuid] ASC) ON [AnnualPS]([InventoryDateKey])
)
ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionLinks_CreatedDate]
    ON [dbo].[tblTransactionLinks]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionLinks_LinkedTransactionLineItemGuid]
    ON [dbo].[tblTransactionLinks]([LinkedTransactionLineItemGuid] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionLinks_LinkedTransID]
    ON [dbo].[tblTransactionLinks]([LinkedTransID] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionLinks_OriginalTransID_TransactionLineItemGuid]
    ON [dbo].[tblTransactionLinks]([OriginalTransID] ASC, [TransactionLineItemGuid] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionLinks_ClusterIdx]
    ON [dbo].[tblTransactionLinks]([InventoryDateKey] ASC, [_ClusterIdx] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO
