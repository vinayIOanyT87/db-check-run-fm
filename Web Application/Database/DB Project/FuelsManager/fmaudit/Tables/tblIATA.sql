CREATE TABLE [fmaudit].[tblIATA](
	[IATAID] nvarchar (50) NULL
,	[Name] nvarchar (200) NULL
,	[CountryID] nvarchar (50) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[IATAGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[Latitude] float NULL
,	[Longitude] float NULL
,	[Zoom] int NULL
,   [TimeZone] NVARCHAR (100) NULL
,   [UserData1] NVARCHAR (60) NULL
,   [UserData2] NVARCHAR (60) NULL
,   [UserData3] NVARCHAR (60) NULL
,   [UserData4] NVARCHAR (60) NULL
,   [UserData5] NVARCHAR (60) NULL
,   [UserData6] NVARCHAR (60) NULL
,   [UserData7] NVARCHAR (60) NULL
,   [UserData8] NVARCHAR (60) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblIATA_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblIATA_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblIATA_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)




GO

CREATE NONCLUSTERED INDEX [IX_tblIATA_AuditGUID] ON [fmaudit].[tblIATA](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblIATA_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblIATA] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblIATA_ClusterIdx] ON [fmaudit].[tblIATA](_ClusterIdx ASC)