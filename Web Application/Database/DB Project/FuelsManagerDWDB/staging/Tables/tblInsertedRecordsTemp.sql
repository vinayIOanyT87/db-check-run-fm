/*
	DROP TABLE [staging].[tblInsertedRecordsTemp]
*/
CREATE TABLE [staging].[tblInsertedRecordsTemp](
	[InsertedRecordSKey] [int] IDENTITY(1,1) NOT NULL,
	[RecordKey] [varchar](50) NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblInsertedRecordsTemp] PRIMARY KEY CLUSTERED 
(
	[InsertedRecordSKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblInsertedRecordsTemp_RecordKey] ON [staging].[tblInsertedRecordsTemp]
(
	[RecordKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]