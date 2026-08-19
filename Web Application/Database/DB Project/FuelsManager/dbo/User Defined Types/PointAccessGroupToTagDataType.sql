CREATE TYPE [dbo].[PointAccessGroupToTagDataType] AS TABLE(
	[PointAccessGroupToTagGuid] [uniqueidentifier] NOT NULL,
	TagGuid [uniqueidentifier] NOT NULL,
	[PointAccessGroupGuid] [uniqueidentifier] NOT NULL,
	[View] bit NOT NULL,
	[Modify] bit NOT NULL,
	[ExceedRange] bit NOT NULL,
	[Override] bit NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)