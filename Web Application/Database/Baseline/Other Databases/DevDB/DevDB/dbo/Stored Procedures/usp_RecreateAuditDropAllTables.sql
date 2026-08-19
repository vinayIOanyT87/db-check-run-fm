CREATE PROCEDURE [dbo].[usp_RecreateAuditDropAllTables]
AS
BEGIN

	DECLARE @ObjName NVARCHAR(500)
		,	@Sql NVARCHAR(max)
	DECLARE ObjCursor CURSOR FOR
		SELECT table_name
		FROM FuelsManagerDB.information_schema.tables
		WHERE TABLE_SCHEMA='fmaudit'
		ORDER BY table_name
	OPEN ObjCursor
	FETCH NEXT FROM ObjCursor INTO @ObjName
	WHILE @@FETCH_STATUS=0
	BEGIN
		SET @Sql= 'Drop TABLE [fmaudit].['+@ObjName+']'
		PRINT @Sql
		PRINT 'GO'
		FETCH NEXT FROM ObjCursor INTO @ObjName
	END
	CLOSE ObjCursor
	DEALLOCATE ObjCursor
END
