/*

	DROP TABLE [staging].[tblCompanyComparisonTemp]

*/

CREATE TABLE [staging].[tblCompanyComparisonTemp](
	[SourceTable] [nvarchar](20) NOT NULL,
	[CompanySKey] [int]  NOT NULL,
	[CompanyKey] [nvarchar](50) NULL,
	[SiteKey] [nvarchar](50) NULL,
	[CompanyId] [nvarchar](100) NULL,
	[CompanyName] [nvarchar](100) NULL,
	[CompanyCode] [varchar](20) NULL,
	[Address1] [nvarchar](60) NULL,
	[Address2] [nvarchar](60) NULL,
	[City] [varchar](60) NULL,
	[State] [varchar](20) NULL,
	[Zip] [varchar](11) NULL,
	[Country] [varchar](50) NULL,
	[Phone] [nvarchar](20) NULL,
	[EmergencyContact] [nvarchar](30) NULL,
	[EmergencyPhone] [nvarchar](20) NULL,
	[LockedOut] [bit] NULL,
	[LockedOutReason] [nvarchar](80) NULL,
	[LockedOutDate] [datetimeoffset](7) NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,	
	[RecordChecksum] [int] NULL,
	[SKey] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblCompanyComparisonTemp] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]