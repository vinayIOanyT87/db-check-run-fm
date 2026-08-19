/*
	DROP VIEW [dbo].[vw_WindowsLoginUserToCompany]
*/
CREATE VIEW [dbo].[vw_WindowsLoginUserToCompany] AS
  ------------------------------------------------------------------------------------------------------
  -- View: [dbo].[vw_WindowsLoginUserToCompany]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: DimWindowsLoginUser View to support the OLAP cube.
  ------------------------------------------------------------------------------------------------------
SELECT b.WindowsLoginUserSKey, c.WindowsLoginUserId, a.FMUserSKey, a.CompanySKey 
FROM [dbo].[FactFMUserToCompany] a
INNER JOIN FactWindowsLoginUserToFMUser b
ON b.FMUserSKey = a.FMUserSKey
INNER JOIN DimWindowsLoginUser c
ON c.SKey = b.WindowsLoginUserSKey