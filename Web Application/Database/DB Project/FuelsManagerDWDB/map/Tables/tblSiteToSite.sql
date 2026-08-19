/************************************ 
map.tblSiteToSite  
************************************/

CREATE TABLE [map].[tblSiteToSite]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[SiteToSiteKey] [nvarchar](50) NULL,
[ParentSiteSKey] [int] NULL DEFAULT(0),
[ChildSiteSKey] [int] NULL DEFAULT(0),
[_RecordUpdatedDate] [datetimeoffset](7) NULL,
[_DeletedFlag] [bit] NULL,
[_RowVersion] [timestamp] NULL
 CONSTRAINT [PK_map_tblSiteToSite] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))