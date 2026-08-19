CREATE TABLE [staging].[tblEquipmentToSiteRecordVersion](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[EquipmentToSiteKey] [nvarchar](50) NULL,	
	[EquipmentKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL,	
	[AssignedFromSiteSKey] [int] NULL,
	[RecordVersionKey] [nvarchar](50) NULL,
	[RecordVersionSKey] [int] NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_staging_tblEquipmentToSiteRecordVersion] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentToSiteRecordVersion] ON [staging].[tblEquipmentToSiteRecordVersion]
(
	[EquipmentKey], [SiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)