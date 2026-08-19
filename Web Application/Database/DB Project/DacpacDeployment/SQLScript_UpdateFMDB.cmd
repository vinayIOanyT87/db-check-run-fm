@IF "%1"=="" GOTO :ERROR_NO_DB
@IF "%2"=="" GOTO :ERROR_NO_SERVER
@IF "%3"=="" GOTO :ERROR_NO_DACPAC

"SqlPackage\SQLPackage.exe" @SqlPackageOptions\PublishDacPac.options /TargetDatabaseName:%1 /TargetServerName:%2 /SourceFile:%3

GOTO :EXIT

:ERROR_NO_DB
@ECHO Syntax: SQLScript_UpdateFMDB.cmd [TargetDatabaseName] [TargetServerName] [DACPAC Name]
@ECHO.
@ECHO Please pass in the name of the target database
@ECHO.
GOTO :EXIT

:ERROR_NO_SERVER
@ECHO Syntax: SQLScript_UpdateFMDB.cmd [TargetDatabaseName] [TargetServerName] [DACPAC Name]
@ECHO.
@ECHO Please pass in the name of the target server
@ECHO.
GOTO :EXIT

:ERROR_NO_DACPAC
@ECHO Syntax: SQLScript_UpdateFMDB.cmd [TargetDatabaseName] [TargetServerName] [DACPAC Name]
@ECHO.
@ECHO Please pass in the name of the DACPAC file
@ECHO.
GOTO :EXIT


:EXIT
