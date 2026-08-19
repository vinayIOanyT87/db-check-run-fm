/*

	DROP TABLE [staging].[tblEntityChecksum]

*/

CREATE TABLE [staging].[tblEntityChecksum](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[EntitySKey] [int] NULL,
	[EntityKey] [nvarchar](50) NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[RecordChecksum] [int] NULL,
	[RecordPreviousChecksum] [int] NULL,
	[DimChecksum] [int] NULL,
	[IgnoreRecord] [bit] NOT NULL
 CONSTRAINT [PK_tblEntityChecksum] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblEntityChecksum] ADD  DEFAULT ((0)) FOR [IgnoreRecord]