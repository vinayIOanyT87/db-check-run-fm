/****** Object:  UserDefinedTableType [dbo].[AnimationDataType]    Script Date: 12/22/2016 07:25:27 ******/
CREATE TYPE [dbo].[AnimationDataType] AS TABLE(
	[AnimationGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](50) NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[AnimationTestGroupList] [nvarchar](max) Not NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)