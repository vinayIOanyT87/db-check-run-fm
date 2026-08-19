/************************************ 
	[dbo].[DimCompany] 
*************************************/

CREATE TABLE [dbo].[DimCompany]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[AKey] [nvarchar](50) NULL,
[MasterRecordKey] [nvarchar](50) NULL,
[SiteSKey] [int] NOT NULL DEFAULT(0),
[CompanyId] [nvarchar](100) NULL,
[Name] [nvarchar](100) NULL,
[Code] [varchar](10) NULL,
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
[_IsRecordAddedByETL] [bit] NOT NULL DEFAULT(0)
 CONSTRAINT [PK_DimCompany] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))