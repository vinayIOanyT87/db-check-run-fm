CREATE TABLE [fmaudit].[map_tblTrendPenToDetailTrend]
(
	[TrendPenToDetailTrendGuid] uniqueidentifier NULL
,	[PointTemplateTagGuid] uniqueidentifier NULL
,	[TrendGuid] uniqueidentifier NULL
,	[PenColor] nvarchar(30) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[OriginalRowVersion] binary(8) NULL
,	[_AuditEventType] char(1) NULL
,	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_map_tblTrendPenToDetailTrend_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] uniqueidentifier NULL
,	[_AuditSessionGuid] uniqueidentifier NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] uniqueidentifier NULL
,	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_map_tblTrendPenToDetailTrend_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_map_tblTrendPenToDetailTrend_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL
)
GO

CREATE CLUSTERED INDEX [IX_tmap_blTrendPenToDetailTrend_ClusterIdx] ON [fmaudit].[map_tblTrendPenToDetailTrend](_ClusterIdx ASC)
GO

CREATE NONCLUSTERED INDEX [IX_map_tblTrendPenToDetailTrend_AuditGUID] ON [fmaudit].[map_tblTrendPenToDetailTrend](_AuditGUID ASC)
GO

CREATE NONCLUSTERED INDEX [IX_fmaudit_map_tblTrendPenToDetailTrend_AuditRowVersion] ON [fmaudit].[map_tblTrendPenToDetailTrend]([_AuditRowVersion]);
GO
