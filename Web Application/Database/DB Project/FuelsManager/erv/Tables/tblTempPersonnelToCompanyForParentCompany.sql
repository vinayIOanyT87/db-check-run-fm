/*
	DROP TABLE [erv].[tblTempPersonnelToCompanyForParentCompany]
*/
CREATE TABLE [erv].[tblTempPersonnelToCompanyForParentCompany](
	[MappingIndex] [int] IDENTITY(1,1) NOT NULL,
	[TargetSiteGuid] [uniqueidentifier] NOT NULL,
	[PersonnelGuid] [uniqueidentifier] NOT NULL,
	[PersonnelMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[CompanyGuid] [uniqueidentifier] NOT NULL,
	[CompanyMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[CompanyParentSiteGuid] [uniqueidentifier] NULL,
	[ParentCompanyGuid] [uniqueidentifier] NULL,
	[PersonnelParentSiteGuid] [uniqueidentifier] NULL,
	[PersonnelGuidForParentCompany] [uniqueidentifier] NULL,
	[DriversFCM] [nvarchar](20) NULL,
	[IsMasterRecordCompany] [bit] NULL,
	[CompanyOwnsRecordAtAssignedFromSitegroup] [bit] NULL,
	[Processed] [bit] NULL,
	[_CallingReferenceGuid] [uniqueidentifier] NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblTempPersonnelToCompanyForParentCompany] PRIMARY KEY CLUSTERED 
(
	[MappingIndex] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
