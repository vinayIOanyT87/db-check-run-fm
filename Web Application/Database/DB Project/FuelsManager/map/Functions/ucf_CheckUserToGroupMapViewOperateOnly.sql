CREATE FUNCTION [map].[udf_CheckUserToGroupMapViewOperateOnly]
(@UserGuid uniqueidentifier, @GroupGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ViewOperateOnly bit
	SET @ViewOperateOnly = 0


	IF @UserGuid = '00000000-0000-0000-0000-000000000002' AND 0 < (SELECT COUNT(*) FROM map.tblGroupToRight gtr
	WHERE GroupGuid = @GroupGuid AND LookupRightIndex = 323) -- ViewOperateOnly
		SET @ViewOperateOnly = 1

	RETURN @ViewOperateOnly
END

