CREATE TYPE [dbo].[PointAccessGroupToPointAlarmTestDataType] AS TABLE(
	PointAccessGroupToPointAlarmTestGuid [uniqueidentifier] NOT NULL,
	AlarmTestGuid [uniqueidentifier] NOT NULL,
	[PointAccessGroupGuid] [uniqueidentifier] NOT NULL,
	[View] bit NOT NULL,
	[Acknowledge] bit NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)