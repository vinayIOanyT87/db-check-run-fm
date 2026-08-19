CREATE TABLE [fmaudit].[tblStandingOffers](
	[StandingOfferPrice] float NULL
,	[EffectiveDate] datetimeoffset NULL
,	[ExpirationDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[CreatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[LowerBound] int NULL
,	[UpperBound] int NULL
,	[ReferenceNumber] nvarchar (20) NULL
,	[StandingOfferGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[ProductGuid] uniqueidentifier NULL
,	[SupplierCompanyGuid] uniqueidentifier NULL
,	[LocationIATAGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblStandingOffers_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblStandingOffers_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblStandingOffers_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblStandingOffers_AuditGUID] ON [fmaudit].[tblStandingOffers](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblStandingOffers_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblStandingOffers] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblStandingOffers_ClusterIdx] ON [fmaudit].[tblStandingOffers](_ClusterIdx ASC)