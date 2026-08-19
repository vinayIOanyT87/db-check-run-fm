/*
	DROP TABLE [map].[tblPersonnelToMasterRecord]
*/
CREATE TABLE [map].[tblPersonnelToMasterRecord](
	[PersonnelToMasterRecordSKey] [int] IDENTITY(1,1) NOT NULL,
	[PersonnelKey] [nvarchar](50) NULL,
	[MasterRecordKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_map_tblPersonnelToMasterRecord] PRIMARY KEY CLUSTERED 
(
	[PersonnelToMasterRecordSKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblPersonnelToMasterRecord_PersonnelKey] ON [map].[tblPersonnelToMasterRecord]
(
	[PersonnelKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblPersonnelToMasterRecord_MasterRecordKey] ON [map].[tblPersonnelToMasterRecord]
(
	[MasterRecordKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblPersonnelToMasterRecord_SiteSKey] ON [map].[tblPersonnelToMasterRecord]
(
	[SiteSKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]