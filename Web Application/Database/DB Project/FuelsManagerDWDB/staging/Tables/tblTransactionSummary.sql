/*
	DROP TABLE [staging].[tblTransactionSummary]
*/
CREATE TABLE [staging].[tblTransactionSummary](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[TransactionKey] [nvarchar](50) NULL,
	
	[BillToCode] [nvarchar](10) NULL,
	[BillToCompanyKey] [nvarchar](50) NULL,
	[BillToCompanySKey] [int] NULL,
	[CarrierCompanyKey] [nvarchar](50) NULL,
	[CarrierCompanySKey] [int] NULL,
	[DeleteFlag] [bit] NULL,
	[DestinationEquipment1Key] [nvarchar](50) NULL,
	[DestinationEquipment1SKey] [int] NULL,	
	[DocumentNumber] [nvarchar](30) NULL,
	[InventoryDate] [datetimeoffset](7) NULL,
	[InventoryDateSKey] [int] NULL,
	[Line_MeterMaxStopTime] [datetimeoffset](7) NULL,
	[Line_MeterMinStartTime] [datetimeoffset](7) NULL,
	[Line_MeterMinStartMaxStopTimeDiff] [int] NULL,
	[Line_MaxMeterStopTimeOutDiff] [int] NULL,
	[Line_TimeInMinMeterStartDiff] [int] NULL,
	[ManagerCompanyKey] [nvarchar](50) NULL,
	[ManagerCompanySKey] [int] NULL,
	[OperatorPersonnelKey] [nvarchar](50) NULL,
	[OperatorPersonnelSKey] [int] NULL,	
	[OwnerCompanyKey] [nvarchar](50) NULL,
	[OwnerCompanySKey] [int] NULL,
	[ReasonCodeKey] [nvarchar](50) NULL,
	[ReasonCodeSKey] [int] NULL,	
	[ReversalType] [nvarchar](2) NULL,
	[ShipperCompanyKey] [nvarchar](50) NULL,
	[ShipperCompanySKey] [int] NULL,
	[ShipToCompanyKey] [nvarchar](50) NULL,
	[ShipToCompanySKey] [int] NULL,
	[SiteKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,
	[SourceEquipment1Key] [nvarchar](50) NULL,
	[SourceEquipment1SKey] [int] NULL,	
	[SubType] [nvarchar](20) NULL,
	[SupplierCompanyKey] [nvarchar](50) NULL,
	[SupplierCompanySKey] [int] NULL,
	[TimeIn] [datetimeoffset](7) NULL,
	[TimeInDateSKey] [int] NULL,
	[TimeInTimeSKey] [int] NULL,
	[TimeInTimeOutDiff] [int] NULL,
	[TimeOut] [datetimeoffset](7) NULL,
	[TimeOutDateSKey] [int] NULL,
	[TimeOutTimeSKey] [int] NULL,
	[TransactionAliasKey] [nvarchar](50) NULL,
	[TransactionAliasSKey] [int] NULL,	
	[TransactionStatusIndex] [int] NULL,
	[TransactionStatusName] [nvarchar](100) NULL,
	[TransDateSKey] [int] NULL,
	[TransDateTime] [datetimeoffset](7) NULL,
	[TransID] [nvarchar](64) NULL,
	[TransTimeSKey] [int] NULL,
	[TransactionTypeKey] [nvarchar](50) NULL,
	[TransactionTypeSKey] [int] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,

	--System Fields
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[CombinedUpdatedDate] [datetimeoffset](7) NULL,
	[CombinedUpdatedDateSKey] [int] NULL,
	[IsRecordDeleted] [bit] NOT NULL,
	[IsRecordAddedByETL] [bit] NOT NULL,
	[SourceFactSKey] [int] NULL,
	[IgnoreRecord] [bit] NOT NULL,
	[IsProcessed] [bit] NOT NULL,
	[CDCSKey] [int] NULL,
	[SourceRowVersion] [bigint] NULL,
	[CDCRowVersion] [bigint] NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblTransactionSummary] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblTransactionSummary] ADD  DEFAULT ((0)) FOR [IsRecordDeleted]
GO
ALTER TABLE [staging].[tblTransactionSummary] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblTransactionSummary] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO
ALTER TABLE [staging].[tblTransactionSummary] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionSummary_TransactionKey] ON [staging].[tblTransactions]
(
	[TransactionKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionSummary_IgnoreRecord] ON [staging].[tblTransactions]
(
	[IgnoreRecord] ASC
)
INCLUDE ( [IsProcessed] )
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionSummary_IsProcessed] ON [staging].[tblTransactions]
(
	[IsProcessed] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionSummary_TransId] ON [staging].[tblTransactions]
(
	[TransID] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
