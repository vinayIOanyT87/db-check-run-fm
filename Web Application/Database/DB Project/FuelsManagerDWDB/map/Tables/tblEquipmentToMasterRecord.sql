/*
	DROP TABLE [map].[tblEquipmentToMasterRecord]
*/
CREATE TABLE [map].[tblEquipmentToMasterRecord](
	[EquipmentToMasterRecordSKey] [int] IDENTITY(1,1) NOT NULL,
	[EquipmentKey] [nvarchar](50) NULL,
	[MasterRecordKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_map_tblEquipmentToMasterRecord] PRIMARY KEY CLUSTERED 
(
	[EquipmentToMasterRecordSKey] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentToMasterRecord_EquipmentKey] ON [map].[tblEquipmentToMasterRecord]
(
	[EquipmentKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentToMasterRecord_MasterRecordKey] ON [map].[tblEquipmentToMasterRecord]
(
	[MasterRecordKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentToMasterRecord_SiteSKey] ON [map].[tblEquipmentToMasterRecord]
(
	[SiteSKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]