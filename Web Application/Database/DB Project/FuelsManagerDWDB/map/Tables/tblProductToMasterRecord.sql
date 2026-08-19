/*
	DROP TABLE [map].[tblProductToMasterRecord]
*/
CREATE TABLE [map].[tblProductToMasterRecord](
	[ProductToMasterRecordSKey] [int] IDENTITY(1,1) NOT NULL,
	[ProductKey] [nvarchar](50) NULL,
	[MasterRecordKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_map_tblProductToMasterRecord] PRIMARY KEY CLUSTERED 
(
	[ProductToMasterRecordSKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblProductToMasterRecord_ProductKey] ON [map].[tblProductToMasterRecord]
(
	[ProductKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblProductToMasterRecord_MasterRecorKey] ON [map].[tblProductToMasterRecord]
(
	[MasterRecordKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblProductToMasterRecord_SiteSKey] ON [map].[tblProductToMasterRecord]
(
	[SiteSKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]