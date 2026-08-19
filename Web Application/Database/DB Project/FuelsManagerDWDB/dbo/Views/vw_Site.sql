/*
	DROP VIEW [dbo].[vw_Site]
*/

CREATE VIEW [dbo].[vw_Site] AS
  ------------------------------------------------------------------------------------------------------
  -- View: [dbo].[vw_Site]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: DimSite View to support the OLAP cube.
  ------------------------------------------------------------------------------------------------------
SELECT 
	[AKey] ,
	[SiteId],
	[City],
	[State],
	[Zip],
	[Country],
	[SiteGroupFlag],
	[_RecordUpdatedDate],
	[_DeletedFlag],
	[SKey]
FROM DimSite