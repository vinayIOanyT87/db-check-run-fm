CREATE TABLE [fmaudit].[tblPictures](
	[PictureGuid] uniqueidentifier NULL
,	[ID] nvarchar (30) NULL
,	[Description] nvarchar (255) NULL
,	[ImageStream] varbinary (max) NULL
,	[IsSystemImage] bit NULL
,	[ImageHash] nvarchar (100) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[ContentType] nvarchar (50) NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblPictures_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblPictures_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblPictures_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
)
GO
CREATE CLUSTERED INDEX [IX_tblPictures_ClusterIdx] ON [fmaudit].[tblPictures](_ClusterIdx ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblPictures_AuditGUID] ON [fmaudit].[tblPictures](_AuditGUID ASC) 
GO
CREATE NONCLUSTERED INDEX [IX_tblPictures_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblPictures] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF) 
GO
