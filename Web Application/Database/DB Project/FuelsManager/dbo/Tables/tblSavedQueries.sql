CREATE TABLE [dbo].[tblSavedQueries] (
    [QueryType]          INT                CONSTRAINT [DF_tblSavedQueries_QueryType] DEFAULT ((0)) NOT NULL,
    [QueryName]          NVARCHAR (50)      CONSTRAINT [DF_tblSavedQueries_QueryName] DEFAULT ('') NOT NULL,
    [TransactionAliases] NVARCHAR (MAX)     NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL,
    [CreatedDate]        DATETIMEOFFSET (7) NULL,
    [UpdatedBy]          [dbo].[udtUserID]  NULL,
    [UpdatedDate]        DATETIMEOFFSET (7) NULL,
    [StartDate]          DATETIMEOFFSET (7) NULL,
    [EndDate]            DATETIMEOFFSET (7) NULL,
    [SavedQueryGuid]     UNIQUEIDENTIFIER   CONSTRAINT [DF_tblSavedQueries_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    [SiteGuid]           UNIQUEIDENTIFIER   NOT NULL,
    [UserGuid]           UNIQUEIDENTIFIER   NOT NULL,
    [_ClusterIdx]        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSavedQueries_GUID] PRIMARY KEY NONCLUSTERED ([SavedQueryGuid] ASC),
    CONSTRAINT [FK_tblSavedQueries_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblSavedQueries_UserGuid] FOREIGN KEY ([UserGuid]) REFERENCES [dbo].[tblUsers] ([UserGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblSavedQueries_CreatedDate]
    ON [dbo].[tblSavedQueries]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSavedQueries_ClusterIdx]
    ON [dbo].[tblSavedQueries]([_ClusterIdx] ASC);

