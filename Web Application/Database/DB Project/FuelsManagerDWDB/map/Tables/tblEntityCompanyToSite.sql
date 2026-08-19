/****************************************** 
map.tblEntityCompanyToSite 
********************************************/

CREATE TABLE [map].[tblEntityCompanyToSite]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[CompanyToSiteKey] [nvarchar](50) NULL,
[CompanyKey] [nvarchar](50) NULL,
[AssignedFromSiteSKey] [int] NULL DEFAULT(0),
[SiteSKey] [int] NULL DEFAULT(0),
[CreatedDate] [datetimeoffset](7) NULL,
[CreatedBy] [nvarchar](100) NULL,
[EndedDate] [datetimeoffset](7) NULL,
[EndedBy] [nvarchar](100) NULL,
[_RowVersion] [timestamp] NULL
 CONSTRAINT [PK_map_tblEntityCompanyToSite] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))