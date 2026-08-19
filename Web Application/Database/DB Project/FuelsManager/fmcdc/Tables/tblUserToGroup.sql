CREATE TABLE [fmcdc].[tblUserToGroup](
	[UserToGroupSKey] [int] IDENTITY(1,1) NOT NULL,
	[UserToGroupGuid] [uniqueidentifier] NULL, 
	[UserGuid] [uniqueidentifier] NULL, 
	[GroupGuid] [uniqueidentifier] NULL, 
	[ExpirationDate] [datetime] NULL, 
	[CreatedDate] [datetimeoffset](7) NULL, 
	[CreatedBy] [udtUserID] NULL, 
	[UpdatedDate] [datetimeoffset](7) NULL, 
	[UpdatedBy] [udtUserID] NULL, 
	[SourceRowVersion] [bigint] NULL, 
	[SiteGuid] [uniqueidentifier] NULL, 
	[DenyADPermission] [bit] NULL, 
	[_ClusterIdx] [bigint] NULL, 
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NULL,
	[_RowVersion] [timestamp] NOT NULL,
CONSTRAINT [PK_tblUserToGroup] PRIMARY KEY CLUSTERED
(
	[UserToGroupSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO