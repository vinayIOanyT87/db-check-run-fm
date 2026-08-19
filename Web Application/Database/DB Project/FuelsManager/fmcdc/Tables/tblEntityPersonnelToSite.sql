CREATE TABLE [fmcdc].[tblEntityPersonnelToSite](
	[EntityPersonnelToSiteSKey] [int] IDENTITY(1,1) NOT NULL,
	[PersonnelToSiteGuid] [uniqueidentifier] NULL, 
	[PersonnelGuid] [uniqueidentifier] NULL, 
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
CONSTRAINT [PK_tblEntityPersonnelToSite] PRIMARY KEY CLUSTERED
(
	[EntityPersonnelToSiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO