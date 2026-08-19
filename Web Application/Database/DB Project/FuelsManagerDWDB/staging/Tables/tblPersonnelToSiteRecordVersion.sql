CREATE TABLE [staging].[tblPersonnelToSiteRecordVersion](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[PersonnelToSiteKey] [nvarchar](50) NULL,
	[PersonnelKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[AssignedFromSiteSKey] [int] NULL,
	[RecordVersionKey] [nvarchar](50) NULL,
	[RecordVersionSKey] [int] NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_staging_tblPersonnelToSiteRecordVersion] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblPersonnelToSiteRecordVersion] ON [staging].[tblPersonnelToSiteRecordVersion]
(
	[PersonnelKey], [SiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)