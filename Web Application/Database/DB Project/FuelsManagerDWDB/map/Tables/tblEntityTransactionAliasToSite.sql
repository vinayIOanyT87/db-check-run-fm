CREATE TABLE [map].[tblEntityTransactionAliasToSite]
(
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[TransactionAliasToSiteKey] [nvarchar](50) NULL,
	[TransactionAliasKey] [nvarchar](50) NULL,
	[AssignedFromSiteSKey] [int] NULL DEFAULT(0),
	[SiteSKey] [int] NULL DEFAULT(0),
	[CreatedDate] [datetimeoffset](7) NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[EndedDate] [datetimeoffset](7) NULL,
	[EndedBy] [nvarchar](100) NULL,
	[_RowVersion] [timestamp] NULL
 CONSTRAINT [PK_map_tblEntityTransactionAliasToSite] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]