/************************************* 
map.tblUserToUserGroup 
*************************************/

CREATE TABLE [map].[tblUserToUserGroup]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[UserToUserGroupKey] [nvarchar](50) NULL,
[UserSKey] [int] NULL DEFAULT(0),
[UserGroupKey] [nvarchar](50) NULL,
[SiteSKey] [int] NULL DEFAULT(0),
[_DeletedFlag] [bit] NULL,
[_RecordUpdatedDate] [datetimeoffset](7) NULL
 CONSTRAINT [PK_map_tblUserToUserGroup] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))