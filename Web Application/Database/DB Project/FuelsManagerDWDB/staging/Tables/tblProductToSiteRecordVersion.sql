CREATE TABLE [staging].[tblProductToSiteRecordVersion](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[ProductToSiteKey] [nvarchar](50) NULL,	
	[ProductKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[AssignedFromSiteSKey] [int] NULL,
	[RecordVersionKey] [nvarchar](50) NULL,
	[RecordVersionSKey] [int] NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_staging_tblProductToSiteRecordVersion] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblProductToSiteRecordVersion] ON [staging].[tblProductToSiteRecordVersion]
(
	[ProductKey], [SiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)