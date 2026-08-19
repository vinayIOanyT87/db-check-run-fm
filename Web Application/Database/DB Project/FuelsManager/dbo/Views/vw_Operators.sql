

CREATE VIEW [dbo].[vw_Operators]
WITH SCHEMABINDING
AS
SELECT     UserGuid AS ID, UserID AS Name, Password, LastLoginDate AS LastLoginTime, LastLogoffDate AS LastLogoffTime, SiteGuid
FROM         dbo.tblUsers;