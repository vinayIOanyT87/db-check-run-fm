CREATE TABLE [dbo].[tblSiteAdmin] (
    [SiteAdminGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblSiteAdmin_GUID] DEFAULT (newid()) NOT NULL,
    [SiteGuid]      UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]   DATETIMEOFFSET (7) CONSTRAINT [DF_tblSiteAdmin_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]     [dbo].[udtUserID]  CONSTRAINT [DF_tblSiteAdmin_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]   DATETIMEOFFSET (7) CONSTRAINT [DF_tblSiteAdmin_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]     [dbo].[udtUserID]  CONSTRAINT [DF_tblSiteAdmin_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]   ROWVERSION         NOT NULL,
    [_ClusterIdx]   BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSiteAdmin] PRIMARY KEY NONCLUSTERED ([SiteAdminGuid] ASC),
    CONSTRAINT [FK_tblSiteAdmin_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblSiteAdmin_CreatedDate]
    ON [dbo].[tblSiteAdmin]([CreatedDate] ASC);




GO
CREATE NONCLUSTERED INDEX [UX_tblSiteAdmin_SiteGuid]
    ON [dbo].[tblSiteAdmin]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSiteAdmin_ClusterIdx]
    ON [dbo].[tblSiteAdmin]([_ClusterIdx] ASC);

