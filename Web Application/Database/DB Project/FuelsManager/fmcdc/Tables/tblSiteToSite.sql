CREATE TABLE [fmcdc].[tblSiteToSite](
	[SiteToSiteSKey] [int] IDENTITY(1,1) NOT NULL,
	[SiteToSiteGuid] [uniqueidentifier] NULL, 
	[ParentSiteGuid] [uniqueidentifier] NULL, 
	[ChildSiteGuid] [uniqueidentifier] NULL, 
	[CreatedDate] [datetimeoffset](7) NULL, 
	[CreatedBy] [udtUserID] NULL, 
	[UpdatedDate] [datetimeoffset](7) NULL, 
	[UpdatedBy] [udtUserID] NULL, 
	[SourceRowVersion] [bigint] NULL, 
	[_ClusterIdx] [bigint] NULL, 
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NULL,
	[_RowVersion] [timestamp] NOT NULL,
CONSTRAINT [PK_tblSiteToSite] PRIMARY KEY CLUSTERED
(
	[SiteToSiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO