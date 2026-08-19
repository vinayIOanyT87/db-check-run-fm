/*

	DROP TABLE [staging].[tblTransactionLinks]

*/
CREATE TABLE [staging].[tblTransactionLinks] (
    [OriginalTransID]               NVARCHAR (64)		NULL,
    [LinkedTransID]                 NVARCHAR (64)		NULL,
    [Level]                         INT					NULL,
    [CreatedBy]                     [dbo].[udtUserID]	NULL,
    [CreatedDate]                   DATETIMEOFFSET (7)	NULL,
    [UpdatedBy]                     [dbo].[udtUserID]	NULL,
    [UpdatedDate]                   DATETIMEOFFSET (7)	NULL,
    [TransactionLinkGuid]           UNIQUEIDENTIFIER	NULL,    
    [SiteGuid]                      UNIQUEIDENTIFIER	NULL,
    [LinkedTransactionLineItemGuid] UNIQUEIDENTIFIER	NULL,
    [TransactionLineItemGuid]       UNIQUEIDENTIFIER	NULL,
	[InventoryDateKey]				INT                 NULL,
	[TransactionGuid]				UNIQUEIDENTIFIER	NULL,
	[ArchiveDate]					DATETIMEOFFSET (7)  NULL,
	[ETLProcessKey]					BIGINT			    NULL,
	[SourceClusterIdx]				BIGINT				NULL,
	[SourceRowVersion]				BIGINT				NULL,
	[IgnoreRecord]					BIT					NOT NULL,
	[IsProcessed]					BIT					NOT NULL,
	[_RowVersion]					ROWVERSION			NOT NULL,
	[SKey]							INT					IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblTransactionLinks_SKey] PRIMARY KEY CLUSTERED ([SKey] ASC));
GO


ALTER TABLE [staging].[tblTransactionLinks] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblTransactionLinks] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO