/******************************* 
	[dbo].[DimPersonnel] 
*******************************/

CREATE TABLE [dbo].[DimPersonnel]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[AKey] [nvarchar](50) NULL,
[MasterRecordKey] [nvarchar](50) NULL,
[SiteSKey] [int] NOT NULL DEFAULT(0),
[PersonID] [nvarchar](50) NULL,
[FirstName] [nvarchar](20) NULL,
[MiddleName] [nvarchar](20) NULL,
[LastName] [nvarchar](30) NULL,
[LockedOut] [bit] NULL,
[LockedOutReason] [nvarchar](80) NULL,
[LockedOutDate] [datetimeoffset](7) NULL,
[StartDate] [datetimeoffset](7) NOT NULL,
[EndDate] [datetimeoffset](7) NULL,
[_IsRecordAddedByETL] [bit] NOT NULL DEFAULT(0)
 CONSTRAINT [PK_DimPersonnel] PRIMARY KEY CLUSTERED 
([SKey] ASC
))