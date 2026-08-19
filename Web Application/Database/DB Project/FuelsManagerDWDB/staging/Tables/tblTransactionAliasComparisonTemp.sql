/*

	DROP TABLE [staging].[tblTransactionAliasComparisonTemp]

*/

CREATE TABLE [staging].[tblTransactionAliasComparisonTemp](
	[SourceTable] [nvarchar](20) NOT NULL,
	[TransactionAliasSKey] [int] NOT NULL,
	[TransactionAliasKey] [nvarchar](50) NULL,
	[SiteKey] [nvarchar](50) NULL,
	[AliasName] [nvarchar](32) NOT NULL,
	[TransactionTypeSKey] [int] NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,	
	[RecordChecksum] [int] NULL,
	[SKey] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblTransactionAliasComparisonTemp] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]