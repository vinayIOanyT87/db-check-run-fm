CREATE TABLE [erv].[tblTempEntityToSiteHierarchy] (
    [EntityToSiteHierarchyGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTempEntityToSiteHierarchy_GUID] DEFAULT (newid()) NOT NULL,
    [EntityTypeId]              NVARCHAR (100)     NULL,
    [EntityGuid]                UNIQUEIDENTIFIER   NULL,
    [SiteGuid]                  UNIQUEIDENTIFIER   NULL,
    [SiteId]                    NVARCHAR (30)      NULL,
    [HierarchyLevel]            INT                NULL,
    [Processed]                 BIT                NULL,
    [_CallingReferenceGuid]     UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]               DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempEntityToSiteHierarchy_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                 [dbo].[udtUserID]  CONSTRAINT [DF_tblTempEntityToSiteHierarchy_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]               DATETIMEOFFSET (7) CONSTRAINT [DF_tblTempEntityToSiteHierarchy_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                 [dbo].[udtUserID]  CONSTRAINT [DF_tblTempEntityToSiteHierarchy_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]               ROWVERSION         NOT NULL,
    [_ClusterIdx]               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTempEntityToSiteHierarchy] PRIMARY KEY NONCLUSTERED ([EntityToSiteHierarchyGuid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblTempEntityToSiteHierarchy_SiteGuid]
    ON [erv].[tblTempEntityToSiteHierarchy]([SiteGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTempEntityToSiteHierarchy_CallingReferenceGuid]
    ON [erv].[tblTempEntityToSiteHierarchy]([_CallingReferenceGuid] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTempEntityToSiteHierarchy_ClusterIdx]
    ON [erv].[tblTempEntityToSiteHierarchy]([_ClusterIdx] ASC);

