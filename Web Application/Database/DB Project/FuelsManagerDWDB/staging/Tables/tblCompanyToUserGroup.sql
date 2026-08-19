CREATE TABLE [staging].[tblCompanyToUserGroup](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[CompanyToUserGroupGuid] [uniqueidentifier] NULL,
	[CompanyToUserGroupKey] nvarchar(50) NULL,
	[CompanyGuid] [uniqueidentifier] NULL,
	[CompanyKey] nvarchar(50) NULL,
	[CompanySKey] [int] NULL,
	[GroupGuid] [uniqueidentifier] NULL,
	[GroupKey] nvarchar(50) NULL,
	[SiteGuid] [uniqueidentifier] NULL,
	[SiteKey] nvarchar(50) NULL,
	[SiteSKey] [int] NULL,
	[ID] [nvarchar](30) NULL,
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
 CONSTRAINT [PK_map_tblCompanyToUserGroup] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [staging].[tblCompanyToUserGroup] ADD  DEFAULT ((0)) FOR [IsRecordDeleted]
GO
ALTER TABLE [staging].[tblCompanyToUserGroup] ADD  DEFAULT ((0)) FOR [IsRecordAddedByETL]
GO
ALTER TABLE [staging].[tblCompanyToUserGroup] ADD  DEFAULT ((0)) FOR [IgnoreRecord]