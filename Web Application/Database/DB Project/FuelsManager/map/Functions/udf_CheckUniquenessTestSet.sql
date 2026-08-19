
CREATE FUNCTION [map].[udf_CheckUniquenessTestSet]
(@TestSetDefinitionGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @TestSetName nvarchar(80)
			, @Exists bit
	SET @Exists = 1

	SET @TestSetName = (SELECT TestSetName FROM tblTestSetDefinitions e WHERE e.TestSetDefinitionGuid = @TestSetDefinitionGuid)
	IF 0 < (SELECT COUNT(*) FROM tblTestSetDefinitions e 
	RIGHT JOIN map.tblEntityTestSetToSite em ON em.SiteGuid = @SiteGuid AND em.TestSetDefinitionGuid = e.TestSetDefinitionGuid 
	WHERE e.TestSetDefinitionGuid <> @TestSetDefinitionGuid
	AND e.TestSetName = @TestSetName)
		SET @Exists = 0

	RETURN @Exists
END

