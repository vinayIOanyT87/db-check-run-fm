/****** Object:  UserDefinedTableType [dbo].[PointTagAlarmStatusDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[PointTagAlarmStatusDataType] AS TABLE(
	[PointTagAlarmStatusGuid] [uniqueidentifier] NOT NULL,
	[AlarmTestGuid] [uniqueidentifier] NOT NULL,
	[Acknowledged] [Bit] Not NULL,
	[AcknowledgedTimestamp] [datetimeoffset](7) NULL,
	[AcknowledgedBy] [dbo].[udtUserID] NULL,
	[AcknowledgedComment] [nvarchar](MAX) NULL,
	[Silenced] [Bit] Not NULL DEFAULT (0),
	[SilencedTimestamp] [datetimeoffset](7) NULL,
	[SilencedBy] [dbo].[udtUserID] NULL,
	[AlarmTestFailed] [Bit] Not NULL,
	[AlarmTestFailedTimestamp] [datetimeoffset](7) NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)