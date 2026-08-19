/*

	DROP TABLE [staging].[tblAuditLog]

*/
CREATE TABLE [staging].[tblAuditLog] (
    [SessionID]			NVARCHAR (50)		NULL,
    [ActionID]			NVARCHAR (20)		NULL,
    [TypeID]			NVARCHAR (50)		NULL,
    [ID]				NVARCHAR (256)		NULL,
    [PropertyID]		NVARCHAR (50)		NULL,
    [NewValue]			NVARCHAR (2000)		NULL,
    [OldValue]			NVARCHAR (2000)		NULL,
    [CreatedDate]		DATETIMEOFFSET (7)	NULL,
    [CreatedBy]			[dbo].[udtUserID]	NULL,
    [ParentTypeID]		NVARCHAR (50)		NULL,
    [AuditLogGuid]		UNIQUEIDENTIFIER	NULL,    
    [SiteGuid]			UNIQUEIDENTIFIER	NULL,
    [AuditedDate]		DATETIMEOFFSET (7)	NULL,    
    [SourceNode]		NVARCHAR (256)		NULL,
    [AuditContext]		VARBINARY(128)		NULL,
	[SourceClusterIdx]  BIGINT				NULL,
	[SourceRowVersion]	BIGINT				NULL,
	[AuditedDateKey]	INT					NULL,
	[ArchiveDate]       DATETIMEOFFSET (7)  NULL,
	[ETLProcessKey]	    BIGINT			    NULL,
	[IgnoreRecord]		BIT					NOT NULL,
	[IsProcessed]		BIT					NOT NULL,
	[_RowVersion]       ROWVERSION			NOT NULL,
	[SKey]				INT					IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblAuditLog_SKey] PRIMARY KEY CLUSTERED ([SKey] ASC)
);
GO


ALTER TABLE [staging].[tblAuditLog] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblAuditLog] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO
