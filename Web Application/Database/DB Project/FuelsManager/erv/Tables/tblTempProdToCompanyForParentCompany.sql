CREATE TABLE [erv].[tblTempProdToCompanyForParentCompany](
	[MappingIndex] int identity NOT NULL,
	[TargetSiteGuid] [uniqueidentifier] NOT NULL,
	[ProductGuid] [uniqueidentifier] NOT NULL,
	[ProductMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[CompanyGuid] [uniqueidentifier] NOT NULL,
	[CompanyMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[CompanyParentSiteGuid] [uniqueidentifier] NULL,
	[ParentCompanyGuid] [uniqueidentifier] NULL,
	[ProductParentSiteGuid] [uniqueidentifier] NULL,
	[ProductGuidForParentCompany] [uniqueidentifier] NULL,
	[ShipToAuthorizedProductsFCM] [nvarchar](20) NULL,
	[UnavailableInventoriesFCM] [nvarchar](20) NULL,
	[SupplierAuthorizedProductsFCM] [nvarchar](20) NULL,
	[IsMasterRecordCompany] bit NULL,
	[CompanyOwnsRecordAtAssignedFromSitegroup] bit NULL,
	Processed bit NULL,
	[_CallingReferenceGuid] uniqueidentifier NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,	
 CONSTRAINT [PK_tblTempProdToCompanyForParentCompany] PRIMARY KEY CLUSTERED 
(
	[MappingIndex] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) 
)
GO
/****** Object:  Index [IX_tblTempProdToCompanyForParentCompany_01]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempProdToCompanyForParentCompany_01] ON [erv].[tblTempProdToCompanyForParentCompany]
(
	[ProductGuid], [CompanyGuid]
)
INCLUDE 
( 	[ParentCompanyGuid],
	[ProductGuidForParentCompany]
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_tblTempProdToCompanyForParentCompany_02]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempProdToCompanyForParentCompany_02] ON [erv].[tblTempProdToCompanyForParentCompany]
(
	[_CallingReferenceGuid] ASC
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)