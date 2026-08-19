/*
	DROP TABLE [dbo].[FactSiteHierarchyBridge]
*/
CREATE TABLE [dbo].[FactSiteHierarchyBridge](
	[SKey] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FactSiteHierarchyBridge] PRIMARY KEY CLUSTERED,
	[ParentSKey] [int] NOT NULL,
	[ChildSKey] [int] NOT NULL,
	[_RecordUpdatedDate]         DATETIMEOFFSET (7) NULL,
    [_DeletedFlag]               BIT  CONSTRAINT [DF_FactSiteHierarchyBridge] DEFAULT ((0)) NULL,
)
GO
CREATE NONCLUSTERED INDEX [IX_FactSiteHierarchyBridge_ParentSKey] ON [dbo].[FactSiteHierarchyBridge]
(
	[ParentSKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_FactSiteHierarchyBridge_ChildSKey] ON [dbo].[FactSiteHierarchyBridge]
(
	[ChildSKey] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]