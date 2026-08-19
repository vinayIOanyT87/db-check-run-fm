CREATE TABLE [staging].[tblPersonnelMasterRecordToDateRange](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[MasterRecordKey] [nvarchar](50) NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_tblPersonnelMasterRecordToDateRange] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblPersonnelMasterRecordToDateRange] ON [staging].[tblPersonnelMasterRecordToDateRange]
(
	[MasterRecordKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)