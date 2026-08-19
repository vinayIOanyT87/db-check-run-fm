CREATE TYPE [dbo].[PointAccessGroupToUserGroupDataType] AS TABLE(
	[PointAccessGroupToUserGroupGuid] [uniqueidentifier] NOT NULL,
	[PointAccessGroupGuid] [uniqueidentifier] NOT NULL,
	[UserGroupGuid] [uniqueidentifier] NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)