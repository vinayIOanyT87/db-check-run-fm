CREATE TABLE [dbo].[DimTransactionAlias](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[AKey] [nvarchar](50) NULL,
	[MasterRecordKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NOT NULL DEFAULT(0),
	[AliasName] [nvarchar](32) NOT NULL,
	[TransactionTypeSKey] [int] NULL DEFAULT(0),
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,
	[_IsRecordAddedByETL] [bit] NOT NULL DEFAULT(0)
 CONSTRAINT [PK_DimTransactionAlias] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]