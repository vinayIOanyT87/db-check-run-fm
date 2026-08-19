@IF "%1"=="" GOTO :ERROR_NO_DB
@IF "%2"=="" GOTO :ERROR_NO_SERVER

@REM -I means to use Quoted Identifiers, if not, some FK creation may throw errors
@SET TARGETSERVER=-S %2 -I
@SET TARGETDATABASE=-d %1

@ECHO "You are about to DROP the following database: %1"
@ECHO "Press any key to continue or CTRL-C to Abort"

sqlcmd %TARGETSERVER% -Q "ALTER DATABASE %1 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;"
sqlcmd %TARGETSERVER% -Q "DROP DATABASE %1;"

GOTO :EXIT

:ERROR_NO_DB
@ECHO Syntax: SQLScript_ResetFMDB.cmd [TargetDatabaseName] [TargetServerName]
@ECHO.
@ECHO Please pass in the name of the target database
@ECHO.
GOTO :EXIT

:ERROR_NO_SERVER
@ECHO Syntax: SQLScript_ResetFMDB.cmd [TargetDatabaseName] [TargetServerName]
@ECHO.
@ECHO Please pass in the name of the target server
@ECHO.
GOTO :EXIT

:EXIT
