/*

	DROP TABLE [staging].[tblInsertedRecords]

*/
CREATE TABLE [staging].[tblInsertedRecords](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[TargetTableName] [nvarchar](100) NULL,
	[RecordGuid] [uniqueidentifier] NOT NULL,	
	[RecordIndex] [bigint] NOT NULL,
	[ParentRecordGuid] [uniqueidentifier] NULL
 CONSTRAINT [PK_tblInsertedRecords] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_tblInsertedRecords_1] ON [staging].[tblInsertedRecords]
(
	[TargetTableName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO