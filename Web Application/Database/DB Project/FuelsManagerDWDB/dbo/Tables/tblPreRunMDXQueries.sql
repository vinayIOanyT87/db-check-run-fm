CREATE TABLE [dbo].[tblPreRunMDXQueries](
	[SKey] [int] NOT NULL,
	[QueryDefinition] [varchar](max) NOT NULL,
	[QueryDescription] [nvarchar](250) NULL,
	[PriorityLevel] [int] NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedBy] [nvarchar](100) NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblPreRunMDXQueries] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
