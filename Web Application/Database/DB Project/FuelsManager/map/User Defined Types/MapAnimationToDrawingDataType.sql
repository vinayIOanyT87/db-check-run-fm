/****** Object:  UserDefinedTableType [map].[MapAnimtionatToDrawingDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [map].[MapAnimationToDrawingDataType] AS TABLE(
	[AnimationToDrawingGuid] [uniqueidentifier] NOT NULL,
	[AnimationGuid] [uniqueidentifier] NOT NULL,
	[DrawingGuid] [uniqueidentifier] NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)
