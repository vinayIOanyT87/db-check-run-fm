CREATE PROC [dbo].[usp_GenerateInsertScriptForLookup]
AS
BEGIN
	DECLARE @TableName VARCHAR(1000)
		,	@TableId INT
		,	@ColumnName VARCHAR(1000)
		,	@ColumnType VARCHAR(500)
		,	@Command NVARCHAR(4000)
		
		,	@Index INT
		,	@Code NVARCHAR(100)
		,	@Name NVARCHAR(100)
		,	@Guid UNIQUEIDENTIFIER
		,	@User NVARCHAR(100)
		,	@DatabaseType NVARCHAR(100)

		
		,	@LineReturn NVARCHAR(50)
		,	@ColPrefix NVARCHAR(500)
		,	@RecordIndex INT
		,	@Parameters NVARCHAR(4000)
		,	@RecordCount INT
		,	@TotalRecords INT
		,	@Date as DATETIME

	SET @LineReturn = CHAR(13)+CHAR(10)
	SET @Date = '1900-01-01'

	SET @User='Administrator'
	SET @RecordIndex=0

	-- GET LIST OF TABLES
	DECLARE TableCursor CURSOR FOR
		SELECT	tab.name AS TableName
			,	tab.object_id as TableId
		FROM FuelsManagerDB.sys.tables tab
		INNER JOIN FuelsManagerDB.sys.schemas sch on sch.schema_id=tab.schema_id
		WHERE sch.name = 'Lookup'
		AND tab.name <>'tblCompanyCrossReferenceMap'
		ORDER BY tab.name

	OPEN TableCursor
	FETCH NEXT FROM TableCursor INTO @TableName,@TableId
	WHILE @@FETCH_STATUS=0
	BEGIN
		-- GET TOTAL NUMBER OF RECORDS FOR THE TABLE
		SET @ColPrefix = REPLACE(@TableName,'tbl','')
		SET @Parameters='@TotalRecords INT OUTPUT'
		SET @RecordCount=1
		SET @TotalRecords=0
		SET @RecordIndex=0
		SET @Command='SELECT @TotalRecords=COUNT(*) FROM [FuelsManagerDB].[lookup].['+@TableName+']; '
		EXEC sp_executesql @Statment=@Command, @Params=@Parameters,@TotalRecords=@TotalRecords OUTPUT
		WHILE @RecordCount<=@TotalRecords
		BEGIN
			-- ONE RECORD AT TIME AND BUILD INSERT SQL
			SET @Command= ' SELECT @RecordIndex=MIN(['+@ColPrefix+'Index]) FROM [FuelsManagerDB].[lookup].['+@TableName+'] WHERE ['+@ColPrefix+'Index] >=@RecordIndex '
			SET @ParameterS='@RecordIndex INT OUTPUT'
			EXEC sp_executesql @Statment=@command, @Params=@ParameterS,@RecordIndex=@RecordIndex OUTPUT
			
			--GET VALUES FROM THE RECORD
			SET @Code=NULL
			SET @Name=NULL
			SET @Guid=NULL
			IF @TableName <> 'tblVariantType'
			BEGIN
				SET @Command	= 'SELECT	@Code=['+@ColPrefix+'Code],
											@Name=['+@ColPrefix+'Name],
											@Guid=['+@ColPrefix+'Guid] 
									FROM [FuelsManagerDB].[lookup].['+@TableName+'] WHERE ['+@ColPrefix+'Index]=@RecordIndex '
				SET @Parameters='@Code NVARCHAR(100) OUTPUT,@Name NVARCHAR(100) output,@Guid UNIQUEIDENTIFIER OUTPUT,@RecordIndex INT'
				EXEC sp_executesql @Statment=@Command,@Params=@Parameters,@Code=@Code OUTPUT,@Name=@Name OUTPUT,@Guid=@Guid OUTPUT,@RecordIndex=@RecordIndex
				
				--BUILD INSERT STATEMENT
				SET @Command= 'INSERT INTO [lookup].['+@TableName+'](['+@ColPrefix+'Index],['+@ColPrefix+'Code],['+@ColPrefix+'Name],['+@ColPrefix+'Guid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) '+@LineReturn
				SET @Command +='VALUES('+CAST(@RecordIndex AS VARCHAR(10))+','''+@Code+''','''+@Name+''','''+CAST(@Guid AS VARCHAR(80))+''','''+CAST(@Date AS VARCHAR(50))+''','''+@User+''','''+CAST(@Date AS VARCHAR(50))+''','''+@User+''')'+@LineReturn+'GO'+@LineReturn+@LineReturn
				PRINT @Command
				
			END
			ELSE
			BEGIN
				SET @Command	= 'SELECT	@Code=[CodeType],
											@DatabaseType=[DatabaseType],
											@Guid=['+@ColPrefix+'Guid] 
									FROM [FuelsManagerDB].[lookup].['+@TableName+'] WHERE ['+@ColPrefix+'Index]=@RecordIndex '
				SET @Parameters='@Code NVARCHAR(100) OUTPUT,@DatabaseType NVARCHAR(100) output,@Guid UNIQUEIDENTIFIER OUTPUT,@RecordIndex INT'
				EXEC sp_executesql @Statment=@Command,@Params=@Parameters,@Code=@Code OUTPUT,@DatabaseType=@DatabaseType OUTPUT,@Guid=@Guid OUTPUT,@RecordIndex=@RecordIndex
				
				--BUILD INSERT STATEMENT
				SET @Command= 'INSERT INTO [lookup].['+@TableName+'](['+@ColPrefix+'Index],[CodeType],[DatabaseType],['+@ColPrefix+'Guid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy]) '+@LineReturn
				SET @Command +='VALUES('+CAST(@RecordIndex AS VARCHAR(10))+','''+@Code+''','''+@DatabaseType+''','''+CAST(@Guid AS VARCHAR(80))+''','''+CAST(@Date AS VARCHAR(50))+''','''+@User+''','''+CAST(@Date AS VARCHAR(50))+''','''+@User+''')'+@LineReturn+'GO'+@LineReturn+@LineReturn
				PRINT @Command
			
			END
			
			SET @RecordIndex += 1
			SET @RecordCount += 1
		END 

		FETCH NEXT FROM TableCursor INTO @TableName,@TableId
	END	
	CLOSE TableCursor
	DEALLOCATE TableCursor
END
