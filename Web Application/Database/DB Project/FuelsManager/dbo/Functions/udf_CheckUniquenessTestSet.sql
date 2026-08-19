
CREATE FUNCTION [dbo].[udf_CheckUniquenessTestSetDefinition]
(@TestSetDefinitionGuid uniqueidentifier, @SiteGuid uniqueidentifier, @TestSetName nvarchar(80))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblTestSetDefinition
	IF 0 < (SELECT COUNT(*) FROM tblTestSetDefinitions e
	LEFT JOIN map.tblEntityTestSetToSite em1 ON em1.TestSetDefinitionGuid = e.TestSetDefinitionGuid
	RIGHT JOIN map.tblEntityTestSetToSite em2 ON em2.TestSetDefinitionGuid = @TestSetDefinitionGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.TestSetDefinitionGuid <> @TestSetDefinitionGuid
	AND TestSetName = @TestSetName)
		SET @Exists = 0

	RETURN @Exists
END

