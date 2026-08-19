CREATE TABLE [fmcdc].[tblCompanyCompanyToUserGroup](
	[CompanyCompanyToUserGroupSKey] [int] IDENTITY(1,1) NOT NULL,
	[CompanyCompanyToUserGroupGuid] [uniqueidentifier] NULL, 
	[CompanyGuid] [uniqueidentifier] NULL, 
	[GroupGuid] [uniqueidentifier] NULL, 
	[SiteGuid] [uniqueidentifier] NULL, 
	[ID] [nvarchar](30) NULL, 
	[CreatedDate] [datetimeoffset](7) NULL, 
	[CreatedBy] [udtUserID] NULL, 
	[UpdatedDate] [datetimeoffset](7) NULL, 
	[UpdatedBy] [udtUserID] NULL, 
	[SourceRowVersion] [bigint] NULL, 
	[_ClusterIdx] [bigint] NULL, 
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NULL,
	[_RowVersion] [timestamp] NOT NULL,
CONSTRAINT [PK_tblCompanyCompanyToUserGroup] PRIMARY KEY CLUSTERED
(
	[CompanyCompanyToUserGroupSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO