/*
	DROP  TABLE [erv].[tblTempPersonnelToCompanyForParentPersonnel]
*/
CREATE TABLE [erv].[tblTempPersonnelToCompanyForParentPersonnel](
	[MappingIndex] [int] IDENTITY(1,1) NOT NULL,
	[TargetSiteGuid] [uniqueidentifier] NOT NULL,
	[CompanyGuid] [uniqueidentifier] NOT NULL,
	[CompanyMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[PersonnelGuid] [uniqueidentifier] NOT NULL,
	[PersonnelMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[PersonnelParentSiteGuid] [uniqueidentifier] NULL,
	[ParentPersonnelGuid] [uniqueidentifier] NULL,
	[CompanyParentSiteGuid] [uniqueidentifier] NULL,
	[CompanyGuidForParentPersonnel] [uniqueidentifier] NULL,
	[CarrierFCM] [nvarchar](20) NULL,
	[IsMasterRecordPersonnel] [bit] NULL,
	[PersonnelOwnsRecordAtAssignedFromSitegroup] [bit] NULL,
	[Processed] [bit] NULL,
	[_CallingReferenceGuid] [uniqueidentifier] NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblTempPersonnelToCompanyForParentPersonnel] PRIMARY KEY CLUSTERED 
(
	[MappingIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
