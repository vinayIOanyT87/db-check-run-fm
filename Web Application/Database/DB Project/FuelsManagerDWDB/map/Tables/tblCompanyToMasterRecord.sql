/*
	DROP TABLE [map].[tblCompanyToMasterRecord]
*/
CREATE TABLE [map].[tblCompanyToMasterRecord](
	[CompanyToMasterRecordSKey] [int] IDENTITY(1,1) NOT NULL,
	[CompanyKey] [nvarchar](50) NULL,
	[MasterRecordKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_map_tblCompanyToMasterRecord] PRIMARY KEY CLUSTERED 
(
	[CompanyToMasterRecordSKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblCompanyToMasterRecord_CompanyKey] ON [map].[tblCompanyToMasterRecord]
(
	[CompanyKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblCompanyToMasterRecord_MasterRecordKey] ON [map].[tblCompanyToMasterRecord]
(
	[MasterRecordKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblCompanyToMasterRecord_SiteSKey] ON [map].[tblCompanyToMasterRecord]
(
	[SiteSKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]