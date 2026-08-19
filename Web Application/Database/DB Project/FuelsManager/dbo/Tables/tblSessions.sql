CREATE TABLE [dbo].[tblSessions] (
    [CreatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblSessions_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblSessions_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblSessions_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblSessions_UpdatedBy] DEFAULT ('') NOT NULL,
    [Timeout]                 INT                NOT NULL,
    [SessionGuid]             UNIQUEIDENTIFIER   CONSTRAINT [DF_tblSessions_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [SiteGuid]                UNIQUEIDENTIFIER   NULL,
    [LoginSiteGuid]           UNIQUEIDENTIFIER   NULL,
    [UserGuid]                UNIQUEIDENTIFIER   NULL,
    [SqlServerSessionID]      INT                NULL,
    [SynchronizationNodeGuid] UNIQUEIDENTIFIER   NULL,
    [ClientIpAddress]         NVARCHAR (50)      NULL,
    [WebServerName]           NVARCHAR (500)     NULL,
    [WebServerIpAddress]      NVARCHAR (50)      NULL,
    [SessionTokenID]          UNIQUEIDENTIFIER   NULL,
    [SessionFailedFlag]       BIT                CONSTRAINT [DF_tblSessions_SessionFailedFlag] DEFAULT ((0)) NOT NULL,
    [CSRFToken]               NVARCHAR (256)     NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    [OperateAlarmRefreshInterval] INT NULL,
    [OperateTagRefreshInterval] INT NULL,
    CONSTRAINT [PK_tblSessions_GUID] PRIMARY KEY NONCLUSTERED ([SessionGuid] ASC),
    CONSTRAINT [FK_tblSessions_LoginSiteGuid] FOREIGN KEY ([LoginSiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblSessions_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblSessions_UserGuid] FOREIGN KEY ([UserGuid]) REFERENCES [dbo].[tblUsers] ([UserGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblSessions_CreatedDate]
    ON [dbo].[tblSessions]([CreatedDate] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_tblSessions_SqlServerSessionID]
    ON [dbo].[tblSessions]([SqlServerSessionID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblSessions_UserGuid]
    ON [dbo].[tblSessions]([UserGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblSessions_UpdatedDate_Timeout]
    ON [dbo].[tblSessions]([UpdatedDate] ASC, [Timeout] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblSessions_SessionGuid] 
    ON [dbo].[tblSessions]([SessionGuid] ASC)
    INCLUDE ([SiteGuid], [UserGuid], [SessionTokenID], [SynchronizationNodeGuid])
    WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSessions_ClusterIdx]
    ON [dbo].[tblSessions]([_ClusterIdx] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_tblSessions_UserGuid_Timeout]
    ON [dbo].[tblSessions]([UserGuid] ASC, [Timeout] ASC) WITH (FILLFACTOR = 100);