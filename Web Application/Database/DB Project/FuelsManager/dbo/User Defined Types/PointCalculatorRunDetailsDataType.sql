CREATE TYPE [dbo].[PointCalculatorRunDetailsDataType] AS TABLE(
	[TagName] [nvarchar](50) NOT NULL,
	[Units] [nvarchar](50) NULL,
	[Acronym] [nvarchar](50) NULL,
	[BeginValue] [nvarchar](50) NOT NULL,
	[EndValue] [nvarchar](50) NOT NULL,
	[DiffValue] [nvarchar](50) NULL,
	[DisplayOrder] [int] NOT NULL
)