CREATE TABLE [sync].[tblSiteType] (
    [SiteTypeIndex] BIGINT             NOT NULL,
    [SiteTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [SiteTypeID]    NVARCHAR (30)      NOT NULL,
    [SiteTypeName]  NVARCHAR (80)      NOT NULL,
    [CreatedDate]   DATETIMEOFFSET (7) CONSTRAINT [DF_tblSiteType_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]     [dbo].[udtUserID]  CONSTRAINT [DF_tblSiteType_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]   DATETIMEOFFSET (7) CONSTRAINT [DF_tblSiteType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]     [dbo].[udtUserID]  CONSTRAINT [DF_tblSiteType_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]   ROWVERSION         NOT NULL,
    [_ClusterIdx]   BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_SiteType] PRIMARY KEY NONCLUSTERED ([SiteTypeIndex] ASC)
);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSiteType_ClusterIdx]
    ON [sync].[tblSiteType]([_ClusterIdx] ASC);

