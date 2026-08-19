@ECHO OFF
@IF "%1"=="" GOTO :ERROR_NO_DB
@IF "%2"=="" GOTO :ERROR_NO_SERVER

@ECHO Updating Encrypted Fields
.\DataImportExportWizard.exe /a:EncryptOnly /aes /i:%2 /db:%1
GOTO :EXIT

:ERROR_NO_DB
@ECHO Syntax: UpgradeEncryption.cmd [TargetDatabaseName] [SQLSERVER]\[SQLINSTANCENAME]
@ECHO.
@ECHO Please pass in the name of the target database
@ECHO.
GOTO :EXIT

:ERROR_NO_SERVER
@ECHO Syntax: UpgradeEncryption.cmd [TargetDatabaseName] [SQLSERVER]\[SQLINSTANCENAME]
@ECHO.
@ECHO Please pass in the name of the SQL Server\Instance Name
@ECHO.
GOTO :EXIT

:EXIT
