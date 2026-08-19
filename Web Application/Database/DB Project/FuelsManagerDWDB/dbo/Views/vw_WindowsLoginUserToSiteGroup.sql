/*
	DROP VIEW [dbo].[v_wWindowsLoginUserToSiteGroup]
*/

CREATE VIEW [dbo].[vw_WindowsLoginUserToSiteGroup] AS
  ------------------------------------------------------------------------------------------------------
  -- View: [dbo].[vw_WindowsLoginUserToSiteGroup]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: DimWindowsLoginUser-to-FMUser View for SiteGroups to support the OLAP cube.
  ------------------------------------------------------------------------------------------------------
SELECT b.WindowsLoginUserSKey, c.WindowsLoginUserId, a.FMUserSKey, d.SKey SiteGroupSKey
FROM [dbo].[FactFMUserToSite] a
INNER JOIN dbo.FactWindowsLoginUserToFMUser b
ON b.FMUserSKey = a.FMUserSKey
INNER JOIN DimWindowsLoginUser c
ON c.SKey = b.WindowsLoginUserSKey
INNER JOIN dbo.vw_SiteGroup d
ON d.SKey = a.SiteSKey