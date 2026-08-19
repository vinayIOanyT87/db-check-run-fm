CREATE TABLE [map].[tblSSASPartitionToRangeCriteria](
	[SKey] [int] NOT NULL,
	[DatabaseID] [varchar](50) NULL,
	[CubeID] [varchar](50) NULL,
	[MeasureGroupID] [varchar](50) NULL,
	[PartitionID] [varchar](50) NULL,
	[PartitionName] [varchar](50) NULL,
	[LowerRange] [int] NULL,
	[UpperRange] [int] NULL,
 CONSTRAINT [PK_tblSSASPartitionToRangeCriteria] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO