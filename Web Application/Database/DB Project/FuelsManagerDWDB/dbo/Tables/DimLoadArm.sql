CREATE TABLE [dbo].[DimLoadArm]
(
	[SKey] [int] IDENTITY(1,1) NOT NULL,

	[AKey] [nvarchar](50) NULL,
	[StationSKey] [int] NULL DEFAULT(0),		
	[ArmNumber] [int] NULL,	
	[SwingArm] [bit] NULL,
	[LoadRackText] [nvarchar](9) NULL,
	[BayId] [nvarchar](10) NULL,
	[_IsRecordAddedByETL] [bit] NOT NULL DEFAULT(0),	
	[_DeletedFlag] [bit] NULL,
	[_RecordUpdatedDate] [datetimeoffset](7) NULL
	CONSTRAINT [PK_DimLoadArm] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))
