/*

	DROP TABLE [staging].[tblProductComparisonTemp]

*/

CREATE TABLE [staging].[tblProductComparisonTemp](
	[SourceTable] [nvarchar](20) NOT NULL,
	[ProductSKey] [int] NOT NULL,
	[ProductKey] [nvarchar](50) NULL,
	[SiteKey] [nvarchar](50) NULL,
	[ProductId] [nvarchar](30) NULL,	
	[ProductCode] [nvarchar](15) NULL,
	[Description] [nvarchar](50) NULL,
	[ProductTypeName] [nvarchar](100) NULL,
	[TrackingProductSKey] [int] NULL,
	[TrackingProductId] [nvarchar](30) NULL,
	[VolumeDecimalPlaces] [tinyint] NULL,
	[AviationFuelFlag] [bit] NULL,
	[GroundFuel] [bit] NULL,
	[LockedOut] [bit] NULL,
	[LockedOutReason] [nvarchar](80) NULL,
	[LockedOutDate] [datetimeoffset](7) NULL,
	[VarianceTolerance] [float] NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,	
	[RecordChecksum] [int] NULL,
	[SKey] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblProductComparisonTemp] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]