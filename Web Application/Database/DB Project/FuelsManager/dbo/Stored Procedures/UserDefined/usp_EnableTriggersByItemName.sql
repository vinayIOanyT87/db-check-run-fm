CREATE PROCEDURE [dbo].[usp_EnableTriggersByItemName]
@sTriggerNameLike [sysname], @bEnable TINYINT=1
AS
BEGIN

	-- Prevent extra result sets from interfering with SELECT statements.
	SET NOCOUNT ON

	-- These lines must appear at the top of every batch, for error-handling.
	DECLARE @nMyErr			INT				SET @nMyErr        = 0
	DECLARE @nRowsAffected	INT				SET @nRowsAffected = 0
	DECLARE @sMsg				NVARCHAR(400)	SET @sMsg          = ''
	DECLARE @sCmd				NVARCHAR(4000)	SET @sCmd			 = ''
	DECLARE @sWord				sysname			SET @sWord			 = CASE @bEnable WHEN 1 THEN 'ENABLE' ELSE 'DISABLE' END
	
	-- Declare, use, and deallocate cursor.
	DECLARE @sTableName sysname
	DECLARE @sTriggerName sysname

	DECLARE TheCursor CURSOR FOR
		SELECT OBJECT_NAME(parent_id), name FROM sys.triggers
		 WHERE Name LIKE '%' + @sTriggerNameLike + '%'
		   AND is_ms_shipped = 0
		  
	OPEN TheCursor

	FETCH NEXT FROM TheCursor
	 INTO @sTableName, @sTriggerName

	WHILE @@FETCH_STATUS = 0
	BEGIN

		IF OBJECT_ID('dbo.'+ @sTriggerName, 'TR') IS NOT NULL
		BEGIN
			SET @sCmd = @sWord + ' TRIGGER dbo.' + @sTriggerName + N' ON dbo.' + @sTableName
			PRINT @sCmd
			EXEC (@sCmd)
			SELECT @nMyErr = @@ERROR, @nRowsAffected = @@ROWCOUNT  IF @nMyErr != 0  BEGIN  SET @sMsg = '   *** Failed to enable trigger ' + @sTriggerName + ' - @@ERROR: ' + CAST(@nMyErr AS NVARCHAR)   RAISERROR(@sMsg, 16, 1)   IF 0 < @@TRANCOUNT ROLLBACK   RETURN  END
		END

	FETCH NEXT FROM TheCursor
	 INTO @sTableName, @sTriggerName
	END

	-- Close and deallocate the cursor.
	CLOSE TheCursor
	DEALLOCATE TheCursor

END