

CREATE FUNCTION [dbo].[udf_CheckUniquenessUser]
(@UserGuid uniqueidentifier, @SiteGuid uniqueidentifier, @UserID nvarchar(30))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblUser
	IF 0 < (SELECT COUNT(*) FROM tblUsers e
	LEFT JOIN map.tblEntityUserToSite em1 ON em1.UserGuid = e.UserGuid
	RIGHT JOIN map.tblEntityUserToSite em2 ON em2.UserGuid = @UserGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.UserGuid <> @UserGuid
	AND UserID = @UserID)
		SET @Exists = 0

	RETURN @Exists
END

