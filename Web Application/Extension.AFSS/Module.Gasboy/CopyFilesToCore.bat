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
del /Q "%targetPath%\Web Application\FuelsManager\bin\FuelsManager.GasboyRICE.WebApp.dll"
del /Q "%targetPath%\Web Application\FMBusinessServices\bin\FuelsManager.GasboyRICE.BusinessObjects.dll"
del /Q "%targetPath%\Web Application\FMBusinessServices\bin\FuelsManager.AfssRICE.BusinessObjects.dll"
del /Q "%targetPath%\Web Application\FuelsManager\bin\FuelsManager.GasboyRICE.BusinessObjects.dll"
del /Q "%targetPath%\Web Application\FuelsManager\bin\FuelsManager.AfssRICE.BusinessObjects.dll"
rmdir /S /Q "%targetPath%\Web Application\FuelsManager\GasboyRICE.WebApp"

::Copy the GasboyWebApp.dll to the FuelsManager bin directory and copy the entire NspaWebApp directory
::to the FuelsManager directory.
xcopy "%sourcePath%\Gasboy RICE\Web Application\GasboyRICE.WebApp\bin\FuelsManager.GasboyRICE.WebApp.dll" "%targetPath%\Web Application\FuelsManager\bin" /R /Y
xcopy "%sourcePath%\Gasboy RICE\Web Application\GasboyRICE.WebApp\bin\FuelsManager.GasboyRICE.BusinessObjects.dll" "%targetPath%\Web Application\FMBusinessServices\bin" /R /Y
xcopy "%sourcePath%\Gasboy RICE\Web Application\GasboyRICE.WebApp\bin\FuelsManager.AfssRICE.BusinessObjects.dll" "%targetPath%\Web Application\FMBusinessServices\bin" /R /Y
xcopy "%sourcePath%\Gasboy RICE\Web Application\GasboyRICE.WebApp\bin\FuelsManager.GasboyRICE.BusinessObjects.dll" "%targetPath%\Web Application\FuelsManager\bin" /R /Y
xcopy "%sourcePath%\Gasboy RICE\Web Application\GasboyRICE.WebApp\bin\FuelsManager.AfssRICE.BusinessObjects.dll" "%targetPath%\Web Application\FuelsManager\bin" /R /Y
xcopy "%sourcePath%\Gasboy RICE\Web Application\GasboyRICE.WebApp" "%targetPath%\Web Application\FuelsManager\GasboyRICE.WebApp" /S /E /I /C /R /Y
robocopy "%sourcePath%\Gasboy RICE\Web Application\GasboyRICE.WebApp\Areas" "%targetPath%\Web Application\FuelsManager\Areas" *.cshtml /E /NJH

::Restart the web server
net start "World Wide Web Publishing Service"

GOTO END
:PrintHelp
echo CopyNspaFileToCore.bat /S source path /T target path
echo Where source path and target path must be in double quotes.
echo Example, CopyFilesToCore.bat /S "C:\FuelsManager.NET\Automated Fuel Service Station RICE\Gasboy RICE" /T "C:\FuelsManager.NET\FuelsManager"

:END
echo
echo Finished

