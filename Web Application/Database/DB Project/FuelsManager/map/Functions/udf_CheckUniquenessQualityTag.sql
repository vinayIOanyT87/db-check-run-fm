

CREATE FUNCTION [map].[udf_CheckUniquenessQualityTag]
(@QualityTagGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @Name nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @Name = (SELECT Name FROM tblQualityTags e WHERE e.QualityTagGuid = @QualityTagGuid)
	IF 0 < (SELECT COUNT(*) FROM tblQualityTags e 
	RIGHT JOIN map.tblEntityQualityTagToSite em ON em.SiteGuid = @SiteGuid AND em.QualityTagGuid = e.QualityTagGuid 
	WHERE e.QualityTagGuid <> @QualityTagGuid
	AND e.Name = @Name)
		SET @Exists = 0

	RETURN @Exists
END