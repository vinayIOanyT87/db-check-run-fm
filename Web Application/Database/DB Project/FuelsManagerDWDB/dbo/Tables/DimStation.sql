CREATE TABLE [dbo].[DimStation]
(
	[SKey] [int] IDENTITY(1,1) NOT NULL,

	[AKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL DEFAULT(0),
	[StationId] [nvarchar](30) NULL,
	[StationInterfaceTypeCode] [nvarchar](100) NULL,
	[_IsRecordAddedByETL] [bit] NOT NULL DEFAULT(0),	
	[_DeletedFlag] [bit] NULL,
	[_RecordUpdatedDate] [datetimeoffset](7) NULL
	CONSTRAINT [PK_DimStation] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))