CREATE TABLE [staging].[tblTransactionAliasToSiteRecordVersionTemp](
	[TransactionAliasToSiteRecordVersionSKey] [int] IDENTITY(1,1) NOT NULL,
	[TransactionAliasToSiteKey] [nvarchar](50) NULL,	
	[TransactionAliasKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[AssignedFromSiteSKey] [int] NULL,
	[RunningSiteSKey] [int] NULL,	
	[RunningAssignedFromSiteSKey] [int] NULL,
	[RecordVersionKey] [nvarchar](50) NULL,
	[RecordVersionSKey] [int] NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
	[RecordVersionSequenceBreakIndex] [int] NULL,
 CONSTRAINT [PK_staging_tblTransactionAliasToSiteRecordVersionTemp] PRIMARY KEY CLUSTERED 
(
	[TransactionAliasToSiteRecordVersionSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
