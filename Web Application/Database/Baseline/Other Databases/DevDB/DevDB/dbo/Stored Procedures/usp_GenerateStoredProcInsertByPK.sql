


CREATE PROCEDURE [dbo].[usp_GenerateStoredProcInsertByPK](@TableNameList NVARCHAR(MAX)=NULL)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @CreateTemplate VARCHAR(8000)
		,	@ParemeterListTemplate VARCHAR(8000)
		,	@NullOverrideTemplate VARCHAR(8000)
		,	@NoteTemplate VARCHAR(8000)
		,	@ValidateTemplate VARCHAR(8000)
		,	@InsertFieldsTemplate VARCHAR(8000)
		,	@InsertCriteriaTemplate VARCHAR(8000)
		,	@ExceptionTemplate	VARCHAR(8000)
		,	@RefreshRowVersionTemplate VARCHAR(8000)
		,	@LineReturn VARCHAR(10)
		,	@Tab VARCHAR(10)
		,	@TableExceptionList VARCHAR(max)
		,	@Version VARCHAR(50)

	DECLARE @Create VARCHAR(8000)
		,	@ParemeterList VARCHAR(8000)
		,	@NullOverrideList VARCHAR(8000)
		,	@Note VARCHAR(8000)
		,	@Validate VARCHAR(8000)
		,	@InsertFields VARCHAR(8000)
		,	@Exception	VARCHAR(8000)
		,	@ParameterLine VARCHAR(8000)
		,	@NullOverrideParLine VARCHAR(8000)
		,	@InsertLine VARCHAR(8000)
		,	@InsertCriteria VARCHAR(8000)
		,	@RefreshRowVersion VARCHAR(8000)
		,	@InsertBlock VARCHAR(8000)
		,	@ParameterCount AS INT
		,	@TotalParameters AS INT
		,	@ParameterDef VARCHAR(8000)
		
		
	DECLARE 
			@SchemaName VARCHAR(100)
		,	@TableName VARCHAR(300)
		,	@ColumnName VARCHAR(300)
		,	@ColumnType VARCHAR(100)
		,	@ColumnLength INT
		,	@IsNullable BIT
		,	@IsPrimaryKey BIT
		,	@IsForeignKey BIT
		,	@HasRowVersion BIT
		,	@IsIdentity BIT
		,	@WorkingTable VARCHAR(300)
		,	@WorkingSchemaName VARCHAR(100)
		,	@ColPkName NVARCHAR(300)
		,	@RowNumber INT
		,	@TotalRows INT
		,	@TotalOverrides INT
		,	@OverrideCount INT
	/********************************************/

	SET @Version = '1.0.001'

	/********************************************/

	SET @TableExceptionList = 'tblB2BResults,tblSessionToSQLProcess'

--	SET @CreateTemplate =
--	'IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N''[#SCHEMA#].[gsp_#LOGICAL_TABLE#InsertByPK]'') AND type in (N''P'', N''PC''))      
--	DROP PROCEDURE [#SCHEMA#].[gsp_#LOGICAL_TABLE#InsertByPK];  
--GO 

SET @CreateTemplate = '	
CREATE PROCEDURE [#SCHEMA#].[gsp_#LOGICAL_TABLE#InsertByPK]
('
	
	
	SET @ParemeterListTemplate =
	'	#PAREMETER_LIST#
'
	
	
	
	SET @NoteTemplate =
'	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [#SCHEMA#].[gsp_#LOGICAL_TABLE#InsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: #VERSION# / #DATE#
	-- Purpose: Insert into table [#SCHEMA#].[#PHISICAL_TABLE#]
	-- Notes:
	------------------------------------------------------------------------------------------------------'
		

	SET @InsertFieldsTemplate =
	'		SET @#COL_GUID#=NEWID();
			#CREATEDDATE#;
			INSERT INTO	[#SCHEMA#].[#PHYSICALTABLENAME#]        
			('

				
	SET @RefreshRowVersionTemplate = 
	'		SELECT @_RowVersion=_RowVersion        
		FROM [#SCHEMA#].[#PHYSICALTABLENAME#]           
		WHERE #COL_GUID#=@#COL_GUID#;
	'
	SET @ExceptionTemplate=
	'	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = ''Error: '' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ ''Number: '' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ ''Procedure Name: gsp_#LOGICAL_TABLE#InsertByPK'' + CHAR(13)+CHAR(10)                  
						+ ''Line Number: '' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'''') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	'
	DECLARE @ParamenterTable TABLE(RowNumber INT IDENTITY,ParameterDefinition VARCHAR(8000))
	CREATE TABLE #InsertLineTable (RowNumber INT IDENTITY, UpdateLineDefinition VARCHAR(8000))
	CREATE TABLE #ParameterTable (RowVersion INT IDENTITY, ParameterDefinition VARCHAR(8000))


	SET @WorkingTable = ''
	SET @WorkingSchemaName = ''
	SET @LineReturn = CHAR(13)+CHAR(10)
	SET @Tab= CHAR(9)	

	--dbo.udf_SplitTableName(@TableNameList) stn ON (stn.TableName=fki.TableName)
	IF @TableNameList IS NULL
	BEGIN
		DECLARE TableInfoCursor CURSOR FOR
			SELECT	SchemaName
				,	TableName
				,	ColumnName
				,	ColumnType
				,	ColumnLength
				,	IsNullable
				,	IsPrimaryKey
				,	IsForeignKey
				,	IsIdentity

			FROM dbo.udf_GetTableColumnInfo() ta
			WHERE ta.SchemaName IN('dbo','map')
			--AND ta.ColumnName NOT LIKE 'Created%'
			AND ta.TableName NOT IN ('tblAuditLog','tblCompanyCrossReferenceMap','tblSessionToSQLProcess')
			AND ta.TableName NOT LIKE 'tblEntity%'
			ORDER BY SchemaName,TableName,(CASE WHEN ColumnName = '_RowVersion' THEN 9999 WHEN IsPrimaryKey='1' THEN 0 ELSE ColumnID END)
	END
	ELSE
	BEGIN
		DECLARE TableInfoCursor CURSOR FOR
			SELECT	tab.SchemaName
				,	tab.TableName
				,	tab.ColumnName
				,	tab.ColumnType
				,	tab.ColumnLength
				,	tab.IsNullable
				,	tab.IsPrimaryKey
				,	tab.IsForeignKey
				,	tab.IsIdentity

			FROM dbo.udf_GetTableColumnInfo() tab
			INNER JOIN dbo.udf_SplitTableName(@TableNameList) stn ON (stn.TableName=tab.TableName)
			WHERE tab.SchemaName IN('dbo','map')
			--AND tab.ColumnName NOT LIKE 'Created%'
			AND tab.TableName NOT IN ('tblAuditLog','tblSessionToSQLProcess')	
			AND tab.TableName NOT LIKE 'tblEntity%'		
			ORDER BY tab.SchemaName,tab.TableName,(CASE WHEN tab.ColumnName = '_RowVersion' THEN 9999 WHEN tab.IsPrimaryKey='1' THEN 0 ELSE tab.ColumnID END)	
	END

		
	OPEN TableInfoCursor
	FETCH NEXT FROM TableInfoCursor INTO
		@SchemaName,@TableName,@ColumnName,@ColumnType,@ColumnLength,@IsNullable,@IsPrimaryKey,@IsForeignKey,@IsIdentity
	WHILE @@FETCH_STATUS=0
	BEGIN

		IF @WorkingTable <> @TableName
		BEGIN
			SET @Create = @CreateTemplate
			SET @ParemeterList = @ParemeterListTemplate
			SET @Validate = @ValidateTemplate
			SET @InsertFields = @InsertFieldsTemplate
			SET @InsertCriteria = @InsertCriteriaTemplate
			SET @Exception	= @ExceptionTemplate
			SET	@RefreshRowVersion= @RefreshRowVersionTemplate
			
			SET @Note=@NoteTemplate

			SET @WorkingTable=@TableName
			SET @WorkingSchemaName=@SchemaName
			SET @HasRowVersion = 0


			SET @Create=REPLACE(@Create,'#SCHEMA#',@WorkingSchemaName)
			SET @Create=REPLACE(@Create,'#LOGICAL_TABLE#',REPLACE(@TableName,'tbl',''))
		
		
			SET @Exception=REPLACE(@Exception,'#LOGICAL_TABLE#',REPLACE(@TableName,'tbl',''))
				
			SET @Validate=REPLACE(@Validate,'#SCHEMA#',@WorkingSchemaName)
			SET @Validate=REPLACE(@Validate,'#PHISICAL_TABLE#',@TableName)

			SET @Note=REPLACE(@Note,'#SCHEMA#',@WorkingSchemaName)
			SET @Note=REPLACE(@Note,'#LOGICAL_TABLE#',REPLACE(@TableName,'tbl',''))
			SET @Note=REPLACE(@Note,'#PHISICAL_TABLE#',@TableName)
			SET	@Note=REPLACE(@Note,'#VERSION#',@Version)
			SET @Note=REPLACE(@Note,'#DATE#',CAST(SYSDATETIMEOFFSET() AS VARCHAR(50)))
			SET @ColPkName = ''
			PRINT @Create
		END
		SET @ParameterLine = NULL

		SET	@InsertLine = NULL
		SET @NullOverrideParLine = NULL
		SET @InsertBlock = ''
		--IF @IsPrimaryKey=1
		--	SET @ColPkName = @ColumnName
		
		TRUNCATE TABLE #InsertLineTable
		TRUNCATE TABLE #ParameterTable
		
		SET @RowNumber=0
		SET @TotalRows = 0
		SET @ParameterCount=0
		SET @TotalParameters = 0
		
		SET @ColPkName = NULL
		WHILE @WorkingTable=@TableName AND @@FETCH_STATUS=0
		BEGIN
						
			IF @IsPrimaryKey=1
				SET @ColPkName = @ColumnName

			-- BUILD PARAMETER LIST
			IF @TotalParameters = 0
				SET @ParameterLine = '	@'+@ColumnName+' ' + REPLACE(@ColumnType,'timestamp','BINARY(8)')
			ELSE
				SET @ParameterLine = ',	@'+@ColumnName+' ' + @ColumnType
				
			IF @ColumnType IN ('VARCHAR','NVARCHAR','VARBINARY')
				IF @ColumnLength <> -1
					SET @ParameterLine += '(' + CAST(@ColumnLength/2 AS VARCHAR(5)) + ')'
				ELSE
					SET @ParameterLine += '(max)'
			IF @ColumnType = 'DATETIMEOFFSET'
				SET @ParameterLine += '(7)'
			IF @IsPrimaryKey=0 --AND @ColumnName <> 'UpdatedBy'
				SET @ParameterLine += '=NULL'
				
			IF @ColumnName = '_RowVersion' 
			BEGIN
				SET @ParameterLine += ' OUTPUT'
				SET @HasRowVersion = 1
			END

			IF @IsPrimaryKey = 1			
			BEGIN
				SET @ParameterLine += '=NULL OUTPUT'
			END

			INSERT INTO #ParameterTable(ParameterDefinition) VALUES(@ParameterLine)
			SET @TotalParameterS += 1;
			
			
			-- BUILD INSERT COLUMNS
			IF @IsIdentity = 0
			BEGIN
				IF (@ColumnName<>'_RowVersion') -- (@IsPrimaryKey = 0) AND 
				BEGIN
					IF @InsertLine IS NULL
					BEGIN
						SET @InsertLine = '	' 
					END
					ELSE
					BEGIN
						SET @InsertLine += ',	' 
					END
					IF @IsForeignKey=1
					BEGIN	
						SET @InsertLine += '[@'+@ColumnName + ']'

					END
					ELSE
					BEGIN
						SET @InsertLine += '[@'+@ColumnName + ']'
					END	
					
					-- print @InsertLine
					
					IF @InsertLine != ''
						
						INSERT INTO #InsertLineTable(UpdateLineDefinition) 
						VALUES(@InsertLine)
						SET @TotalRows += 1;
					

						
					SET @InsertLine = NULL
					

				END
			END
			--IF @IsPrimaryKey = 1
			--BEGIN
			--	SET @IsPrimaryKey =0
				
			FETCH NEXT FROM TableInfoCursor INTO
				@SchemaName,@TableName,@ColumnName,@ColumnType,@ColumnLength,@IsNullable,@IsPrimaryKey,@IsForeignKey,@IsIdentity
		END  

	
		SET @ParemeterList = REPLACE(@ParemeterList,'#PAREMETER_LIST#',@ParameterLine)
		SET @InsertFields = REPLACE(@InsertFields,'#COL_GUID#',@ColPkName)
		SET @InsertFields = REPLACE(@InsertFields,'#SCHEMA#',@WorkingSchemaName)
		SET @InsertFields = REPLACE(@InsertFields,'#PHYSICALTABLENAME#',@TableName)
	
		
		----PRINT @ParemeterList

		-- PRINT PARAMETER DEFINITION
		SET @ParameterCount = 1
		WHILE @ParameterCount <= @TotalParameters
		BEGIN
			SELECT @ParameterDef = ParameterDefinition
			FROM #ParameterTable
			WHERE RowVersion = @ParameterCount
			
			PRINT '	'+@ParameterDef
			SET @ParameterCount += 1
		END

	PRINT 
	')
AS
BEGIN'
		SET @Note=REPLACE(@Note,'#COL_GUID#',@ColPkName)
		PRINT @Note
		
		PRINT '	SET NOCOUNT ON;'
		PRINT '	BEGIN TRY'
		
		IF @HasRowVersion=1
		BEGIN
			SET @Validate = REPLACE(@Validate,'#COL_GUID#',@ColPkName)
			PRINT @Validate
		END
			
		PRINT ''
		PRINT '		SET @'+@ColPkName+'=NEWID();'
	
		IF EXISTS(SELECT 1 FROM #InsertLineTable WHERE UpdateLineDefinition LIKE '%@CreatedDate%')
			PRINT '		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())'
		
		PRINT ''
		PRINT '		INSERT INTO ['+@WorkingSchemaName+'].['+@WorkingTable+'] 
		('
		SET @RowNumber = 1;
		WHILE @RowNumber <= @TotalRows
		BEGIN
			SELECT @InsertBlock=UpdateLineDefinition
			FROM #InsertLineTable
			WHERE RowNumber=@RowNumber
			
			IF @RowNumber = 1
				PRINT '		' + REPLACE(@InsertBlock,'@','')
			ELSE
				PRINT '		,'+REPLACE(@InsertBlock,'@','')
			
			SET @RowNumber += 1;
		END
		PRINT '		)
		VALUES
		('
					
		SET @RowNumber = 1;
		WHILE @RowNumber <= @TotalRows
		BEGIN
			SELECT @InsertBlock=UpdateLineDefinition
			FROM #InsertLineTable
			WHERE RowNumber=@RowNumber
			
			IF @RowNumber = 1
				PRINT '		' + REPLACE(REPLACE(@InsertBlock,'[',''),']','')
			ELSE
				PRINT '		,'+ REPLACE(REPLACE(@InsertBlock,'[',''),']','')
			
			SET @RowNumber += 1;
		END
		PRINT '		)'
							
		SET @InsertCriteria=REPLACE(@InsertCriteria,'#COL_GUID#',@ColPkName)
		PRINT @InsertCriteria

		
		IF @HasRowVersion = 1
		BEGIN
			SET @RefreshRowVersion=REPLACE(@RefreshRowVersion,'#SCHEMA#',@WorkingSchemaName)
			SET @RefreshRowVersion=REPLACE(@RefreshRowVersion,'#PHYSICALTABLENAME#',@WorkingTable)
			SET @RefreshRowVersion=REPLACE(@RefreshRowVersion,'#COL_GUID#',@ColPkName)

			PRINT @RefreshRowVersion
			PRINT ''
		END
			
		PRINT '	END TRY'
		
		PRINT @Exception
		PRINT 'GO'
		PRINT ''
		TRUNCATE TABLE #InsertLineTable
		
		SET @TotalRows = 0;
	END
	DROP TABLE #InsertLineTable

	CLOSE TableInfoCursor
	DEALLOCATE TableInfoCursor

END


