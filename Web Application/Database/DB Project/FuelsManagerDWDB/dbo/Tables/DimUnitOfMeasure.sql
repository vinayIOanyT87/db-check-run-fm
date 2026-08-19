CREATE TABLE [dbo].[DimUnitOfMeasure]
(
	[SKey] [int] NOT NULL,
	[UnitOfMeasureCode] [nvarchar](100) NULL,
	[UnitOfMeasureName] [nvarchar](100) NULL,
	[Description] [nvarchar](250) NULL,
	[VolumeSIToUnitConvFactor] [float] NULL,
	[MassSIToUnitConvFactor] [float] NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_DimUnitOfMeasure] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]