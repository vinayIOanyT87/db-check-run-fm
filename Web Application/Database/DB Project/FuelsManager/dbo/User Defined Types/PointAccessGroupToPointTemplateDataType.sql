CREATE TYPE [dbo].[PointAccessGroupToPointTemplateDataType] AS TABLE(
	PointAccessGroupToPointTemplateGuid [uniqueidentifier] NOT NULL,
	PointAccessGroupGuid [uniqueidentifier] NOT NULL,
	PointTemplateGuid [uniqueidentifier] NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)