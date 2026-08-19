CREATE TABLE [staging].[tblCompanyToSiteRecordVersion](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[CompanyToSiteKey] [nvarchar](50) NULL,	
	[CompanyKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[AssignedFromSiteSKey] [int] NULL,
	[RecordVersionKey] [nvarchar](50) NULL,
	[RecordVersionSKey] [int] NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_staging_tblCompanyToSiteRecordVersion] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblCompanyToSiteRecordVersion] ON [staging].[tblCompanyToSiteRecordVersion]
(
	[CompanyKey], [SiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)