CREATE TABLE [staging].[tblMissingEntitiesTempTwo](
	RunningKey int Identity,
	RecordKey nvarchar(50) NULL,
	RecordId nvarchar(100),
	SiteKey nvarchar(50) NULL,
	FieldA nvarchar(100) NULL,
	FieldB int NULL,
	IsProcessed bit
 CONSTRAINT [PK_staging_tblMissingEntitiesTempTwo] PRIMARY KEY CLUSTERED 
(
	[RunningKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)