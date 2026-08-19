echo off

IF (%1)==(/S) GOTO SET_SOURCE_DIR
IF (%1)==(/s) GOTO SET_SOURCE_DIR
IF (%1)==(-S) GOTO SET_SOURCE_DIR
IF (%1)==(-s) GOTO SET_SOURCE_DIR
IF (%1)==(/?) GOTO PrintHelp
IF (%1)==(/H) GOTO PrintHelp
IF (%1)==(/h) GOTO PrintHelp
GOTO PrintHelp
:SET_SOURCE_DIR
IF (%2)==("") GOTO PrintHelp
set sourcePath=%~2

IF (%3)==(/S2) GOTO SET_BSME_SOURCE_DIR
IF (%3)==(/s2) GOTO SET_BSME_SOURCE_DIR
IF (%3)==(-S2) GOTO SET_BSME_SOURCE_DIR
IF (%3)==(-t2) GOTO SET_BSME_SOURCE_DIR
GOTO PrintHelp
:SET_BSME_SOURCE_DIR
IF (%4)==("") GOTO PrintHelp
set sourceBsmePath=%~4

IF (%5)==(/T) GOTO SET_TARGET_DIR
IF (%5)==(/t) GOTO SET_TARGET_DIR
IF (%5)==(-T) GOTO SET_TARGET_DIR
IF (%5)==(-t) GOTO SET_TARGET_DIR
GOTO PrintHelp
:SET_TARGET_DIR
IF (%6)==("") GOTO PrintHelp
set targetPath=%~6

rmdir /S /Q "%targetPath%\FuelsManager"
mkdir "%targetPath%\FuelsManager"

xcopy "%sourcePath%\FuelsManager" "%targetPath%\FuelsManager\FuelsManager" /S /E /I /C /R /Y /exclude:fuelsManagerExcludeList.txt
xcopy "%sourcePath%\Binaries" "%targetPath%\FuelsManager\SupportBinaries" /S /E /I /C /R /Y 
xcopy "%sourcePath%\FMBusinessServices" "%targetPath%\FuelsManager\FMBusinessServices" /S /E /I /C /R /Y /exclude:FMBusinessServicesExcludeList.txt
xcopy "%sourcePath%\FuelsManagerService" "%targetPath%\FuelsManager\FuelsManagerService" /S /E /I /C /R /Y /exclude:FuelsManagerServicesExcludeList.txt
xcopy "%sourcePath%\Dispatch Desktop" "%targetPath%\FuelsManager\ClientDispatch" /S /E /I /C /R /Y /exclude:ClientDispatchExcludeList.txt
xcopy "%sourceBsmePath%\BsmeBusinessServices" "%targetPath%\FuelsManager\BsmeBusinessServices" /S /E /I /C /R /Y /exclude:BsmeBusinessServicesExcludeList.txt

mkdir "%targetPath%\FuelsManager\SqlClrLedger"
copy "%sourcePath%\LedgerCore\bin\Debug\LedgerCore.dll" "%targetPath%\FuelsManager\SqlClrLedger"
copy "%sourceBsmePath%\SQLCLRStoredProcedure\bin\Debug\FMCLRStoredProcedureAssembly.dll" "%targetPath%\FuelsManager\SqlClrLedger"
copy "%sourceBsmePath%\SQLCLRStoredProcedure\SQL Create SP Assembly Script.sql" "%targetPath%\FuelsManager\SqlClrLedger"


GOTO END
:PrintHelp
echo CopyDefenseBuild.bat /S source path /S2 BSME source Path /T target path
echo Where source path, BSME source path, and target path must be in double quotes.
echo Example, CopyDefenseBuild.bat /S "C:\FuelsManager.NET\FuelsManager\Web Application" /S2 "C:\FuelsManager.NET\BSM-E RICE\Web Application" /T "C:\E-Drive\DefenseBuild"

:END
echo
echo Finished

