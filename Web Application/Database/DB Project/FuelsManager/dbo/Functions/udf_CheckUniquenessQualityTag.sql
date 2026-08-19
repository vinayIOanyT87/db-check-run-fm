

CREATE FUNCTION [dbo].[udf_CheckUniquenessQualityTag]
(@QualityTagGuid uniqueidentifier, @SiteGuid uniqueidentifier, @Name nvarchar(50))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblQualityTag
	IF 0 < (SELECT COUNT(*) FROM tblQualityTags e
	LEFT JOIN map.tblEntityQualityTagToSite em1 ON em1.QualityTagGuid = e.QualityTagGuid
	RIGHT JOIN map.tblEntityQualityTagToSite em2 ON em2.QualityTagGuid = @QualityTagGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.QualityTagGuid <> @QualityTagGuid
	AND Name = @Name)
		SET @Exists = 0

	RETURN @Exists
END

