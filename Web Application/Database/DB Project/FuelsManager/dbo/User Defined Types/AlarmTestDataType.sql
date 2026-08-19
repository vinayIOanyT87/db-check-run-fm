/****** Object:  UserDefinedTableType [dbo].[AlarmTestDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[AlarmTestDataType] AS TABLE(
	[AlarmTestGuid] [uniqueidentifier] NOT NULL,
	[AlarmGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[LimitTagGuid] [uniqueidentifier] NOT NULL,
	[TagField] INT NOT NULL,
	[AlarmPriorityGuid] [uniqueidentifier] NOT NULL,
	[NormalUnacknowledgedAlarmPriorityGuid] [uniqueidentifier] NOT NULL,
	[TestType] [int] NOT NULL,  -- this is an enum for the different comparison types. See slide 5
	[BitMask] BIGINT NOT NULL, 
	[Enabled] [Bit] Not NULL,
	[Order] [int] NOT NULL,
	[AlarmState] [nvarchar](100) NOT NULL,
	[Holdoff] [float] NOT NULL,  -- between 0 and 1 a percentage of the delta between the tag Max and Min.
	[AlarmText] [nvarchar](256) NULL,
	[HelpFile] [nvarchar](Max) NULL, 	
	[DrawingGuid] [uniqueidentifier] NULL,	
	[UpdatedBy] [dbo].[udtUserID] NOT NULL,
	[BitwiseOperator] [int] NOT NULL,  
	[TimedHoldOffInSeconds] [int] NOT NULL,
	[AlarmTestTemplateGuid] [uniqueidentifier] NULL
)