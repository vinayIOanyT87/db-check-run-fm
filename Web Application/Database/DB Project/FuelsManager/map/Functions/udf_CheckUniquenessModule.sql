CREATE FUNCTION [map].[udf_CheckUniquenessModule]
(@ModuleGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(100)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblModule e WHERE e.ModuleGuid = @ModuleGuid)
	IF 0 < (SELECT COUNT(*) FROM tblModule e 
	RIGHT JOIN map.tblEntityModuleToSite em ON em.SiteGuid = @SiteGuid AND em.ModuleGuid = e.ModuleGuid 
	WHERE e.ModuleGuid <> @ModuleGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END