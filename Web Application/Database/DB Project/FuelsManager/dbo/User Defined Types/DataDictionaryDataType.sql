CREATE TYPE [dbo].[DataDictionaryDataType] AS TABLE(
	[Key] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
	[Value] [nvarchar](100) NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [dbo].[udtUserID] NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL
)