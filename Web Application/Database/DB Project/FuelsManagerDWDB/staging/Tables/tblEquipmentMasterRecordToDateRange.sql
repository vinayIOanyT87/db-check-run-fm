CREATE TABLE [staging].[tblEquipmentMasterRecordToDateRange](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[MasterRecordKey] [nvarchar](50) NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_tblEquipmentMasterRecordToDateRange] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentMasterRecordToDateRange] ON [staging].[tblEquipmentMasterRecordToDateRange]
(
	[MasterRecordKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)