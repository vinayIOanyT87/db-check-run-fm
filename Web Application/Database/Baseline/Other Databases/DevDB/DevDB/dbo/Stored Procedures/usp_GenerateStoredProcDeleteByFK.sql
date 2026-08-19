



CREATE PROCEDURE [dbo].[usp_GenerateStoredProcDeleteByFK](@TableNameList NVARCHAR(MAX)=NULL)
AS
BEGIN
	/*------------------------------------------------
	Purpose: Generate script that creates stored procedures to delete records based on Foreign Keys
	Parameters: List of tables in which the stored procedures will be created. If parameter is not supplied then it generates script for all tables
	--------------------------------------------------
	*/
	
	SET NOCOUNT ON
	
	
	
	DECLARE @Stm as NVARCHAR(max)
		,	@SpTemplate as NVARCHAR(max)
		,	@LineReturn AS NVARCHAR(100)
		,	@Tab AS NVARCHAR(20)
		,	@SpScript NVARCHAR(max)
		,	@CurrentDb NVARCHAR(100)
		,	@DropTemplate NVARCHAR(max)
		,	@DropStmt NVARCHAR(max)


	SET @LineReturn = CHAR(13)+CHAR(10)
	SET @Tab= CHAR(9)
	
	--SELECT @SpTemplate=Template
	--FROM dbo.StoredProcedureTemplate
	--WHERE TemplateCode='Delete'

SET @SpTemplate = 
--'IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N''[#SCHEMA#].[gsp_#LOGICALTABLENAME#Delete#SCOPE#]'') AND type in (N''P'', N''PC''))
--	DROP PROCEDURE [#SCHEMA#].[gsp_#LOGICALTABLENAME#Delete#SCOPE#];
--#GOCMD# 
'CREATE PROCEDURE [#SCHEMA#].[gsp_#LOGICALTABLENAME#Delete#SCOPE#](@#COLUMNNAME# #COLUMNTYPE#,@DetachOnly BIT=NULL,#SWAPDEFINITION#,@_RowVersion BINARY(8)=NULL)
AS
BEGIN 
	BEGIN TRY
		SET NOCOUNT ON; 
		IF @_RowVersion IS NOT NULL 
		BEGIN 
			 IF NOT EXISTS(SELECT 1 FROM [#SCHEMA#].[#PHYSICALTABLENAME#] WHERE [#COLUMNNAME#] = @#COLUMNNAME# AND [_RowVersion]=@_RowVersion) 
			 BEGIN 
				 RAISERROR(''Attempted to delete a stale version of #LOGICALTABLENAME#.'',18,1); 
				 RETURN; 
			END 
		END 
		--
		-- REPLACE #COLUMNNAME# PER @SwapToGuid IF @SwapToGuid IS NOT NULL
		--
		IF NOT #SWAPPARAM# IS NULL
		BEGIN
			UPDATE [#SCHEMA#].[#PHYSICALTABLENAME#]
			SET [#COLUMNNAME#]=#SWAPPARAM#
			WHERE [#COLUMNNAME#] = @#COLUMNNAME#;
		END
		ELSE
		BEGIN
			IF ISNULL(@DetachOnly,0) = 1
			BEGIN
				UPDATE [#SCHEMA#].[#PHYSICALTABLENAME#]
				SET [#COLUMNNAME#]=NULL
				WHERE [#COLUMNNAME#] = @#COLUMNNAME#;
			END
			ELSE
			BEGIN
				#DELETEORUPDATE# [#SCHEMA#].[#PHYSICALTABLENAME#] #UPDATECOLUMN# WHERE [#COLUMNNAME#] = @#COLUMNNAME#; 
			END
		END
	END TRY
	BEGIN CATCH
		DECLARE @ErrMessage NVARCHAR(2048)
			,	@ErrNumber INT
			,	@ErrProcName NVARCHAR(126)
			,	@LineNumber INT
		
		SET @ErrMessage = ERROR_MESSAGE()
		SET	@ErrNumber = ERROR_NUMBER()
		SET @ErrProcName= ERROR_PROCEDURE()
		SET @LineNumber = ERROR_LINE()
		
		SET @ErrMessage =		''Error: '' + @ErrMessage + CHAR(13)+CHAR(10)
							+	''Number: '' + CAST(@ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10) 
							+	''Procedure Name: '' + ISNULL(@ErrProcName,OBJECT_NAME(@@PROCID)) + CHAR(13)+CHAR(10) 
							+	''Line Number: '' + ISNULL(CAST(@LineNumber AS VARCHAR(20)),'''') + CHAR(13)+CHAR(10) 
		RAISERROR(@ErrMessage,18,1)
	END CATCH
END 
#GOCMD# '

	DECLARE @FkList TABLE (RowNumber INT IDENTITY,SchemaName NVARCHAR(100),TableName NVARCHAR(300),ColumnName NVARCHAR(500),ColumnType NVARCHAR(300),DeleteColumnName NVARCHAR(100))
	DECLARE @TotalRows INT
		,	@RowCount INT
		,	@SchemaName NVARCHAR(100)
		,	@TableName NVARCHAR(500)
		,	@ColumnName NVARCHAR(500)
		,	@ColumnType NVARCHAR(300)
		,	@DeleteColumnName NVARCHAR(100)
	IF @TableNameList IS NULL
		INSERT INTO @FkList(SchemaName,TableName,ColumnName,ColumnType,DeleteColumnName)
		SELECT SchemaName,TableName,ColumnName,ColumnType,DeleteColumnName 
		FROM dbo.udf_GetForeignKeyInfo() 
		WHERE SchemaName IN('dbo','map')
		AND TableName <> 'tblCompanyCrossReferenceMap'
		AND TableName NOT LIKE 'tblEntity%' -- tblEntity tables will raise error severity as 16
		ORDER BY SchemaName,TableName,ColumnName

	ELSE
		INSERT INTO @FkList(SchemaName,TableName,ColumnName,ColumnType,DeleteColumnName)
		SELECT fki.SchemaName,fki.TableName,fki.ColumnName,fki.ColumnType,fki.DeleteColumnName 
		FROM dbo.udf_GetForeignKeyInfo() fki
		INNER JOIN dbo.udf_SplitTableName(@TableNameList) stn ON (stn.TableName=fki.TableName)
		WHERE fki.SchemaName IN('dbo','map')
		AND stn.TableName NOT LIKE 'tblEntity%' -- tblEntity tables will raise error severity as 16
		ORDER BY fki.SchemaName,fki.TableName,fki.ColumnName

	SET @TotalRows=@@ROWCOUNT

	SET @RowCount=1
	WHILE @RowCount<=@TotalRows
	BEGIN
		--SET @DropStmt=@DropTemplate
		SET @SpScript= @SpTemplate
		SELECT	@SchemaName=SchemaName
			,	@TableName=TableName
			,	@ColumnName=ColumnName
			,	@ColumnType=ColumnType
			,	@DeleteColumnName=DeleteColumnName
		FROM @FkList
		WHERE RowNumber=@RowCount


	
		SET @SpScript=REPLACE(@SpScript,'#COLUMNTYPE#',@ColumnType)
		SET @SpScript=REPLACE(@SpScript,'#SCHEMA#',@SchemaName)
		SET @SpScript=REPLACE(@SpScript,'#LOGICALTABLENAME#',REPLACE(@TableName,'tbl',''))
		SET @SpScript=REPLACE(@SpScript,'#SCOPE#','By'+@ColumnName)
		SET @SpScript=REPLACE(@SpScript,'#PHYSICALTABLENAME#',@TableName)
		SET @SpScript=REPLACE(@SpScript,'#COLUMNNAME#',@ColumnName)
		--SET @SpScript=REPLACE(@SpScript,'#SWAPPARAM#',CASE @ColumnType WHEN 'uniqueidentifier' THEN '@SwapToGuid' WHEN 'tinyint' THEN 'CAST(@SwapToIndex AS tinyint)' ELSE '@SwapToIndex' END)
		SET @SpScript=REPLACE(@SpScript,'#SWAPPARAM#',CASE @ColumnType WHEN 'uniqueidentifier' THEN '@SwapToGuid' ELSE '@SwapToIndex' END)
		SET @SpScript=REPLACE(@SpScript,'#SWAPDEFINITION#',CASE @ColumnType WHEN 'uniqueidentifier' THEN '@SwapToGuid '+@ColumnType+'=NULL' ELSE '@SwapToIndex ' +@ColumnType+'=NULL' END)
	
		IF @DeleteColumnName IS NOT NULL
		BEGIN
			SET @SpScript=REPLACE(@SpScript,'#DELETEORUPDATE#','UPDATE')
			SET @SpScript=REPLACE(@SpScript,' #UPDATECOLUMN#','SET ['+@DeleteColumnName+']=1 ')
		END
		ELSE
		BEGIN
			SET @SpScript=REPLACE(@SpScript,'#DELETEORUPDATE#','DELETE')
			SET @SpScript=REPLACE(@SpScript,' #UPDATECOLUMN#','')

		END
		
		SET @SpScript=REPLACE(@SpScript,'#GOCMD#','GO')
		PRINT @SpScript
		SET @RowCount+=1
	END

END


