/****** Object:  UserDefinedTableType [dbo].[AlarmTemplateDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[AlarmTemplateDataType] AS TABLE(
	[AlarmTemplateGuid] [uniqueidentifier] NOT NULL,
	[InputTemplateTagGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[Enabled] [Bit] Not NULL,
	[AlarmCategoryApplicationStringGuid] [uniqueidentifier] NOT NULL,
	[Order] [int] NOT NULL,
	[NotAlarmState] [nvarchar](100) NOT NULL,
	[Comment] [nvarchar](256) NULL,
	[ShelvedStartTimeStamp] [datetimeoffset](7) NULL,
	[ShelvedEndTimeStamp] [datetimeoffset](7) NULL,
	[ShelvedOneShot] [Bit]NOT NULL,
	[ShelvedBy] [dbo].[udtUserID]  NULL,
	[Suppressed] [Bit]NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL,
	[AlarmStateTemplateTagGuid] [uniqueidentifier] NOT NULL,
	[ExclusiveAlarm] BIT NOT NULL
)