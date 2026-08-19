CREATE TABLE [staging].[tblEntityPersonnelToSite](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[PersonnelToSiteGuid] [uniqueidentifier] NULL,
	[PersonnelToSiteKey] [nvarchar](50) NULL,
	[PersonnelGuid] [uniqueidentifier] NULL,
	[PersonnelKey] [nvarchar](50) NULL,
	[PersonnelSKey] int NULL,
	[AssignedFromSiteGuid] [uniqueidentifier] NULL,
	[AssignedFromSiteKey] [nvarchar](50) NULL,
	[AssignedFromSiteSKey] int NULL,
	[SiteSKey] int NULL,
	[SiteGuid] [uniqueidentifier] NULL,
	[SiteKey] [nvarchar](50) NULL,
	[_ClusterIdx] [bigint] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[CreatedDate] [datetimeoffset](7) NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[CombinedUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NOT NULL,
	[IsRecordAddedByETL] [bit] NOT NULL,
	[IgnoreRecord] [bit] NOT NULL,
	[CDCSKey] [int] NULL,
	[SourceRowVersion] [bigint] NULL,
	[CDCRowVersion] [bigint] NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_map_tblEntityPersonnelToSite] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblEntityPersonnelToSite] ADD  DEFAULT ((0)) FOR [IsRecordDeleted]
GO
ALTER TABLE [staging].[tblEntityPersonnelToSite] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblEntityPersonnelToSite] ADD  DEFAULT ((0)) FOR [IgnoreRecord]