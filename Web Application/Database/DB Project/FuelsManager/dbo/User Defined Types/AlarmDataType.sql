/****** Object:  UserDefinedTableType [dbo].[AlarmDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[AlarmDataType] AS TABLE(
	[AlarmGuid] [uniqueidentifier] NOT NULL,
	[InputTagGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[AlarmCategoryApplicationStringGuid] [uniqueidentifier] NOT NULL,
	[Order] [int] NOT NULL,
	[NotAlarmState] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](256) NULL,
	[ShelvedStartTimeStamp] [datetimeoffset](7) NULL,
	[ShelvedEndTimeStamp] [datetimeoffset](7) NULL,
	[ShelvedOneShot] [bit] NOT NULL,
	[ShelvedBy] [dbo].[udtUserID] NULL,
	[Suppressed] [bit] NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL,
	[AlarmStateTagGuid] [uniqueidentifier] NOT NULL,
	[ExclusiveAlarm] [bit] NOT NULL,
	[AlarmTemplateGuid] [uniqueidentifier] NULL,
	[Notify] [bit] NOT NULL
)