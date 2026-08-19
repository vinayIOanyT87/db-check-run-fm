CREATE TABLE [fmaudit].[tblFCEEMapping]
(
	[FCEDeviceGuid] UNIQUEIDENTIFIER NULL
,	[MsgType] [int] NULL
,	[Index] [int] NULL
,	[Device] [int] NULL
,	[TagSelection] [int] NULL
,	[PointGuid] uniqueidentifier NULL
,	[FCEEMappingGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_map_tblFCEEMapping_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblFCEEMapping_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblFCEEMapping_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblFCEEMapping_ClusterIdx] ON [fmaudit].[tblFCEEMapping](_ClusterIdx ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblFCEEMapping_AuditGUID] ON [fmaudit].[tblFCEEMapping](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_tblFCEEMapping_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblFCEEMapping] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)