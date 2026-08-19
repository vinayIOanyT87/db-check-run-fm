CREATE FUNCTION [map].[udf_CheckUniquenessEmailGroup]
(@EmailGroupGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(30)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblEmailGroups e WHERE e.EmailGroupGuid = @EmailGroupGuid)
	IF 0 < (SELECT COUNT(*) FROM tblEmailGroups e 
	RIGHT JOIN map.tblEntityEmailGroupToSite em ON em.SiteGuid = @SiteGuid AND em.EmailGroupGuid = e.EmailGroupGuid 
	WHERE e.EmailGroupGuid <> @EmailGroupGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END

