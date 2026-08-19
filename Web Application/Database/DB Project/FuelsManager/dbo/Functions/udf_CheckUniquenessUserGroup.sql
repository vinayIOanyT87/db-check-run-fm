

CREATE FUNCTION [dbo].[udf_CheckUniquenessUserGroup]
(@GroupGuid uniqueidentifier, @SiteGuid uniqueidentifier, @GroupID nvarchar(30))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblGroup
	IF @GroupID IS NULL OR 0 < (SELECT COUNT(*) FROM tblGroups e
	LEFT JOIN map.tblEntityUserGroupToSite em1 ON em1.GroupGuid = e.GroupGuid
	RIGHT JOIN map.tblEntityUserGroupToSite em2 ON em2.GroupGuid = @GroupGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.GroupGuid <> @GroupGuid
	AND GroupID = @GroupID)
		SET @Exists = 0

	RETURN @Exists
END

