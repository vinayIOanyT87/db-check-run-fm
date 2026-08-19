CREATE TABLE [lookup].[tblLookup](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[LookupType] [nvarchar](50) NULL,
	[LookupIndex] [int] NOT NULL,
	[LookupCode] [nvarchar](100) NULL,
	[LookupName] [nvarchar](100) NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblLookup] PRIMARY KEY NONCLUSTERED 
(
	[RowIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
