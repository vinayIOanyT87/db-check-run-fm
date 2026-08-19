CREATE TABLE [map].[tblSessionToSQLProcess] (
    [SessionToSQLProcessIndex] BIGINT             IDENTITY (1, 1) NOT NULL,
    [SessionGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [SqlServerSessionID]       INT                NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblSessionToSQLProcess_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblSessionToSQLProcess_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblSessionToSQLProcess_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblSessionToSQLProcess_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    CONSTRAINT [PK_tblSessionToSQLProcess] PRIMARY KEY CLUSTERED ([SessionToSQLProcessIndex] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_tblSessionToSQLProcess_SqlServerSessionID]
ON [map].[tblSessionToSQLProcess]([SqlServerSessionID] ASC, [SessionGuid] ASC);
GO


