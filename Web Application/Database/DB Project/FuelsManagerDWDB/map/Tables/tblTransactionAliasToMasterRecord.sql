/*
	DROP TABLE [map].[tblTransactionAliasToMasterRecord]
*/
CREATE TABLE [map].[tblTransactionAliasToMasterRecord](
	[TransactionAliasToMasterRecordSKey] [int] IDENTITY(1,1) NOT NULL,
	[TransactionAliasKey] [nvarchar](50) NULL,
	[MasterRecordKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_map_tblTransactionAliasToMasterRecord] PRIMARY KEY CLUSTERED 
(
	[TransactionAliasToMasterRecordSKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliasToMasterRecord_TransactionAliasKey] ON [map].[tblTransactionAliasToMasterRecord]
(
	[TransactionAliasKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliasToMasterRecord_MasterRecordKey] ON [map].[tblTransactionAliasToMasterRecord]
(
	[MasterRecordKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliasToMasterRecord_SiteSKey] ON [map].[tblTransactionAliasToMasterRecord]
(
	[SiteSKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]