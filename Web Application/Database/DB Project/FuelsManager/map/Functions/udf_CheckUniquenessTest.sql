

CREATE FUNCTION [map].[udf_CheckUniquenessTest]
(@TestDefinitionGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @TestName nvarchar(80)
			, @Exists bit
	SET @Exists = 1

	SET @TestName = (SELECT TestName FROM tblTestDefinitions e WHERE e.TestDefinitionGuid = @TestDefinitionGuid)
	IF 0 < (SELECT COUNT(*) FROM tblTestDefinitions e 
	RIGHT JOIN map.tblEntityTestToSite em ON em.SiteGuid = @SiteGuid AND em.TestDefinitionGuid = e.TestDefinitionGuid 
	WHERE e.TestDefinitionGuid <> @TestDefinitionGuid
	AND e.TestName = @TestName)
		SET @Exists = 0

	RETURN @Exists
END
