CREATE TYPE [dbo].[PointAccessGroupToAlarmTestDataType] AS TABLE(
	PointAccessGroupToAlarmTestGuid [uniqueidentifier] NOT NULL,
	AlarmTestTemplateGuid [uniqueidentifier] NOT NULL,
	[PointAccessGroupGuid] [uniqueidentifier] NOT NULL,
	[View] bit NOT NULL,
	[Acknowledge] bit NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)