CREATE TABLE [fmaudit].[tblDrawings](
	[DrawingGuid] uniqueidentifier NULL
,	[ID] nvarchar (30) NULL
,	[Description] nvarchar (255) NULL
,	[Image] nvarchar (max) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[PanelType]	int NULL
,	[PointTemplateGuid] UNIQUEIDENTIFIER NULL
,	[Published]	BIT NULL 
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_tblDrawings_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_tblDrawings_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_tblDrawings_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblDrawings_ClusterIdx] ON [fmaudit].[tblDrawings](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblDrawings_AuditGUID] ON [fmaudit].[tblDrawings](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblDrawings_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblDrawings] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)