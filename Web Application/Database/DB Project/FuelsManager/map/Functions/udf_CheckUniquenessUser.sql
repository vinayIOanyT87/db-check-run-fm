

CREATE FUNCTION [map].[udf_CheckUniquenessUser]
(@UserGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @UserID nvarchar(100)
			, @Exists bit
	SET @Exists = 1

	SET @UserID = (SELECT UserID FROM tblUsers e WHERE e.UserGuid = @UserGuid)
	IF 0 < (SELECT COUNT(*) FROM tblUsers e 
	RIGHT JOIN map.tblEntityUserToSite em ON em.SiteGuid = @SiteGuid AND em.UserGuid = e.UserGuid 
	WHERE e.UserGuid <> @UserGuid
	AND e.UserID = @UserID)
		SET @Exists = 0

	RETURN @Exists
END

