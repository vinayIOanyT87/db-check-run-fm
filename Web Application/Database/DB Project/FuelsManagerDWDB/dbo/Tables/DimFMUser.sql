CREATE TABLE [dbo].[DimFMUser](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[AKey] [nvarchar](50) NULL,
	[FMUserID] [nvarchar](100) NULL,
	[Name] [nvarchar](50) NOT NULL,
	[EmailAddress] [nvarchar](50) NULL,
	[InactivityLockout] [bit] NULL,
	[_RecordUpdatedDate] [datetimeoffset](7) NULL,
	[_DeletedFlag] [bit] NULL
 CONSTRAINT [PK_DimFMUser] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]