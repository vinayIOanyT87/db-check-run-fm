CREATE TYPE [dbo].[PointCalculatorRunDataType] AS TABLE(
	[SiteId] [nvarchar](50) NOT NULL,
	[PointId] [nvarchar](50) NOT NULL,
	[CalculationMode] [nvarchar](50) NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[PointGuid] [uniqueidentifier] NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[Token] [uniqueidentifier] NOT NULL
)