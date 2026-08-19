CREATE FUNCTION [dbo].[udf_GetForeignKeyInfo_New]()
RETURNS TABLE AS RETURN

	SELECT	sch.name as SchemaName
		,	tbl.name as TableName
		,	frk.name as ForeignKeyName
		--,	fko.name AS ForeignKeyName
		,	rsc.name as ReferencedSchemaName
		,	rft.name as ReferencedTableName
		,	par.name as ColumnName
		,	typ.name as ColumnType
		,	CASE delete_referential_action WHEN 0 THEN 'No' ELSE 'Yes' END AS IsDeleteCascade
		,	CASE update_referential_action WHEN 0 THEN 'No' ELSE 'Yes' END AS IsUpdateCascade
		,	del.name as DeleteColumnName
	from 
			[$(FMDB)].sys.foreign_key_columns fkc
	inner join [$(FMDB)].sys.foreign_keys frk on frk.object_id=fkc.constraint_object_id
	inner join [$(FMDB)].sys.tables tbl on tbl.object_id=fkc.parent_object_id
	inner join [$(FMDB)].sys.schemas sch on sch.schema_id=tbl.schema_id
	inner join [$(FMDB)].sys.tables rft on rft.object_id=fkc.referenced_object_id
	
	inner join [$(FMDB)].sys.columns clm on (clm.object_id=fkc.referenced_object_id and clm.column_id=fkc.referenced_column_id)
	inner join [$(FMDB)].sys.columns par on (par.object_id=fkc.parent_object_id and par.column_id=fkc.parent_column_id)
	inner join [$(FMDB)].sys.types typ on typ.system_type_id=par.system_type_id
	left join [$(FMDB)].sys.columns del on (del.object_id=tbl.object_id and del.name IN('DeleteFlag','_DeleteFlag'))
	left join [$(FMDB)].sys.schemas rsc on rsc.Schema_id=rft.Schema_id