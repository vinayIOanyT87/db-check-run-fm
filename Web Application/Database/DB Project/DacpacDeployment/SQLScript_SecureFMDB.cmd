@IF "%1"=="" GOTO :ERROR_NO_DB
@IF "%2"=="" GOTO :ERROR_NO_SERVER
@IF "%3"=="" GOTO :ERROR_NO_USER

@REM -I means to use Quoted Identifiers, if not, some FK creation may throw errors
@SET TARGETSERVER=-S %2 -I
@SET TARGETDATABASE=-d %1

sqlcmd %TARGETSERVER% %TARGETDATABASE% -Q "CREATE USER [%3] FOR LOGIN [%3] WITH DEFAULT_SCHEMA=[dbo]"

sqlcmd %TARGETSERVER% %TARGETDATABASE% -Q "sys.sp_addrolemember @rolename = N'db_owner', @membername = N'%3'"

GOTO :EXIT

:ERROR_NO_DB
@ECHO Syntax: SQLScript_SecureFMDB.cmd [TargetDatabaseName] [TargetServerName] [UserName]
@ECHO.
@ECHO Please pass in the name of the target database
@ECHO.
GOTO :EXIT

:ERROR_NO_SERVER
@ECHO Syntax: SQLScript_SecureFMDB.cmd [TargetDatabaseName] [TargetServerName] [UserName]
@ECHO.
@ECHO Please pass in the name of the target server
@ECHO.
GOTO :EXIT


:ERROR_NO_USER
@ECHO Syntax: SQLScript_SecureFMDB.cmd [TargetDatabaseName] [TargetServerName] [UserName]
@ECHO.
@ECHO Please pass in the name of the user
@ECHO.
GOTO :EXIT

:EXIT
