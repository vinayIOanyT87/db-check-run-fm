CREATE TABLE [fmaudit].[tblUserDataListValueIATA] (
    [UserDataListValueIATAGuid] UNIQUEIDENTIFIER   NULL,
    [UserDataFieldIATAGuid]     UNIQUEIDENTIFIER   NULL,
    [Value]                     NVARCHAR (120)     NULL,
    [CreatedDate]               DATETIMEOFFSET (7) NULL,
    [CreatedBy]                 NVARCHAR (100)     NULL,
    [UpdatedDate]               DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                 NVARCHAR (100)     NULL,
    [OriginalRowVersion]        BINARY (8)         NULL,
    [_AuditEventType]           CHAR (1)           NULL,
    [_AuditEventSequence]       TINYINT            NULL,
    [_AuditSiteGuid]            UNIQUEIDENTIFIER   NULL,
    [_AuditSessionGuid]         UNIQUEIDENTIFIER   NULL,
    [_AuditUserID]              [dbo].[udtUserID]  NULL,
    [_AuditSessionTokenID]      UNIQUEIDENTIFIER   NULL,
    [_AuditCreatedDate]         DATETIMEOFFSET (7) NULL,
    [_AuditGUID]                UNIQUEIDENTIFIER   NOT NULL,
    [_AuditRowVersion]          ROWVERSION         NOT NULL,
    [_ClusterIdx]             BIGINT IDENTITY (1, 1) NOT NULL, 
    [_AuditContext]           VARBINARY(128) NULL 
);
GO
ALTER TABLE [fmaudit].[tblUserDataListValueIATA]
    ADD CONSTRAINT [DF_tblUserDataListValueIATA_AuditGUID] DEFAULT (newid()) FOR [_AuditGUID];
GO
ALTER TABLE [fmaudit].[tblUserDataListValueIATA]
    ADD CONSTRAINT [DF_tblUserDataListValueIATA_AuditCreatedDate] DEFAULT (sysdatetimeoffset()) FOR [_AuditCreatedDate];
GO
ALTER TABLE [fmaudit].[tblUserDataListValueIATA]
    ADD CONSTRAINT [DF_tblUserDataListValueIATA_AuditEventSequence] DEFAULT ((0)) FOR [_AuditEventSequence];
GO
CREATE CLUSTERED INDEX [IX_tblUserDataListValueIATA_AuditCreatedDate]
    ON [fmaudit].[tblUserDataListValueIATA]([_AuditCreatedDate] ASC) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_tblUserDataListValueIATA_AuditGUID]
    ON [fmaudit].[tblUserDataListValueIATA]([_AuditGUID] ASC) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_tblUserDataListValueIATA_AuditRowVersion_EventType_EventSequence]
    ON [fmaudit].[tblUserDataListValueIATA]([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC)
    INCLUDE([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) WITH (FILLFACTOR = 100);