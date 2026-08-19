CREATE TYPE [dbo].[PointAccessGroupToExposedSettingDataType] AS TABLE(
	[PointAccessGroupToExposedSettingGuid] [uniqueidentifier] NOT NULL,
	[ExposedSettingGuid] [uniqueidentifier] NOT NULL,
	[PropertyID] nvarchar( 60 ) NOT NULL,
	[PointAccessGroupGuid] [uniqueidentifier] NOT NULL,
	[ValueType] int NOT NULL,
	[View] bit NOT NULL,
	[Modify] bit NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)