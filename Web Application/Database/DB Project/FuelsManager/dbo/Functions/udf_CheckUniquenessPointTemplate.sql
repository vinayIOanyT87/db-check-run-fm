CREATE FUNCTION [dbo].[udf_CheckUniquenessPointTemplate]
(@PointTemplateGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(30))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblPointTemplate
	IF 0 < (SELECT COUNT(*) FROM tblPointTemplate e
	LEFT JOIN map.tblEntityPointTemplateToSite em1 ON em1.PointTemplateGuid = e.PointTemplateGuid
	RIGHT JOIN map.tblEntityPointTemplateToSite em2 ON em2.PointTemplateGuid = @PointTemplateGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.PointTemplateGuid <> @PointTemplateGuid
	AND ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
