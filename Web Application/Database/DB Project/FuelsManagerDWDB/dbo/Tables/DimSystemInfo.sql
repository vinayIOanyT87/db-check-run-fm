CREATE TABLE [dbo].[DimSystemInfo](
	[SKey] [int] NOT NULL,
	[FirstLoadDate] [datetimeoffset](7) NULL,
	[LastLoadDate] [datetimeoffset](7) NULL,
	[LastLoadDateStr] [nvarchar](100) NULL,
	[DataWarehouseVersion] [nvarchar](100) NULL,
	[CDCActivationDate] [datetimeoffset](7) NULL,
	[ReportingDefaultSite] [nvarchar](100) NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_DimSystemInfo] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]