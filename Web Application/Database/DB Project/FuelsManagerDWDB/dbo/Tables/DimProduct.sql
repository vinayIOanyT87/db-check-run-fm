/******************************* 
	dbo.DimProduct 
*******************************/

CREATE TABLE [dbo].[DimProduct]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[AKey] [nvarchar](50) NULL,
[MasterRecordKey] [nvarchar](50) NULL,
[SiteSKey] [int] NOT NULL DEFAULT(0),
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
[_IsRecordAddedByETL] [bit] NOT NULL DEFAULT(0)
CONSTRAINT [PK_DimProduct] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))