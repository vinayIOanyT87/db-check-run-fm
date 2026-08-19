CREATE FUNCTION [dbo].[udf_SplitTableName](@TableList NVARCHAR(4000))
RETURNS @TableNameList TABLE(SchemaName VARCHAR(100) NULL, TableName VARCHAR(300) NOT NULL)
AS
BEGIN
	DECLARE @Position INT
		,	@TableName NVARCHAR(300)
		,	@Start INT
		,	@SchemaName VARCHAR(100)
	SET @Position = 0
	SET @Start = 1
	WHILE @Position <= LEN(@TableList)
	BEGIN 
		SET @Position = CHARINDEX(',',@TableList,@Start)
		IF @Position = 0
		BEGIN
			SET @TableName=SUBSTRING(@TableList,@Start,LEN(@TableList))
			SET @Position = LEN(@TableList) + 1
		END
		ELSE
		BEGIN
			SET @TableName=SUBSTRING(@TableList,@Start,@Position-1)
			SET @Start = @Position + 1
		END
		IF CHARINDEX('.',@TableName,1) > 0
		BEGIN
			SET @SchemaName = SUBSTRING(@TableName,1,CHARINDEX('.',@TableName,1)-1)
			SET @TableName = REPLACE(@TableName,@SchemaName+'.','')
		END
		ELSE
		BEGIN
			SET @SchemaName='dbo'
		END 
		INSERT INTO @TableNameList(SchemaName,TableName)
		VALUES(@SchemaName,@TableName)
	END
	RETURN
END