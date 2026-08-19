CREATE TABLE [track].[tblDispatchGrid] (
    [ChangeIndex]         BIGINT             IDENTITY (1, 1) NOT NULL,
    [InsertedDate]        DATETIMEOFFSET (7) NOT NULL,
    [InsertedContext]     VARBINARY (128)    NULL,
    [InsertedRowVersion]  VARBINARY (8)      NOT NULL,
    [UpdatedDate]         DATETIMEOFFSET (7) NULL,
    [UpdatedContext]      VARBINARY (128)    NULL,
    [UpdatedRowVersion]   VARBINARY (8)      NULL,
    [DeletedDate]         DATETIMEOFFSET (7) NULL,
    [DeletedContext]      VARBINARY (128)    NULL,
    [DeletedRowVersion]   VARBINARY (8)      NULL,
    [CurrentSiteGuid]     UNIQUEIDENTIFIER   NULL,
    [PreviousSiteGuid]    UNIQUEIDENTIFIER   NULL,
    [PK_DispatchGridGuid] UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_track_tblDispatchGrid_ChangeIndex] PRIMARY KEY CLUSTERED ([ChangeIndex] ASC)
);

