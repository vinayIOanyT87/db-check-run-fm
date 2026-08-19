CREATE TABLE [erv].[tblTempProdToCompanyForParentProduct](

	[MappingIndex] int identity NOT NULL,
	[TargetSiteGuid] [uniqueidentifier] NOT NULL,
	[CompanyGuid] [uniqueidentifier] NOT NULL,
	[CompanyMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[ProductGuid] [uniqueidentifier] NOT NULL,
	[ProductMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[ProductParentSiteGuid] [uniqueidentifier] NULL,
	[ParentProductGuid] [uniqueidentifier] NULL,
	[CompanyParentSiteGuid] [uniqueidentifier] NULL,
	[CompanyGuidForParentProduct] [uniqueidentifier] NULL,
	[AuthorizedCustomersFCM] [nvarchar](20) NULL,
	[UnavailableInventoriesFCM] [nvarchar](20) NULL,
	[SupplierAuthorizedProductsFCM] [nvarchar](20) NULL,
	[IsMasterRecordProduct] bit NULL,
	[ProductOwnsRecordAtAssignedFromSitegroup] bit NULL,
	Processed bit NULL,
	[_CallingReferenceGuid] uniqueidentifier NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,	
 CONSTRAINT [PK_tblTempProdToCompanyForParentProduct] PRIMARY KEY CLUSTERED 
(
	[MappingIndex] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) 
)
GO
/****** Object:  Index [IX_tblTempProdToCompanyForParentProduct_01]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempProdToCompanyForParentProduct_01] ON [erv].[tblTempProdToCompanyForParentProduct]
(
	[CompanyGuid], [ProductGuid]
)
INCLUDE 
( 	[ParentProductGuid],
	[CompanyGuidForParentProduct]
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_tblTempProdToCompanyForParentProduct_02]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempProdToCompanyForParentProduct_02] ON [erv].[tblTempProdToCompanyForParentProduct]
(
	[_CallingReferenceGuid] ASC
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)