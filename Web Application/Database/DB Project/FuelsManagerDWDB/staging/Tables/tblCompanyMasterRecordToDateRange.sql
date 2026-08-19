CREATE TABLE [staging].[tblCompanyMasterRecordToDateRange](
	[RowIndex] [int] IDENTITY(1,1) NOT NULL,
	[MasterRecordKey] [nvarchar](50) NULL,
	[StartDate] [datetimeoffset](7) NULL,
	[EndDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_tblCompanyMasterRecordToDateRange] PRIMARY KEY CLUSTERED 
(
	[RowIndex] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO
CREATE NONCLUSTERED INDEX [IX_tblCompanyMasterRecordToDateRange] ON [staging].[tblCompanyMasterRecordToDateRange]
(
	[MasterRecordKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)