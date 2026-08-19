CREATE TABLE [fmaudit].[tblMobileDispatchSiteIntegrationInfo]
(
	[MobileDispatchSiteIntegrationInfoGuid]   UNIQUEIDENTIFIER  NULL
,	[IntegrationGuid]		UNIQUEIDENTIFIER   NULL
,	[API_Username]			NVARCHAR (255)     NULL
,	[API_Password]			NVARCHAR (255)     NULL
,	[StationIATA]			NVARCHAR(3)		   NULL
,	[Facility]				NVARCHAR(100)	   NULL
,	[Vendor]				NVARCHAR(100)      NULL
,	[BaseURL]				NVARCHAR(512)      NULL
,	[RequestedURL]			NVARCHAR(512)      NULL
,	[SiteGuid]				UNIQUEIDENTIFIER   NULL
,   [CreatedBy]             nvarchar (100)     NULL
,   [CreatedDate]           DATETIMEOFFSET     NULL
,   [UpdatedBy]             nvarchar (100)     NULL
,   [UpdatedDate]           DATETIMEOFFSET     NULL
,   [OriginalRowVersion]    BINARY(8)          NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblMobileDispatchSiteIntegrationInfo_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblMobileDispatchSiteIntegrationInfo_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblMobileDispatchSiteIntegrationInfo_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblMobileDispatchSiteIntegrationInfo_AuditGUID] ON [fmaudit].[tblMobileDispatchSiteIntegrationInfo](_AuditGUID ASC)
GO

CREATE NONCLUSTERED INDEX [IX_tblMobileDispatchSiteIntegrationInfo_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblMobileDispatchSiteIntegrationInfo] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblMobileDispatchSiteIntegrationInfo_ClusterIdx] ON [fmaudit].[tblMobileDispatchSiteIntegrationInfo](_ClusterIdx ASC)
