
CREATE PROCEDURE [dbo].[usp_UsersList]
@LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER
AS
SET NOCOUNT ON

	SELECT dbo.tblUsers.UserGuid, dbo.tblUsers.UserID
	  FROM dbo.tblUsers WHERE dbo.tblUsers.SiteGuid = @LoginSiteGuid OR dbo.tblUsers.SiteGuid = @LoginSiteGuid
	 ORDER BY dbo.tblUsers.UserID