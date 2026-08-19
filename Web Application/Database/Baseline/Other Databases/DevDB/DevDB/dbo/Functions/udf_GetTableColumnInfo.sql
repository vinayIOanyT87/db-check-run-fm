CREATE FUNCTION [dbo].[udf_GetTableColumnInfo]()
RETURNS TABLE AS RETURN

	SELECT	DISTINCT
			tab.name as TableName
		,	sch.name as SchemaName
		,	col.name as ColumnName
		,	typ.name as ColumnType
		,	col.Max_Length as ColumnLength
		,	col.is_nullable as IsNullable
		,	CASE WHEN icl.object_id IS NULL THEN '0' ELSE '1' END AS IsPrimaryKey
		,	col.Column_id as ColumnID
		,	CASE WHEN fky.ForeignKeyName IS NULL THEN 0 ELSE 1 END AS IsForeignKey
		,	col.Is_Identity as IsIdentity
		
	FROM	[$(FMDB)].sys.tables tab
	INNER JOIN [$(FMDB)].sys.schemas sch on sch.schema_id=tab.schema_id
	INNER JOIN [$(FMDB)].sys.columns col on col.object_id=tab.object_id
	INNER JOIN [$(FMDB)].sys.types typ on typ.user_type_id=col.user_type_id
	LEFT JOIN [$(FMDB)].sys.indexes idx on idx.object_id=tab.object_id
	LEFT JOIN [$(FMDB)].sys.index_columns icl on (icl.object_id=idx.object_id AND icl.index_id=idx.index_id and icl.column_id=col.column_id)
	LEFT JOIN dbo.udf_GetForeignKeyInfo() fky ON (fky.schemaname=sch.name AND fky.TableName=tab.name AND fky.ColumnName=col.name)
	WHERE idx.is_primary_key = 1