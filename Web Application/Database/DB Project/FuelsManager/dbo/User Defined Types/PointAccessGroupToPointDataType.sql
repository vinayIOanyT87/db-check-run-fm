CREATE TYPE [dbo].[PointAccessGroupToPointDataType] AS TABLE(
	PointAccessGroupToPointGuid [uniqueidentifier] NOT NULL,
	PointAccessGroupGuid [uniqueidentifier] NOT NULL,
	PointGuid [uniqueidentifier] NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)