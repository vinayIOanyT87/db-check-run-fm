/*
	DROP TABLE [staging].[tblPartialTransactionSegment]
*/

CREATE TABLE [staging].[tblPartialTransactionSegment](
	[RunningKey] [int] IDENTITY(1,1) NOT NULL,
	[RecordKey] [nvarchar](50) NULL,
	[SegmentType] [nvarchar](100) NULL,
	[SourceFactTransactionSKey] [int] NULL,
	[IsNewMainSegment] [bit] NULL,
	[MissingSegmentType] [nvarchar](100) NULL,
	[InventoryDateChanged] [bit] NULL,
	[IsProcessed] [bit] NULL,
 CONSTRAINT [PK_staging_tblPartialTransactionSegment] PRIMARY KEY CLUSTERED 
(
	[RunningKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
