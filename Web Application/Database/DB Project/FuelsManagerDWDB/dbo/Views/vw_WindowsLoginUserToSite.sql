/*
	DROP VIEW [dbo].[vw_WindowsLoginUserToSite]
*/
CREATE VIEW [dbo].[vw_WindowsLoginUserToSite] AS
  ------------------------------------------------------------------------------------------------------
  -- View: [dbo].[vw_WindowsLoginUserToSite]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: DimWindowsLoginUser-to-FMUser View for Sites to support the OLAP cube.
  ------------------------------------------------------------------------------------------------------
SELECT b.WindowsLoginUserSKey, c.WindowsLoginUserId, a.FMUserSKey, d.SKey SiteSKey
FROM [dbo].[FactFMUserToSite] a
INNER JOIN FactWindowsLoginUserToFMUser b
ON b.FMUserSKey = a.FMUserSKey
INNER JOIN DimWindowsLoginUser c
ON c.SKey = b.WindowsLoginUserSKey
INNER JOIN vw_Site d
ON d.SKey = a.SiteSKey