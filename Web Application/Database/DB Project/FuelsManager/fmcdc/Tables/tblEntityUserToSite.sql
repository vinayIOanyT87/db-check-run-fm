CREATE TABLE [fmcdc].[tblEntityUserToSite](
	[EntityUserToSiteSKey] [int] IDENTITY(1,1) NOT NULL,
	[UserToSiteGuid] [uniqueidentifier] NULL, 
	[UserGuid] [uniqueidentifier] NULL, 
	[SiteGuid] [uniqueidentifier] NULL, 
	[CreatedDate] [datetimeoffset](7) NULL, 
	[CreatedBy] [udtUserID] NULL, 
	[UpdatedDate] [datetimeoffset](7) NULL, 
	[UpdatedBy] [udtUserID] NULL, 
	[SourceRowVersion] [bigint] NULL, 
	[AssignedFromSiteGuid] [uniqueidentifier] NULL, 
	[_ClusterIdx] [bigint] NULL, 
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NULL,
	[_RowVersion] [timestamp] NOT NULL,
CONSTRAINT [PK_tblEntityUserToSite] PRIMARY KEY CLUSTERED
(
	[EntityUserToSiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO