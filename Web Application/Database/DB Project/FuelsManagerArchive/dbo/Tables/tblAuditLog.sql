CREATE TABLE [dbo].[tblAuditLog] (
    [SessionID]    NVARCHAR (50)      CONSTRAINT [DF_tblAuditLog_SessionID] DEFAULT ('') NOT NULL,
    [ActionID]     NVARCHAR (20)      CONSTRAINT [DF_tblAuditLog_ActionID] DEFAULT ('') NOT NULL,
    [TypeID]       NVARCHAR (50)      CONSTRAINT [DF_tblAuditLog_TypeID] DEFAULT ('') NOT NULL,
    [ID]           NVARCHAR (256)     CONSTRAINT [DF_tblAuditLog_ID] DEFAULT ('') NOT NULL,
    [PropertyID]   NVARCHAR (50)      CONSTRAINT [DF_tblAuditLog_PropertyID] DEFAULT ('') NOT NULL,
    [NewValue]     NVARCHAR (2000)    CONSTRAINT [DF_tblAuditLog_NewValue] DEFAULT ('') NOT NULL,
    [OldValue]     NVARCHAR (2000)    CONSTRAINT [DF_tblAuditLog_OldValue] DEFAULT ('') NOT NULL,
    [CreatedDate]  DATETIMEOFFSET (7) CONSTRAINT [DF_tblAuditLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]    [dbo].[udtUserID]  CONSTRAINT [DF_tblAuditLog_CreatedBy] DEFAULT ('') NOT NULL,
    [ParentTypeID] NVARCHAR (50)      CONSTRAINT [DF_tblAuditLog_ParentTypeID] DEFAULT ('') NOT NULL,
    [AuditLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblAuditLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]  ROWVERSION         NOT NULL,
    [SiteGuid]     UNIQUEIDENTIFIER   NOT NULL,
    [AuditedDate]  DATETIMEOFFSET (7) CONSTRAINT [DF_tblAuditLog_AuditedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    CONSTRAINT [PK_tblAuditLog_GUID] PRIMARY KEY NONCLUSTERED ([AuditLogGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblAuditLog_AuditedDate]
    ON [dbo].[tblAuditLog]([AuditedDate] ASC);

GO
