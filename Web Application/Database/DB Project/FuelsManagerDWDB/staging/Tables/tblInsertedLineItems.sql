/*
	DROP TABLE [staging].[tblInsertedLineItems]
*/
CREATE TABLE [staging].[tblInsertedLineItems](
	[InsertedLineItemSKey] [int] IDENTITY(1,1) NOT NULL,
	[TransactionLineItemKey] nvarchar(50) NULL,
	[TransactionKey] nvarchar(50) NULL,
	[CombinedUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordAddedByETL] [bit] NOT NULL,
	[IsProcessed] [bit] NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblInsertedLineItems] PRIMARY KEY CLUSTERED 
(
	[InsertedLineItemSKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblInsertedLineItems] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblInsertedLineItems] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO
CREATE NONCLUSTERED INDEX [IX_tblInsertedLineItems_TransactionKey] ON [staging].[tblInsertedLineItems]
(
	[TransactionKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblInsertedLineItems_IsProcessed] ON [staging].[tblInsertedLineItems]
(
	[IsProcessed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]