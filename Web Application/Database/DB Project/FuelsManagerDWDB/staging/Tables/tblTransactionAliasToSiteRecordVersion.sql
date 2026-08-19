CREATE TABLE [staging].[tblTransactionAliasToSiteRecordVersion](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[TransactionAliasToSiteKey] [nvarchar](50) NULL,
	[TransactionAliasKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[AssignedFromSiteSKey] [int] NULL,
	[RecordVersionKey] [nvarchar](50) NULL,
	[RecordVersionSKey] [int] NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_staging_tblTransactionAliasToSiteRecordVersion] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliasToSiteRecordVersion] ON [staging].[tblTransactionAliasToSiteRecordVersion]
(
	[TransactionAliasKey], [SiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)