/*

	DROP TABLE [dbo].[tblAuditLog]

*/
CREATE TABLE [dbo].[tblAuditLog] (
    [SessionID]    NVARCHAR (50)      NULL,
    [ActionID]     NVARCHAR (20)      NULL,
    [TypeID]       NVARCHAR (50)      NULL,
    [ID]           NVARCHAR (256)     NULL,
    [PropertyID]   NVARCHAR (50)      NULL,
    [NewValue]     NVARCHAR (2000)    NULL,
    [OldValue]     NVARCHAR (2000)    NULL,
    [CreatedDate]  DATETIMEOFFSET (7) NULL,
    [CreatedBy]    [dbo].[udtUserID]  NULL,
    [ParentTypeID] NVARCHAR (50)      NULL,
    [AuditLogGuid] UNIQUEIDENTIFIER   NOT NULL,    
    [SiteGuid]     UNIQUEIDENTIFIER   NOT NULL,
    [AuditedDate]  DATETIMEOFFSET (7) NULL,    
    [SourceNode]   NVARCHAR (256)     NULL,
    [AuditContext] VARBINARY(128)	  NULL, 
	[AuditedDateKey] INT			  NOT NULL,
	[ArchiveDate]	DATETIMEOFFSET (7) NULL,	
	[ETLProcessKey]	BIGINT			  NULL,
	[_RowVersion]  ROWVERSION         NOT NULL,
	[_ClusterIdx]  BIGINT             IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblAuditLog_GUID] PRIMARY KEY NONCLUSTERED ([AuditedDateKey] ASC, [AuditLogGuid] ASC) ON [AnnualPS]([AuditedDateKey])
) ON [AnnualPS]([AuditedDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblAuditLog_CreatedDate]
    ON [dbo].[tblAuditLog]([CreatedDate] ASC)
	ON [AnnualPS]([AuditedDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblAuditLog_ClusterIdx]
    ON [dbo].[tblAuditLog]([AuditedDateKey] ASC, [_ClusterIdx] ASC)
	ON [AnnualPS]([AuditedDateKey]);
GO
