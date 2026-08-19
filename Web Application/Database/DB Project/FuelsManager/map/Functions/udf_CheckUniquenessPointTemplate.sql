CREATE FUNCTION [map].[udf_CheckUniquenessPointTemplate]
(@PointTemplateGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(100)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblPointTemplate e WHERE e.PointTemplateGuid = @PointTemplateGuid)
	IF 0 < (SELECT COUNT(*) FROM tblPointTemplate e 
	RIGHT JOIN map.tblEntityPointTemplateToSite em ON em.SiteGuid = @SiteGuid AND em.PointTemplateGuid = e.PointTemplateGuid 
	WHERE e.PointTemplateGuid <> @PointTemplateGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END