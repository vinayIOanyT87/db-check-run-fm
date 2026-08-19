CREATE FUNCTION [dbo].[udf_GetPrimaryKeyInfo]()
RETURNS TABLE AS RETURN
	select	sch.name as SchemaName
		,	tbl.name as TableName
		,	tix.name as IndexName
		,	CASE is_primary_key WHEN 1 THEN 'Yes' ELSE '' END AS IsPrimaryKey
		,	cln.name as ColumnName 
		,	del.name as DeleteColumnName
	from [$(FMDB)].sys.tables tbl
	inner join [$(FMDB)].sys.schemas sch on sch.schema_id=tbl.schema_id
	inner join [$(FMDB)].sys.columns cln on cln.object_id=tbl.object_id
	inner join [$(FMDB)].sys.indexes tix on tix.object_id=tbl.object_id
	inner join [$(FMDB)].sys.index_columns cix on (--cix.object_id=tix.object_id and
	cix.index_id=tix.index_id and cix.object_id=cln.object_id and cix.column_id=cln.column_id)
	left join [$(FMDB)].sys.columns del on (del.object_id=tbl.object_id and del.name IN('DeleteFlag','_DeleteFlag'))
	where is_primary_key = 1