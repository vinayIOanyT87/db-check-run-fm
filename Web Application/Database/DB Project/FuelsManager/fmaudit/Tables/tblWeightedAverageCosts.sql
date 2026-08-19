CREATE TABLE [fmaudit].[tblWeightedAverageCosts](
	[WacValue] float NULL
,	[IsManualOverride] bit NULL
,	[Source] nvarchar (64) NULL
,	[Notes] nvarchar (2048) NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[InventoryDate] date NULL
,	[WeightedAverageCostGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ProductGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblWeightedAverageCosts_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblWeightedAverageCosts_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblWeightedAverageCosts_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)






GO

CREATE NONCLUSTERED INDEX [IX_tblWeightedAverageCosts_AuditGUID] ON [fmaudit].[tblWeightedAverageCosts](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblWeightedAverageCosts_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblWeightedAverageCosts] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
CREATE CLUSTERED INDEX [IX_tblWeightedAverageCosts_ClusterIdx] ON [fmaudit].[tblWeightedAverageCosts](_ClusterIdx ASC)