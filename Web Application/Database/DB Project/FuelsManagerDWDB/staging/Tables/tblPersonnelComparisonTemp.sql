/*

	DROP TABLE [staging].[tblPersonnelComparisonTemp]

*/

CREATE TABLE [staging].[tblPersonnelComparisonTemp](
	[SourceTable] [nvarchar](20) NOT NULL,
	[PersonnelSKey] [int] NOT NULL,
	[PersonnelKey] [nvarchar](50) NULL,
	[SiteKey] nvarchar(50) NULL,
	[PersonID] [nvarchar](50) NULL,
	[FirstName] [nvarchar](20) NULL,
	[MiddleName] [nvarchar](20) NULL,
	[LastName] [nvarchar](30) NULL,
	[LockedOut] [bit] NULL,
	[LockedOutReason] [nvarchar](80) NULL,
	[LockedOutDate] [datetimeoffset](7) NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,	
	[RecordChecksum] [int] NULL,
	[SKey] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_tblPersonnelComparisonTemp] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]