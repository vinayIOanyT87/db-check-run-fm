/*********************************** 
	DimSite 
************************************/
CREATE TABLE [dbo].[DimSite]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[AKey] [nvarchar](50) NULL,
[SiteId] [nvarchar](30) NULL,
[SiteGroupFlag] [bit] NULL,
[Contact1Name] [nvarchar](30) NULL,
[Address1] [nvarchar](30) NULL,
[Address2] [nvarchar](30) NULL,
[City] [nvarchar](50) NULL,
[State] [nvarchar](20) NULL,
[Zip] [nvarchar](11) NULL,
[Country] [nvarchar](50) NULL,
[Phone] [nvarchar](20) NULL,
[TimeZone] [nvarchar](50) NULL,
[TemperatureDecimalPlaces] [tinyint] NULL,
[TemperatureUnitIndex] [int] NULL,
[DensityDecimalPlaces] [tinyint] NULL,
[DensityUnitIndex] [int] NULL,
[VolumeDecimalPlaces] [tinyint] NULL,
[VolumeUnitIndex] [int] NULL,
[Enabled] [bit] NULL,
[_RecordUpdatedDate] [datetimeoffset](7) NULL,
[_DeletedFlag] [bit] NULL
 CONSTRAINT [PK_DimSite] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))