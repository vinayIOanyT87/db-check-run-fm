CREATE TABLE [dbo].[tblAuditHandler] (
    [TableName]     NVARCHAR (100)     NOT NULL,
    [TypeID]        NVARCHAR (50)      NULL,
    [ParentTypeID]  NVARCHAR (50)      NULL,
    [IDQuery]       NVARCHAR (MAX)     NULL,
    [CreatedDate]   DATETIMEOFFSET (7) CONSTRAINT [DF_tblAuditHandler_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [SiteGuidQuery] NVARCHAR (MAX)     NULL,
    [_ClusterIdx]   BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblAuditHandler_TableName] PRIMARY KEY NONCLUSTERED ([TableName] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblAuditHandler_CreatedDate]
    ON [dbo].[tblAuditHandler]([CreatedDate] ASC);




GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAuditHandler_ClusterIdx]
    ON [dbo].[tblAuditHandler]([_ClusterIdx] ASC);

