CREATE TABLE [erv].[tblTempTargetEntitySite] (
    [TargetEntitySiteIndex] INT              IDENTITY (1, 1) NOT NULL,
    [SiteGuid]              UNIQUEIDENTIFIER NULL,
    [MasterRecordGuid]      UNIQUEIDENTIFIER NULL,
    [EntityGuid]            UNIQUEIDENTIFIER NULL,
    [ParentEntityGuid]      UNIQUEIDENTIFIER NULL,
    [_CallingReferenceGuid] UNIQUEIDENTIFIER NOT NULL,
    [_RowVersion]           ROWVERSION       NOT NULL,
    CONSTRAINT [PK_tblTempTargetEntitySite] PRIMARY KEY CLUSTERED ([TargetEntitySiteIndex] ASC)
);
GO

/****** Object:  Index [IX_tblTempTargetEntitySite_EntityGuid]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempTargetEntitySite_EntityGuid]
    ON [erv].[tblTempTargetEntitySite]([EntityGuid] ASC);
GO

/****** Object:  Index [IX_tblTempTargetEntitySite_CallingReferenceGuid]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempTargetEntitySite_CallingReferenceGuid] ON [erv].[tblTempTargetEntitySite]
(
	[_CallingReferenceGuid] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)