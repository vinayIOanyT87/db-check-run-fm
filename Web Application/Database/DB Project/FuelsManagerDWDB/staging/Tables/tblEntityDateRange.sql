CREATE TABLE [staging].[tblEntityDateRange](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[EntitySKey] [int] NULL,
	[EntityKey] [nvarchar](50) NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_tblEntityDateRange] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO