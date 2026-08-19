CREATE TABLE [erv].[tblTempProdToTransactionAliasForParentTransactionAlias](
	[MappingIndex] int identity NOT NULL,
	[TargetSiteGuid] [uniqueidentifier] NOT NULL,
	[ProductGuid] [uniqueidentifier] NOT NULL,
	[ProductMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[TransactionAliasGuid] [uniqueidentifier] NOT NULL,
	[TransactionAliasMasterRecordGuid] [uniqueidentifier] NOT NULL,
	[TransactionAliasParentSiteGuid] [uniqueidentifier] NULL,
	[ParentTransactionAliasGuid] [uniqueidentifier] NULL,
	[ProductParentSiteGuid] [uniqueidentifier] NULL,
	[ProductGuidForParentTransactionAlias] [uniqueidentifier] NULL,
	[ProductFCM] [nvarchar](20) NULL,
	[IsMasterRecordTransactionAlias] bit NULL,
	[TransactionAliasOwnsRecordAtAssignedFromSitegroup] bit NULL,
	Processed bit NULL,
	[_CallingReferenceGuid] uniqueidentifier NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,	
 CONSTRAINT [PK_tblTempProdToTransactionAliasForParentTransactionAlias] PRIMARY KEY CLUSTERED 
(
	[MappingIndex] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) 
)
GO
/****** Object:  Index [IX_tblTempProdToTransactionAliasForParentTransactionAlias_01]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempProdToTransactionAliasForParentTransactionAlias_01] ON [erv].[tblTempProdToTransactionAliasForParentTransactionAlias]
(
	[ProductGuid], [TransactionAliasGuid]
)
INCLUDE 
( 	[ParentTransactionaliasGuid],
	[ProductGuidForParentTransactionAlias]
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_tblTempProdToTransactionAliasForParentTransactionAlias_02]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempProdToTransactionAliasForParentTransactionAlias_02] ON [erv].[tblTempProdToTransactionAliasForParentTransactionAlias]
(
	[_CallingReferenceGuid] ASC
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)