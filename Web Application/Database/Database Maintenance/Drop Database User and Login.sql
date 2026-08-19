 /*=============================================
 Author:			Al dos Santos
 Create date:		02/09/2011
 Description:		Drops database users and logins for users listed within the tblUsers table of ConsolidatedDB
 Version:			
 Execution:
			
 Modification History:

=============================================*/


SET NOCOUNT ON

BEGIN TRY

	BEGIN TRAN
	
	DECLARE @DynCmd NVARCHAR(MAX)

	DECLARE @TotalDb INT
		,	@TotalUser INT
		,	@DbCount INT
		,	@UserCount INT
		,	@DatabaseName sysname
		,	@UserID VARCHAR(100)
		,	@CheckCount INT
	;

	--LIST TARGET DATABASES
	DECLARE @DatabaseList TABLE (RowNumber INT IDENTITY PRIMARY KEY, DatabaseName sysname);
	INSERT INTO @DatabaseList(DatabaseName)
	SELECT [name] 
	FROM	sys.databases
	WHERE [name] IN('master','ConsolidatedDB','ConsolidatedDBArchive')

	SET	@TotalDb = @@ROWCOUNT;

	--DISCOVER DATABASE USERS
	DECLARE @UserList TABLE (RowNumber INT IDENTITY PRIMARY KEY, UserID VARCHAR(100));
	INSERT INTO @UserList(UserID)
	SELECT UserID FROM ConsolidatedDB.dbo.tblUsers
	ORDER BY UserID;

	SET @TotalUser = @@ROWCOUNT;

	--REMOVE DATABASE USERS FROM DATABASES
	SET	@DbCount = 1;
	WHILE @DBCount <= @TotalDb
	BEGIN
		SELECT @DatabaseName = DatabaseName
		FROM	@DatabaseList WHERE RowNumber = @DbCount;
		
		SET	@UserCount = 1;
		WHILE @UserCount <= @TotalUser
		BEGIN
			SELECT @UserID = UserID
			FROM	@UserList WHERE RowNumber = @UserCount;
			
			--VERIFY IF USER EXISTS AT THE TARGET DATABASE
			SET	@CheckCount = 0;
			SET	@DynCmd = 'USE [' + @DatabaseName + ']; '
			SET	@DynCmd = @DynCmd + 'SELECT @CheckCount = COUNT(*) FROM sysusers WHERE [name]=''' + @UserId + ''' ;'
			EXEC sp_executesql @stmt=@DynCmd
					,@params = N'@CheckCount INT OUTPUT'
					,@CheckCount = @CheckCount OUTPUT;
					
			IF @CheckCount > 0
			BEGIN
				-- DROP DABATASE USER				
				SET	@DynCmd = 'USE [' + @DatabaseName + ']; '
				SET	@DynCmd = @DynCmd + 'DROP USER [' + @UserID + '];'
				EXEC sp_executesql @stmt=@DynCmd;
			END
			
			SET @UserCount = @UserCount + 1;
		END
		SET @DBCount = @DBCount + 1;
	END

	-- DROP LOGIN ACCOUNTS
	SET @UserCount = 1;
	WHILE @UserCount <= @TotalUser
	BEGIN
		
			SELECT @UserID = UserID
			FROM	@UserList WHERE RowNumber = @UserCount;

			--VERIFY IF LOGIN EXISTS
			SET	@CheckCount = 0;
			SET	@DynCmd = 'SELECT @CheckCount = COUNT(*) FROM master.dbo.syslogins WHERE [name]=''' + @UserId + ''' ;'
			EXEC sp_executesql @stmt=@DynCmd
					,@params = N'@CheckCount INT OUTPUT'
					,@CheckCount = @CheckCount OUTPUT
		
			IF @CheckCount > 0
			BEGIN
				-- DROP DABATASE USER				
				SET	@DynCmd = 'USE master; '
				SET	@DynCmd = @DynCmd + 'DROP LOGIN [' + @UserID + '];'
				print @DynCmd
				EXEC sp_executesql @stmt=@DynCmd;
			END

		SET	@UserCount = @UserCount + 1;
	END
	COMMIT TRANSACTION;
	
END TRY
BEGIN CATCH
	Declare @ErrorMessage VARCHAR(500)
	SET @ErrorMessage = ERROR_MESSAGE()
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION;
		
	RAISERROR('The following error has occured: %s',10,1,@ErrorMessage)
	
END CATCH

