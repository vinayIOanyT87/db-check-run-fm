

CREATE FUNCTION [map].[udf_CheckUniquenessUserGroup]
(@GroupGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @GroupID nvarchar(100)
			, @Exists bit
	SET @Exists = 1

	SET @GroupID = (SELECT GroupID FROM tblGroups e WHERE e.GroupGuid = @GroupGuid)
	IF @GroupID IS NULL OR 0 < (SELECT COUNT(*) FROM tblGroups e 
	RIGHT JOIN map.tblEntityUserGroupToSite em ON em.SiteGuid = @SiteGuid AND em.GroupGuid = e.GroupGuid 
	WHERE e.GroupGuid <> @GroupGuid
	AND e.GroupID = @GroupID)
		SET @Exists = 0

	RETURN @Exists
END
