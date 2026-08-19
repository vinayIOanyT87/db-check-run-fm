IF NOT "%ConfigurationName%" == "Release" exit
echo Packaging Files for Release...

REM
md "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\AccountingComponents\AccountingBLL\Bin\AccountingBLL.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\AccountingComponents\AccountingDAL\Bin\AccountingDAL.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\AccountingComponents\AccountingServices\Bin\AccountingServices.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SharedComponents\ConsolidatedBLL\bin\ConsolidatedBLL.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SharedComponents\ConsolidatedDAL\bin\ConsolidatedDAL.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SharedComponents\ConsolidatedDataObjects\bin\ConsolidatedDataObjects.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SharedComponents\ConsolidatedUtilities\Release\ConsolidatedUtilities.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\FinanceComponents\FinanceDataObjects\bin\Release\FinanceDataObjects.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\FinanceComponents\FinanceBLL\bin\Release\FinanceBLL.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\FinanceComponents\FinanceDAL\bin\Release\FinanceDAL.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SharedComponents\FMCommon\bin\FMCommon.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\FMSecurityAuthentication\bin\Release\FMSecurityAuthentication.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\GetTransFields\bin\Release\gettransfields.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\AccountingComponents\Helpers\bin\Helpers.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SharedComponents\ConsolidatedUtilities\bin\Interop.ConsolidatedUtilities.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\LoadRackComponents\LoadRackLibrary\bin\LoadRackLibrary.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\ReportingComponents\FMReporting\ReportingBLL\bin\ReportingBll.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\ReportingComponents\FMReporting\ReportingDAL\bin\ReportingDAL.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\ReportingComponents\FMReporting\ReportingServices\bin\ReportingServices.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SharedComponents\VolumeCorrection\bin\VolumeCorrectionDotNet.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\AccountingComponents\XMLImport\Bin\XMLImport.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SQLCLRComponents\SQLCLRFunctions\bin\Release\FMCLRAssembly.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SQLCLRComponents\SQLCLRStoredProcedures\bin\Release\FMCLRStoredProcedureAssembly.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\SQLCLRComponents\SQLCLRStoredProcedures\SQL Create SP Assembly Script.sql" "\FuelsManager.NET\Build_Fusion\db"
copy "\FuelsManager.NET\Fusion\Web Application\FMLogger\LogClient\bin\LogClient.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\PIDXComponents\PIDXTransactions\Bin\Release\PIDXTransactions.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\PIDXComponents\PIDXCommunications\Bin\Release\PIDXCommunications.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\EntityImportExportComponents\EntityImportExport\bin\Release\EntityImportExport.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\PriceCalculator\bin\Release\PriceCalculator.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\ADFPriceCalculator\bin\Release\ADFPriceCalculator.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\ReserveLevelCalculator\bin\Release\ReserveLevelCalculator.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
copy "\FuelsManager.NET\Fusion\Web Application\ADFTransactionCustomFields\bin\Release\ADFTransactionCustomFields.dll" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared"
xcopy /Y /E /I /Q "\FuelsManager.NET\Fusion\Web Application\Binaries" "\FuelsManager.NET\Build_Fusion\FuelsManager Shared" /EXCLUDE:..\..\ExcludeList.txt

REM
md "\FuelsManager.NET\Build_Fusion\FuelsManager"
copy "\FuelsManager.NET\Fusion\Web Application\AccountingComponents\AccountingEOM\Bin\AccountingEOM.exe" "\FuelsManager.NET\Build_Fusion\FuelsManager"
copy "\FuelsManager.NET\Fusion\Web Application\LoadRackComponents\LoadRackService\bin\Release\LoadRackService.exe" "\FuelsManager.NET\Build_Fusion\FuelsManager"
copy "\FuelsManager.NET\Fusion\Web Application\FMLogger\LogService\bin\LogService.exe" "\FuelsManager.NET\Build_Fusion\FuelsManager"
copy "\FuelsManager.NET\Fusion\Web Application\TransactionArchivingService\bin\Release\TransactionArchivingService.exe" "\FuelsManager.NET\Build_Fusion\FuelsManager"

REM
md "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir"
xcopy /Y /E /I /Q "C:\FuelsManager.NET\Fusion\Web Application\FuelsManager" "C:\FuelsManager.NET\Build_Fusion\FuelsManagerVDir" /EXCLUDE:..\..\ExcludeList.txt

REM
md "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\AcculoadOPC\AcculoadOPCObjects\Release\AcculoadOPCObjects.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\AcculoadOPC\AcculoadOPCServer\Release\AcculoadOPCServer.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\OptomuxOPC\OptomuxOPCObjects\Release\OptomuxOPCObjects.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\OptomuxOPC\OptomuxOPCServer\Release\OptomuxOPCServer.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\OsdpOPC\OsdpOPCObjects\Release\OsdpOPCObjects.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\OsdpOPC\OsdpOPCServer\Release\OsdpOPCServer.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\WebTicketingComponents\WebTicketingBLL\bin\WebTicketingBLL.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\WebTicketingComponents\WebTicketingDataObjects\bin\WebTicketingDataObjects.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\WeightScaleOPC\WeightScaleOPCObjects\Release\WeightScaleOPCObjects.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\WeightScaleOPC\WeightScaleOPCServer\Release\WeightScaleOPCServer.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\DanielOPC\DanielOPCObjects\Release\DanielOPCObjects.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\DanielOPC\DanielOPCServer\Release\DanielOPCServer.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
xcopy /Y /E /I /Q "C:\FuelsManager.NET\Fusion\Web Application\FuelsManager\bin" "C:\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin"
copy "\FuelsManager.NET\Fusion\Web Application\SharedComponents\FMControls\bin\FMControls.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\FMControls.dll"

REM
:: Must put the FMReporting DLL in the FMReporting bin directory since it is an application.
md "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\FMReporting\bin"
copy "\FuelsManager.NET\Fusion\Web Application\FuelsManager\FMReporting\bin\FMReporting.dll" "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\FMReporting\bin"

REM
::Cleanup duplicate dlls from the VDir\Bin 
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\ConsolidatedBLL.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\ConsolidatedDAL.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\ConsolidatedDataObjects.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\FinanceBLL.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\FinanceDAL.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\FinanceDataObjects.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\FMCommon.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\FMSecurityAuthentication.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\Helpers.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManagerVDir\Bin\Interop.VolumeCorrection.dll"
del "\FuelsManager.NET\Build_Fusion\FuelsManager Shared\amqmdnet.dll"

REM
::Can't exclude .cs files from xcopy due to .css files (maybe that's why they invented regular expressions.)
::Therefore, go behind and delete the stinking source code files after copying them. Sad, sad.
FOR /R "C:\FuelsManager.NET\Build_Fusion\FuelsManagerVDir" %%f IN (*.cs) DO del "%%f"

REM
xcopy /Y /E /I /Q "C:\FuelsManager.NET\Fusion\Web Application\AccountingImportExport" "C:\FuelsManager.NET\Build_Fusion\AccountingImportExportVDir" /EXCLUDE:..\..\ExcludeList.txt
xcopy /Y /E /I /Q "C:\FuelsManager.NET\Fusion\Web Application\AccountingImportExport\bin" "C:\FuelsManager.NET\Build_Fusion\AccountingImportExportVDir\bin"

REM
md "\FuelsManager.NET\Build_Fusion\db"
md "\FuelsManager.NET\Build_Fusion\db\Iteration 1 changes"
md "\FuelsManager.NET\Build_Fusion\db\Iteration 2 changes"
md "\FuelsManager.NET\Build_Fusion\db\Iteration 3 changes"
md "\FuelsManager.NET\Build_Fusion\db\Iteration 4 changes"
md "\FuelsManager.NET\Build_Fusion\db\Iteration 5 changes"
md "\FuelsManager.NET\Build_Fusion\db\Iteration 6 changes"
md "\FuelsManager.NET\Build_Fusion\db\Iteration 7 changes"
md "\FuelsManager.NET\Build_Fusion\db\Iteration 8 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 10 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 11 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 12 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 13 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 14 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 15 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 16 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 17 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 18 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 19 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 20 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 21 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 22 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 23 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 24 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 25 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 26 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 27 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 28 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 29 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 30 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 31 changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 30 Hot Fix 1 Changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 30 Hot Fix 2 Changes"
md "\FuelsManager.NET\Build_Fusion\db\Build 30 Hot Fix 2 Changes\ADF Reports"
md "\FuelsManager.NET\Build_Fusion\db\Build 30 Hot Fix 3 Changes"
md "\FuelsManager.NET\Build_Fusion\db\Report SPs\ADF"
md "\FuelsManager.NET\Build_Fusion\db\Report SPs\Standard"
md "\FuelsManager.NET\Build_Fusion\db\Report SPs\BOL"
md "\FuelsManager.NET\Build_Fusion\db\Signature Capture Script"
copy "\FuelsManager.NET\Fusion\Web Application\AcculoadOPC\Database\AcculoadOPC.sql" "\FuelsManager.NET\Build_Fusion\db"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\ConsolidatedDB.sql" "\FuelsManager.NET\Build_Fusion\db"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Configure Assemblies After Restore to New System.sql" "\FuelsManager.NET\Build_Fusion\db"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Iteration 1 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Iteration 1 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Iteration 2 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Iteration 2 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Iteration 3 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Iteration 3 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Iteration 4 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Iteration 4 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Iteration 5 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Iteration 5 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Iteration 6 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Iteration 6 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Iteration 7 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Iteration 7 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Iteration 8 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Iteration 8 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 10 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 10 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 11 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 11 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 12 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 12 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 13 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 13 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 14 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 14 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 15 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 15 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 16 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 16 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 17 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 17 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 18 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 18 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 19 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 19 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 20 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 20 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 21 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 21 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 22 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 22 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 23 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 23 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 24 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 24 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 25 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 25 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 26 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 26 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 27 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 27 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 28 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 28 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 29 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 29 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 30 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 30 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 31 changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 31 changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 30 Hot Fix 1 Changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 30 Hot Fix 1 Changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 30 Hot Fix 2 Changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 30 Hot Fix 2 Changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 30 Hot Fix 2 Changes\ADF Reports\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 30 Hot Fix 2 Changes\ADF Reports"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Build 30 Hot Fix 3 Changes\*.sql" "\FuelsManager.NET\Build_Fusion\db\Build 30 Hot Fix 3 Changes"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Report SPs\ADF\*.sql" "\FuelsManager.NET\Build_Fusion\db\Report SPs\ADF"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Report SPs\Standard\*.sql" "\FuelsManager.NET\Build_Fusion\db\Report SPs\Standard"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Report SPs\BOL\*.sql" "\FuelsManager.NET\Build_Fusion\db\Report SPs\BOL"
copy "\FuelsManager.NET\Fusion\Web Application\Database - Development\Signature Capture Script\*.sql" "\FuelsManager.NET\Build_Fusion\db\Signature Capture Script"
copy "\FuelsManager.NET\Fusion\Web Application\WebTicketingComponents\Database\FuelTicketsConsolidatedDB.sql" "\FuelsManager.NET\Build_Fusion\db"
copy "\FuelsManager.NET\Fusion\Web Application\OptomuxOPC\Database\OptomuxOPC.sql" "\FuelsManager.NET\Build_Fusion\db"
copy "\FuelsManager.NET\Fusion\Web Application\OsdpOPC\Database\OsdpOPC.sql" "\FuelsManager.NET\Build_Fusion\db"
copy "\FuelsManager.NET\Fusion\Web Application\WeightScaleOPC\Database\WeightScaleOPC.sql" "\FuelsManager.NET\Build_Fusion\db"

REM
md "\FuelsManager.NET\Build_Fusion\Projects"
xcopy /Y /E /I /Q "\FuelsManager.NET\Fusion\Web Application\Projects\*.rd*" "\FuelsManager.NET\Build_Fusion\Projects"
xcopy /Y /E /I /Q "\FuelsManager.NET\Fusion\Web Application\Projects\BOL Report\BOL Report.rdl" "\FuelsManager.NET\Build_Fusion\Projects"
xcopy /Y /E /I /Q "\FuelsManager.NET\Fusion\Web Application\Projects\BOL Report\ConsolidatedDB.rds" "\FuelsManager.NET\Build_Fusion\Projects"

REM
rmdir /S /Q "\FuelsManager.NET\Build_Fusion\Projects\Online Documentation"
md "\FuelsManager.NET\Build_Fusion\FuelsManager\Online Documentation"
xcopy /Y /E /I /Q "\FuelsManager.NET\Fusion\Web Application\Projects\Online Documentation\ADF Documentation\FuelsManager Defence Online Help" "\FuelsManager.NET\Build_Fusion\FuelsManager\Online Documentation\OnlineHelpDoc"
xcopy /Y /E /I /Q "\FuelsManager.NET\Fusion\Web Application\Projects\Online Documentation\ADF Documentation\FuelsManager Defence SCADA Online Help" "\FuelsManager.NET\Build_Fusion\FuelsManager\Online Documentation\OnlineSCADADoc"
xcopy /Y /E /I /Q "\FuelsManager.NET\Fusion\Web Application\Projects\Online Documentation\ADF Documentation\Tutorials" "\FuelsManager.NET\Build_Fusion\FuelsManager\Online Documentation\OnlineTutorialDoc"

REM
md "\FuelsManager.NET\Build_Fusion\Projects\QueryReport"
copy "\FuelsManager.NET\Fusion\Web Application\Query\QueryReport\*.rdl" "\FuelsManager.NET\Build_Fusion\Projects\QueryReport"

REM
md "\FuelsManager.NET\Build_Fusion\Computer Setup"
copy "\FuelsManager.NET\Fusion\Web Application\Computer Setup\*.reg" "\FuelsManager.NET\Build_Fusion\Computer Setup"

REM
md "\FuelsManager.NET\Build_Fusion\Utils"
copy "\FuelsManager.NET\Fusion\Web Application\FMBuildUtils\Install.bat" "\FuelsManager.NET\Build_Fusion\Utils"
copy "\FuelsManager.NET\Fusion\Web Application\FMBuildUtils\Uninstall.bat" "\FuelsManager.NET\Build_Fusion\Utils"
copy "\FuelsManager.NET\Fusion\Web Application\FMBuildUtils\gacutil.exe" "\FuelsManager.NET\Build_Fusion\Utils"

REM
::Why do this? Because the silly for statement will return an error code and indicate 
::that the post build fails if it finds no files to delete. Silly, but true.
IF %ERRORLEVEL%==1 exit 0
