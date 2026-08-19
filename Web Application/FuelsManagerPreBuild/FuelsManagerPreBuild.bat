IF NOT "%ConfigurationName%"=="Release" exit

IF EXIST "C:\FuelsManager.NET\Build_Fusion" rmdir /S /Q "C:\FuelsManager.NET\Build_Fusion"
mkdir "C:\FuelsManager.NET\Build_Fusion"
mkdir "C:\FuelsManager.NET\Build_Fusion\utils"

echo %ConfigurationName% build started %date% %time% > "C:\FuelsManager.NET\Build_Fusion\%ConfigurationName%.txt"

xcopy /Y /I /Q "C:\FuelsManager.NET\Fusion\Web Application\FMBuildUtils\Install.bat" "C:\FuelsManager.NET\Build_Fusion\utils"
xcopy /Y /I /Q "C:\FuelsManager.NET\Fusion\Web Application\FMBuildUtils\Uninstall.bat" "C:\FuelsManager.NET\Build_Fusion\utils"
