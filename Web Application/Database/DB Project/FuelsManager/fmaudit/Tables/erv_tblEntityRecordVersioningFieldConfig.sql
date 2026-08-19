CREATE TABLE [fmaudit].[erv_tblEntityRecordVersioningFieldConfig](
	[FieldConfigGuid] uniqueidentifier NULL
,	[EntitySegmentTemplateGuid] uniqueidentifier NULL
,	[SiteGroupGuid] uniqueidentifier NULL
,	[TargetField] nvarchar (100) NULL
,	[IsExternalAttribute] bit NULL
,	[InternalFieldName] nvarchar (100) NULL
,	[FilterValueGuid] uniqueidentifier NULL
,	[FilterValueName] nvarchar (100) NULL
,	[InheritedControlMode] nvarchar (20) NULL
,	[ForwardControlMode] nvarchar (20) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_erv_tblEntityRecordVersioningFieldConfig_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_erv_tblEntityRecordVersioningFieldConfig_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_erv_tblEntityRecordVersioningFieldConfig_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)


GO

CREATE NONCLUSTERED INDEX [IX_erv_tblEntityRecordVersioningFieldConfig_AuditGUID] ON [fmaudit].[erv_tblEntityRecordVersioningFieldConfig](_AuditGUID ASC)
GO
CREATE NONCLUSTERED INDEX [IX_erv_tblEntityRecordVersioningFieldConfig_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[erv_tblEntityRecordVersioningFieldConfig] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_erv_tblEntityRecordVersioningFieldConfig_ClusterIdx] ON [fmaudit].[erv_tblEntityRecordVersioningFieldConfig](_ClusterIdx ASC)