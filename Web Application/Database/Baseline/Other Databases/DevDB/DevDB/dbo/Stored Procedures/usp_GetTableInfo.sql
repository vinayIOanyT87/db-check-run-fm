CREATE PROCEDURE [dbo].[usp_GetTableInfo]
AS
BEGIN
	SELECT	SchemaName
		,	TableName
		,	ColumnName
		,	ColumnType
		,	ColumnLength
		,	IsNullable
		,	IsPrimaryKey
	FROM DevDB.dbo.udf_GetTableColumnInfo()
	WHERE SchemaName = 'dbo'
	ORDER BY SchemaName,TableName,(CASE WHEN ColumnName = '_RowVersion' THEN 9999 WHEN IsPrimaryKey='1' THEN 0 ELSE ColumnID END)
END