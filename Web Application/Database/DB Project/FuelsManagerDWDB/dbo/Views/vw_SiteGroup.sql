/*
	DROP VIEW vw_SiteGroup
*/

CREATE VIEW [dbo].[vw_SiteGroup] AS
  ------------------------------------------------------------------------------------------------------
  -- View: [dbo].[vw_SiteGroup]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: DimSiteGroup View to support the OLAP cube.
  ------------------------------------------------------------------------------------------------------
SELECT 
	[AKey] ,
	[SiteId],
	[City],
	[State],
	[Zip],
	[Country],
	[_RecordUpdatedDate],
	[_DeletedFlag],
	[SKey]
FROM DimSite
WHERE SiteGroupFlag = 1