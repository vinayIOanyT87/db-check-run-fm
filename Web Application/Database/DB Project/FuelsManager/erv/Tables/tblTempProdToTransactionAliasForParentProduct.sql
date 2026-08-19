CREATE TABLE [erv].[tblTempProdToTransactionAliasForParentProduct](
	[MappingIndex] int identity NOT NULL,
	[TargetSiteGuid] [uniqueidentifier] NOT NULL,
	[TransactionAliasGuid] [uniqueidentifier] NOT NULL,
	[TransactionAliasMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[ProductGuid] [uniqueidentifier] NOT NULL,
	[ProductMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[ProductParentSiteGuid] [uniqueidentifier] NULL,
	[ParentProductGuid] [uniqueidentifier] NULL,
	[TransactionAliasParentSiteGuid] [uniqueidentifier] NULL,
	[TransactionAliasGuidForParentProduct] [uniqueidentifier] NULL,
	[TransactionAliasExclusionFCM] [nvarchar](20) NULL,
	[IsMasterRecordProduct] bit NULL,
	[ProductOwnsRecordAtAssignedFromSitegroup] bit NULL,
	Processed bit NULL,
	[_CallingReferenceGuid] uniqueidentifier NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,	
 CONSTRAINT [PK_tblTempProdToTransactionAliasForParentProduct] PRIMARY KEY CLUSTERED 
(
	[MappingIndex] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) 
)
GO
/****** Object:  Index [IX_tblTempProdToTransactionAliasForParentProduct_01]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempProdToTransactionAliasForParentProduct_01] ON [erv].[tblTempProdToTransactionAliasForParentProduct]
(
	[TransactionAliasGuid], [ProductGuid]
)
INCLUDE 
( 	[ParentProductGuid],
	[TransactionAliasGuidForParentProduct]
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_tblTempProdToTransactionAliasForParentProduct_02]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempProdToTransactionAliasForParentProduct_02] ON [erv].[tblTempProdToTransactionAliasForParentProduct]
(
	[_CallingReferenceGuid] ASC
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)