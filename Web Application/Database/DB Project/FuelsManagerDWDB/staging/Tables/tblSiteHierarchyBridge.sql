CREATE TABLE [staging].[tblSiteHierarchyBridge](
	[SiteToSiteSKey] [int] IDENTITY(1,1) NOT NULL,
	[ParentSiteSKey] [int] NULL,	
	[ChildSiteSKey] [int] NULL,
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NOT NULL DEFAULT 0,
	[IgnoreRecord] [bit] NOT NULL DEFAULT 0,
 CONSTRAINT [PK_staging_tblSiteHierarchyBridge] PRIMARY KEY CLUSTERED 
(
	[SiteToSiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)