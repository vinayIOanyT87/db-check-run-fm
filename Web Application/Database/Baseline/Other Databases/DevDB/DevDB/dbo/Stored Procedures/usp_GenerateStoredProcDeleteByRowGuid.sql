




CREATE PROCEDURE [dbo].[usp_GenerateStoredProcDeleteByRowGuid](@TableNameList NVARCHAR(MAX)=NULL)
AS
BEGIN
	
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
	
	SELECT @SpTemplate=Template
	FROM dbo.StoredProcedureTemplate
	WHERE TemplateCode='Delete'


	DECLARE @PkList TABLE (RowNumber INT IDENTITY,SchemaName NVARCHAR(100),TableName NVARCHAR(300),PkColumnName NVARCHAR(500),DeleteColumnName NVARCHAR(100))
	DECLARE @TotalRows INT
		,	@RowCount INT
		,	@SchemaName NVARCHAR(100)
		,	@TableName NVARCHAR(500)
		,	@ColumnName NVARCHAR(500)
		,	@DeleteColumnName NVARCHAR(100)
	IF @TableNameList IS NULL
		INSERT INTO @PkList(SchemaName,TableName,PkColumnName,DeleteColumnName)
		SELECT SchemaName,TableName,ColumnName,DeleteColumnName 
		FROM dbo.udf_GetPrimaryKeyInfo() 
		WHERE SchemaName IN('dbo' ,'map')
		AND TableName NOT IN('tblConfigurationSetting','tblFilterViews','tblSettings','tblCompanyCrossReferenceMap','tblSessionToSQLProcess')
		AND TableName NOT LIKE 'tblEntity%'
		ORDER BY SchemaName,TableName,ColumnName
	ELSE
		INSERT INTO @PkList(SchemaName,TableName,PkColumnName,DeleteColumnName)
		SELECT pki.SchemaName,pki.TableName,pki.ColumnName,pki.DeleteColumnName 
		FROM dbo.udf_GetPrimaryKeyInfo() pki
		INNER JOIN dbo.udf_SplitTableName(@TableNameList) stn ON (stn.TableName=pki.TableName)
		WHERE pki.SchemaName IN('dbo' ,'map') 
		AND stn.TableName NOT LIKE 'tblEntity%'
		--AND pki.TableName NOT IN('tblConfigurationSetting','tblFilterViews','tblSettings')
		ORDER BY pki.SchemaName,pki.TableName,pki.ColumnName

	SET @TotalRows=@@ROWCOUNT

	SET @RowCount=1
	WHILE @RowCount<=@TotalRows
	BEGIN

		--SET @DropStmt=@DropTemplate
		SET @SpScript= @SpTemplate
		SELECT	@SchemaName=SchemaName
			,	@TableName=TableName
			,	@ColumnName=PkColumnName
			,	@DeleteColumnName=DeleteColumnName
		FROM @PkList
		WHERE RowNumber=@RowCount
		SET @SpScript=REPLACE(@SpScript,'#COLUMNTYPE#','UNIQUEIDENTIFIER')
		SET @SpScript=REPLACE(@SpScript,'#SCHEMA#',@SchemaName)
		SET @SpScript=REPLACE(@SpScript,'#LOGICALTABLENAME#',REPLACE(@TableName,'tbl',''))
		SET @SpScript=REPLACE(@SpScript,'#SCOPE#','ByRowGuid')
		SET @SpScript=REPLACE(@SpScript,'#PHYSICALTABLENAME#',@TableName)
		SET @SpScript=REPLACE(@SpScript,'#COLUMNNAME#',@ColumnName)
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
		SET @SpScript=REPLACE(@SpScript,',16,1)',',18,1)')
		PRINT @SpScript
		SET @RowCount+=1
	END

END



