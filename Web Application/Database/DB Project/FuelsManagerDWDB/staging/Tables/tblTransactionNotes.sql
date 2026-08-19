CREATE TABLE [staging].[tblTransactionNotes](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[TransactionNoteGuid] [uniqueidentifier] NULL,
	[TransactionNoteKey] [nvarchar](50) NULL,
	[TransactionGuid] [uniqueidentifier] NULL,
	[TransactionKey] [nvarchar](50) NULL,
	[Notes] [nvarchar](1000) NULL,
	[AdditionalInformation] [nvarchar](1000) NULL,	
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDate] [datetimeoffset](7) NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[CombinedUpdatedDate] [datetimeoffset](7) NULL,
	[CombinedUpdatedDateSKey] [int] NULL,
	[IsRecordDeleted] [bit] NOT NULL,
	[IsRecordAddedByETL] [bit] NOT NULL,
	[IgnoreRecord] [bit] NOT NULL,
	[IsProcessed] [bit] NOT NULL,
	[CDCSKey] [int] NULL,
	[SourceRowVersion] [bigint] NULL,
	[CDCRowVersion] [bigint] NULL,
	[_RowVersion] [timestamp] NOT NULL,		
 CONSTRAINT [PK_tblTransactionNotes] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblTransactionNotes] ADD  DEFAULT ((0)) FOR [IsRecordDeleted]
GO
ALTER TABLE [staging].[tblTransactionNotes] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblTransactionNotes] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO
ALTER TABLE [staging].[tblTransactionNotes] ADD  DEFAULT ((0)) FOR [IsProcessed]