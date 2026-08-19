CREATE TABLE [fmaudit].[tblAllocations](
	[EffectiveDate] datetimeoffset NULL
,	[ExpirationDate] datetimeoffset NULL
,	[LoadWarning] float NULL
,	[LoadDenial] float NULL
,	[ContractNumber] nvarchar (10) NULL
,	[AllocationGroupIndex] int NULL
,	[LastAllocationResetDate] datetimeoffset NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[AllocationGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[CompanyBillToToShipperGuid] uniqueidentifier NULL
,	[CompanyLoadOwnerToManagerGuid] uniqueidentifier NULL
,	[CompanyOffLoadOwnerToManagerGuid] uniqueidentifier NULL
,	[CompanyShipperToOwnerGuid] uniqueidentifier NULL
,	[CompanyShipToToBillToGuid] uniqueidentifier NULL
,	[CompanySupplierToOwnerGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupCompanyMapTypeIndex] int NULL
,	[AllocationGroupApplicationStringGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblAllocations_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblAllocations_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblAllocations_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblAllocations_AuditGUID] ON [fmaudit].[tblAllocations](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblAllocations_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblAllocations] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblAllocations_ClusterIdx] ON [fmaudit].[tblAllocations](_ClusterIdx ASC)