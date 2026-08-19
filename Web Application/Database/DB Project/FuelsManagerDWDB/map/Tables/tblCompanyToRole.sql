/***************************************** 
	[map].[tblCompanyToRole] 
****************************************/

CREATE TABLE [map].[tblCompanyToRole]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[CompanyToRoleKey] [nvarchar](50) NULL,
[CompanySKey] [int] NULL DEFAULT(0),
[LookupCompanyRoleIndex] [int] NULL,
[SiteSKey] [int] NULL DEFAULT(0),
[_RecordUpdatedDate] [datetimeoffset](7) NULL,
[_DeletedFlag] [bit] NULL,
[_RowVersion] [timestamp] NULL
 CONSTRAINT [PK_map_tblCompanyToRole] PRIMARY KEY NONCLUSTERED 
(
[SKey] ASC
))