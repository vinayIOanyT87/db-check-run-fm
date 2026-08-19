/*
	DROP TABLE [map].[tblProductToSiteRecordVersion]
*/
CREATE TABLE [map].[tblProductToSiteRecordVersion](
	[ProductToSiteSKey] [int] IDENTITY(1,1) NOT NULL,
	[ProductToSiteKey] [nvarchar](50) NULL,
	[ProductKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[AssignedFromSiteSKey] [int] NULL,
	[RecordVersionKey] [nvarchar](50) NULL,
	[RecordVersionSKey] int NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_map_tblProductToSiteRecordVersion] PRIMARY KEY CLUSTERED 
(
	[ProductToSiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblEntityProductToSiteRecordVersion_ProductKey] ON [map].[tblProductToSiteRecordVersion]
(
	[ProductKey] ASC
)
INCLUDE ( [RecordVersionKey] )
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblEntityProductToSiteRecordVersion_SiteSKey] ON [map].[tblProductToSiteRecordVersion]
(
	[SiteSKey] ASC
)
INCLUDE ( [AssignedFromSiteSKey] )
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]