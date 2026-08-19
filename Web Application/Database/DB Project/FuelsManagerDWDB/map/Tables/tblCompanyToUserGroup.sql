CREATE TABLE [map].[tblCompanyToUserGroup]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[CompanyToUserGroupKey] [nvarchar](50) NULL,
[CompanySKey] [int] NULL DEFAULT(0),
[CompanyKey] [nvarchar](50) NULL,
[UserGroupKey] [nvarchar](50) NULL,
[SiteSKey] [int] NULL DEFAULT(0),
[ID] [nvarchar](30) NULL,
[_DeletedFlag] [bit] NULL,
[_RecordUpdatedDate] [datetimeoffset](7) NULL,
[_RowVersion] [timestamp] NULL
 CONSTRAINT [PK_map_tblCompanyToUserGroup] PRIMARY KEY NONCLUSTERED 
(
[SKey] ASC
))