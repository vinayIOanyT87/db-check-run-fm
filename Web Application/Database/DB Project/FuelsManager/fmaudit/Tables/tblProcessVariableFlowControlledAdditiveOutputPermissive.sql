CREATE TABLE [fmaudit].[tblProcessVariableFlowControlledAdditiveOutputPermissive](
	[ProcessVariableProductToPresetFlowControlledAdditiveGuid] uniqueidentifier NULL
,	[LookupProcessVariableTypeIndex] int NULL
,	[InstanceNumber] int NULL
,	[ProductToPresetFlowControlledAdditiveGuid] uniqueidentifier NULL
,	[OPCConnectionGuid] uniqueidentifier NULL
,	[OPCItemID] nvarchar (255) NULL
,	[DataType] int NULL
,	[ServerEngineeringUnitsIndex] int NULL
,	[Quality] smallint NULL
,	[SIValue] varbinary (max) NULL
,	[LookupSIValueVariantTypeIndex] int NULL
,	[DateTimeStamp] datetimeoffset NULL
,	[Maximum] varbinary (max) NULL
,	[LookupMaximumVariantTypeIndex] int NULL
,	[Minimum] varbinary (max) NULL
,	[LookupMinimumVariantTypeIndex] int NULL
,	[DataTypeEnabled] bit NULL
,	[Input] bit NULL
,	[InputEnabled] bit NULL
,	[MessageApplicationStringGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblProcessVariableFlowControlledAdditiveOutputPermissive_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblProcessVariableFlowControlledAdditiveOutputPermissive_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblProcessVariableFlowControlledAdditiveOutputPermissive_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblProcessVariableFlowControlledAdditiveOutputPermissive_AuditGUID] ON [fmaudit].[tblProcessVariableFlowControlledAdditiveOutputPermissive](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblProcessVariableFlowControlledAdditiveOutputPermissive_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblProcessVariableFlowControlledAdditiveOutputPermissive] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblProcessVariableFlowControlledAdditiveOutputPermissive_ClusterIdx] ON [fmaudit].[tblProcessVariableFlowControlledAdditiveOutputPermissive](_ClusterIdx ASC)