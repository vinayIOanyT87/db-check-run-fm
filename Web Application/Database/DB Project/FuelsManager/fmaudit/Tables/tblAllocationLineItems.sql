CREATE TABLE [fmaudit].[tblAllocationLineItems](
	[Limit] float NULL
,	[Next] float NULL
,	[ResetMultiple] int NULL
,	[ResetDate] datetimeoffset NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[AllocationLineItemGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[LookupAllocationTypeIndex] int NULL
,	[LookupResetMethodIndex] int NULL
,	[LookupResetPeriodIndex] int NULL
,	[AllocationGuid] uniqueidentifier NULL
,	[AssignedProductGuid] uniqueidentifier NULL
,	[AssignedApplicationStringGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAllocationLineItems_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAllocationLineItems_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAllocationLineItems_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO


CREATE NONCLUSTERED INDEX [IX_tblAllocationLineItems_AuditGUID] ON [fmaudit].[tblAllocationLineItems](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblAllocationLineItems_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAllocationLineItems] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblAllocationLineItems_ClusterIdx] ON [fmaudit].[tblAllocationLineItems](_ClusterIdx ASC)