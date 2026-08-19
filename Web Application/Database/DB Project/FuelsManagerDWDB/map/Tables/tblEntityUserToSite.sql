/************************************* 
map.tblEntityUserToSite 
****************************************/

CREATE TABLE [map].[tblEntityUserToSite]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[UserToSiteKey] [nvarchar](50) NULL,
[UserSKey] [int] NULL DEFAULT(0),
[AssignedFromSiteSKey] [int] NULL DEFAULT(0),
[SiteSKey] [int] NULL DEFAULT(0),
[_RecordUpdatedDate] [datetimeoffset](7) NULL,
[_DeletedFlag] [bit] NULL,
[_RowVersion] [timestamp] NULL
 CONSTRAINT [PK_map_tblEntityUserToSite] PRIMARY KEY NONCLUSTERED 
(
[SKey] ASC
))