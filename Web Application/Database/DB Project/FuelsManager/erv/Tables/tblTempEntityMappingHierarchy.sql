CREATE TABLE [erv].[tblTempEntityMappingHierarchy] (
    [MappingIndex]          INT              IDENTITY (1, 1) NOT NULL,
    [EntityMasterGuid]      UNIQUEIDENTIFIER NOT NULL,
    [EntityGuid]            UNIQUEIDENTIFIER NULL,
    [AssignedFromSiteGuid]  UNIQUEIDENTIFIER NULL,
    [AssignedToSiteGuid]    UNIQUEIDENTIFIER NOT NULL,
    [MappingLevel]          INT              NOT NULL,
    [_CallingReferenceGuid] UNIQUEIDENTIFIER NOT NULL,
    [_RowVersion]           ROWVERSION       NOT NULL,
    CONSTRAINT [PK_tblTempEntityMappingHierarchy] PRIMARY KEY CLUSTERED ([MappingIndex] ASC)
);
GO

/****** Object:  Index [IX_tblTempEntityMappingHierarchy_MasterGuid_SiteGuid]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempEntityMappingHierarchy_MasterGuid_SiteGuid]
    ON [erv].[tblTempEntityMappingHierarchy]([EntityMasterGuid] ASC, [AssignedToSiteGuid] ASC);
GO

/****** Object:  Index [IX_tblTempEntityMappingHierarchy_CallingReferenceGuid]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempEntityMappingHierarchy_CallingReferenceGuid] ON [erv].[tblTempEntityMappingHierarchy]
(
	[_CallingReferenceGuid] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)