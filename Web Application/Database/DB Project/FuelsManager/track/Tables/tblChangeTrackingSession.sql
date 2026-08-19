CREATE TABLE [track].[tblChangeTrackingSession] (
    [ChangeTrackingSessionGuid] UNIQUEIDENTIFIER   NOT NULL,
    [SqlServerSessionID]        INT                NOT NULL,
    [ContextName]               NVARCHAR (100)     NOT NULL,
    [BypassTrackingFlags]       INT                NOT NULL,
    [BypassReason]              NVARCHAR (512)     NULL,
    [CreatedDate]               DATETIMEOFFSET (7) NOT NULL,
    [_ClusterIdx]               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblChangeTrackingSession] PRIMARY KEY NONCLUSTERED ([ChangeTrackingSessionGuid] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblChangeTrackingSession_SqlServerSessionID]
    ON [track].[tblChangeTrackingSession]([SqlServerSessionID] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblChangeTrackingSession_ClusterIdx]
    ON [track].[tblChangeTrackingSession]([_ClusterIdx] ASC);

