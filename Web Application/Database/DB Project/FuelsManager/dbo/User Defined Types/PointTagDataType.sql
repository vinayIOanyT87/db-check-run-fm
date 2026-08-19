CREATE TYPE [dbo].[PointTagDataType] AS TABLE
(
	[PointTagGuid] [uniqueidentifier] NOT NULL,
	[EngineeringUnitsType] [INT]	NULL,
	[EngineeringUnitsIndex]	[INT] NULL,
	[DecimalPlaces] [TINYINT]	NULL,
	[Maximum] [FLOAT]	NULL,
	[Minimum] [FLOAT]	NULL,
	[Value] [xml] NULL,
	[Status] [bigint] NULL,
	[ServerTimeStamp] [datetimeoffset](7) NULL,
	[SourceTimeStamp] [datetimeoffset](7) NULL
)
