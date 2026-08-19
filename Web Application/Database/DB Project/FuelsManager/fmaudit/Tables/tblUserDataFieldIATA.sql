CREATE TABLE [fmaudit].[tblUserDataFieldIATA] (
    [UserDataFieldIATAGuid]   UNIQUEIDENTIFIER   NULL,
    [TransactionAliasGuid]    UNIQUEIDENTIFIER   NULL,
    [SiteGuid]                UNIQUEIDENTIFIER   NULL,
    [Number]                  TINYINT            NULL,
    [DisplayOrder]            INT                NULL,
    [DisplayName]             NVARCHAR (30)      NULL,
    [LookupUserDataTypeIndex] INT                NULL,
    [Required]                BIT                NULL,
    [UserGroupGuid]           UNIQUEIDENTIFIER   NULL,
    [CreatedDate]             DATETIMEOFFSET (7) NULL,
    [CreatedBy]               NVARCHAR (100)     NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) NULL,
    [UpdatedBy]               NVARCHAR (100)     NULL,
    [OriginalRowVersion]      BINARY (8)         NULL,
    [DispatchField]           BIT                NULL,
    [ClearOnNew]              BIT                NULL,
    [_AuditEventType]         CHAR (1)           NULL,
    [_AuditEventSequence]     TINYINT            NULL,
    [_AuditSiteGuid]          UNIQUEIDENTIFIER   NULL,
    [_AuditSessionGuid]       UNIQUEIDENTIFIER   NULL,
    [_AuditUserID]            [dbo].[udtUserID]  NULL,
    [_AuditSessionTokenID]    UNIQUEIDENTIFIER   NULL,
    [_AuditCreatedDate]       DATETIMEOFFSET (7) NULL,
    [_AuditGUID]              UNIQUEIDENTIFIER   NOT NULL,
    [_AuditRowVersion]        ROWVERSION         NOT NULL,
    [_ClusterIdx]             BIGINT IDENTITY (1, 1) NOT NULL, 
    [_AuditContext]           VARBINARY(128) NULL ,   
    [ReadOnly]                BIT NULL,   
    [Visibility]              INT NULL,	
    [DefaultValue]            NVARCHAR(120) NULL
);
GO
ALTER TABLE [fmaudit].[tblUserDataFieldIATA]
    ADD CONSTRAINT [DF_tblUserDataFieldIATA_AuditCreatedDate] DEFAULT (sysdatetimeoffset()) FOR [_AuditCreatedDate];
GO
ALTER TABLE [fmaudit].[tblUserDataFieldIATA]
    ADD CONSTRAINT [DF_tblUserDataFieldIATA_AuditGUID] DEFAULT (newid()) FOR [_AuditGUID];
GO
ALTER TABLE [fmaudit].[tblUserDataFieldIATA]
    ADD CONSTRAINT [DF_tblUserDataFieldIATA_AuditEventSequence] DEFAULT ((0)) FOR [_AuditEventSequence];
GO
CREATE CLUSTERED INDEX [IX_tblUserDataFieldIATA_AuditCreatedDate]
    ON [fmaudit].[tblUserDataFieldIATA]([_AuditCreatedDate] ASC) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_tblUserDataFieldIATA_AuditGUID]
    ON [fmaudit].[tblUserDataFieldIATA]([_AuditGUID] ASC) WITH (FILLFACTOR = 100);
GO
CREATE NONCLUSTERED INDEX [IX_tblUserDataFieldIATA_AuditRowVersion_EventType_EventSequence]
    ON [fmaudit].[tblUserDataFieldIATA]([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC)
    INCLUDE([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) WITH (FILLFACTOR = 100);