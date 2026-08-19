

CREATE FUNCTION [dbo].[udf_CheckUniquenessTestDefinition]
(@TestDefinitionGuid uniqueidentifier, @SiteGuid uniqueidentifier, @TestName nvarchar(80))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblTestDefinition
	IF 0 < (SELECT COUNT(*) FROM tblTestDefinitions e
	LEFT JOIN map.tblEntityTestToSite em1 ON em1.TestDefinitionGuid = e.TestDefinitionGuid
	RIGHT JOIN map.tblEntityTestToSite em2 ON em2.TestDefinitionGuid = @TestDefinitionGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.TestDefinitionGuid <> @TestDefinitionGuid
	AND TestName = @TestName)
		SET @Exists = 0

	RETURN @Exists
END

