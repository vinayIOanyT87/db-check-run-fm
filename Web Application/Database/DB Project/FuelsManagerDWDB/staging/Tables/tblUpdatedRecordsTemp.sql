/*
	DROP TABLE [staging].[tblUpdatedRecordsTemp]
*/
CREATE TABLE [staging].[tblUpdatedRecordsTemp](
	[UpdatedRecordSKey] [int] IDENTITY(1,1) NOT NULL,
	[RecordKey] [varchar](50) NULL,	
	[RecordSKey] [int] NULL,
	[IsNewRecord] [bit] NULL,
	[_RowVersion] [rowversion] NOT NULL,
 CONSTRAINT [PK_tblUpdatedRecordsTemp] PRIMARY KEY CLUSTERED 
(
	[UpdatedRecordSKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblUpdatedRecordsTemp_RecordKey] ON [staging].[tblUpdatedRecordsTemp]
(
	[RecordKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]