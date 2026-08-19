/*

	DROP TABLE [staging].[tblEquipmentComparisonTemp]

*/
CREATE TABLE [staging].[tblEquipmentComparisonTemp](
	[SourceTable] [nvarchar](20) NOT NULL,
	[EquipmentSKey] [int] NOT NULL,
	[EquipmentKey] [nvarchar](50) NULL,
	[SiteKey] [nvarchar](50) NULL,
	[Id] [nvarchar](30) NULL,
	[EquipmentDescription] [nvarchar](50) NULL,
	[EquipmentTypeSKey] [int] NULL,
	[Make] [nvarchar](20) NULL,
	[Model] [nvarchar](50) NULL,
	[InUse] [bit] NULL,
	[SerialNumber] [nvarchar](30) NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,		
	[RecordChecksum] [int] NULL,
	[SKey] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblEquipmentComparisonTemp] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]