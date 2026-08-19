
CREATE VIEW [dbo].[vw_DatabaseDictionaryKeyValue]
AS
	-- TABLE NAME
	SELECT DISTINCT tb.name AS [Key]
				,	dbo.udf_GetDisplayName(tb.name,1) AS [Value]
	FROM sys.tables tb
	INNER JOIN sys.schemas sc ON tb.schema_id=sc.schema_id
	WHERE sc.name IN('dbo','map')
	UNION ALL
	SELECT DISTINCT cl.name as [KEY]
				,	dbo.udf_GetDisplayName(cl.name,1) AS [Value]
	FROM sys.tables tb
	INNER JOIN sys.schemas sc ON sc.schema_id=tb.schema_id
	INNER JOIN sys.columns cl ON cl.object_id=tb.object_id
	WHERE sc.name IN('dbo','map')
	AND LEFT(cl.name,1) <> '_'
	AND cl.name NOT LIKE('%GUID%')
	AND cl.name NOT LIKE('%LOOKUP%')
