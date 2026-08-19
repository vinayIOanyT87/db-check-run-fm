CREATE FUNCTION [dbo].[udf_CheckUniquenessEmailGroup]
(@EmailGroupGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(80))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblEmailGroup
	IF 0 < (SELECT COUNT(*) FROM tblEmailGroups e
	LEFT JOIN map.tblEntityEmailGroupToSite em1 ON em1.EmailGroupGuid = e.EmailGroupGuid
	RIGHT JOIN map.tblEntityEmailGroupToSite em2 ON em2.EmailGroupGuid = @EmailGroupGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.EmailGroupGuid <> @EmailGroupGuid
	AND ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END

