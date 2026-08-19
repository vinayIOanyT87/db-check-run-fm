CREATE FUNCTION [map].[udf_CheckGroupToRightMapViewOperateOnly]
(@GroupGuid uniqueidentifier, @LookupRightIndex int)
RETURNS BIT
AS
BEGIN
	DECLARE @ViewOperateOnly bit
	SET @ViewOperateOnly = 0


	IF @LookupRightIndex = 323 AND 0 < (SELECT COUNT(*) FROM map.tblUserToGroup utg
	WHERE GroupGuid = @GroupGuid AND UserGuid = '00000000-0000-0000-0000-000000000002')
		SET @ViewOperateOnly = 1

	RETURN @ViewOperateOnly
END


