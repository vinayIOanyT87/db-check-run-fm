/*

	DROP TABLE [archive].[tblTransactionLastProcessedRecords]

*/
CREATE TABLE [archive].[tblTransactionLastProcessedRecords](
	[SKey]					BIGINT IDENTITY(1,1) NOT NULL,
    [SourceArchiveTable]	NVARCHAR(100) NOT NULL,
	[RecordGuid]			UNIQUEIDENTIFIER NOT NULL,
	[ParentRecordGuid]		UNIQUEIDENTIFIER NULL,
    [RecordIndex]			BIGINT NOT NULL,
	[ProcessType]			VARCHAR(50) NULL,
	[IsProcessed]			BIT NOT NULL,
	[CreatedDate]			DATETIMEOFFSET(7) NOT NULL

    CONSTRAINT [PK_tblTransactionLastProcessedRecords] PRIMARY KEY CLUSTERED ([SKey] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionProcessedRecords_1] ON [archive].[tblTransactionLastProcessedRecords]
(
	[SourceArchiveTable] ASC,
	[RecordIndex] ASC
)
INCLUDE([RecordGuid]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [archive].[tblTransactionLastProcessedRecords] ADD  DEFAULT (GETDATE()) FOR [CreatedDate]
GO

ALTER TABLE [archive].[tblTransactionLastProcessedRecords] ADD  DEFAULT (0) FOR [IsProcessed]
GO
