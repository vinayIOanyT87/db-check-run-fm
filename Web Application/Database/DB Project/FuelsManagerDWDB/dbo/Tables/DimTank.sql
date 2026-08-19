CREATE TABLE [dbo].[DimTank]
(
	[SKey] [int] IDENTITY(1,1) NOT NULL,

	[AKey] [nvarchar](50) NULL,
	[SiteSKey] [int] NULL DEFAULT(0),
	[TankId] [nvarchar](50) NULL,
	[VesselTypeName] [nvarchar](100) NULL,
	[_IsRecordAddedByETL] [bit] NOT NULL DEFAULT(0),	
	[_DeletedFlag] [bit] NULL,
	[_RecordUpdatedDate] [datetimeoffset](7) NULL
	CONSTRAINT [PK_Tank] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))