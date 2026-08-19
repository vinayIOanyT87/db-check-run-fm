/*
	DROP TABLE [staging].[tblTransactionLineItemUserData]
*/
CREATE TABLE [staging].[tblTransactionLineItemUserData](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[TransactionLineItemUserDataGuid] [uniqueidentifier] NULL,
	[TransactionLineItemUserDataKey] [nvarchar](50) NULL,

	[TransactionLineItemGuid] [uniqueidentifier] NULL,
	[TransactionLineItemKey] [nvarchar](50) NULL,
	[UserData1] [nvarchar](60) NULL,
	[UserData2] [nvarchar](60) NULL,
	[UserData3] [nvarchar](60) NULL,
	[UserData4] [nvarchar](60) NULL,
	[UserData5] [nvarchar](60) NULL,
	[UserData5SI] [float] NULL,
	[UserData6] [nvarchar](60) NULL,
	[UserData6SI] [float] NULL,
	[UserData6USGallon] [float] NULL,
	[UserData7] [nvarchar](60) NULL,
	[UserData7SI] [float] NULL,
	[UserData7USGallon] [float] NULL,
	[UserData8] [nvarchar](60) NULL,
	[UserData9] [nvarchar](60) NULL,
	[UserData10] [nvarchar](60) NULL,
	[UserData11] [nvarchar](60) NULL,
	[UserData12] [nvarchar](60) NULL,
	[UserData13] [nvarchar](60) NULL,
	[UserData14] [nvarchar](60) NULL,
	[UserData15] [nvarchar](60) NULL,
	[UserData16] [nvarchar](60) NULL,
	[UserData17] [nvarchar](60) NULL,
	[UserData18] [nvarchar](60) NULL,
	[UserData19] [nvarchar](60) NULL,
	[UserData20] [nvarchar](60) NULL,
	[UserData21] [nvarchar](60) NULL,
	[UserData22] [nvarchar](60) NULL,
	[UserData23] [nvarchar](60) NULL,
	[UserData24] [nvarchar](60) NULL,
	[CreatedDate] [datetimeoffset](7) NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	--System Fields
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
 CONSTRAINT [PK_tblTransactionLineItemUserData] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
ALTER TABLE [staging].[tblTransactionLineItemUserData] ADD  DEFAULT ((0)) FOR [IsRecordDeleted]
GO
ALTER TABLE [staging].[tblTransactionLineItemUserData] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblTransactionLineItemUserData] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO
ALTER TABLE [staging].[tblTransactionLineItemUserData] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItemUserData_TransactionLineItemKey] ON [staging].[tblTransactionLineItemUserData]
(
	[TransactionLineItemGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItemUserData_TransactionLineItemUserDataKey] ON [staging].[tblTransactionLineItemUserData]
(
	[TransactionLineItemUserDataGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]