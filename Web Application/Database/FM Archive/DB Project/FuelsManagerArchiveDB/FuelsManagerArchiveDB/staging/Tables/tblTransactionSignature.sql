/*

	DROP TABLE [staging].[tblTransactionSignature]

*/
CREATE TABLE [staging].[tblTransactionSignature] (
    [Signature]					VARBINARY (MAX)		NULL,
    [CreatedBy]					[dbo].[udtUserID]	NULL,
    [CreatedDate]				DATETIMEOFFSET (7)	NULL,
    [UpdatedBy]					[dbo].[udtUserID]	NULL,
    [UpdatedDate]				DATETIMEOFFSET (7)	NULL,
    [TransactionSignatureGuid]	UNIQUEIDENTIFIER	NULL,    
    [TransactionGuid]			UNIQUEIDENTIFIER	NULL,
	[InventoryDateKey]			INT                 NULL,
	[ArchiveDate]               DATETIMEOFFSET (7)  NULL,
	[ETLProcessKey]			    BIGINT			    NULL,
	[SourceClusterIdx]			BIGINT				NULL,
	[SourceRowVersion]			BIGINT				NULL,
	[IgnoreRecord]				BIT					NOT NULL,
	[IsProcessed]				BIT					NOT NULL,
	[_RowVersion]				ROWVERSION			NOT NULL,
	[SKey]						INT					IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblTransactionSignature_SKey] PRIMARY KEY CLUSTERED ([SKey] ASC)
);
GO


ALTER TABLE [staging].[tblTransactionSignature] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblTransactionSignature] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO