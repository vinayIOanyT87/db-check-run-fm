/*

	DROP TABLE [dbo].[tblTransactionNotes] 

*/
CREATE TABLE [dbo].[tblTransactionNotes] (
    [Notes]                 NVARCHAR (1000)		NULL,
    [CreatedBy]             [dbo].[udtUserID]	NULL,
    [CreatedDate]           DATETIMEOFFSET (7)	NULL,
    [UpdatedBy]             [dbo].[udtUserID]	NULL,
    [UpdatedDate]           DATETIMEOFFSET (7)	NULL,
    [AdditionalInformation] NVARCHAR (1000)		NULL,
    [TransactionNoteGuid]   UNIQUEIDENTIFIER	NOT NULL,    
    [TransactionGuid]       UNIQUEIDENTIFIER	NOT NULL,
	[InventoryDateKey]	    INT                NOT NULL,
	[ArchiveDate]           DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]			BIGINT			   NULL,
	[_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionNotes_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [TransactionNoteGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionNotes_ClusterIdx] 
	ON [dbo].[tblTransactionNotes]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionNotes_CreatedDate]
    ON [dbo].[tblTransactionNotes]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionNotes_TransactionGuid]
    ON [dbo].[tblTransactionNotes]([TransactionGuid] ASC)
    INCLUDE([TransactionNoteGuid])
	ON [AnnualPS]([InventoryDateKey]);
GO