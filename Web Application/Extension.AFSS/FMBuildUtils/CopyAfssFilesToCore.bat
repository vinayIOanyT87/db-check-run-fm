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

IF (%3)==(/T) GOTO SET_TARGET_DIR
IF (%3)==(/t) GOTO SET_TARGET_DIR
IF (%3)==(-T) GOTO SET_TARGET_DIR
IF (%3)==(-t) GOTO SET_TARGET_DIR
GOTO PrintHelp
:SET_TARGET_DIR
IF (%4)==("") GOTO PrintHelp
set targetPath=%~4


::Shut down the web server first, as it sometimes holds on to a few files we want to delete.
net stop "World Wide Web Publishing Service"

::Delete the NSPA Web App DLL from the FuelsManager core bin directory.
::Remove the NSPA Web APP directory from the FuelsManager core directory.
del /Q "%targetPath%\FMBusinessServices\bin\FuelsManager.Afss.BusinessObjects.dll"
del /Q "%targetPath%\FMBusinessServices\bin\FuelsManager.Afss.BusinessServices.dll"
del /Q "%targetPath%\FMBusinessServices\bin\FuelsManager.Afss.Module.Gasboy.BusinessObjects.dll"

del /Q "%targetPath%\FuelsManager\bin\FuelsManager.Afss.WebApp.dll"
del /Q "%targetPath%\FuelsManager\bin\FuelsManager.Afss.Module.Gasboy.BusinessObjects.dll"
del /Q "%targetPath%\FuelsManager\bin\FuelsManager.Afss.BusinessObjects.dll"
rmdir /S /Q "%targetPath%\FuelsManager\AFSSWebApp"

::Copy the AFSSWebApp.dll to the FuelsManager bin directory and copy the entire AFSSWebApp directory
::to the FuelsManager directory.
xcopy "%sourcePath%\Web Application\Extension.AFSS\Afss.BusinessServices\bin\FuelsManager.Afss.BusinessServices.dll" "%targetPath%\FMBusinessServices\bin" /R /Y
xcopy "%sourcePath%\Web Application\Extension.AFSS\Afss.WebApp\bin\FuelsManager.Afss.BusinessObjects.dll" "%targetPath%\FMBusinessServices\bin" /R /Y
xcopy "%sourcePath%\Web Application\Extension.AFSS\Afss.WebApp\bin\FuelsManager.Afss.Module.Gasboy.BusinessObjects.dll" "%targetPath%\FMBusinessServices\bin" /R /Y

xcopy "%sourcePath%\Web Application\Extension.AFSS\Afss.WebApp\bin\FuelsManager.Afss.WebApp.dll" "%targetPath%\FuelsManager\bin" /R /Y
xcopy "%sourcePath%\Web Application\Extension.AFSS\Afss.WebApp\bin\FuelsManager.Afss.BusinessObjects.dll" "%targetPath%\FuelsManager\bin" /R /Y
xcopy "%sourcePath%\Web Application\Extension.AFSS\Afss.WebApp\bin\FuelsManager.Afss.Module.Gasboy.BusinessObjects.dll" "%targetPath%\FuelsManager\bin" /R /Y
xcopy "%sourcePath%\Web Application\Extension.AFSS\Afss.WebApp" "%targetPath%\FuelsManager\AFSSWebApp" /S /E /I /C /R /Y
xcopy "%sourcePath%\Web Application\Extension.AFSS\Afss.WebApp\Areas" "%targetPath%\FuelsManager\Areas" /S /E /I /C /R /Y


::Restart the web server
net start "World Wide Web Publishing Service"

GOTO END
:PrintHelp
echo CopyAfssFilesToCore.bat /S source path /T target path
echo Where source path and target path must be in double quotes.
echo Example, CopyAfssFilesToCore.bat /S "C:\Projects\FuelsManager\Defense Merge" /T "C:\Projects\FuelsManager\Defense Merge\Web Application"

:END
pause
echo
echo Finished

