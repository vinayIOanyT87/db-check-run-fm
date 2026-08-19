CREATE TABLE [dbo].[tblChangeLog] (
    [TableName]      VARCHAR (128)      NOT NULL,
    [RowID]          VARCHAR (MAX)      NOT NULL,
    [DmlType]        CHAR (1)           NOT NULL,
    [DateEvent]      DATETIMEOFFSET (7) NOT NULL,
    [ColumnsBefore]  XML                NULL,
    [ColumnsAfter]   XML                NULL,
    [UserID]         VARCHAR (50)       NOT NULL,
    [ASPSessionID]   CHAR (24)          NOT NULL,
    [Token]          UNIQUEIDENTIFIER   NOT NULL,
    [SPID]           SMALLINT           NOT NULL,
    [ClientDomain]   VARCHAR (32)       NOT NULL,
    [ClientUserName] VARCHAR (20)       NOT NULL,
    [Workstation]    VARCHAR (31)       NOT NULL,
    [ClientIPAddr]   INT                NOT NULL,
    [AppName]        VARCHAR (128)      NOT NULL,
    [ChangeLogGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_tblChangeLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]    ROWVERSION         NOT NULL,
    [_ClusterIdx]    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblChangeLog_GUID] PRIMARY KEY NONCLUSTERED ([ChangeLogGuid] ASC)
);




GO



GO
CREATE NONCLUSTERED INDEX [IX_tblChangeLog_DateEvent]
    ON [dbo].[tblChangeLog]([DateEvent] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblChangeLog_ClusterIdx]
    ON [dbo].[tblChangeLog]([_ClusterIdx] ASC);

