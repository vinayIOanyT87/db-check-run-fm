CREATE TABLE [fmaudit].[tblMovementSummary]
(
	[MovementSummaryGuid] [uniqueidentifier] NOT NULL
,	[ID] [nvarchar](30) NOT NULL
,	[Description] [nvarchar](50) NULL
,	[MovementSummaryType] [int] NULL
,	[ColumnsDefinition] [nvarchar](MAX) NULL
,	[FontSize] [int] NULL
,	[RowsDefinition] [nvarchar](MAX) NULL
,	[OwnerUserGuid] [uniqueidentifier] NOT NULL
,	[SiteGuid] [uniqueidentifier] NOT NULL
,	[CreatedDate] [datetimeoffset](7) NOT NULL
,	[CreatedBy] [dbo].[udtUserID] NOT NULL
,	[UpdatedDate] [datetimeoffset](7) NOT NULL
,	[UpdatedBy] [dbo].[udtUserID] NOT NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblMovementSummary_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblMovementSummary_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblMovementSummary_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL )
GO
CREATE CLUSTERED INDEX [IX_tblMovementSummary_ClusterIdx] ON [fmaudit].[tblMovementSummary](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblMovementSummary_AuditGUID] ON [fmaudit].[tblMovementSummary](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblMovementSummary_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblMovementSummary] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)