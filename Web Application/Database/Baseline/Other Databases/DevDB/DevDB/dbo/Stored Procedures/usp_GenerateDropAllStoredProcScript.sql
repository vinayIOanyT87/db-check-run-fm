
CREATE PROCEDURE [dbo].[usp_GenerateDropAllStoredProcScript]
AS
BEGIN
	DECLARE @Cmd NVARCHAR(max)
		,	@ProName NVARCHAR(500)
		,	@Schema NVARCHAR(200)

	DECLARE ProcCursor CURSOR FOR
		SELECT sc.name, pr.name 
		FROM FuelsManagerDB.sys.procedures pr
		INNER JOIN FuelsManagerDB.sys.schemas sc ON pr.schema_id=sc.schema_id
		WHERE pr.name like 'gsp_%'
		and sc.name IN('dbo','map')
		ORDER BY sc.name, pr.name
	OPEN ProcCursor
	FETCH NEXT FROM ProcCursor INTO @Schema,@ProName
	WHILE @@FETCH_STATUS=0
	BEGIN
		SET @Cmd = 'DROP PROC ['+@Schema+'].['+@ProName+']'
		PRINT @Cmd
		--EXEC sp_executesql @Cmd
		PRINT 'GO'
		FETCH NEXT FROM ProcCursor INTO @Schema,@ProName
	END
	CLOSE ProcCursor
	DEALLOCATE ProcCursor
END

